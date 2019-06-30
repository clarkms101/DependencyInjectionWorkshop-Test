using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DependencyInjectionWorkshop.Models;

namespace MyConsole
{
    class Program
    {

        static void Main(string[] args)
        {
            var authenticationService = new AuthenticationService(new ProfileDao(), new Sha256Adapter(), new OtpService());

            var notificationDecorator = new NotificationDecorator(authenticationService, new SlackAdapter());
            var failedCounterDecorator = new FailedCounterDecorator(notificationDecorator, new FailedCounter());
            var logFaileCounterDecorator = new LogFaileCounterDecorator(failedCounterDecorator,new FailedCounter(), new NLogAdapter());

            var isValid = logFaileCounterDecorator.Verify("clark", "123", "456");
            Console.WriteLine(isValid);
        }

    }
}
