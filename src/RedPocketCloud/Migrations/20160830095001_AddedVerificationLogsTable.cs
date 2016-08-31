using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RedPocketCloud.Migrations
{
    public partial class AddedVerificationLogsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VerificationLogs",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MyCat:ValueGeneratedOnAdd", true),
                    CouponId = table.Column<long>(nullable: false),
                    ProviderId = table.Column<long>(nullable: false),
                    Time = table.Column<DateTime>(nullable: false),
                    WalletId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VerificationLogs", x => x.Id);
                });

            migrationBuilder.AddColumn<long>(
                name: "ProviderId",
                table: "Wallets",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_VerificationLogs_CouponId",
                table: "VerificationLogs",
                column: "CouponId");

            migrationBuilder.CreateIndex(
                name: "IX_VerificationLogs_ProviderId",
                table: "VerificationLogs",
                column: "ProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_VerificationLogs_Time",
                table: "VerificationLogs",
                column: "Time");

            migrationBuilder.CreateIndex(
                name: "IX_VerificationLogs_WalletId",
                table: "VerificationLogs",
                column: "WalletId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProviderId",
                table: "Wallets");

            migrationBuilder.DropTable(
                name: "VerificationLogs");
        }
    }
}
