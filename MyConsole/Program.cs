﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DependencyInjectionWorkshop.Models;

namespace MyConsole
{
    class Program
    {


//static void Main(string[] args)
        //{
        //    IProfile profile = new ProfileDao();
        //    IHash hash = new Sha256Adapter();
        //    IOtpService otpService = new OtpService();
        //    IFailedCounter failedCounter = new FailedCounter();
        //    ILogger logger = new NLogAdapter();
        //    INotification notification = new SlackAdapter();

        //    var authenticationService =
        //        new AuthenticationService(profile, hash, otpService);

        //    var notificationDecorator = new NotificationDecorator(authenticationService, notification);
        //    var failedCounterDecorator = new FailedCounterDecorator(failedCounter,notificationDecorator);
        //    var logDecorator = new LogFailedCountDecorator(failedCounterDecorator,failedCounter,logger);

        //    var finalAuthentication = logDecorator;

        //    var isValid = finalAuthentication.Verify("joey", "pw", "123457");

        //    Console.WriteLine(isValid);
        //}

        static void Main(string[] args)
        {
            IProfile profile = new FakeProfile();
            //IProfile profile = new ProfileRepo();
            IHash hash = new FakeHash();
            //IHash hash = new Sha256Adapter();
            IOtpService otpService = new FakeOtp();
            //IOtp otpService = new OtpService();
            IFailedCounter failedCounter = new FakeFailedCounter();
            //IFailedCounter failedCounter = new FailedCounter();
            ILogger logger = new ConsoleAdapter();
            //ILogger logger = new NLogAdapter();
            INotification notification = new FakeSlack();
            //INotification notification = new SlackAdapter();

            var authenticationService =
                new AuthenticationService(profile, hash, otpService);

            var notificationDecorator = new NotificationDecorator(authenticationService, notification);
            var failedCounterDecorator = new FailedCounterDecorator(notificationDecorator, failedCounter);
            var logDecorator = new LogFaileCounterDecorator(failedCounterDecorator, failedCounter, logger);

            var finalAuthentication = logDecorator;

            var isValid = finalAuthentication.Verify("joey", "pw", "123456");

            Console.WriteLine(isValid);
        }
    }

    internal class ConsoleAdapter : ILogger
    {
        public void Info(string message)
        {
            Console.WriteLine(message);
        }
    }

    internal class FakeSlack : INotification
    {
        public void PushMessage(string message)
        {
            Console.WriteLine($"{nameof(FakeSlack)}.{nameof(PushMessage)}({message})");
        }
    }

    internal class FakeFailedCounter : IFailedCounter
    {
        public void ResetFailedCount(string accountId)
        {
            Console.WriteLine($"{nameof(FakeFailedCounter)}.{nameof(ResetFailedCount)}({accountId})");
        }

        public void AddFailedCount(string accountId)
        {
            Console.WriteLine($"{nameof(FakeFailedCounter)}.{nameof(AddFailedCount)}({accountId})");
        }

        public int GetFailedCount(string accountId)
        {
            Console.WriteLine($"{nameof(FakeFailedCounter)}.{nameof(GetFailedCount)}({accountId})");
            return 91;
        }

        public bool IsAccountLocked(string accountId)
        {
            Console.WriteLine($"{nameof(FakeFailedCounter)}.{nameof(IsAccountLocked)}({accountId})");
            return false;
        }
    }

    internal class FakeOtp : IOtpService
    {
        public string GetCurrentOtp(string accountId)
        {
            Console.WriteLine($"{nameof(FakeOtp)}.{nameof(GetCurrentOtp)}({accountId})");
            return "123456";
        }
    }

    internal class FakeHash : IHash
    {
        public string Compute(string plainText)
        {
            Console.WriteLine($"{nameof(FakeHash)}.{nameof(Compute)}({plainText})");
            return "my hashed password";
        }
    }

    internal class FakeProfile : IProfile
    {
        public string GetPassword(string accountId)
        {
            Console.WriteLine($"{nameof(FakeProfile)}.{nameof(GetPassword)}({accountId})");
            return "my hashed password";
        }
    }
}
