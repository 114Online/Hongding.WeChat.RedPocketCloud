using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pomelo.AspNetCore.TimedJob;
using RedPocketCloud.Models;

namespace RedPocketCloud.Jobs
{
    public class CouponJob : Job
    {
        [Invoke(Begin = "2016-01-01 2:00:00", SkipWhileExecuting = true)]
        public void ClearExpiredCoupon(RpcContext DB)
        {
            
        }
    }
}
