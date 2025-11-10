using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace nexumApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddFlagsFaleConoscoModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Ip",
                table: "FaleConoscoModels");

            migrationBuilder.AddColumn<bool>(
                name: "Arquivada",
                table: "FaleConoscoModels",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Respondida",
                table: "FaleConoscoModels",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Arquivada",
                table: "FaleConoscoModels");

            migrationBuilder.DropColumn(
                name: "Respondida",
                table: "FaleConoscoModels");

            migrationBuilder.AddColumn<string>(
                name: "Ip",
                table: "FaleConoscoModels",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true);
        }
    }
}
