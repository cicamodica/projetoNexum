using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace nexumApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class filials1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Descriçao",
                table: "Filials",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Nome",
                table: "Filials",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OngId",
                table: "Filials",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Filials",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Filials_OngId",
                table: "Filials",
                column: "OngId");

            migrationBuilder.AddForeignKey(
                name: "FK_Filials_Ongs_OngId",
                table: "Filials",
                column: "OngId",
                principalTable: "Ongs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Filials_Ongs_OngId",
                table: "Filials");

            migrationBuilder.DropIndex(
                name: "IX_Filials_OngId",
                table: "Filials");

            migrationBuilder.DropColumn(
                name: "Descriçao",
                table: "Filials");

            migrationBuilder.DropColumn(
                name: "Nome",
                table: "Filials");

            migrationBuilder.DropColumn(
                name: "OngId",
                table: "Filials");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Filials");
        }
    }
}
