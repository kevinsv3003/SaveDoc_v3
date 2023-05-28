using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infraestructura.AccesoDatos.Migrations
{
    public partial class cuentaMigra : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "PER");

            migrationBuilder.CreateTable(
                name: "Cuenta",
                schema: "PER",
                columns: table => new
                {
                    CuentaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreCuenta = table.Column<string>(type: "varchar(200)", nullable: true),
                    NumCuenta = table.Column<string>(type: "varchar(200)", nullable: true),
                    SaldoActual = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cuenta", x => x.CuentaId);
                });

            migrationBuilder.CreateTable(
                name: "TipoMovimiento",
                schema: "PER",
                columns: table => new
                {
                    TipoMovimientoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TipoMov = table.Column<string>(type: "varchar(200)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipoMovimiento", x => x.TipoMovimientoId);
                });

            migrationBuilder.CreateTable(
                name: "Movimiento",
                schema: "PER",
                columns: table => new
                {
                    MovimientoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CuentaId = table.Column<int>(type: "int", nullable: false),
                    TipoMovimientoId = table.Column<int>(type: "int", nullable: false),
                    Concepto = table.Column<string>(type: "varchar(200)", nullable: true),
                    Monto = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FechaMovimiento = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Movimiento", x => x.MovimientoId);
                    table.ForeignKey(
                        name: "FK_Movimiento_Cuenta_CuentaId",
                        column: x => x.CuentaId,
                        principalSchema: "PER",
                        principalTable: "Cuenta",
                        principalColumn: "CuentaId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Movimiento_TipoMovimiento_TipoMovimientoId",
                        column: x => x.TipoMovimientoId,
                        principalSchema: "PER",
                        principalTable: "TipoMovimiento",
                        principalColumn: "TipoMovimientoId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Movimiento_CuentaId",
                schema: "PER",
                table: "Movimiento",
                column: "CuentaId");

            migrationBuilder.CreateIndex(
                name: "IX_Movimiento_TipoMovimientoId",
                schema: "PER",
                table: "Movimiento",
                column: "TipoMovimientoId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Movimiento",
                schema: "PER");

            migrationBuilder.DropTable(
                name: "Cuenta",
                schema: "PER");

            migrationBuilder.DropTable(
                name: "TipoMovimiento",
                schema: "PER");

        }
    }
}
