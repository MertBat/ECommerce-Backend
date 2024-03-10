using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerce.Persistance.Migrations
{
    public partial class update7 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OrderCıde",
                table: "Orders",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrderCıde",
                table: "Orders");
        }
    }
}
