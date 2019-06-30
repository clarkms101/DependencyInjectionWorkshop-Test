﻿namespace DependencyInjectionWorkshop.Models
{
    public interface IAuthentication
    {
        bool Verify(string accountId, string password, string otp);
    }

    public class Authentication : IAuthentication
    {
        private IProfile _profile;
        private IFailedCounter _failedCounter;
        private IHash _hash;
        private IOtpService _otpService;
        private ILogger _logger;

        public Authentication(
            IProfile profile,
            IFailedCounter failedCounter,
            IHash hash,
            IOtpService otpService,
            ILogger logger,
            INotification notification)
        {
            _profile = profile;
            _failedCounter = failedCounter;
            _hash = hash;
            _otpService = otpService;
            _logger = logger;
        }
        public Authentication()
        {
            _profile = new ProfileDao();
            _failedCounter = new FailedCounter();
            _hash = new Sha256Adapter();
            _otpService = new OtpService();
            _logger = new NLogAdapter();
            new SlackAdapter();
        }

        public bool Verify(string accountId, string password, string otp)
        {
            var currentPassword = _profile.GetPassword(accountId);

            var hashPassword = _hash.Compute(password);

            var currentOtp = _otpService.GetCurrentOtp(accountId);

            if (hashPassword == currentPassword && otp == currentOtp)
            {
                // 驗證成功，重設失敗次數
                _failedCounter.ResetFailedCount(accountId);

                return true;
            }
            else
            {
                // 驗證失敗，累計失敗次數
                _failedCounter.AddFailedCount(accountId);

                int failedCount = _failedCounter.GetFailedCount(accountId);
                _logger.Info($"accountId:{accountId} failed times:{failedCount}");

                return false;
            }
        }

    }
}