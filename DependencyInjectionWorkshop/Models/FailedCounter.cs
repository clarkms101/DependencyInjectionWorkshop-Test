﻿using System;
using System.Net.Http;

namespace DependencyInjectionWorkshop.Models
{
    public interface IFailedCounter
    {
        void ResetFailedCount(string accountId);
        void AddFailedCount(string accountId);
        int GetFailedCount(string accountId);
        bool IsAccountLocked(string accountId);
    }

    public class FailedCounter : IFailedCounter
    {
        public void ResetFailedCount(string accountId)
        {
            var resetResponse = 
                new HttpClient(){ BaseAddress = new Uri("http://joey.com/") }
                    .PostAsJsonAsync("api/failedCounter/Reset", accountId).Result;
            resetResponse.EnsureSuccessStatusCode();
        }

        public void AddFailedCount(string accountId)
        {
            var addFailedCountResponse = 
                new HttpClient(){ BaseAddress = new Uri("http://joey.com/") }
                    .PostAsJsonAsync("api/failedCounter/Add", accountId).Result;
            addFailedCountResponse.EnsureSuccessStatusCode();
        }

        public int GetFailedCount(string accountId)
        {
            var failedCountResponse =
                new HttpClient(){ BaseAddress = new Uri("http://joey.com/") }
                    .PostAsJsonAsync("api/failedCounter/GetFailedCount", accountId).Result;

            failedCountResponse.EnsureSuccessStatusCode();

            var failedCount = failedCountResponse.Content.ReadAsAsync<int>().Result;
            return failedCount;
        }

        public bool IsAccountLocked(string accountId)
        {
            var isLockedResponse = new HttpClient(){ BaseAddress = new Uri("http://joey.com/") }
                .PostAsJsonAsync("api/failedCounter/IsLocked", accountId).Result;
            isLockedResponse.EnsureSuccessStatusCode();
            var isLocked = isLockedResponse.Content.ReadAsAsync<bool>().Result;
            return isLocked;
        }
    }
}