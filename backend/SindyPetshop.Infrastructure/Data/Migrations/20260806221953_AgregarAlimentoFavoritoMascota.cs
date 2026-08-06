using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SindyPetshop.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AgregarAlimentoFavoritoMascota : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "AlimentoFavoritoActualizadoEn",
                table: "Mascotas",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AlimentoFavoritoActualizadoPor",
                table: "Mascotas",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AlimentoFavoritoDescripcion",
                table: "Mascotas",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AlimentoFavoritoProductoId",
                table: "Mascotas",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Mascotas_AlimentoFavoritoProductoId",
                table: "Mascotas",
                column: "AlimentoFavoritoProductoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Mascotas_Productos_AlimentoFavoritoProductoId",
                table: "Mascotas",
                column: "AlimentoFavoritoProductoId",
                principalTable: "Productos",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Mascotas_Productos_AlimentoFavoritoProductoId",
                table: "Mascotas");

            migrationBuilder.DropIndex(
                name: "IX_Mascotas_AlimentoFavoritoProductoId",
                table: "Mascotas");

            migrationBuilder.DropColumn(
                name: "AlimentoFavoritoActualizadoEn",
                table: "Mascotas");

            migrationBuilder.DropColumn(
                name: "AlimentoFavoritoActualizadoPor",
                table: "Mascotas");

            migrationBuilder.DropColumn(
                name: "AlimentoFavoritoDescripcion",
                table: "Mascotas");

            migrationBuilder.DropColumn(
                name: "AlimentoFavoritoProductoId",
                table: "Mascotas");
        }
    }
}
