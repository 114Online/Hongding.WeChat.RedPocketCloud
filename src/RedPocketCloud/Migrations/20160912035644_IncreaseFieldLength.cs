using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RedPocketCloud.Migrations
{
    public partial class IncreaseFieldLength : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "OpenId",
                table: "RedPockets",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NickName",
                table: "RedPockets",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AvatarUrl",
                table: "RedPockets",
                maxLength: 512,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "OpenId",
                table: "RedPockets",
                maxLength: 32,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NickName",
                table: "RedPockets",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AvatarUrl",
                table: "RedPockets",
                maxLength: 256,
                nullable: true);
        }
    }
}
