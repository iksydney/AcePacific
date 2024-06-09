using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AcePacific.Data.Migrations
{
    public partial class Plumbing : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccountName",
                table: "AdminPendingTransactions");

            migrationBuilder.DropColumn(
                name: "AccountNumber",
                table: "AdminPendingTransactions");

            migrationBuilder.RenameColumn(
                name: "SendersName",
                table: "AdminPendingTransactions",
                newName: "ToAccountName");

            migrationBuilder.RenameColumn(
                name: "SendersEmail",
                table: "AdminPendingTransactions",
                newName: "FromAccountName");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreated",
                table: "AdminPendingTransactions",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "TransactionIdPending",
                table: "AdminPendingTransactions",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateCreated",
                table: "AdminPendingTransactions");

            migrationBuilder.DropColumn(
                name: "TransactionIdPending",
                table: "AdminPendingTransactions");

            migrationBuilder.RenameColumn(
                name: "ToAccountName",
                table: "AdminPendingTransactions",
                newName: "SendersName");

            migrationBuilder.RenameColumn(
                name: "FromAccountName",
                table: "AdminPendingTransactions",
                newName: "SendersEmail");

            migrationBuilder.AddColumn<string>(
                name: "AccountName",
                table: "AdminPendingTransactions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AccountNumber",
                table: "AdminPendingTransactions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
