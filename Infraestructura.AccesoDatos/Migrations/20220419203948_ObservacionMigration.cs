using Microsoft.EntityFrameworkCore.Migrations;

namespace Infraestructura.AccesoDatos.Migrations
{
    public partial class ObservacionMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Observacion",
                schema: "JS",
                columns: table => new
                {
                    ObservacionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ObservacionDet = table.Column<string>(type: "varchar(200)", nullable: false),
                    PersonalId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Observacion", x => x.ObservacionId);
                    table.ForeignKey(
                        name: "FK_Observacion_Personal_PersonalId",
                        column: x => x.PersonalId,
                        principalSchema: "JS",
                        principalTable: "Personal",
                        principalColumn: "PersonalId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Observacion_PersonalId",
                schema: "JS",
                table: "Observacion",
                column: "PersonalId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Observacion",
                schema: "JS");
        }
    }
}
