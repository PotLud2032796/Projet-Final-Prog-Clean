using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Atelier.Data.Migrations
{
    public partial class AjoutIsHybride : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsHybride",
                table: "Vetement",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsHybride",
                table: "Vetement");
        }
    }
}
