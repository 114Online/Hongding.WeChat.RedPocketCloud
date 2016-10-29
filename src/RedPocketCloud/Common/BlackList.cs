using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Caching.Distributed;
using RedPocketCloud.Models;

namespace RedPocketCloud.Common
{
    public static class BlackList
    {
        public static List<string> BlackListCache = new List<string>();

        public static void BuildBlackListCache(RpcContext DB)
        {
            BlackListCache = DB.BlackLists
                .OrderBy(x => x.Id)
                .Select(x => x.OpenId)
                .ToList();
        }

        public static void InitBlackList(IServiceProvider services)
        {
            var DB = services.GetRequiredService<RpcContext>();
            BuildBlackListCache(DB);
        }
    }
}
