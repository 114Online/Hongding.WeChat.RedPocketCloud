using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Pomelo.AspNetCore.TimedJob;
using RedPocketCloud.Models;
using static RedPocketCloud.Common.BlackList;

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

        /// <summary>
        /// 缓存黑名单
        /// </summary>
        /// <param name="DB"></param>
        /// <param name="Cache"></param>
        [Invoke(Begin = "2016-01-01 2:00:00", SkipWhileExecuting = true, Interval = 1000 * 60 * 60 * 24)]
        public void CachingBlackList(RpcContext DB)
        {
            BuildBlackListCache(DB);
        }

        /// <summary>
        /// 分析恶意行为
        /// </summary>
        [Invoke(Begin = "2016-01-01 1:00:00", SkipWhileExecuting = true, Interval = 1000 * 60 * 60 * 24)]
        public void AnalyzeDistrictedAction(RpcContext DB)
        {
            var begin = DateTime.Now.Date.AddDays(-1).AddHours(1);
            var activities = DB.Activities
                .Where(x => x.Begin >= begin)
                .ToList();
            foreach(var act in activities)
            {
                var redpockets = DB.RedPockets
                    .Where(x => x.ActivityId == act.Id)
                    .OrderBy(x => x.ReceivedTime)
                    .GroupBy(x => x.OpenId)
                    .Select(x => new
                    {
                        OpenId = x.Key,
                        Count = x.Count(),
                        RedPockets = x.ToList()
                    });
                foreach(var x in redpockets)
                {
                    if (x.Count <= 1 || DB.BlackLists.FirstOrDefault(y => y.OpenId == x.OpenId) != null)
                        continue;
                    for(var i = 0; i < x.Count - 2; i++)
                        if ((x.RedPockets[i + 1].ReceivedTime.Value - x.RedPockets[i].ReceivedTime.Value).Milliseconds <= 15000)
                            DB.BlackLists.Add(new BlackList { OpenId = x.OpenId, Unlock = DateTime.Now.AddDays(15) });
                        else if (x.Count > act.Limit)
                            DB.BlackLists.Add(new BlackList { OpenId = x.OpenId, Unlock = DateTime.Now.AddDays(15) });
                }
            }
            DB.SaveChanges();
        }
    }
}