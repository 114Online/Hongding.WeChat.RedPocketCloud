﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace RedPocketCloud.Models
{
    public static class SampleData
    {
        public static async Task InitDB(IServiceProvider services)
        {
            var DB = services.GetRequiredService<RpcContext>();
            var UserManager = services.GetRequiredService<UserManager<User>>();
            var RoleManager = services.GetRequiredService<RoleManager<IdentityRole<long>>>();
            await RoleManager.CreateAsync(new IdentityRole<long>() { Name = "Root" });
            await RoleManager.CreateAsync(new IdentityRole<long>() { Name = "Tenant" });
            if (DB.Users.Count() == 0)
            {
                var user = new User { UserName = "admin", Balance = 10000.00, Merchant = "宏鼎科技" };
                await UserManager.CreateAsync(user, "Hdkj2016");
                await UserManager.AddToRoleAsync(user, "Root");
            }
        }
    }
}
