using Microsoft.EntityFrameworkCore.Migrations;

namespace API.Company.Infraestructure.Data.Migrations
{
    public partial class Procesos : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Procesos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Proceso = table.Column<string>(maxLength: 50, nullable: false),
                    IdOrganizacion = table.Column<int>(nullable: false),
                    Obs = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Procesos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Procesos_Organizations",
                        column: x => x.IdOrganizacion,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Procesos_IdOrganizacion",
                table: "Procesos",
                column: "IdOrganizacion");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Procesos");
        }
    }
}
