using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Pomelo.AspNetCore.Extensions.BlobStorage.Models;
using Microsoft.EntityFrameworkCore;

namespace RedPocketCloud.Models
{
    public class RpcContext : IdentityDbContext<User, IdentityRole<long>, long>, IBlobStorageDbContext<Blob, long>
    {
        public RpcContext(DbContextOptions opt) 
            : base(opt)
        {
        }

        public DbSet<Blob> Blobs { get; set; }

        public DbSet<Activity> Activities { get; set; }

        public DbSet<RedPocket> RedPockets { get; set; }

        public DbSet<PayLog> PayLogs { get; set; }

        public DbSet<Template> Templates { get; set; }

        public DbSet<Coupon> Coupons { get; set; }

        public DbSet<Wallet> Wallets { get; set; }

        public DbSet<VerificationLog> VerificationLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.SetupBlobStorage<Blob, long>();

            builder.Entity<PayLog>(e =>
            {
                e.HasIndex(x => x.Price);
                e.HasIndex(x => x.Time);
                e.HasIndex(x => x.MerchantId);
            });

            builder.Entity<Activity>(e =>
            {
                e.HasIndex(x => x.Price);
                e.HasIndex(x => x.Begin);
                e.HasIndex(x => x.End);
                e.HasIndex(x => x.MerchantId);
                e.HasIndex(x => x.IsBegin);
                e.HasIndex(x => x.Attend);
            });

            builder.Entity<RedPocket>(e =>
            {
                e.HasIndex(x => x.ReceivedTime);
                e.HasIndex(x => x.ActivityId);
                e.HasIndex(x => x.Price);
                e.HasIndex(x => x.Type);
            });

            builder.Entity<Wallet>(e => 
            {
                e.HasIndex(x => x.OpenId);
                e.HasIndex(x => x.MerchantId);
            });

            builder.Entity<Coupon>(e =>
            {
                e.HasIndex(x => x.MerchantId);
            });

            builder.Entity<VerificationLog>(e =>
            {
                e.HasIndex(x => x.ProviderId);
                e.HasIndex(x => x.Time);
                e.HasIndex(x => x.CouponId);
                e.HasIndex(x => x.WalletId);
            });
        }
    }
}
