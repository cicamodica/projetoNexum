using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace nexumApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AtualizaModeloMeta : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "QuantidadeNecessaria",
                table: "Metas",
                newName: "ValorAtual");

            migrationBuilder.RenameColumn(
                name: "QuantidadeDisponivel",
                table: "Metas",
                newName: "ValorAlvo");

            migrationBuilder.RenameColumn(
                name: "IdRecurso",
                table: "Metas",
                newName: "OngId");

            migrationBuilder.AddColumn<string>(
                name: "Descricao",
                table: "Metas",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Metas_OngId",
                table: "Metas",
                column: "OngId");

            migrationBuilder.AddForeignKey(
                name: "FK_Metas_Ongs_OngId",
                table: "Metas",
                column: "OngId",
                principalTable: "Ongs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Metas_Ongs_OngId",
                table: "Metas");

            migrationBuilder.DropIndex(
                name: "IX_Metas_OngId",
                table: "Metas");

            migrationBuilder.DropColumn(
                name: "Descricao",
                table: "Metas");

            migrationBuilder.RenameColumn(
                name: "ValorAtual",
                table: "Metas",
                newName: "QuantidadeNecessaria");

            migrationBuilder.RenameColumn(
                name: "ValorAlvo",
                table: "Metas",
                newName: "QuantidadeDisponivel");

            migrationBuilder.RenameColumn(
                name: "OngId",
                table: "Metas",
                newName: "IdRecurso");
        }
    }
}
