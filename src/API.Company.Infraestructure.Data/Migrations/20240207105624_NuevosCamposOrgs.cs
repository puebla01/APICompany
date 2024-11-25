using Microsoft.EntityFrameworkCore.Migrations;

namespace API.Company.Infraestructure.Data.Migrations
{
    public partial class NuevosCamposOrgs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Entorno",
                table: "Organizations",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Estado",
                table: "Organizations",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Version",
                table: "Organizations",
                maxLength: 20,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Entorno",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "Estado",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "Organizations");
        }
    }
}
