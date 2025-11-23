using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace nexumApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarWebsiteOng : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Website",
                table: "Ongs",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Website",
                table: "Ongs");
        }
    }
}
