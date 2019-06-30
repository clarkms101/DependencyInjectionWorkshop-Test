namespace DependencyInjectionWorkshop.Models
{
    public class LogFaileCounterDecorator : BaseAuthenticationDecorator
    {
        private readonly IFailedCounter _failedCounter;
        private readonly ILogger _logger;

        public LogFaileCounterDecorator(IAuthentication authentication, IFailedCounter failedCounter, ILogger logger)
            : base(authentication)
        {
            _failedCounter = failedCounter;
            _logger = logger;
        }

        public override bool Verify(string accountId, string password, string otp)
        {
            var isVerify = base.Verify(accountId, password, otp);
            if (!isVerify)
            {
                LogFaileCount(accountId);
            }

            return isVerify;
        }

        private void LogFaileCount(string accountId)
        {
            int failedCount = _failedCounter.GetFailedCount(accountId);
            _logger.Info($"accountId:{accountId} failed times:{failedCount}");
        }
    }
}