using System;
using Pomelo.AspNetCore.TimedJob;
using RedPocketCloud.Controllers;
using Microsoft.Extensions.Configuration;

namespace RedPocketCloud.Jobs
{
    public class CurrentLimitingJob : Job
    {
        /// <summary>
        /// 限流器
        /// </summary>
        [Invoke(SkipWhileExecuting = false, Interval = 1000)]
        public void ClearExpiredCoupon(IConfiguration Config)
        {
            var cnt = WeChatController.RequestCount;
            WeChatController.RequestCount = 0;
            if (cnt >= Convert.ToInt64(Config["Host:Limit"]))
                WeChatController.Limiting = 500.0 / cnt;
            else
                WeChatController.Limiting = 1;
            if (cnt >= 2000)
                GC.Collect();
        }
    }
}
