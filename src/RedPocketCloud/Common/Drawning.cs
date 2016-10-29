using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.EntityFrameworkCore;
using RedPocketCloud.Models;

namespace RedPocketCloud.Common
{
    public static class Drawning
    {
        private static Random Random = new Random();

        /// <summary>
        /// 获取红包缓存
        /// </summary>
        /// <param name="DB"></param>
        /// <param name="Cache"></param>
        /// <param name="ActivityId"></param>
        /// <returns></returns>
        public static async Task<List<long>> GetRedPocketCaching(RpcContext DB, IDistributedCache Cache, long ActivityId)
        {
            var rps = await Cache.GetObjectAsync<List<long>>("ACTIVITY_REDPOCKET_CACHE_" + ActivityId);
            if (rps == null)
            {
                rps = (await DB.RedPockets
                    .AsNoTracking()
                    .Where(x => x.ActivityId == ActivityId && string.IsNullOrEmpty(x.OpenId))
                    .Select(x => x.Id)
                    .ToListAsync()) // Store the result to memory
                    .OrderBy(x => Guid.NewGuid()) // Random order
                    .ToList();
                await Cache.SetObjectAsync("ACTIVITY_REDPOCKET_CACHE_" + ActivityId, rps);
            }
            return rps;
        }

        /// <summary>
        /// 抽取一个红包
        /// </summary>
        /// <param name="DB"></param>
        /// <param name="Cache"></param>
        /// <param name="ActivityId"></param>
        /// <returns></returns>
        public static async Task<RedPocket> GetRedPocket(RpcContext DB, IDistributedCache Cache, long ActivityId)
        {
            var rps = await GetRedPocketCaching(DB, Cache, ActivityId);
            if (rps.Count > 0)
            {
                var id = rps[Random.Next(0, rps.Count)];
                rps.Remove(id);
                Cache.SetObjectAsync("ACTIVITY_REDPOCKET_CACHE_" + ActivityId, rps);
                return DB.RedPockets
                    .AsNoTracking()
                    .SingleOrDefault(x => x.Id == id && string.IsNullOrEmpty(x.OpenId));
            }
            else
            {
                return null;
            }
        }
    }
}
