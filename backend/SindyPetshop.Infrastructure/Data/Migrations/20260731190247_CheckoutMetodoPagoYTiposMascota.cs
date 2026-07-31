using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SindyPetshop.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class CheckoutMetodoPagoYTiposMascota : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiraReservaEn",
                table: "Pedidos",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MetodoPago",
                table: "Pedidos",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpiraReservaEn",
                table: "Pedidos");

            migrationBuilder.DropColumn(
                name: "MetodoPago",
                table: "Pedidos");
        }
    }
}
