using Microsoft.EntityFrameworkCore.Migrations;

namespace Infraestructura.AccesoDatos.Migrations
{
    public partial class barrioCodMigra : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CodBarrio",
                schema: "JS",
                table: "Personal",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CodBarrio",
                schema: "JS",
                table: "Personal");
        }
    }
}
