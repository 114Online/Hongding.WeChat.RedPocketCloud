﻿using System;
using Pomelo.AspNetCore.TimedJob;
using RedPocketCloud.Controllers;

namespace RedPocketCloud.Jobs
{
    public class CurrentLimitingJob : Job
    {
        /// <summary>
        /// 限流器
        /// </summary>
        [Invoke(SkipWhileExecuting = false, Interval = 1000 * 10)]
        public void ClearExpiredCoupon()
        {
            var cnt = WeChatController.RequestCount;
            WeChatController.RequestCount = 0;
            if (cnt >= 3000)
                WeChatController.Limiting = 5000 / cnt;
            else
                WeChatController.Limiting = 1;
            if (cnt >= 1000)
                GC.Collect();
        }
    }
}
