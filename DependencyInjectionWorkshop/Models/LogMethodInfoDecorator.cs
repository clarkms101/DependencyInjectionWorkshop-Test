namespace DependencyInjectionWorkshop.Models
{
    public class LogMethodInfoDecorator : BaseAuthenticationDecorator
    {
        private readonly ILogger _logger;

        public LogMethodInfoDecorator(IAuthentication authentication, ILogger logger)
            : base(authentication)
        {
            _logger = logger;
        }

        public override bool Verify(string accountId, string password, string otp)
        {
            var msg = $"{nameof(LogMethodInfoDecorator)}-{nameof(Verify)}-{accountId}-{password}-{otp}";
            _logger.Info(msg);
            var isVerify = base.Verify(accountId, password, otp);
            _logger.Info(isVerify.ToString());
            return isVerify;
        }

    }
}