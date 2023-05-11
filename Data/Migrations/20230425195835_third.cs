using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Atelier.Data.Migrations
{
    public partial class third : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Image",
                table: "Vetement",
                newName: "ImageVetement");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImageVetement",
                table: "Vetement",
                newName: "Image");
        }
    }
}
