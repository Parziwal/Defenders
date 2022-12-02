using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CaffWebApp.DAL.Migrations
{
    public partial class DurationForCiff : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CaffImages_AspNetUsers_UploadedById",
                table: "CaffImages");

            migrationBuilder.DropColumn(
                name: "AnimationDuration",
                table: "CaffImages");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedById",
                table: "Comments",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<int>(
                name: "Duration",
                table: "CiffImages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "UploadedById",
                table: "CaffImages",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddForeignKey(
                name: "FK_CaffImages_AspNetUsers_UploadedById",
                table: "CaffImages",
                column: "UploadedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CaffImages_AspNetUsers_UploadedById",
                table: "CaffImages");

            migrationBuilder.DropColumn(
                name: "Duration",
                table: "CiffImages");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedById",
                table: "Comments",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UploadedById",
                table: "CaffImages",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AnimationDuration",
                table: "CaffImages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_CaffImages_AspNetUsers_UploadedById",
                table: "CaffImages",
                column: "UploadedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
