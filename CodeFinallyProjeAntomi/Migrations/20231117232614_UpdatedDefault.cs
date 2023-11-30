using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CodeFinallyProjeAntomi.Migrations
{
    public partial class UpdatedDefault : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDefault",
                table: "Address",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDefault",
                table: "Address");
        }
    }
}
