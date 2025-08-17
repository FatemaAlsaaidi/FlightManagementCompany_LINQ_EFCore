using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlightManagementCompany_LINQ_EFCore.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDeleteBehaviorOFTiketInBaggage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Baggages_Tickets_TicketId",
                table: "Baggages");

            migrationBuilder.AddForeignKey(
                name: "FK_Baggages_Tickets_TicketId",
                table: "Baggages",
                column: "TicketId",
                principalTable: "Tickets",
                principalColumn: "TicketId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Baggages_Tickets_TicketId",
                table: "Baggages");

            migrationBuilder.AddForeignKey(
                name: "FK_Baggages_Tickets_TicketId",
                table: "Baggages",
                column: "TicketId",
                principalTable: "Tickets",
                principalColumn: "TicketId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
