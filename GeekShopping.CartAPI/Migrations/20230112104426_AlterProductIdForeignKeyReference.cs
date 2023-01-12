using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GeekShopping.CartAPI.Migrations
{
    public partial class AlterProductIdForeignKeyReference : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_cart_detail_product_CartHeaderId",
                table: "cart_detail");

            migrationBuilder.CreateIndex(
                name: "IX_cart_detail_ProductId",
                table: "cart_detail",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_cart_detail_product_ProductId",
                table: "cart_detail",
                column: "ProductId",
                principalTable: "product",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_cart_detail_product_ProductId",
                table: "cart_detail");

            migrationBuilder.DropIndex(
                name: "IX_cart_detail_ProductId",
                table: "cart_detail");

            migrationBuilder.AddForeignKey(
                name: "FK_cart_detail_product_CartHeaderId",
                table: "cart_detail",
                column: "CartHeaderId",
                principalTable: "product",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
