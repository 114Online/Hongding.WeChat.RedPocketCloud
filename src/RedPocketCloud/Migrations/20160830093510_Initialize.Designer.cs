using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using RedPocketCloud.Models;
using RedPocketCloud.ViewModels;

namespace RedPocketCloud.Migrations
{
    [DbContext(typeof(RpcContext))]
    [Migration("20160830093510_Initialize")]
    partial class Initialize
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.0.0-rtm-21431");

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole<long>", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<string>("NormalizedName")
                        .HasAnnotation("MaxLength", 256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .HasName("RoleNameIndex");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRoleClaim<long>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<long>("RoleId");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserClaim<long>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<long>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserLogin<long>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<long>("UserId");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserRole<long>", b =>
                {
                    b.Property<long>("UserId");

                    b.Property<long>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserToken<long>", b =>
                {
                    b.Property<long>("UserId");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("RedPocketCloud.Models.Activity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<long>("Attend");

                    b.Property<DateTime>("Begin");

                    b.Property<long>("BriberiesCount");

                    b.Property<DateTime?>("End");

                    b.Property<bool>("IsBegin");

                    b.Property<int>("Limit");

                    b.Property<long>("MerchantId");

                    b.Property<long>("Price");

                    b.Property<double>("Ratio");

                    b.Property<long>("ReceivedCount");

                    b.Property<JsonObject<List<RuleViewModel>>>("Rules");

                    b.Property<long>("TemplateId");

                    b.Property<string>("Title");

                    b.HasKey("Id");

                    b.HasIndex("Attend");

                    b.HasIndex("Begin");

                    b.HasIndex("End");

                    b.HasIndex("IsBegin");

                    b.HasIndex("MerchantId");

                    b.HasIndex("Price");

                    b.ToTable("Activities");
                });

            modelBuilder.Entity("RedPocketCloud.Models.Blob", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<byte[]>("Bytes");

                    b.Property<long>("ContentLength");

                    b.Property<string>("ContentType")
                        .HasAnnotation("MaxLength", 128);

                    b.Property<string>("FileName")
                        .HasAnnotation("MaxLength", 128);

                    b.Property<DateTime>("Time");

                    b.HasKey("Id");

                    b.HasIndex("FileName");

                    b.HasIndex("Time");

                    b.ToTable("Blobs");
                });

            modelBuilder.Entity("RedPocketCloud.Models.Coupon", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Description");

                    b.Property<long>("ImageId");

                    b.Property<long>("MerchantId");

                    b.Property<string>("Provider");

                    b.Property<int>("Time");

                    b.Property<string>("Title");

                    b.HasKey("Id");

                    b.HasIndex("MerchantId");

                    b.ToTable("Coupons");
                });

            modelBuilder.Entity("RedPocketCloud.Models.PayLog", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<double>("Balance");

                    b.Property<long>("MerchantId");

                    b.Property<double>("Price");

                    b.Property<DateTime>("Time");

                    b.HasKey("Id");

                    b.HasIndex("MerchantId");

                    b.HasIndex("Price");

                    b.HasIndex("Time");

                    b.ToTable("PayLogs");
                });

            modelBuilder.Entity("RedPocketCloud.Models.RedPocket", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<long>("ActivityId");

                    b.Property<string>("AvatarUrl")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasAnnotation("MaxLength", 64);

                    b.Property<long?>("CouponId");

                    b.Property<string>("Ip")
                        .HasAnnotation("MaxLength", 64);

                    b.Property<string>("NickName")
                        .HasAnnotation("MaxLength", 64);

                    b.Property<string>("OpenId")
                        .HasAnnotation("MaxLength", 32);

                    b.Property<long>("Price");

                    b.Property<DateTime?>("ReceivedTime");

                    b.Property<int>("Type");

                    b.Property<string>("Url")
                        .HasAnnotation("MaxLength", 256);

                    b.HasKey("Id");

                    b.HasIndex("ActivityId");

                    b.HasIndex("Price");

                    b.HasIndex("ReceivedTime");

                    b.HasIndex("Type");

                    b.ToTable("RedPockets");
                });

            modelBuilder.Entity("RedPocketCloud.Models.Template", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<long?>("BackgroundId");

                    b.Property<long?>("BottomPartId");

                    b.Property<long?>("DrawnId");

                    b.Property<long>("MerchantId");

                    b.Property<long?>("PendingId");

                    b.Property<string>("RuleUrl")
                        .HasAnnotation("MaxLength", 64);

                    b.Property<long?>("TopPartId");

                    b.Property<int>("Type");

                    b.Property<long?>("UndrawnId");

                    b.HasKey("Id");

                    b.ToTable("Templates");
                });

            modelBuilder.Entity("RedPocketCloud.Models.User", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccessFailedCount");

                    b.Property<double>("Balance");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Email")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<int>("Limit");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("Merchant");

                    b.Property<string>("NormalizedEmail")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<string>("NormalizedUserName")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasAnnotation("MaxLength", 256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("RedPocketCloud.Models.Wallet", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<long>("CouponId");

                    b.Property<DateTime>("Expire");

                    b.Property<long>("MerchantId");

                    b.Property<string>("OpenId")
                        .HasAnnotation("MaxLength", 32);

                    b.Property<DateTime>("Time");

                    b.Property<string>("VerifyCode")
                        .HasAnnotation("MaxLength", 20);

                    b.HasKey("Id");

                    b.HasIndex("MerchantId");

                    b.HasIndex("OpenId");

                    b.ToTable("Wallets");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRoleClaim<long>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole<long>")
                        .WithMany("Claims")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserClaim<long>", b =>
                {
                    b.HasOne("RedPocketCloud.Models.User")
                        .WithMany("Claims")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserLogin<long>", b =>
                {
                    b.HasOne("RedPocketCloud.Models.User")
                        .WithMany("Logins")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserRole<long>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole<long>")
                        .WithMany("Users")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("RedPocketCloud.Models.User")
                        .WithMany("Roles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
