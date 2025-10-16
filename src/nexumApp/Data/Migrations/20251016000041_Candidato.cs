using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace nexumApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class Candidato : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Candidato",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", maxLength: 3, nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdVoluntario = table.Column<int>(type: "int", maxLength: 3, nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(90)", maxLength: 90, nullable: false),
                    CPF = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    Telefone = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    DataInscricao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TemExperiencia = table.Column<bool>(type: "bit", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Candidato", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Candidato");
        }
    }
}
