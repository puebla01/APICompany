using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace API.Company.Infraestructure.Data.Migrations
{
    public partial class Applications : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Organizations_Nombre",
                table: "Organizations");

            migrationBuilder.AlterColumn<string>(
                name: "Version",
                table: "Organizations",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Estado",
                table: "Organizations",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Entorno",
                table: "Organizations",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10,
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IdApplication",
                table: "Organizations",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Applications",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    SourceUpdate = table.Column<int>(nullable: false),
                    fxaltareg = table.Column<DateTime>(type: "datetime", nullable: false),
                    fxmodreg = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Applications", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Organizations_IdApplication",
                table: "Organizations",
                column: "IdApplication");

            migrationBuilder.CreateIndex(
                name: "IX_Organizations_Nombre_IdApplication",
                table: "Organizations",
                columns: new[] { "Nombre", "IdApplication" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Organizations_Applications",
                table: "Organizations",
                column: "IdApplication",
                principalTable: "Applications",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Organizations_Applications",
                table: "Organizations");

            migrationBuilder.DropTable(
                name: "Applications");

            migrationBuilder.DropIndex(
                name: "IX_Organizations_IdApplication",
                table: "Organizations");

            migrationBuilder.DropIndex(
                name: "IX_Organizations_Nombre_IdApplication",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "IdApplication",
                table: "Organizations");

            migrationBuilder.AlterColumn<string>(
                name: "Version",
                table: "Organizations",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "Estado",
                table: "Organizations",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "Entorno",
                table: "Organizations",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 10);

            migrationBuilder.CreateIndex(
                name: "IX_Organizations_Nombre",
                table: "Organizations",
                column: "Nombre",
                unique: true);
        }
    }
}
