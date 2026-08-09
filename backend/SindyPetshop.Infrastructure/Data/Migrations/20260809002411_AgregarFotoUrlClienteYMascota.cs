using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SindyPetshop.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AgregarFotoUrlClienteYMascota : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FotoUrl",
                table: "Mascotas",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FotoUrl",
                table: "Clientes",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FotoUrl",
                table: "Mascotas");

            migrationBuilder.DropColumn(
                name: "FotoUrl",
                table: "Clientes");
        }
    }
}
