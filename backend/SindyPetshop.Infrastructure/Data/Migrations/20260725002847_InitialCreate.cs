using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SindyPetshop.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categorias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nombre = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categorias", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Productos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nombre = table.Column<string>(type: "TEXT", nullable: false),
                    Descripcion = table.Column<string>(type: "TEXT", nullable: true),
                    ImagenUrl = table.Column<string>(type: "TEXT", nullable: true),
                    Activo = table.Column<bool>(type: "INTEGER", nullable: false),
                    CategoriaId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Productos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Productos_Categorias_CategoriaId",
                        column: x => x.CategoriaId,
                        principalTable: "Categorias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VariantesProducto",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ProductoId = table.Column<int>(type: "INTEGER", nullable: false),
                    Atributo = table.Column<string>(type: "TEXT", nullable: false),
                    Valor = table.Column<string>(type: "TEXT", nullable: false),
                    Precio = table.Column<decimal>(type: "TEXT", precision: 10, scale: 2, nullable: false),
                    StockFisico = table.Column<int>(type: "INTEGER", nullable: false),
                    StockMinimoWeb = table.Column<int>(type: "INTEGER", nullable: false),
                    EsFraccionable = table.Column<bool>(type: "INTEGER", nullable: false),
                    VarianteDestinoId = table.Column<int>(type: "INTEGER", nullable: true),
                    CantidadFraccionable = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VariantesProducto", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VariantesProducto_Productos_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "Productos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VariantesProducto_VariantesProducto_VarianteDestinoId",
                        column: x => x.VarianteDestinoId,
                        principalTable: "VariantesProducto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HistorialStock",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    VarianteId = table.Column<int>(type: "INTEGER", nullable: false),
                    TipoMovimiento = table.Column<string>(type: "TEXT", nullable: false),
                    Cantidad = table.Column<int>(type: "INTEGER", nullable: false),
                    Fecha = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistorialStock", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HistorialStock_VariantesProducto_VarianteId",
                        column: x => x.VarianteId,
                        principalTable: "VariantesProducto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HistorialStock_VarianteId",
                table: "HistorialStock",
                column: "VarianteId");

            migrationBuilder.CreateIndex(
                name: "IX_Productos_CategoriaId",
                table: "Productos",
                column: "CategoriaId");

            migrationBuilder.CreateIndex(
                name: "IX_VariantesProducto_ProductoId",
                table: "VariantesProducto",
                column: "ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_VariantesProducto_VarianteDestinoId",
                table: "VariantesProducto",
                column: "VarianteDestinoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HistorialStock");

            migrationBuilder.DropTable(
                name: "VariantesProducto");

            migrationBuilder.DropTable(
                name: "Productos");

            migrationBuilder.DropTable(
                name: "Categorias");
        }
    }
}
