using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace nexumApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarHeaderImageOng : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HeaderImageURL",
                table: "Ongs",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HeaderImageURL",
                table: "Ongs");
        }
    }
}
