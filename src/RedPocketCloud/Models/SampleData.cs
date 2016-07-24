using System;
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
            DB.Database.EnsureCreated();
            await RoleManager.CreateAsync(new IdentityRole<long>() { Name = "Root" });
            await RoleManager.CreateAsync(new IdentityRole<long>() { Name="Member" });
            var user = new User { UserName = "admin", Balance = 10000.00 };
            await UserManager.CreateAsync(user, "123456");
            await UserManager.AddToRoleAsync(user, "Root");
        }
    }
}
