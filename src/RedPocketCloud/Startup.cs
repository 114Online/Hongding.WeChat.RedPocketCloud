using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using RedPocketCloud.Models;

namespace RedPocketCloud
{
    public class Startup
    {
        public static IConfiguration Config;

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddConfiguration(out Config);
            services.AddMvc();
            services.AddRedis(x =>
            {
                x.ConnectionString = Config["Host:Redis"];
                x.Database = 0;
                x.EventKey = "REDPOCKET_SIGNALR_INSTANCE_";
            })
                .AddSignalR();
            services.AddSmartUser<User, long>();
            services.AddSmartCookies();
            services.AddBlobStorage()
                .AddEntityFrameworkStorage<RpcContext, Blob, long>();
            services.AddDistributedRedisCache(x => 
            {
                x.InstanceName = "REDPOCKET_INSTANCE_";
                x.Configuration = Config["Host:Redis"];
            });
            services.AddSession(o =>
            {
                o.IdleTimeout = new TimeSpan(0, 20, 0);
            });
            services.AddLogging();
            if (Config["Host:Mode"] == "MySQL")
                services.AddDbContext<RpcContext>(x => 
                {
                    x.UseMySql(Config["Host:ConnectionString"]);
                    x.UseMySqlLolita();
                });
            else
                services.AddDbContext<RpcContext>(x => 
                {
                    x.UseMyCat(Config["Host:ConnectionString"]);
                    x.UseMySqlLolita();
                });
            services.AddIdentity<User, IdentityRole<long>>(x =>
            {
                x.Password.RequireDigit = false;
                x.Password.RequiredLength = 0;
                x.Password.RequireLowercase = false;
                x.Password.RequireNonAlphanumeric = false;
                x.Password.RequireUppercase = false;
                x.User.AllowedUserNameCharacters = null;
            })
                .AddDefaultTokenProviders()
                .AddEntityFrameworkStores<RpcContext, long>();
        }

        public async void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(LogLevel.Error);
            app.UseSession();
            app.UseSignalR();
            app.UseIdentity();
            app.UseBlobStorage<Blob, long>("/assets/js/jquery.pomelo.fileupload.js");
            app.UseStaticFiles();
            app.UseDeveloperExceptionPage();
            app.UseMvcWithDefaultRoute();

            await SampleData.InitDB(app.ApplicationServices);
        }
    }
}
