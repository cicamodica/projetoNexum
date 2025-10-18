using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace nexumApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class OngUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Ongs",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ongs_UserId",
                table: "Ongs",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Ongs_AspNetUsers_UserId",
                table: "Ongs",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ongs_AspNetUsers_UserId",
                table: "Ongs");

            migrationBuilder.DropIndex(
                name: "IX_Ongs_UserId",
                table: "Ongs");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Ongs");
        }
    }
}
