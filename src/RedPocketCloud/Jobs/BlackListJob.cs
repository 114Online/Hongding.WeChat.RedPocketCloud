using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Pomelo.AspNetCore.TimedJob;
using RedPocketCloud.Models;

namespace RedPocketCloud.Jobs
{
    public class BlackListJob : Job
    {
        /// <summary>
        /// 每日凌晨2时黑名单解锁
        /// </summary>
        /// <param name="DB"></param>
        [Invoke(Begin = "2016-01-01 2:00:00", SkipWhileExecuting = true, Interval = 1000 * 60 * 60 * 24)]
        public void ClearExpiredCoupon(RpcContext DB)
        {
            DB.BlackLists
                .Where(x => x.Unlock.HasValue && x.Unlock.Value <= DateTime.Now)
                .DeleteAsync();
        }
    }
}
