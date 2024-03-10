using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerce.Persistance.Migrations
{
    public partial class update8 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OrderCıde",
                table: "Orders",
                newName: "OrderCode");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OrderCode",
                table: "Orders",
                newName: "OrderCıde");
        }
    }
}
