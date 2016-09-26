using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using RedPocketCloud.Models;

namespace RedPocketCloud.Migrations
{
    public partial class ImplementCommandActivity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Command",
                table: "Activities",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Activities",
                nullable: false,
                defaultValue: ActivityType.Convention);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Command",
                table: "Activities");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Activities");
        }
    }
}
