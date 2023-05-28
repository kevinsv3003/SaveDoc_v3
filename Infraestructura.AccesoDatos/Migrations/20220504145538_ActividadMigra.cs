using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infraestructura.AccesoDatos.Migrations
{
    public partial class ActividadMigra : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Actividad",
                schema: "JS",
                columns: table => new
                {
                    ActividadId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreActividad = table.Column<string>(type: "varchar(200)", nullable: false),
                    Lugar = table.Column<string>(type: "varchar(200)", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EsEspecial = table.Column<bool>(type: "bit", nullable: false),
                    Descripcion = table.Column<string>(type: "varchar(200)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Actividad", x => x.ActividadId);
                });

            migrationBuilder.CreateTable(
                name: "DetalleActividad",
                schema: "JS",
                columns: table => new
                {
                    DetalleActividadId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ActividadId = table.Column<int>(type: "int", nullable: false),
                    PersonalId = table.Column<int>(type: "int", nullable: false),
                    Asistio = table.Column<bool>(type: "bit", nullable: false),
                    Observacion = table.Column<string>(type: "varchar(200)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetalleActividad", x => x.DetalleActividadId);
                    table.ForeignKey(
                        name: "FK_DetalleActividad_Actividad_ActividadId",
                        column: x => x.ActividadId,
                        principalSchema: "JS",
                        principalTable: "Actividad",
                        principalColumn: "ActividadId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DetalleActividad_Personal_PersonalId",
                        column: x => x.PersonalId,
                        principalSchema: "JS",
                        principalTable: "Personal",
                        principalColumn: "PersonalId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DetalleActividad_ActividadId",
                schema: "JS",
                table: "DetalleActividad",
                column: "ActividadId");

            migrationBuilder.CreateIndex(
                name: "IX_DetalleActividad_PersonalId",
                schema: "JS",
                table: "DetalleActividad",
                column: "PersonalId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DetalleActividad",
                schema: "JS");

            migrationBuilder.DropTable(
                name: "Actividad",
                schema: "JS");
        }
    }
}
