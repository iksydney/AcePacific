using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AcePacific.Data.Migrations
{
    public partial class PropertyAddition : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AccountName",
                table: "TransactionLogs",
                newName: "SenderAccountName");

            migrationBuilder.AddColumn<string>(
                name: "RecipientAccountName",
                table: "TransactionLogs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RecipientAccountName",
                table: "TransactionLogs");

            migrationBuilder.RenameColumn(
                name: "SenderAccountName",
                table: "TransactionLogs",
                newName: "AccountName");
        }
    }
}
