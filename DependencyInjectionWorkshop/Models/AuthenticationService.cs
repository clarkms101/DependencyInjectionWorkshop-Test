namespace DependencyInjectionWorkshop.Models
{
    public class AuthenticationService : IAuthentication
    {
        private IProfile _profile;
        private IFailedCounter _failedCounter;
        private IHash _hash;
        private IOtpService _otpService;
        private ILogger _logger;

        public AuthenticationService(
            IProfile profile,
            IFailedCounter failedCounter,
            IHash hash,
            IOtpService otpService,
            ILogger logger)
        {
            _profile = profile;
            _failedCounter = failedCounter;
            _hash = hash;
            _otpService = otpService;
            _logger = logger;
        }
        public AuthenticationService()
        {
            _profile = new ProfileDao();
            _failedCounter = new FailedCounter();
            _hash = new Sha256Adapter();
            _otpService = new OtpService();
            _logger = new NLogAdapter();
        }

        public bool Verify(string accountId, string password, string otp)
        {
            var currentPassword = _profile.GetPassword(accountId);

            var hashPassword = _hash.Compute(password);

            var currentOtp = _otpService.GetCurrentOtp(accountId);

            if (hashPassword == currentPassword && otp == currentOtp)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}