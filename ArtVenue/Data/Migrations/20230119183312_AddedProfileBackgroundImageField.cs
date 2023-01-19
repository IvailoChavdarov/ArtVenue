using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArtVenue.Data.Migrations
{
    public partial class AddedProfileBackgroundImageField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProfileBackground",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfileBackground",
                table: "AspNetUsers");
        }
    }
}
