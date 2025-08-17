using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlightManagementCompany_LINQ_EFCore.Migrations
{
    /// <inheritdoc />
    public partial class Update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TicketId1",
                table: "Baggages",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Baggages_TicketId1",
                table: "Baggages",
                column: "TicketId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Baggages_Tickets_TicketId1",
                table: "Baggages",
                column: "TicketId1",
                principalTable: "Tickets",
                principalColumn: "TicketId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Baggages_Tickets_TicketId1",
                table: "Baggages");

            migrationBuilder.DropIndex(
                name: "IX_Baggages_TicketId1",
                table: "Baggages");

            migrationBuilder.DropColumn(
                name: "TicketId1",
                table: "Baggages");
        }
    }
}
