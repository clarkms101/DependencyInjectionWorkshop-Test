namespace DependencyInjectionWorkshop.Models
{
    public class FailedCounterDecorator : BaseAuthenticationDecorator
    {
        private readonly IFailedCounter _failedCounter;

        public FailedCounterDecorator(IAuthentication authentication, IFailedCounter failedCounter) 
            : base(authentication)
        {
            _failedCounter = failedCounter;
        }

        public override bool Verify(string accountId, string password, string otp)
        {
            CheckAccountIsLock(accountId);
            return base.Verify(accountId, password, otp);
        }

        private void CheckAccountIsLock(string accountId)
        {
            // 驗證前先檢查帳號是否被鎖
            var isLocked = _failedCounter.IsAccountLocked(accountId);
            if (isLocked)
            {
                throw new FailedTooManyTimesException();
            }
        }
    }
}