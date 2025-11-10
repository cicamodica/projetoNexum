using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace nexumApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddImagemUrlDataFimToMetas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            /*migrationBuilder.DropForeignKey(
                name: "FK_Filials_Ongs_OngId1",
                table: "Filials");

            migrationBuilder.DropColumn(
                name: "OngId1",
                table: "Filials");*/

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Metas",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Recurso",
                table: "Metas",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Descricao",
                table: "Metas",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DataFim",
                table: "Metas",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImagemUrl",
                table: "Metas",
                type: "nvarchar(max)",
                nullable: true);

           /* migrationBuilder.AddForeignKey(
                name: "FK_Filials_Ongs_OngId",
                table: "Filials",
                column: "OngId",
                principalTable: "Ongs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);*/
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
           /* migrationBuilder.DropForeignKey(
                name: "FK_Filials_Ongs_OngId",
                table: "Filials");*/

            migrationBuilder.DropColumn(
                name: "DataFim",
                table: "Metas");

            migrationBuilder.DropColumn(
                name: "ImagemUrl",
                table: "Metas");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Metas",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Recurso",
                table: "Metas",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Descricao",
                table: "Metas",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

           /* migrationBuilder.AddColumn<int>(
                name: "OngId1",
                table: "Filials",
                type: "int",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Filials_Ongs_OngId1",
                table: "Filials",
                column: "OngId1",
                principalTable: "Ongs",
                principalColumn: "Id");*/
        }
    }
}
