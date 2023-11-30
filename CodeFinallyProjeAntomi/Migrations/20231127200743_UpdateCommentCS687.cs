using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CodeFinallyProjeAntomi.Migrations
{
    public partial class UpdateCommentCS687 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CommentText",
                table: "Comments",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CommentText",
                table: "Comments");
        }
    }
}
