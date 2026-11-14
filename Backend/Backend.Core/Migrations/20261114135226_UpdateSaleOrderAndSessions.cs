using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Core.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSaleOrderAndSessions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SaleOrderItems_Products_ProductId",
                table: "SaleOrderItems");

            migrationBuilder.DropIndex(
                name: "IX_SaleOrderItems_ProductId",
                table: "SaleOrderItems");

            migrationBuilder.RenameColumn(
                name: "ProductId",
                table: "SaleOrderItems",
                newName: "ItemId");

            migrationBuilder.AddColumn<string>(
                name: "ItemName",
                table: "SaleOrderItems",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ItemType",
                table: "SaleOrderItems",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Services",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    ShortDescription = table.Column<string>(type: "TEXT", nullable: false),
                    LongDescription = table.Column<string>(type: "TEXT", nullable: false),
                    Tags = table.Column<string>(type: "TEXT", nullable: false),
                    Cost = table.Column<decimal>(type: "TEXT", nullable: false),
                    Price = table.Column<decimal>(type: "TEXT", nullable: false),
                    Sku = table.Column<string>(type: "TEXT", nullable: false),
                    IsForSale = table.Column<bool>(type: "INTEGER", nullable: false),
                    QuantityType = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Services", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserSessions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<long>(type: "INTEGER", nullable: false),
                    Token = table.Column<string>(type: "TEXT", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSessions", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserSessions_Token",
                table: "UserSessions",
                column: "Token",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Services");

            migrationBuilder.DropTable(
                name: "UserSessions");

            migrationBuilder.DropColumn(
                name: "ItemName",
                table: "SaleOrderItems");

            migrationBuilder.DropColumn(
                name: "ItemType",
                table: "SaleOrderItems");

            migrationBuilder.RenameColumn(
                name: "ItemId",
                table: "SaleOrderItems",
                newName: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrderItems_ProductId",
                table: "SaleOrderItems",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_SaleOrderItems_Products_ProductId",
                table: "SaleOrderItems",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
