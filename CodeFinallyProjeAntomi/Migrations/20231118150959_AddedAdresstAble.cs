using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CodeFinallyProjeAntomi.Migrations
{
    public partial class AddedAdresstAble : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PostalCode",
                table: "Address",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PostalCode",
                table: "Address");
        }
    }
}
