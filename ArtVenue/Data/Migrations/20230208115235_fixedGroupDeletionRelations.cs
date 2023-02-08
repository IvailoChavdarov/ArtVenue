using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArtVenue.Data.Migrations
{
    public partial class fixedGroupDeletionRelations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Groups_Members_AspNetUsers_MemberId",
                table: "Groups_Members");

            migrationBuilder.DropForeignKey(
                name: "FK_Groups_Members_Groups_GroupId",
                table: "Groups_Members");

            migrationBuilder.DropForeignKey(
                name: "FK_Groups_Requests_AspNetUsers_MemberId",
                table: "Groups_Requests");

            migrationBuilder.DropForeignKey(
                name: "FK_Groups_Requests_Groups_GroupId",
                table: "Groups_Requests");

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_Members_AspNetUsers_MemberId",
                table: "Groups_Members",
                column: "MemberId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_Members_Groups_GroupId",
                table: "Groups_Members",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_Requests_AspNetUsers_MemberId",
                table: "Groups_Requests",
                column: "MemberId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_Requests_Groups_GroupId",
                table: "Groups_Requests",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Groups_Members_AspNetUsers_MemberId",
                table: "Groups_Members");

            migrationBuilder.DropForeignKey(
                name: "FK_Groups_Members_Groups_GroupId",
                table: "Groups_Members");

            migrationBuilder.DropForeignKey(
                name: "FK_Groups_Requests_AspNetUsers_MemberId",
                table: "Groups_Requests");

            migrationBuilder.DropForeignKey(
                name: "FK_Groups_Requests_Groups_GroupId",
                table: "Groups_Requests");

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_Members_AspNetUsers_MemberId",
                table: "Groups_Members",
                column: "MemberId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_Members_Groups_GroupId",
                table: "Groups_Members",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_Requests_AspNetUsers_MemberId",
                table: "Groups_Requests",
                column: "MemberId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_Requests_Groups_GroupId",
                table: "Groups_Requests",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id");
        }
    }
}
