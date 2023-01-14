using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArtVenue.Data.Migrations
{
    public partial class FixedDirectChatsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_AspNetUsers_RecieverId",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Messages_RecieverId",
                table: "Messages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DirectChats",
                table: "DirectChats");

            migrationBuilder.DropColumn(
                name: "RecieverId",
                table: "Messages");

            migrationBuilder.AddColumn<int>(
                name: "DirectChatId",
                table: "Messages",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "DirectChats",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DirectChats",
                table: "DirectChats",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_DirectChatId",
                table: "Messages",
                column: "DirectChatId");

            migrationBuilder.CreateIndex(
                name: "IX_DirectChats_FirstUserId",
                table: "DirectChats",
                column: "FirstUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_DirectChats_DirectChatId",
                table: "Messages",
                column: "DirectChatId",
                principalTable: "DirectChats",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_DirectChats_DirectChatId",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Messages_DirectChatId",
                table: "Messages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DirectChats",
                table: "DirectChats");

            migrationBuilder.DropIndex(
                name: "IX_DirectChats_FirstUserId",
                table: "DirectChats");

            migrationBuilder.DropColumn(
                name: "DirectChatId",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "DirectChats");

            migrationBuilder.AddColumn<string>(
                name: "RecieverId",
                table: "Messages",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_DirectChats",
                table: "DirectChats",
                columns: new[] { "FirstUserId", "SecondUserId" });

            migrationBuilder.CreateIndex(
                name: "IX_Messages_RecieverId",
                table: "Messages",
                column: "RecieverId");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_AspNetUsers_RecieverId",
                table: "Messages",
                column: "RecieverId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
