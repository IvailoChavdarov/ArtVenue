using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArtVenue.Data.Migrations
{
    public partial class FixedTablesRelationsForDeletion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_AspNetUsers_UserId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Publications_PublicationId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Interests_AspNetUsers_UserId",
                table: "Interests");

            migrationBuilder.DropForeignKey(
                name: "FK_Saved_AspNetUsers_UserId",
                table: "Saved");

            migrationBuilder.DropForeignKey(
                name: "FK_Saved_Publications_PublicationId",
                table: "Saved");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_AspNetUsers_UserId",
                table: "Comments",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Publications_PublicationId",
                table: "Comments",
                column: "PublicationId",
                principalTable: "Publications",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Interests_AspNetUsers_UserId",
                table: "Interests",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Saved_AspNetUsers_UserId",
                table: "Saved",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Saved_Publications_PublicationId",
                table: "Saved",
                column: "PublicationId",
                principalTable: "Publications",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_AspNetUsers_UserId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Publications_PublicationId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Interests_AspNetUsers_UserId",
                table: "Interests");

            migrationBuilder.DropForeignKey(
                name: "FK_Saved_AspNetUsers_UserId",
                table: "Saved");

            migrationBuilder.DropForeignKey(
                name: "FK_Saved_Publications_PublicationId",
                table: "Saved");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_AspNetUsers_UserId",
                table: "Comments",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Publications_PublicationId",
                table: "Comments",
                column: "PublicationId",
                principalTable: "Publications",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Interests_AspNetUsers_UserId",
                table: "Interests",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Saved_AspNetUsers_UserId",
                table: "Saved",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Saved_Publications_PublicationId",
                table: "Saved",
                column: "PublicationId",
                principalTable: "Publications",
                principalColumn: "Id");
        }
    }
}
