using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlightManagementCompany_LINQ_EFCore.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Aircraft",
                columns: table => new
                {
                    AircraftId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TailNumber = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Model = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Capacity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Aircraft", x => x.AircraftId);
                });

            migrationBuilder.CreateTable(
                name: "Airports",
                columns: table => new
                {
                    AirportId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IATA = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    City = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Country = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TimeZone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Airports", x => x.AirportId);
                });

            migrationBuilder.CreateTable(
                name: "CrewMembers",
                columns: table => new
                {
                    CrewId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Fname = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Lname = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Role = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    LicenseNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CrewMembers", x => x.CrewId);
                    table.CheckConstraint("CK_CrewMember_Role", "[Role] IN ('Pilot','CoPilot','FlightAttendant')");
                });

            migrationBuilder.CreateTable(
                name: "Passengers",
                columns: table => new
                {
                    PassengerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Fname = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Lname = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PassportNo = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    Nationality = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    DOB = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Passengers", x => x.PassengerId);
                });

            migrationBuilder.CreateTable(
                name: "AircraftMaintenances",
                columns: table => new
                {
                    MaintenanceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaintenanceDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AircraftId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AircraftMaintenances", x => x.MaintenanceId);
                    table.ForeignKey(
                        name: "FK_AircraftMaintenances_Aircraft_AircraftId",
                        column: x => x.AircraftId,
                        principalTable: "Aircraft",
                        principalColumn: "AircraftId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Routes",
                columns: table => new
                {
                    RouteId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DistanceKm = table.Column<int>(type: "int", nullable: false),
                    OriginAirportId = table.Column<int>(type: "int", nullable: false),
                    DestinationAirportId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Routes", x => x.RouteId);
                    table.ForeignKey(
                        name: "FK_Routes_Airports_DestinationAirportId",
                        column: x => x.DestinationAirportId,
                        principalTable: "Airports",
                        principalColumn: "AirportId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Routes_Airports_OriginAirportId",
                        column: x => x.OriginAirportId,
                        principalTable: "Airports",
                        principalColumn: "AirportId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Bookings",
                columns: table => new
                {
                    BookingId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookingRef = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    BookingDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PassengerId = table.Column<int>(type: "int", nullable: false),
                    PassengerId1 = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bookings", x => x.BookingId);
                    table.CheckConstraint("CK_Booking_Status", "[Status] IN ('Confirmed','Cancelled','Pending')");
                    table.ForeignKey(
                        name: "FK_Bookings_Passengers_PassengerId",
                        column: x => x.PassengerId,
                        principalTable: "Passengers",
                        principalColumn: "PassengerId",
                        onDelete: ReferentialAction.Cascade);
                    
                });

            migrationBuilder.CreateTable(
                name: "Flights",
                columns: table => new
                {
                    FlightId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FlightNumber = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    DepartureUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ArrivalUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    RouteId = table.Column<int>(type: "int", nullable: false),
                    AircraftId = table.Column<int>(type: "int", nullable: false),
                    AircraftId1 = table.Column<int>(type: "int", nullable: false),
                    RouteId1 = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Flights", x => x.FlightId);
                    table.CheckConstraint("CK_Flight_Status", "[Status] IN ('Scheduled','Delayed','Cancelled','Completed')");
                    table.ForeignKey(
                        name: "FK_Flights_Aircraft_AircraftId",
                        column: x => x.AircraftId,
                        principalTable: "Aircraft",
                        principalColumn: "AircraftId",
                        onDelete: ReferentialAction.Restrict);
                    
                    table.ForeignKey(
                        name: "FK_Flights_Routes_RouteId",
                        column: x => x.RouteId,
                        principalTable: "Routes",
                        principalColumn: "RouteId",
                        onDelete: ReferentialAction.Restrict);
                    
                });

            migrationBuilder.CreateTable(
                name: "FlightCrews",
                columns: table => new
                {
                    CrewId = table.Column<int>(type: "int", nullable: false),
                    FlightId = table.Column<int>(type: "int", nullable: false),
                    RoleOnFlight = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FlightId1 = table.Column<int>(type: "int", nullable: false),
                    CrewMemberCrewId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlightCrews", x => new { x.FlightId, x.CrewId });
                    table.CheckConstraint("CK_FlightCrew_RoleOnFlight", "[RoleOnFlight] IN ('Pilot','CoPilot','FlightAttendant')");
                    table.ForeignKey(
                        name: "FK_FlightCrews_CrewMembers_CrewId",
                        column: x => x.CrewId,
                        principalTable: "CrewMembers",
                        principalColumn: "CrewId",
                        onDelete: ReferentialAction.Cascade);
                    
                    table.ForeignKey(
                        name: "FK_FlightCrews_Flights_FlightId",
                        column: x => x.FlightId,
                        principalTable: "Flights",
                        principalColumn: "FlightId",
                        onDelete: ReferentialAction.Cascade);
                    
                });

            migrationBuilder.CreateTable(
                name: "Tickets",
                columns: table => new
                {
                    TicketId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SeatNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Fare = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CheckedIn = table.Column<bool>(type: "bit", nullable: false),
                    BookingId = table.Column<int>(type: "int", nullable: false),
                    FlightId = table.Column<int>(type: "int", nullable: false),
                    BookingId1 = table.Column<int>(type: "int", nullable: false),
                    FlightId1 = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tickets", x => x.TicketId);
                    table.CheckConstraint("CK_Ticket_CheckedIn", "[CheckedIn] IN (0,1)");
                    table.ForeignKey(
                        name: "FK_Tickets_Bookings_BookingId",
                        column: x => x.BookingId,
                        principalTable: "Bookings",
                        principalColumn: "BookingId",
                        onDelete: ReferentialAction.Cascade);
                   
                    table.ForeignKey(
                        name: "FK_Tickets_Flights_FlightId",
                        column: x => x.FlightId,
                        principalTable: "Flights",
                        principalColumn: "FlightId",
                        onDelete: ReferentialAction.Restrict);
                    
                });

            migrationBuilder.CreateTable(
                name: "Baggages",
                columns: table => new
                {
                    BaggageId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WeightKg = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TagNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TicketId = table.Column<int>(type: "int", nullable: false),
                    TicketId1 = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Baggages", x => x.BaggageId);
                    table.ForeignKey(
                        name: "FK_Baggages_Tickets_TicketId",
                        column: x => x.TicketId,
                        principalTable: "Tickets",
                        principalColumn: "TicketId",
                        onDelete: ReferentialAction.Cascade);
                    
                });

            migrationBuilder.CreateIndex(
                name: "IX_Aircraft_TailNumber",
                table: "Aircraft",
                column: "TailNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AircraftMaintenances_AircraftId",
                table: "AircraftMaintenances",
                column: "AircraftId");

            migrationBuilder.CreateIndex(
                name: "IX_Airports_IATA",
                table: "Airports",
                column: "IATA",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Baggages_TicketId",
                table: "Baggages",
                column: "TicketId");

            

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_BookingRef",
                table: "Bookings",
                column: "BookingRef",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_PassengerId",
                table: "Bookings",
                column: "PassengerId");

          
            migrationBuilder.CreateIndex(
                name: "IX_FlightCrews_CrewId",
                table: "FlightCrews",
                column: "CrewId");

            migrationBuilder.CreateIndex(
                name: "IX_FlightCrews_CrewMemberCrewId",
                table: "FlightCrews",
                column: "CrewMemberCrewId");

            migrationBuilder.CreateIndex(
                name: "IX_FlightCrews_FlightId",
                table: "FlightCrews",
                column: "FlightId");

           

            migrationBuilder.CreateIndex(
                name: "IX_Flights_AircraftId",
                table: "Flights",
                column: "AircraftId");

           

            migrationBuilder.CreateIndex(
                name: "IX_Flights_RouteId",
                table: "Flights",
                column: "RouteId");

            

            migrationBuilder.CreateIndex(
                name: "IX_Passengers_PassportNo",
                table: "Passengers",
                column: "PassportNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Routes_DestinationAirportId",
                table: "Routes",
                column: "DestinationAirportId");

            migrationBuilder.CreateIndex(
                name: "IX_Routes_OriginAirportId",
                table: "Routes",
                column: "OriginAirportId");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_BookingId",
                table: "Tickets",
                column: "BookingId");


            migrationBuilder.CreateIndex(
                name: "IX_Tickets_FlightId",
                table: "Tickets",
                column: "FlightId");

          
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AircraftMaintenances");

            migrationBuilder.DropTable(
                name: "Baggages");

            migrationBuilder.DropTable(
                name: "FlightCrews");

            migrationBuilder.DropTable(
                name: "Tickets");

            migrationBuilder.DropTable(
                name: "CrewMembers");

            migrationBuilder.DropTable(
                name: "Bookings");

            migrationBuilder.DropTable(
                name: "Flights");

            migrationBuilder.DropTable(
                name: "Passengers");

            migrationBuilder.DropTable(
                name: "Aircraft");

            migrationBuilder.DropTable(
                name: "Routes");

            migrationBuilder.DropTable(
                name: "Airports");
        }
    }
}
