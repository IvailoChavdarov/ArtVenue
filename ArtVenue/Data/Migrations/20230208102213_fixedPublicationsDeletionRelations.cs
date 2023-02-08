using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArtVenue.Data.Migrations
{
    public partial class fixedPublicationsDeletionRelations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Publications_Groups_GroupId",
                table: "Publications");

            migrationBuilder.AddForeignKey(
                name: "FK_Publications_Groups_GroupId",
                table: "Publications",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Publications_Groups_GroupId",
                table: "Publications");

            migrationBuilder.AddForeignKey(
                name: "FK_Publications_Groups_GroupId",
                table: "Publications",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id");
        }
    }
}
