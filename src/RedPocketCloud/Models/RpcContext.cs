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

        public DbSet<Bribery> Briberies { get; set; }

        public DbSet<PayLog> PayLogs { get; set; }

        public DbSet<Template> Templates { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.SetupBlobStorage();

            builder.Entity<Bribery>(e =>
            {
                e.HasIndex(x => x.ReceivedTime);
                e.HasIndex(x => x.Price);
            });

            builder.Entity<PayLog>(e =>
            {
                e.HasIndex(x => x.Price);
                e.HasIndex(x => x.Time);
            });

            builder.Entity<Activity>(e =>
            {
                e.HasIndex(x => x.Price);
                e.HasIndex(x => x.Begin);
                e.HasIndex(x => x.End);
            });

            builder.Entity<Bribery>(e =>
            {
                e.HasIndex(x => x.ReceivedTime);
            });
        }
    }
}
