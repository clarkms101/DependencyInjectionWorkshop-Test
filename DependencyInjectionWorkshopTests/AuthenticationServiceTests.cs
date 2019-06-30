using System;
using DependencyInjectionWorkshop.Models;
using NSubstitute;
using NUnit.Framework;

namespace DependencyInjectionWorkshopTests
{
    [TestFixture]
    public class AuthenticationServiceTests
    {
        private const string DefaultAccount = "clark";
        private const string DefaultInputPassword = "9487";
        private const string DefaultOtp = "9527";
        private const string DefaultHashedPassword = "abc";
        private const int DefaultFailedCount = 91;
        private IAuthentication _authentication;
        private IProfile _profile;
        private IHash _hash;
        private IOtpService _otpService;
        private IFailedCounter _failedCounter;
        private INotification _notification;
        private ILogger _logger;

        [SetUp]
        public void SetUp()
        {
            _profile = Substitute.For<IProfile>();
            _hash = Substitute.For<IHash>();
            _otpService = Substitute.For<IOtpService>();
            _failedCounter = Substitute.For<IFailedCounter>();
            _notification = Substitute.For<INotification>();
            _logger = Substitute.For<ILogger>();
            // 主體
            var authenticationService = new AuthenticationService(
                _profile,
                _failedCounter,
                _hash,
                _otpService,
                _logger);

            // 主體(_authentication)以界面作接口
            // authenticationService 配上 Notification => notificationDecorator
            var notificationDecorator = new NotificationDecorator(authenticationService, _notification);
            // notificationDecorator 配上 FailedCounterDecorator => 主體
            _authentication = new FailedCounterDecorator(notificationDecorator, _failedCounter);
        }

        [Test]
        public void is_valid()
        {
            var isValid = WhenValid();
            ShouldBeValid(isValid);
        }

        [Test]
        public void is_invalid_when_otp_is_wrong()
        {
            var isValid = WhenInvalid();
            ShouldBeInvalid(isValid);
        }

        [Test]
        public void should_notify_when_invalid()
        {
            WhenInvalid();
            ShouldNotify(DefaultAccount);
        }

        [Test]
        public void should_add_failed_count_when_invalid()
        {
            WhenInvalid();
            ShouldAddFailedCount(DefaultAccount);
        }

        [Test]
        public void should_reset_failed_count_when_valid()
        {
            WhenValid();
            ShouldResetFailedCount(DefaultAccount);
        }

        [Test]
        public void should_log_failed_count_when_invalid()
        {
            GivenFailedCount(DefaultFailedCount);
            WhenInvalid();

            ShouldLog(DefaultAccount, DefaultFailedCount.ToString());
        }

        [Test]
        public void account_is_locked()
        {
            GivenAccountIsLocked();
            ShouldThrow<FailedTooManyTimesException>();
        }

        private void ShouldResetFailedCount(string accountId)
        {
            _failedCounter.Received().ResetFailedCount(accountId);
        }

        private void ShouldLog(string account, string failedCount)
        {
            _logger.Received().Info(Arg.Is<string>(m => m.Contains(account) && m.Contains(failedCount)));
        }

        private void GivenFailedCount(int failedCount)
        {
            _failedCounter.GetFailedCount(DefaultAccount).ReturnsForAnyArgs(failedCount);
        }

        private void ShouldAddFailedCount(string accountId)
        {
            _failedCounter.Received().AddFailedCount(accountId);
        }

        private void ShouldThrow<TException>() where TException : Exception
        {
            TestDelegate action = () => WhenValid();

            Assert.Throws<TException>(action);
        }

        private void GivenAccountIsLocked()
        {
            _failedCounter.IsAccountLocked(DefaultAccount).ReturnsForAnyArgs(true);
        }

        private void ShouldNotify(string account)
        {
            _notification.Received().PushMessage(Arg.Is<string>(m => m.Contains(account)));
        }

        private bool WhenValid()
        {
            GivenPasswordFromDb(DefaultAccount, DefaultHashedPassword);
            GivenHashPassword(DefaultInputPassword, DefaultHashedPassword);
            GivenOtp(DefaultAccount, DefaultOtp);

            var isValid = WhenVerify(DefaultAccount, DefaultInputPassword, DefaultOtp);
            return isValid;
        }

        private bool WhenInvalid()
        {
            GivenPasswordFromDb(DefaultAccount, DefaultHashedPassword);
            GivenHashPassword(DefaultInputPassword, DefaultHashedPassword);
            GivenOtp(DefaultAccount, DefaultOtp);

            var isValid = WhenVerify(DefaultAccount, DefaultInputPassword, "wrong otp");
            return isValid;
        }

        private static void ShouldBeInvalid(bool isValid)
        {
            Assert.IsFalse(isValid);
        }

        private static void ShouldBeValid(bool isValid)
        {
            Assert.IsTrue(isValid);
        }

        private bool WhenVerify(string accountId, string password, string otp)
        {
            return _authentication.Verify(accountId, password, otp);
        }

        private void GivenOtp(string accountId, string otp)
        {
            _otpService.GetCurrentOtp(accountId).ReturnsForAnyArgs(otp);
        }

        private void GivenHashPassword(string password, string hashedPassword)
        {
            _hash.Compute(password).ReturnsForAnyArgs(hashedPassword);
        }

        private void GivenPasswordFromDb(string accountId, string passwordFromDb)
        {
            _profile.GetPassword(accountId).ReturnsForAnyArgs(passwordFromDb);
        }
    }
}