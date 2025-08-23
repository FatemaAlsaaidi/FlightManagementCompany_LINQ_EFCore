using System;
using System.Collections.Generic;
using System.Linq;
using FlightManagementCompany_LINQ_EFCore.Models;
using FlightManagementCompany_LINQ_EFCore.Repositories;

namespace FlightManagementCompany_LINQ_EFCore.SeedData
{
    public static class DatabaseSeeder
    {
        public static void CreateSampleData(
            FlightDatabaseContext db,
            IAirportRepo airportRepo,
            IRouteRepo routeRepo,
            IAircraftRepo aircraftRepo,
            IFlightRepo flightRepo,
            IPassengerRepo passengerRepo,
            IBookingRepo bookingRepo,
            ITicketRepo ticketRepo,
            IBaggageRepo baggageRepo,
            ICrewMemberRepo crewRepo,
            IFlightCrewRepo flightCrewRepo,
            IAircraftMaintenanceRepo maintenanceRepo
        )
        {
            // ===== AIRPORTS =====
            if (!airportRepo.GetAllAirports().Any())
            {
                var airports = new List<Airport>
                {
                    new Airport { IATA = "MCT", Name = "Muscat Intl", City = "Muscat", Country = "Oman", TimeZone = "Asia/Muscat" },
                    new Airport { IATA = "DXB", Name = "Dubai Intl", City = "Dubai", Country = "UAE", TimeZone = "Asia/Dubai" },
                    new Airport { IATA = "DOH", Name = "Hamad Intl", City = "Doha", Country = "Qatar", TimeZone = "Asia/Qatar" },
                    new Airport { IATA = "LHR", Name = "Heathrow", City = "London", Country = "UK", TimeZone = "Europe/London" },
                    new Airport { IATA = "JFK", Name = "John F. Kennedy", City = "New York", Country = "USA", TimeZone = "America/New_York" },
                    new Airport { IATA = "CDG", Name = "Charles de Gaulle", City = "Paris", Country = "France", TimeZone = "Europe/Paris" },
                    new Airport { IATA = "FRA", Name = "Frankfurt Main", City = "Frankfurt", Country = "Germany", TimeZone = "Europe/Berlin" },
                    new Airport { IATA = "IST", Name = "Istanbul Airport", City = "Istanbul", Country = "Turkey", TimeZone = "Europe/Istanbul" },
                    new Airport { IATA = "DEL", Name = "Indira Gandhi Intl", City = "Delhi", Country = "India", TimeZone = "Asia/Kolkata" },
                    new Airport { IATA = "SIN", Name = "Changi Airport", City = "Singapore", Country = "Singapore", TimeZone = "Asia/Singapore" }
                };
                foreach (var a in airports) airportRepo.AddAirport(a);
                db.SaveChanges();
            }

            // ===== ROUTES =====
            if (!routeRepo.GetAllRoutes().Any())
            {
                var airports = db.Airports.ToList();
                var routes = new List<Route>
                {
                    new Route { OriginAirportId = airports[0].AirportId, DestinationAirportId = airports[1].AirportId, DistanceKm = 414 },
                    new Route { OriginAirportId = airports[0].AirportId, DestinationAirportId = airports[3].AirportId, DistanceKm = 5820 },
                    new Route { OriginAirportId = airports[1].AirportId, DestinationAirportId = airports[4].AirportId, DistanceKm = 11000 },
                    new Route { OriginAirportId = airports[2].AirportId, DestinationAirportId = airports[5].AirportId, DistanceKm = 5000 },
                    new Route { OriginAirportId = airports[3].AirportId, DestinationAirportId = airports[6].AirportId, DistanceKm = 650 },
                    new Route { OriginAirportId = airports[4].AirportId, DestinationAirportId = airports[7].AirportId, DistanceKm = 9500 },
                    new Route { OriginAirportId = airports[5].AirportId, DestinationAirportId = airports[8].AirportId, DistanceKm = 7000 },
                    new Route { OriginAirportId = airports[6].AirportId, DestinationAirportId = airports[9].AirportId, DistanceKm = 8700 },
                    new Route { OriginAirportId = airports[7].AirportId, DestinationAirportId = airports[2].AirportId, DistanceKm = 3000 },
                    new Route { OriginAirportId = airports[8].AirportId, DestinationAirportId = airports[0].AirportId, DistanceKm = 2400 }
                };
                foreach (var r in routes) routeRepo.AddRoute(r);
                db.SaveChanges();
            }

            // ===== AIRCRAFT =====
            if (!aircraftRepo.GetAllAircrafts().Any())
            {
                var aircrafts = new List<Aircraft>
                {
                    new Aircraft { TailNumber = "A40-OM1",  Model = "Boeing 737",       Capacity = 160 },
                    new Aircraft { TailNumber = "A40-OM2",  Model = "Airbus A320",      Capacity = 180 },
                    new Aircraft { TailNumber = "A40-OM3",  Model = "Boeing 787",       Capacity = 250 },
                    new Aircraft { TailNumber = "A40-OM4",  Model = "Airbus A330",      Capacity = 290 },
                    new Aircraft { TailNumber = "A40-OM5",  Model = "Boeing 747",       Capacity = 400 },
                    new Aircraft { TailNumber = "A40-OM6",  Model = "Airbus A350",      Capacity = 350 },
                    new Aircraft { TailNumber = "A40-OM7",  Model = "Embraer 190",      Capacity = 100 },
                    new Aircraft { TailNumber = "A40-OM8",  Model = "Boeing 777",       Capacity = 370 },
                    new Aircraft { TailNumber = "A40-OM9",  Model = "ATR 72",           Capacity = 70  },
                    new Aircraft { TailNumber = "A40-OM10", Model = "Bombardier Q400", Capacity = 78  }
                };
                foreach (var ac in aircrafts) aircraftRepo.AddAircraft(ac);
                db.SaveChanges();
            }

            // ===== FLIGHTS =====
            if (!flightRepo.GetAllFlights().Any())
            {
                var routes = db.Routes.ToList();
                var aircrafts = db.Aircraft.ToList();

                var flights = new List<Flight>
                {
                    new Flight { FlightNumber = "WY101", DepartureUtc = DateTime.UtcNow.AddHours(2),  ArrivalUtc = DateTime.UtcNow.AddHours(5), ActualArrivalUtc = DateTime.UtcNow.AddHours(15) ,Status = "Scheduled", RouteId = routes[0].RouteId, AircraftId = aircrafts[0].AircraftId },
                    new Flight { FlightNumber = "WY102", DepartureUtc = DateTime.UtcNow.AddHours(4),  ArrivalUtc = DateTime.UtcNow.AddHours(7),ActualArrivalUtc= DateTime.UtcNow.AddHours(7) ,Status = "Scheduled", RouteId = routes[1].RouteId, AircraftId = aircrafts[1].AircraftId },
                    new Flight { FlightNumber = "WY103", DepartureUtc = DateTime.UtcNow.AddHours(6),  ArrivalUtc = DateTime.UtcNow.AddHours(11), ActualArrivalUtc = DateTime.UtcNow.AddHours(13)  ,Status = "Delayed",   RouteId = routes[2].RouteId, AircraftId = aircrafts[2].AircraftId },
                    new Flight { FlightNumber = "WY104", DepartureUtc = DateTime.UtcNow.AddHours(8),  ArrivalUtc = DateTime.UtcNow.AddHours(13), ActualArrivalUtc = DateTime.UtcNow.AddHours(13) ,Status = "Delayed",   RouteId = routes[3].RouteId, AircraftId = aircrafts[3].AircraftId },
                    new Flight { FlightNumber = "WY105", DepartureUtc = DateTime.UtcNow.AddHours(10), ArrivalUtc = DateTime.UtcNow.AddHours(12),ActualArrivalUtc=  DateTime.UtcNow.AddHours(20) ,Status = "Cancelled", RouteId = routes[4].RouteId, AircraftId = aircrafts[4].AircraftId },
                    new Flight { FlightNumber = "WY106", DepartureUtc = DateTime.UtcNow.AddHours(12), ArrivalUtc = DateTime.UtcNow.AddHours(15),ActualArrivalUtc= DateTime.UtcNow.AddHours(15) ,Status = "Scheduled", RouteId = routes[5].RouteId, AircraftId = aircrafts[5].AircraftId },
                    new Flight { FlightNumber = "WY107", DepartureUtc = DateTime.UtcNow.AddHours(14), ArrivalUtc = DateTime.UtcNow.AddHours(16), ActualArrivalUtc= DateTime.UtcNow.AddHours(16) ,Status = "Cancelled", RouteId = routes[6].RouteId, AircraftId = aircrafts[6].AircraftId },
                    new Flight { FlightNumber = "WY108", DepartureUtc = DateTime.UtcNow.AddHours(16), ArrivalUtc = DateTime.UtcNow.AddHours(22),ActualArrivalUtc= DateTime.UtcNow.AddHours(30),Status = "Scheduled", RouteId = routes[7].RouteId, AircraftId = aircrafts[7].AircraftId },
                    new Flight { FlightNumber = "WY109", DepartureUtc = DateTime.UtcNow.AddHours(18), ArrivalUtc = DateTime.UtcNow.AddHours(20),ActualArrivalUtc= DateTime.UtcNow.AddHours(20) ,Status = "Delayed",   RouteId = routes[8].RouteId, AircraftId = aircrafts[8].AircraftId },
                    new Flight { FlightNumber = "WY110", DepartureUtc = DateTime.UtcNow.AddHours(20), ArrivalUtc = DateTime.UtcNow.AddHours(23),ActualArrivalUtc= DateTime.UtcNow.AddHours(23) ,Status = "Cancelled", RouteId = routes[9].RouteId, AircraftId = aircrafts[9].AircraftId }
                };
                foreach (var f in flights) flightRepo.AddFlight(f);
                db.SaveChanges();
            }

            // ===== PASSENGERS =====
            if (!passengerRepo.GetAllPassengers().Any())
            {
                var passengers = new List<Passenger>
                {
                    new Passenger { Fname = "Fatema",  Lname= "Hamed",   PassportNo = "A1234567", Nationality = "Omani",      DOB = new DateOnly(2000,5,20) },
                    new Passenger { Fname = "John",    Lname ="Smith",   PassportNo = "B7654321", Nationality = "British",    DOB = new DateOnly(1995,3,10) },
                    new Passenger { Fname = "Emma",    Lname ="Johnson", PassportNo = "C1112233", Nationality = "American",   DOB = new DateOnly(1990,8,15) },
                    new Passenger { Fname = "Ali",     Lname="Khan",     PassportNo = "D2223344", Nationality = "Pakistani",  DOB = new DateOnly(1985,1,5)  },
                    new Passenger { Fname = "Sophia",  Lname ="Lee",     PassportNo = "E3334455", Nationality = "Singaporean",DOB = new DateOnly(1998,7,12) },
                    new Passenger { Fname = "Carlos",  Lname="Garcia",   PassportNo = "F4445566", Nationality = "Spanish",    DOB = new DateOnly(1987,2,23) },
                    new Passenger { Fname = "Fatima",  Lname="Alsaaidi", PassportNo = "G5556677", Nationality = "Moroccan",   DOB = new DateOnly(1993,11,30)},
                    new Passenger { Fname = "David",   Lname="Brown",    PassportNo = "H6667788", Nationality = "Canadian",   DOB = new DateOnly(1980,4,2)  },
                    new Passenger { Fname = "Maria",   Lname ="Rossi",   PassportNo = "I7778899", Nationality = "Italian",    DOB = new DateOnly(1992,9,19) },
                    new Passenger { Fname = "Chen",    Lname="Wei",      PassportNo = "J8889900", Nationality = "Chinese",    DOB = new DateOnly(1996,12,25)}
                };
                foreach (var p in passengers) passengerRepo.AddPassenger(p);
                db.SaveChanges();
            }

            // ===== BOOKINGS =====
            if (!bookingRepo.GetAllBookings().Any())
            {
                var passengers = db.Passengers.ToList();
                var bookings = new List<Booking>
                {
                    new Booking { BookingRef = "BK001", BookingDate = DateTime.UtcNow, status = "Confirmed", PassengerId = passengers[0].PassengerId },
                    new Booking { BookingRef = "BK002", BookingDate = DateTime.UtcNow, status = "Pending",   PassengerId = passengers[1].PassengerId },
                    new Booking { BookingRef = "BK003", BookingDate = DateTime.UtcNow, status = "Confirmed", PassengerId = passengers[2].PassengerId },
                    new Booking { BookingRef = "BK004", BookingDate = DateTime.UtcNow, status = "Cancelled", PassengerId = passengers[3].PassengerId },
                    new Booking { BookingRef = "BK005", BookingDate = DateTime.UtcNow, status = "Confirmed", PassengerId = passengers[4].PassengerId },
                    new Booking { BookingRef = "BK006", BookingDate = DateTime.UtcNow, status = "Confirmed", PassengerId = passengers[5].PassengerId },
                    new Booking { BookingRef = "BK007", BookingDate = DateTime.UtcNow, status = "Pending",   PassengerId = passengers[6].PassengerId },
                    new Booking { BookingRef = "BK008", BookingDate = DateTime.UtcNow, status = "Confirmed", PassengerId = passengers[7].PassengerId },
                    new Booking { BookingRef = "BK009", BookingDate = DateTime.UtcNow, status = "Confirmed", PassengerId = passengers[8].PassengerId },
                    new Booking { BookingRef = "BK010", BookingDate = DateTime.UtcNow, status = "Cancelled", PassengerId = passengers[9].PassengerId }
                };
                foreach (var b in bookings) bookingRepo.AddBooking(b);
                db.SaveChanges();
            }

            // ===== TICKETS =====
            if (!ticketRepo.GetAllTickets().Any())
            {
                var bookings = db.Bookings.ToList();
                var flights = db.Flights.ToList();

                var tickets = new List<Ticket>
                {
                    new Ticket { SeatNumber = "12A", Fare = 150m, CheckedIn = false, BookingId = bookings[0].BookingId, FlightId = flights[0].FlightId },
                    new Ticket { SeatNumber = "14C", Fare = 200m, CheckedIn = true,  BookingId = bookings[1].BookingId, FlightId = flights[1].FlightId },
                    new Ticket { SeatNumber = "15B", Fare = 300m, CheckedIn = false, BookingId = bookings[2].BookingId, FlightId = flights[2].FlightId },
                    new Ticket { SeatNumber = "16D", Fare = 180m, CheckedIn = false, BookingId = bookings[3].BookingId, FlightId = flights[3].FlightId },
                    new Ticket { SeatNumber = "17A", Fare = 220m, CheckedIn = true,  BookingId = bookings[4].BookingId, FlightId = flights[4].FlightId },
                    new Ticket { SeatNumber = "18B", Fare = 270m, CheckedIn = false, BookingId = bookings[5].BookingId, FlightId = flights[5].FlightId },
                    new Ticket { SeatNumber = "19C", Fare = 190m, CheckedIn = true,  BookingId = bookings[6].BookingId, FlightId = flights[6].FlightId },
                    new Ticket { SeatNumber = "20D", Fare = 250m, CheckedIn = false, BookingId = bookings[7].BookingId, FlightId = flights[7].FlightId },
                    new Ticket { SeatNumber = "21A", Fare = 230m, CheckedIn = false, BookingId = bookings[8].BookingId, FlightId = flights[8].FlightId },
                    new Ticket { SeatNumber = "22B", Fare = 300m, CheckedIn = true,  BookingId = bookings[9].BookingId, FlightId = flights[9].FlightId }
                };
                foreach (var t in tickets) ticketRepo.AddTicket(t);
                db.SaveChanges();
            }

            // ===== BAGGAGE =====
            if (!baggageRepo.GetAllBaggages().Any())
            {
                var tickets = db.Tickets.ToList();
                var baggages = new List<Baggage>
                {
                    new Baggage { TicketId = tickets[0].TicketId, WeightKg = 20.5m, TagNumber = "BG001" },
                    new Baggage { TicketId = tickets[1].TicketId, WeightKg = 18.0m, TagNumber = "BG002" },
                    new Baggage { TicketId = tickets[2].TicketId, WeightKg = 22.3m, TagNumber = "BG003" },
                    new Baggage { TicketId = tickets[3].TicketId, WeightKg = 19.0m, TagNumber = "BG004" },
                    new Baggage { TicketId = tickets[4].TicketId, WeightKg = 23.5m, TagNumber = "BG005" },
                    new Baggage { TicketId = tickets[5].TicketId, WeightKg = 21.2m, TagNumber = "BG006" },
                    new Baggage { TicketId = tickets[6].TicketId, WeightKg = 20.0m, TagNumber = "BG007" },
                    new Baggage { TicketId = tickets[7].TicketId, WeightKg = 24.0m, TagNumber = "BG008" },
                    new Baggage { TicketId = tickets[8].TicketId, WeightKg = 22.5m, TagNumber = "BG009" },
                    new Baggage { TicketId = tickets[9].TicketId, WeightKg = 19.8m, TagNumber = "BG010" }
                };
                foreach (var bg in baggages) baggageRepo.AddBaggage(bg);
                db.SaveChanges();
            }

            // ===== CREW MEMBERS =====
            if (!crewRepo.GetAllCrewMembers().Any())
            {
                var crew = new List<CrewMember>
                {
                    new CrewMember { Fname = "Captain",       Lname="Ahmed",     Role = CrewRole.Pilot,           LicenseNo = "123" },
                    new CrewMember { Fname = "First Officer", Lname="Sara",      Role = CrewRole.CoPilot,         LicenseNo = "123" },
                    new CrewMember { Fname = "John",          Lname="Doe",       Role = CrewRole.FlightAttendant, LicenseNo = "123" },
                    new CrewMember { Fname = "Jane",          Lname="Smith",     Role = CrewRole.FlightAttendant, LicenseNo = "123" },
                    new CrewMember { Fname = "Ali",           Lname="Hassan",    Role = CrewRole.FlightAttendant, LicenseNo = "123" },
                    new CrewMember { Fname = "Fatima",        Lname="Al Balushi",Role = CrewRole.FlightAttendant, LicenseNo = "123" },
                    new CrewMember { Fname = "David",         Lname="Clark",     Role = CrewRole.CoPilot,         LicenseNo = "123" },
                    new CrewMember { Fname = "Emma",          Lname="Watson",    Role = CrewRole.Pilot,           LicenseNo = "123" },
                    new CrewMember { Fname = "Mohammed",      Lname="Saleh",     Role = CrewRole.FlightAttendant, LicenseNo = "123" },
                    new CrewMember { Fname = "Aisha",         Lname="Al Amri",   Role = CrewRole.FlightAttendant, LicenseNo = "123" }
                };
                foreach (var c in crew) crewRepo.AddCrewMember(c);
                db.SaveChanges();
            }

            // ===== FLIGHT CREW ASSIGNMENTS =====
            if (!flightCrewRepo.GetAllFlightCrews().Any())
            {
                var flights = db.Flights.ToList();
                var crew = db.CrewMembers.ToList();

                var assignments = new List<FlightCrew>
                {
                    new FlightCrew { FlightId = flights[0].FlightId, CrewId = crew[0].CrewId, RoleOnFlight = "Pilot" },
                    new FlightCrew { FlightId = flights[0].FlightId, CrewId = crew[1].CrewId, RoleOnFlight = "CoPilot" },
                    new FlightCrew { FlightId = flights[0].FlightId, CrewId = crew[2].CrewId, RoleOnFlight = "FlightAttendant" },
                    new FlightCrew { FlightId = flights[1].FlightId, CrewId = crew[3].CrewId, RoleOnFlight = "FlightAttendant" },
                    new FlightCrew { FlightId = flights[1].FlightId, CrewId = crew[4].CrewId, RoleOnFlight = "FlightAttendant" },
                    new FlightCrew { FlightId = flights[2].FlightId, CrewId = crew[5].CrewId, RoleOnFlight = "FlightAttendant" },
                    new FlightCrew { FlightId = flights[2].FlightId, CrewId = crew[6].CrewId, RoleOnFlight = "CoPilot" },
                    new FlightCrew { FlightId = flights[3].FlightId, CrewId = crew[7].CrewId, RoleOnFlight = "Pilot" },
                    new FlightCrew { FlightId = flights[4].FlightId, CrewId = crew[8].CrewId, RoleOnFlight = "FlightAttendant" },
                    new FlightCrew { FlightId = flights[5].FlightId, CrewId = crew[9].CrewId, RoleOnFlight = "FlightAttendant" }
                };
                foreach (var fc in assignments) flightCrewRepo.AddFlightCrew(fc);
                db.SaveChanges();
            }

            // ===== AIRCRAFT MAINTENANCE =====
            if (!maintenanceRepo.GetAllAircrafMaintenances().Any())
            {
                var aircrafts = db.Aircraft.ToList();
                var maintenances = new List<AircraftMaintenance>
                {
                    new AircraftMaintenance { AircraftId = aircrafts[0].AircraftId, MaintenanceDate = DateTime.UtcNow.AddDays(-30), Type = "Engine check",              Note = "Good" },
                    new AircraftMaintenance { AircraftId = aircrafts[1].AircraftId, MaintenanceDate = DateTime.UtcNow.AddDays(-15), Type = "Landing gear inspection",   Note = "Good" },
                    new AircraftMaintenance { AircraftId = aircrafts[2].AircraftId, MaintenanceDate = DateTime.UtcNow.AddDays(-10), Type = "Avionics software update",  Note = "Good" },
                    new AircraftMaintenance { AircraftId = aircrafts[3].AircraftId, MaintenanceDate = DateTime.UtcNow.AddDays(-5),  Type = "Cabin pressure test",       Note = "Good" },
                    new AircraftMaintenance { AircraftId = aircrafts[4].AircraftId, MaintenanceDate = DateTime.UtcNow.AddDays(-25), Type = "Hydraulic system check",    Note = "Good" },
                    new AircraftMaintenance { AircraftId = aircrafts[5].AircraftId, MaintenanceDate = DateTime.UtcNow.AddDays(-20), Type = "Fuel system inspection",    Note = "Good" },
                    new AircraftMaintenance { AircraftId = aircrafts[6].AircraftId, MaintenanceDate = DateTime.UtcNow.AddDays(-18), Type = "Engine oil replacement",    Note = "Good" },
                    new AircraftMaintenance { AircraftId = aircrafts[7].AircraftId, MaintenanceDate = DateTime.UtcNow.AddDays(-12), Type = "Flight control calibration",Note = "Good" },
                    new AircraftMaintenance { AircraftId = aircrafts[8].AircraftId, MaintenanceDate = DateTime.UtcNow.AddDays(-8),  Type = "Propeller inspection",      Note = "Good" },
                    new AircraftMaintenance { AircraftId = aircrafts[9].AircraftId, MaintenanceDate = DateTime.UtcNow.AddDays(-3),  Type = "Emergency exit check",      Note = "Good" }
                };
                foreach (var m in maintenances) maintenanceRepo.AddAircraftMaintenance(m);
                db.SaveChanges();
            }
        }
    }
}
