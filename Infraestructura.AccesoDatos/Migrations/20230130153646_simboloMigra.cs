using Microsoft.EntityFrameworkCore.Migrations;

namespace Infraestructura.AccesoDatos.Migrations
{
    public partial class simboloMigra : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "simboloMoneda",
                schema: "PER",
                table: "Cuenta",
                type: "varchar(5)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "simboloMoneda",
                schema: "PER",
                table: "Cuenta");
        }
    }
}
