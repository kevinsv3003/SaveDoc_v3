﻿using Microsoft.EntityFrameworkCore.Migrations;

namespace Infraestructura.AccesoDatos.Migrations
{
    public partial class actividadRenta : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RentaId",
                schema: "JS",
                table: "Actividad",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RentaId",
                schema: "JS",
                table: "Actividad");
        }
    }
}
