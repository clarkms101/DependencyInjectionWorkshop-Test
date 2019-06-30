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
            var isValid =  base.Verify(accountId, password, otp);
            if (isValid)
            {
                // 驗證成功，重設失敗次數
                _failedCounter.ResetFailedCount(accountId);
            }
            else
            {
                // 驗證失敗，累計失敗次數
                _failedCounter.AddFailedCount(accountId);
            }

            return isValid;
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