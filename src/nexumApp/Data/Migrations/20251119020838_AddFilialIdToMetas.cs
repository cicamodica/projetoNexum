using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace nexumApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddFilialIdToMetas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FilialId",
                table: "Metas",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Metas_FilialId",
                table: "Metas",
                column: "FilialId");

            migrationBuilder.AddForeignKey(
                name: "FK_Metas_Filials_FilialId",
                table: "Metas",
                column: "FilialId",
                principalTable: "Filials",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Metas_Filials_FilialId",
                table: "Metas");

            migrationBuilder.DropIndex(
                name: "IX_Metas_FilialId",
                table: "Metas");

            migrationBuilder.DropColumn(
                name: "FilialId",
                table: "Metas");
        }
    }
}
