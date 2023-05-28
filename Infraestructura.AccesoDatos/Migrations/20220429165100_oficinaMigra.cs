using Microsoft.EntityFrameworkCore.Migrations;

namespace Infraestructura.AccesoDatos.Migrations
{
    public partial class oficinaMigra : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OficinaId",
                schema: "JS",
                table: "Personal",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Oficina",
                schema: "JS",
                columns: table => new
                {
                    OficinaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    Descripcion = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true),
                    Estado = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Oficina", x => x.OficinaId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Personal_OficinaId",
                schema: "JS",
                table: "Personal",
                column: "OficinaId");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            

            migrationBuilder.DropTable(
                name: "Oficina",
                schema: "JS");

            migrationBuilder.DropIndex(
                name: "IX_Personal_OficinaId",
                schema: "JS",
                table: "Personal");

            migrationBuilder.DropColumn(
                name: "OficinaId",
                schema: "JS",
                table: "Personal");
        }
    }
}
