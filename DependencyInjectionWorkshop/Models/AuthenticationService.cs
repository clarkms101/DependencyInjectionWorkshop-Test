namespace DependencyInjectionWorkshop.Models
{
    public class AuthenticationService : IAuthentication
    {
        private IProfile _profile;
        private IHash _hash;
        private IOtpService _otpService;

        public AuthenticationService(
            IProfile profile,
            IHash hash,
            IOtpService otpService)
        {
            _profile = profile;
            _hash = hash;
            _otpService = otpService;
        }
        public AuthenticationService()
        {
            _profile = new ProfileDao();
            new FailedCounter();
            _hash = new Sha256Adapter();
            _otpService = new OtpService();
            new NLogAdapter();
        }

        public bool Verify(string accountId, string password, string otp)
        {
            var currentPassword = _profile.GetPassword(accountId);

            var hashPassword = _hash.Compute(password);

            var currentOtp = _otpService.GetCurrentOtp(accountId);

            return hashPassword == currentPassword && otp == currentOtp;
        }
    }
}