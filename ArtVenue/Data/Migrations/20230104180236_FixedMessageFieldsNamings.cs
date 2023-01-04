using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArtVenue.Data.Migrations
{
    public partial class FixedMessageFieldsNamings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Reciever_Id",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "Send_Time",
                table: "Messages");

            migrationBuilder.RenameColumn(
                name: "Sender_Id",
                table: "Messages",
                newName: "SendTime");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SendTime",
                table: "Messages",
                newName: "Sender_Id");

            migrationBuilder.AddColumn<string>(
                name: "Reciever_Id",
                table: "Messages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Send_Time",
                table: "Messages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
