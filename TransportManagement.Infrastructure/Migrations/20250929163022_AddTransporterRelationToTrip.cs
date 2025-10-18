using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TransportManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTransporterRelationToTrip : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TransporterId",
                table: "Trips",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Trips_TransporterId",
                table: "Trips",
                column: "TransporterId");

            migrationBuilder.AddForeignKey(
                name: "FK_Trips_Transporters_TransporterId",
                table: "Trips",
                column: "TransporterId",
                principalTable: "Transporters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Trips_Transporters_TransporterId",
                table: "Trips");

            migrationBuilder.DropIndex(
                name: "IX_Trips_TransporterId",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "TransporterId",
                table: "Trips");
        }
    }
}
