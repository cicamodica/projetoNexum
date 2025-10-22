using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace nexumApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class OngModelUpdate2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Documento",
                table: "Ongs");

            migrationBuilder.AddColumn<bool>(
                name: "Aprovaçao",
                table: "Ongs",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Aprovaçao",
                table: "Ongs");

            migrationBuilder.AddColumn<byte[]>(
                name: "Documento",
                table: "Ongs",
                type: "varbinary(max)",
                nullable: true);
        }
    }
}
