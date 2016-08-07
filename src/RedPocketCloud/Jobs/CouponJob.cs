using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Pomelo.AspNetCore.TimedJob;
using RedPocketCloud.Models;

namespace RedPocketCloud.Jobs
{
    public class CouponJob : Job
    {
        /// <summary>
        /// 每日凌晨2时清空过期优惠券
        /// </summary>
        /// <param name="DB"></param>
        [Invoke(Begin = "2016-01-01 2:00:00", SkipWhileExecuting = true, Interval = 1000 * 60 * 60 * 24)]
        public void ClearExpiredCoupon(RpcContext DB)
        {
            var time = DateTime.Now.AddDays(-3);
            var wallets = DB.Wallets
                .Where(x => x.Expire >= time)
                .Delete();
        }
    }
}
