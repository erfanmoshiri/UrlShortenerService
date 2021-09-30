using Microsoft.EntityFrameworkCore.Migrations;

namespace UrlService.Migrations
{
    public partial class urlInfomodified : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "UrlInfo",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "UrlInfo",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "UrlInfo");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "UrlInfo");
        }
    }
}
