using Microsoft.EntityFrameworkCore.Migrations;

namespace Infraestructura.AccesoDatos.Migrations
{
    public partial class barrioMigra : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Barrio",
                schema: "JS",
                columns: table => new
                {
                    BarrioId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreBarrio = table.Column<string>(type: "varchar(200)", nullable: false),
                    MunicipioId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Barrio", x => x.BarrioId);
                    table.ForeignKey(
                        name: "FK_Barrio_Municipio_MunicipioId",
                        column: x => x.MunicipioId,
                        principalSchema: "JS",
                        principalTable: "Municipio",
                        principalColumn: "MunicipioId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Barrio_MunicipioId",
                schema: "JS",
                table: "Barrio",
                column: "MunicipioId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Barrio",
                schema: "JS");
        }
    }
}
