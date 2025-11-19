using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace nexumApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AlterarCandidatoParaFotoUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Foto",
                table: "Candidato");

            migrationBuilder.AddColumn<string>(
                name: "FotoUrl",
                table: "Candidato",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FotoUrl",
                table: "Candidato");

            migrationBuilder.AddColumn<byte[]>(
                name: "Foto",
                table: "Candidato",
                type: "varbinary(max)",
                nullable: true);
        }
    }
}
