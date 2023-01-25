using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArtVenue.Data.Migrations
{
    public partial class FixedPublicationCategoriesDeletion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Publications_Categories_Publications_PublicationId",
                table: "Publications_Categories");

            migrationBuilder.AddForeignKey(
                name: "FK_Publications_Categories_Publications_PublicationId",
                table: "Publications_Categories",
                column: "PublicationId",
                principalTable: "Publications",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Publications_Categories_Publications_PublicationId",
                table: "Publications_Categories");

            migrationBuilder.AddForeignKey(
                name: "FK_Publications_Categories_Publications_PublicationId",
                table: "Publications_Categories",
                column: "PublicationId",
                principalTable: "Publications",
                principalColumn: "Id");
        }
    }
}
