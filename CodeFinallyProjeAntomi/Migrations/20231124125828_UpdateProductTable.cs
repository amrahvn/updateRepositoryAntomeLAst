using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CodeFinallyProjeAntomi.Migrations
{
    public partial class UpdateProductTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserRatings_Products_ProductId",
                table: "UserRatings");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "UserRatings",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "UserRatings",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_UserRatings_UserId",
                table: "UserRatings",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserRatings_AspNetUsers_UserId",
                table: "UserRatings",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserRatings_Products_ProductId",
                table: "UserRatings",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserRatings_AspNetUsers_UserId",
                table: "UserRatings");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRatings_Products_ProductId",
                table: "UserRatings");

            migrationBuilder.DropIndex(
                name: "IX_UserRatings_UserId",
                table: "UserRatings");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "UserRatings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "UserRatings",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRatings_Products_ProductId",
                table: "UserRatings",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
