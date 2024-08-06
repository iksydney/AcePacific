using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AcePacific.Data.Migrations
{
    public partial class recipientCode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "TransactionLogs",
                newName: "SenderUserId");

            migrationBuilder.AddColumn<string>(
                name: "RecipientUserId",
                table: "TransactionLogs",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RecipientUserId",
                table: "TransactionLogs");

            migrationBuilder.RenameColumn(
                name: "SenderUserId",
                table: "TransactionLogs",
                newName: "UserId");
        }
    }
}
