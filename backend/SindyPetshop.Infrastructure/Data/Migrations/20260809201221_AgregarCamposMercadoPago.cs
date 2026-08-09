using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SindyPetshop.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AgregarCamposMercadoPago : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MercadoPagoPaymentId",
                table: "Pedidos",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MercadoPagoPreferenceId",
                table: "Pedidos",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MercadoPagoPaymentId",
                table: "Pedidos");

            migrationBuilder.DropColumn(
                name: "MercadoPagoPreferenceId",
                table: "Pedidos");
        }
    }
}
