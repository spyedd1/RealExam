using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GFLHApp.Data.Migrations
{
    public partial class ContactInquiryReplies : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdminReply",
                table: "ContactInquiries",
                type: "nvarchar(3000)",
                maxLength: 3000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "RepliedAtUtc",
                table: "ContactInquiries",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RepliedByEmail",
                table: "ContactInquiries",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdminReply",
                table: "ContactInquiries");

            migrationBuilder.DropColumn(
                name: "RepliedAtUtc",
                table: "ContactInquiries");

            migrationBuilder.DropColumn(
                name: "RepliedByEmail",
                table: "ContactInquiries");
        }
    }
}
