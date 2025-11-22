using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace nexumApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class RelacionarVagaCandidatoInscricoes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Voluntarios");

            migrationBuilder.DropColumn(
                name: "IdVoluntario",
                table: "Candidato");

            migrationBuilder.AddColumn<int>(
                name: "CandidatoId",
                table: "Inscricoes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VagaIdVaga",
                table: "Inscricoes",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Inscricoes_CandidatoId",
                table: "Inscricoes",
                column: "CandidatoId");

            migrationBuilder.CreateIndex(
                name: "IX_Inscricoes_VagaIdVaga",
                table: "Inscricoes",
                column: "VagaIdVaga");

            migrationBuilder.AddForeignKey(
                name: "FK_Inscricoes_Candidato_CandidatoId",
                table: "Inscricoes",
                column: "CandidatoId",
                principalTable: "Candidato",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Inscricoes_Vaga_VagaIdVaga",
                table: "Inscricoes",
                column: "VagaIdVaga",
                principalTable: "Vaga",
                principalColumn: "IdVaga");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Inscricoes_Candidato_CandidatoId",
                table: "Inscricoes");

            migrationBuilder.DropForeignKey(
                name: "FK_Inscricoes_Vaga_VagaIdVaga",
                table: "Inscricoes");

            migrationBuilder.DropIndex(
                name: "IX_Inscricoes_CandidatoId",
                table: "Inscricoes");

            migrationBuilder.DropIndex(
                name: "IX_Inscricoes_VagaIdVaga",
                table: "Inscricoes");

            migrationBuilder.DropColumn(
                name: "CandidatoId",
                table: "Inscricoes");

            migrationBuilder.DropColumn(
                name: "VagaIdVaga",
                table: "Inscricoes");

            migrationBuilder.AddColumn<int>(
                name: "IdVoluntario",
                table: "Candidato",
                type: "int",
                maxLength: 3,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Voluntarios",
                columns: table => new
                {
                    IdFormulario = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CPF = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataAprovacao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DataNascimento = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Genero = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Habilidades = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImagemUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Telefone = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Voluntarios", x => x.IdFormulario);
                });
        }
    }
}
