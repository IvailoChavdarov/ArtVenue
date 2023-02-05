using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArtVenue.Data.Migrations
{
    public partial class fixedPublicationModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OutsideLink",
                table: "Publications");

            migrationBuilder.AddColumn<string>(
                name: "PublicationTitle",
                table: "Publications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PublicationTitle",
                table: "Publications");

            migrationBuilder.AddColumn<string>(
                name: "OutsideLink",
                table: "Publications",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
