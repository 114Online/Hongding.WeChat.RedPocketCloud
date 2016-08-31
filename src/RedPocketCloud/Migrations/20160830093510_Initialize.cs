using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using RedPocketCloud.ViewModels;

namespace RedPocketCloud.Migrations
{
    public partial class Initialize : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MyCat:ValueGeneratedOnAdd", true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    Name = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<long>(nullable: false),
                    LoginProvider = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                });

            migrationBuilder.CreateTable(
                name: "Activities",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MyCat:ValueGeneratedOnAdd", true),
                    Attend = table.Column<long>(nullable: false),
                    Begin = table.Column<DateTime>(nullable: false),
                    BriberiesCount = table.Column<long>(nullable: false),
                    End = table.Column<DateTime>(nullable: true),
                    IsBegin = table.Column<bool>(nullable: false),
                    Limit = table.Column<int>(nullable: false),
                    MerchantId = table.Column<long>(nullable: false),
                    Price = table.Column<long>(nullable: false),
                    Ratio = table.Column<double>(nullable: false),
                    ReceivedCount = table.Column<long>(nullable: false),
                    Rules = table.Column<JsonObject<List<RuleViewModel>>>(nullable: true),
                    TemplateId = table.Column<long>(nullable: false),
                    Title = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Activities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Blobs",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MyCat:ValueGeneratedOnAdd", true),
                    Bytes = table.Column<byte[]>(nullable: true),
                    ContentLength = table.Column<long>(nullable: false),
                    ContentType = table.Column<string>(maxLength: 128, nullable: true),
                    FileName = table.Column<string>(maxLength: 128, nullable: true),
                    Time = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Blobs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Coupons",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MyCat:ValueGeneratedOnAdd", true),
                    Description = table.Column<string>(nullable: true),
                    ImageId = table.Column<long>(nullable: false),
                    MerchantId = table.Column<long>(nullable: false),
                    Provider = table.Column<string>(nullable: true),
                    Time = table.Column<int>(nullable: false),
                    Title = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coupons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PayLogs",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MyCat:ValueGeneratedOnAdd", true),
                    Balance = table.Column<double>(nullable: false),
                    MerchantId = table.Column<long>(nullable: false),
                    Price = table.Column<double>(nullable: false),
                    Time = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RedPockets",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MyCat:ValueGeneratedOnAdd", true),
                    ActivityId = table.Column<long>(nullable: false),
                    AvatarUrl = table.Column<string>(maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(maxLength: 64, nullable: true),
                    CouponId = table.Column<long>(nullable: true),
                    Ip = table.Column<string>(maxLength: 64, nullable: true),
                    NickName = table.Column<string>(maxLength: 64, nullable: true),
                    OpenId = table.Column<string>(maxLength: 32, nullable: true),
                    Price = table.Column<long>(nullable: false),
                    ReceivedTime = table.Column<DateTime>(nullable: true),
                    Type = table.Column<int>(nullable: false),
                    Url = table.Column<string>(maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RedPockets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Templates",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MyCat:ValueGeneratedOnAdd", true),
                    BackgroundId = table.Column<long>(nullable: true),
                    BottomPartId = table.Column<long>(nullable: true),
                    DrawnId = table.Column<long>(nullable: true),
                    MerchantId = table.Column<long>(nullable: false),
                    PendingId = table.Column<long>(nullable: true),
                    RuleUrl = table.Column<string>(maxLength: 64, nullable: true),
                    TopPartId = table.Column<long>(nullable: true),
                    Type = table.Column<int>(nullable: false),
                    UndrawnId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Templates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MyCat:ValueGeneratedOnAdd", true),
                    AccessFailedCount = table.Column<int>(nullable: false),
                    Balance = table.Column<double>(nullable: false),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    Email = table.Column<string>(maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    Limit = table.Column<int>(nullable: false),
                    LockoutEnabled = table.Column<bool>(nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                    Merchant = table.Column<string>(nullable: true),
                    NormalizedEmail = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(maxLength: 256, nullable: true),
                    PasswordHash = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                    SecurityStamp = table.Column<string>(nullable: true),
                    TwoFactorEnabled = table.Column<bool>(nullable: false),
                    UserName = table.Column<string>(maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Wallets",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MyCat:ValueGeneratedOnAdd", true),
                    CouponId = table.Column<long>(nullable: false),
                    Expire = table.Column<DateTime>(nullable: false),
                    MerchantId = table.Column<long>(nullable: false),
                    OpenId = table.Column<string>(maxLength: 32, nullable: true),
                    Time = table.Column<DateTime>(nullable: false),
                    VerifyCode = table.Column<string>(maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wallets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MyCat:ValueGeneratedOnAdd", true),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true),
                    RoleId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MyCat:ValueGeneratedOnAdd", true),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true),
                    UserId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(nullable: false),
                    ProviderKey = table.Column<string>(nullable: false),
                    ProviderDisplayName = table.Column<string>(nullable: true),
                    UserId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<long>(nullable: false),
                    RoleId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_UserId",
                table: "AspNetUserRoles",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Activities_Attend",
                table: "Activities",
                column: "Attend");

            migrationBuilder.CreateIndex(
                name: "IX_Activities_Begin",
                table: "Activities",
                column: "Begin");

            migrationBuilder.CreateIndex(
                name: "IX_Activities_End",
                table: "Activities",
                column: "End");

            migrationBuilder.CreateIndex(
                name: "IX_Activities_IsBegin",
                table: "Activities",
                column: "IsBegin");

            migrationBuilder.CreateIndex(
                name: "IX_Activities_MerchantId",
                table: "Activities",
                column: "MerchantId");

            migrationBuilder.CreateIndex(
                name: "IX_Activities_Price",
                table: "Activities",
                column: "Price");

            migrationBuilder.CreateIndex(
                name: "IX_Blobs_FileName",
                table: "Blobs",
                column: "FileName");

            migrationBuilder.CreateIndex(
                name: "IX_Blobs_Time",
                table: "Blobs",
                column: "Time");

            migrationBuilder.CreateIndex(
                name: "IX_Coupons_MerchantId",
                table: "Coupons",
                column: "MerchantId");

            migrationBuilder.CreateIndex(
                name: "IX_PayLogs_MerchantId",
                table: "PayLogs",
                column: "MerchantId");

            migrationBuilder.CreateIndex(
                name: "IX_PayLogs_Price",
                table: "PayLogs",
                column: "Price");

            migrationBuilder.CreateIndex(
                name: "IX_PayLogs_Time",
                table: "PayLogs",
                column: "Time");

            migrationBuilder.CreateIndex(
                name: "IX_RedPockets_ActivityId",
                table: "RedPockets",
                column: "ActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_RedPockets_Price",
                table: "RedPockets",
                column: "Price");

            migrationBuilder.CreateIndex(
                name: "IX_RedPockets_ReceivedTime",
                table: "RedPockets",
                column: "ReceivedTime");

            migrationBuilder.CreateIndex(
                name: "IX_RedPockets_Type",
                table: "RedPockets",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Wallets_MerchantId",
                table: "Wallets",
                column: "MerchantId");

            migrationBuilder.CreateIndex(
                name: "IX_Wallets_OpenId",
                table: "Wallets",
                column: "OpenId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "Activities");

            migrationBuilder.DropTable(
                name: "Blobs");

            migrationBuilder.DropTable(
                name: "Coupons");

            migrationBuilder.DropTable(
                name: "PayLogs");

            migrationBuilder.DropTable(
                name: "RedPockets");

            migrationBuilder.DropTable(
                name: "Templates");

            migrationBuilder.DropTable(
                name: "Wallets");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");
        }
    }
}
