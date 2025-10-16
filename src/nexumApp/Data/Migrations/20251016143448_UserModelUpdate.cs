using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace nexumApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class UserModelUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Foto",
                table: "Ongs");

            migrationBuilder.RenameColumn(
                name: "Cnpj",
                table: "AspNetUsers",
                newName: "CPF");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CPF",
                table: "AspNetUsers",
                newName: "Cnpj");

            migrationBuilder.AddColumn<byte[]>(
                name: "Foto",
                table: "Ongs",
                type: "varbinary(max)",
                nullable: true);
        }
    }
}
