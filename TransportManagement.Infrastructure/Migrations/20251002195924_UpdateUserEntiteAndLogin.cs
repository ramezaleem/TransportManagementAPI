using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TransportManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserEntiteAndLogin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EmailLoginCode",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EmailLoginCodeExpiry",
                table: "Users",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmailLoginCode",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "EmailLoginCodeExpiry",
                table: "Users");
        }
    }
}
