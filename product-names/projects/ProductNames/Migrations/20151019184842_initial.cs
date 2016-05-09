using System;
using System.Collections.Generic;
using Microsoft.Data.Entity.Migrations;

namespace ProductNames.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProductName",
                columns: table => new
                {
                    ProductNameID = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductName", x => x.ProductNameID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable("ProductName");
        }
    }
}
