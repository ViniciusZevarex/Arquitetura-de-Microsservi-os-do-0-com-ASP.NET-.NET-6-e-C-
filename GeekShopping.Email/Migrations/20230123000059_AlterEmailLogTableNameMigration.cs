using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GeekShopping.Email.Migrations
{
    public partial class AlterEmailLogTableNameMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_order_logs",
                table: "order_logs");

            migrationBuilder.RenameTable(
                name: "order_logs",
                newName: "email_logs");

            migrationBuilder.AddPrimaryKey(
                name: "PK_email_logs",
                table: "email_logs",
                column: "id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_email_logs",
                table: "email_logs");

            migrationBuilder.RenameTable(
                name: "email_logs",
                newName: "order_logs");

            migrationBuilder.AddPrimaryKey(
                name: "PK_order_logs",
                table: "order_logs",
                column: "id");
        }
    }
}
