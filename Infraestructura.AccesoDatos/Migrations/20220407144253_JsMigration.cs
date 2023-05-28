using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infraestructura.AccesoDatos.Migrations
{
    public partial class JsMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "DOC");

            migrationBuilder.EnsureSchema(
                name: "AUDI");

            migrationBuilder.EnsureSchema(
                name: "JS");

            migrationBuilder.EnsureSchema(
                name: "SEG");
                      

            

            migrationBuilder.CreateTable(
                name: "Renta",
                schema: "JS",
                columns: table => new
                {
                    RentaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreRenta = table.Column<string>(type: "varchar(200)", nullable: false),
                    Descripcion = table.Column<string>(type: "varchar(200)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Renta", x => x.RentaId);
                });

           

            migrationBuilder.CreateTable(
                name: "Personal",
                schema: "JS",
                columns: table => new
                {
                    PersonalId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PrimerNombre = table.Column<string>(type: "varchar(50)", nullable: false),
                    SegundoNombre = table.Column<string>(type: "varchar(50)", nullable: false),
                    PrimerApellido = table.Column<string>(type: "varchar(50)", nullable: false),
                    SegundoApellido = table.Column<string>(type: "varchar(50)", nullable: false),
                    Cedula = table.Column<string>(type: "varchar(20)", nullable: false),
                    Telefono = table.Column<string>(type: "varchar(10)", nullable: false),
                    CodDepartamento = table.Column<int>(type: "int", nullable: false),
                    CodMunicipio = table.Column<int>(type: "int", nullable: false),
                    Direccion = table.Column<string>(type: "varchar(200)", nullable: false),
                    RentaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Personal", x => x.PersonalId);
                    table.ForeignKey(
                        name: "FK_Personal_Renta_RentaId",
                        column: x => x.RentaId,
                        principalSchema: "JS",
                        principalTable: "Renta",
                        principalColumn: "RentaId",
                        onDelete: ReferentialAction.Cascade);
                });

          


            migrationBuilder.CreateIndex(
                name: "IX_Personal_RentaId",
                schema: "JS",
                table: "Personal",
                column: "RentaId");

           

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
           

            migrationBuilder.DropTable(
                name: "Personal",
                schema: "JS");

          


            migrationBuilder.DropTable(
                name: "Renta",
                schema: "JS");

           

        }
    }
}
