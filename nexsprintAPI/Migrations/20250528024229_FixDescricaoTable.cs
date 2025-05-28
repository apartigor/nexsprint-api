using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace nexsprintAPI.Migrations
{
    /// <inheritdoc />
    public partial class FixDescricaoTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "descricao",
                table: "Modulos",
                newName: "Descricao");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Descricao",
                table: "Modulos",
                newName: "descricao");
        }
    }
}
