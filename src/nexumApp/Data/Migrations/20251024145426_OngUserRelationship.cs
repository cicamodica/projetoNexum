using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace nexumApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class OngUserRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ongs_AspNetUsers_UserId",
                table: "Ongs");

            migrationBuilder.AddForeignKey(
                name: "FK_Ongs_AspNetUsers_UserId",
                table: "Ongs",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ongs_AspNetUsers_UserId",
                table: "Ongs");

            migrationBuilder.AddForeignKey(
                name: "FK_Ongs_AspNetUsers_UserId",
                table: "Ongs",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
