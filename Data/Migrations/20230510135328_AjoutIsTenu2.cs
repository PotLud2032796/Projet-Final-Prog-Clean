using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Atelier.Data.Migrations
{
    public partial class AjoutIsTenu2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsHybride",
                table: "Vetement",
                newName: "IsTenu2");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsTenu2",
                table: "Vetement",
                newName: "IsHybride");
        }
    }
}
