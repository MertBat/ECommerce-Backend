using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerce.Persistance.Migrations
{
    public partial class update11 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "OrderStatus",
                table: "CompletedOrders",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrderStatus",
                table: "CompletedOrders");
        }
    }
}
