using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SindyPetshop.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AgregarDetalleHistorialStock : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Detalle",
                table: "HistorialStock",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Detalle",
                table: "HistorialStock");
        }
    }
}
