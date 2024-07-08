using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AcePacific.Data.Migrations
{
    public partial class NullabilityAccount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ToAccountNumber",
                table: "AdminPendingTransactions",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ToAccountNumber",
                table: "AdminPendingTransactions");
        }
    }
}
