using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RedPocketCloud.Migrations
{
    public partial class AddedBlackList : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BlackLists",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MyCat:ValueGeneratedOnAdd", true),
                    OpenId = table.Column<string>(maxLength: 64, nullable: true),
                    Unlock = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlackLists", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BlackLists_OpenId",
                table: "BlackLists",
                column: "OpenId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BlackLists_Unlock",
                table: "BlackLists",
                column: "Unlock");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BlackLists");
        }
    }
}
