using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace nexumApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class OngImg : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "Foto",
                table: "Ongs",
                type: "varbinary(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Foto",
                table: "Ongs");
        }
    }
}
