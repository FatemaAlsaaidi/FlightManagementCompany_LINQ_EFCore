using FlightManagementCompany_LINQ_EFCore.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlightManagementCompany_LINQ_EFCore;

namespace FlightManagementCompany_LINQ_EFCore.SeedData
{
    public static class SeedData
    {
        public static async Task SeedAsync(FlightDatabaseContext db)
        {
            // Idempotent: if Airports exist, assume all seeded
            if (await db.Airports.AnyAsync()) return;

            var rng = new Random(20250815);

            // ---------------------------
            // 1) Airports (>=10)
            // ---------------------------
            var airports = new List<Airport>
            {
                new() { IATA="MCT", Name="Muscat Intl", City="Muscat", Country="Oman", TimeZone="Asia/Muscat"},
                new() { IATA="DXB", Name="Dubai Intl", City="Dubai", Country="UAE", TimeZone="Asia/Dubai"},
                new() { IATA="DOH", Name="Hamad Intl", City="Doha", Country="Qatar", TimeZone="Asia/Qatar"},
                new() { IATA="AUH", Name="Abu Dhabi", City="Abu Dhabi", Country="UAE", TimeZone="Asia/Dubai"},
                new() { IATA="JED", Name="King Abdulaziz", City="Jeddah", Country="Saudi Arabia", TimeZone="Asia/Riyadh"},
                new() { IATA="RUH", Name="King Khalid", City="Riyadh", Country="Saudi Arabia", TimeZone="Asia/Riyadh"},
                new() { IATA="KWI", Name="Kuwait", City="Kuwait City", Country="Kuwait", TimeZone="Asia/Kuwait"},
                new() { IATA="BAH", Name="Bahrain", City="Manama", Country="Bahrain", TimeZone="Asia/Bahrain"},
                new() { IATA="AMM", Name="Queen Alia", City="Amman", Country="Jordan", TimeZone="Asia/Amman"},
                new() { IATA="CAI", Name="Cairo Intl", City="Cairo", Country="Egypt", TimeZone="Africa/Cairo"},
            };
            db.Airports.AddRange(airports);
            await db.SaveChangesAsync();
            var apByCode = airports.ToDictionary(a => a.IATA, a => a.AirportId);
            string[] codes = airports.Select(a => a.IATA).ToArray();

            // ---------------------------
            // 2) Aircraft (>=10; capacity 100–300)
            // ---------------------------
            var aircraft = new List<Aircraft>
            {
                new() { TailNumber="A4O-001", Model="B737-800",  Capacity=180 },
                new() { TailNumber="A4O-002", Model="A320neo",  Capacity=174 },
                new() { TailNumber="A4O-003", Model="A321",     Capacity=200 },
                new() { TailNumber="A4O-004", Model="B787-9",   Capacity=290 },
                new() { TailNumber="A4O-005", Model="A330-300", Capacity=285 },
                new() { TailNumber="A4O-006", Model="B737 MAX", Capacity=186 },
                new() { TailNumber="A4O-007", Model="A320",     Capacity=168 },
                new() { TailNumber="A4O-008", Model="A321neo",  Capacity=215 },
                new() { TailNumber="A4O-009", Model="B787-8",   Capacity=260 },
                new() { TailNumber="A4O-010", Model="A220-300", Capacity=145 },
            };
            db.Aircraft.AddRange(aircraft);
            await db.SaveChangesAsync();

            // ---------------------------
            // 3) Crew Members (>=20)
            // ---------------------------
            var crewSeed = new (string fn, string ln, CrewRole role, string lic)[]
            {
                ("Ali","Hamed",CrewRole.Pilot,"P-OM-1001"),
                ("Omar","Saeed",CrewRole.Pilot,"P-OM-1002"),
                ("Khaled","Musa",CrewRole.Pilot,"P-OM-1003"),
                ("Mariam","Harthi",CrewRole.Pilot,"P-OM-1004"),
                ("Huda","Salem",CrewRole.CoPilot,"CP-OM-2211"),
                ("Sara","Nasser",CrewRole.CoPilot,"CP-OM-2210"),
                ("Yahya","Saleh",CrewRole.CoPilot,"CP-OM-2212"),
                ("Noor","Jabri",CrewRole.CoPilot,"CP-OM-2213"),
                ("Laila","Habib",CrewRole.FlightAttendant,"-"),
                ("Fahad","Amri",CrewRole.FlightAttendant,"-"),
                ("Mona","Khalid",CrewRole.FlightAttendant,"-"),
                ("Yousef","Said",CrewRole.FlightAttendant,"-"),
                ("Reem","Ali",CrewRole.FlightAttendant,"-"),
                ("Rana","Abri",CrewRole.FlightAttendant,"-"),
                ("Aisha","Rahbi",CrewRole.FlightAttendant,"-"),
                ("Hassan","Balushi",CrewRole.FlightAttendant,"-"),
                ("Mahmood","Sultan",CrewRole.FlightAttendant,"-"),
                ("Lina","Amri",CrewRole.FlightAttendant,"-"),
                ("Nasser","Siyabi",CrewRole.FlightAttendant,"-"),
                ("Maha","Lawati",CrewRole.FlightAttendant,"-"),
                ("Saeed","Farsi",CrewRole.FlightAttendant,"-"),
                ("Salim","Hinai",CrewRole.FlightAttendant,"-"),
                ("Yasir","Harthy",CrewRole.FlightAttendant,"-"),
                ("Rashed","Ghafri",CrewRole.FlightAttendant,"-"),
            };
            var crews = crewSeed.Select(c => new CrewMember { Fname = c.fn, Lname = c.ln, Role = c.role, LicenseNo = c.lic }).ToList();
            db.CrewMembers.AddRange(crews);
            await db.SaveChangesAsync();

            // ---------------------------
            // 4) Routes (>=20; allow duplicates of OD pairs)
            // ---------------------------
            var routes = new List<Route>();
            while (routes.Count < 22)
            {
                var o = codes[rng.Next(codes.Length)];
                var d = codes[rng.Next(codes.Length)];
                if (o == d) continue;
                routes.Add(new Route
                {
                    OriginAirportId = apByCode[o],
                    DestinationAirportId = apByCode[d],
                    DistanceKm = 300 + rng.Next(2200) // 300–2500
                });
            }
            db.Routes.AddRange(routes);
            await db.SaveChangesAsync();

            // ---------------------------
            // 5) Flights (>=30 across -30..+60 days; statuses)
            // ---------------------------
            var flights = new List<Flight>();
            DateTime nowUtc = DateTime.UtcNow;
            int flightCount = 32;
            for (int i = 0; i < flightCount; i++)
            {
                var r = routes[rng.Next(routes.Count)];
                var ac = aircraft[rng.Next(aircraft.Count)];

                // Window: from -30 to +60 days, mostly day-time departures
                var dayOffset = rng.Next(-30, 61);
                var dep = nowUtc.Date.AddDays(dayOffset).AddHours(5 + rng.Next(0, 15)); // 05:00–19:59
                var durationHrs = 1.5 + rng.NextDouble() * 5.5; // 1.5–7.0 hours
                var arr = dep.AddHours(durationHrs);

                string status = arr < nowUtc ? "Landed"
                                : (dep <= nowUtc && arr >= nowUtc ? "Departed" : "Scheduled");

                flights.Add(new Flight
                {
                    FlightNumber = $"FM{100 + i}",
                    DepartureUtc = dep,
                    ArrivalUtc = arr,
                    Status = status,
                    RouteId = r.RouteId,
                    AircraftId = ac.AircraftId
                });
            }
            db.Flights.AddRange(flights);
            await db.SaveChangesAsync();

            // Helper: generate all seats for an aircraft capacity as "12A" style
            static List<string> BuildSeatMap(int capacity)
            {
                // Typical narrow/wide-body 6-abreast: A–F
                char[] letters = { 'A', 'B', 'C', 'D', 'E', 'F' };
                int perRow = 6;
                int rows = (int)Math.Ceiling(capacity / (double)perRow);
                var list = new List<string>(rows * perRow);
                for (int row = 1; row <= rows; row++)
                    foreach (var L in letters)
                        list.Add($"{row}{L}");
                return list;
            }

            // Precompute seat maps per aircraftId
            var seatMaps = aircraft.ToDictionary(a => a.AircraftId, a => BuildSeatMap(a.Capacity));

            // ---------------------------
            // 6) Passengers (>=50; mixed nationalities)
            // ---------------------------
            var nationalities = new[] { "OM", "AE", "QA", "SA", "KW", "BH", "JO", "EG", "IN", "PK", "BD", "PH", "UK", "US", "FR", "DE" };
            var pax = new List<Passenger>();
            int paxCount = 54;
            for (int i = 0; i < paxCount; i++)
            {
                string fn = $"PaxFN{i + 1}";
                string ln = $"PaxLN{(char)('A' + (i % 26))}";
                string pass = $"P{(100000 + i)}";
                string nat = nationalities[rng.Next(nationalities.Length)];
                int y = rng.Next(1958, 2012); // 13–67 yrs
                int m = rng.Next(1, 13);
                int d = rng.Next(1, 28);
                pax.Add(new Passenger
                {
                    Fname = fn,
                    Lname = ln,
                    PassportNo = pass,
                    Nationality = nat,
                    DOB = new DateOnly(y, m, d)
                });
            }
            db.Passengers.AddRange(pax);
            await db.SaveChangesAsync();

            // ---------------------------
            // 7) Bookings (>=150) with mix single & connecting
            // ---------------------------
            // We'll create 160 bookings: ~100 single-leg, ~60 connecting (2 legs)
            int bookingCount = 160;
            var bookings = new List<Booking>(bookingCount);
            for (int i = 0; i < bookingCount; i++)
            {
                var f = flights[rng.Next(flights.Count)];
                var p = pax[rng.Next(pax.Count)];
                var refBase = f.DepartureUtc > nowUtc ? f.DepartureUtc : nowUtc;
                var bDate = refBase.AddDays(-rng.Next(2, 18)).Date.AddHours(9 + rng.Next(8)); // 09:00–16:59

                bookings.Add(new Booking
                {
                    BookingRef = $"BKG{1000 + i}",
                    BookingDate = bDate,
                    status = "Confirmed",
                    PassengerId = p.PassengerId
                });
            }
            db.Bookings.AddRange(bookings);
            await db.SaveChangesAsync();

            // ---------------------------
            // 8) Tickets (>=200)
            //    - seat scheme "12A"
            //    - unique per flight
            //    - create connecting itineraries (2 tickets per some booking)
            // ---------------------------
            // Index helpers
            var flightsById = flights.ToDictionary(x => x.FlightId);
            var bookingsById = bookings.ToDictionary(x => x.BookingId);
            var tickets = new List<Ticket>(220);

            // Track used seats per flightId
            var usedSeats = flights.ToDictionary(f => f.FlightId, f => new HashSet<string>());

            // Helper: take next free seat for a flight considering its aircraft capacity
            string NextSeatForFlight(Flight f)
            {
                var seatList = seatMaps[f.AircraftId];
                var used = usedSeats[f.FlightId];
                foreach (var s in seatList)
                    if (!used.Contains(s))
                    {
                        used.Add(s);
                        return s;
                    }
                // fallback (should not happen given our volumes)
                string fallback = $"{rng.Next(1, 60)}{(char)('A' + rng.Next(6))}";
                used.Add(fallback);
                return fallback;
            }

            // Build a simple graph by (originIata -> list of flights departing), to find rough connections
            var flightsByOrigin = flights
                .GroupBy(f => airports.First(a => a.AirportId == routes.First(r => r.RouteId == f.RouteId).OriginAirportId).IATA)
                .ToDictionary(g => g.Key, g => g.OrderBy(x => x.DepartureUtc).ToList());

            // Map flightId -> (originIata, destIata)
            var flightOds = flights.ToDictionary(
                f => f.FlightId,
                f =>
                {
                    var r = routes.First(rr => rr.RouteId == f.RouteId);
                    var o = airports.First(a => a.AirportId == r.OriginAirportId).IATA;
                    var d = airports.First(a => a.AirportId == r.DestinationAirportId).IATA;
                    return (o, d);
                });

            int connectingBookingsTarget = 60; // 60 bookings with two legs
            int createdConnecting = 0;

            foreach (var b in bookings)
            {
                var p = pax.First(pp => pp.PassengerId == b.PassengerId);

                // Decide whether this booking will be connecting
                bool makeConnecting = createdConnecting < connectingBookingsTarget && rng.NextDouble() < 0.45;

                if (!makeConnecting)
                {
                    // Single-leg: pick a random flight near booking date
                    var candidate = flights
                    .OrderBy(f => Math.Abs((f.DepartureUtc - b.BookingDate).TotalDays))
                    .First();
                    tickets.Add(new Ticket
                    {
                        SeatNumber = NextSeatForFlight(candidate),
                        Fare = 80 + rng.Next(70, 251),   // ~150–330
                        CheckedIn = candidate.DepartureUtc <= nowUtc,
                        BookingId = b.BookingId,
                        FlightId = candidate.FlightId
                    });
                }
                else
                {
                    // Try to find 2-leg connection: F1 (o -> x), F2 (x -> y) same day with 2–6h layover
                    createdConnecting++;
                    // Choose F1
                    var f1 = flights[rng.Next(flights.Count)];
                    var (o1, d1) = flightOds[f1.FlightId];

                    // Potential F2s departing from d1, on same date, 2–6 hours after f1 arrival
                    DateTime minDep = f1.ArrivalUtc.AddHours(2);
                    DateTime maxDep = f1.ArrivalUtc.AddHours(6);

                    var f2Candidates = flights
                        .Where(ff =>
                        {
                            var (o2, _) = flightOds[ff.FlightId];
                            return o2 == d1 && ff.DepartureUtc.Date == f1.DepartureUtc.Date
                                   && ff.DepartureUtc >= minDep && ff.DepartureUtc <= maxDep;
                        })
                        .ToList();

                    if (f2Candidates.Count == 0)
                    {
                        // fallback to single if no connection found
                        tickets.Add(new Ticket
                        {
                            SeatNumber = NextSeatForFlight(f1),
                            Fare = 80 + rng.Next(70, 251),
                            CheckedIn = f1.DepartureUtc <= nowUtc,
                            BookingId = b.BookingId,
                            FlightId = f1.FlightId
                        });
                    }
                    else
                    {
                        var f2 = f2Candidates[rng.Next(f2Candidates.Count)];
                        tickets.Add(new Ticket
                        {
                            SeatNumber = NextSeatForFlight(f1),
                            Fare = 60 + rng.Next(60, 201), // leg 1
                            CheckedIn = f1.DepartureUtc <= nowUtc,
                            BookingId = b.BookingId,
                            FlightId = f1.FlightId
                        });
                        tickets.Add(new Ticket
                        {
                            SeatNumber = NextSeatForFlight(f2),
                            Fare = 60 + rng.Next(60, 201), // leg 2
                            CheckedIn = f2.DepartureUtc <= nowUtc,
                            BookingId = b.BookingId,
                            FlightId = f2.FlightId
                        });
                    }
                }
            }

            // Ensure total tickets >= 200 by topping up random single-legs if needed
            while (tickets.Count < 200)
            {
                var b = bookings[rng.Next(bookings.Count)];
                var f = flights[rng.Next(flights.Count)];
                tickets.Add(new Ticket
                {
                    SeatNumber = NextSeatForFlight(f),
                    Fare = 80 + rng.Next(70, 251),
                    CheckedIn = f.DepartureUtc <= nowUtc,
                    BookingId = b.BookingId,
                    FlightId = f.FlightId
                });
            }

            db.Tickets.AddRange(tickets);
            await db.SaveChangesAsync();

            // ---------------------------
            // 9) Baggage (>=150; some overweight > 23kg)
            // ---------------------------
            var bags = new List<Baggage>(160);
            int baggageTarget = 160;
            for (int i = 0; i < baggageTarget; i++)
            {
                var t = tickets[rng.Next(tickets.Count)];
                bool overweight = rng.NextDouble() < 0.25; // ~25% overweight
                decimal wt = overweight
                    ? (decimal)(23.5 + rng.NextDouble() * 10.0) // 23.5–33.5
                    : (decimal)(12.0 + rng.NextDouble() * 10.0); // 12–22
                bags.Add(new Baggage
                {
                    WeightKg = Math.Round(wt, 1),
                    TagNumber = $"BG{8000 + i}",
                    TicketId = t.TicketId
                });
            }
            db.Baggages.AddRange(bags);
            await db.SaveChangesAsync();

            // ---------------------------
            // 10) Aircraft Maintenance (>=15; past dates)
            // ---------------------------
            var maint = new List<AircraftMaintenance>();
            int maintCount = 18;
            for (int i = 0; i < maintCount; i++)
            {
                var ac = aircraft[rng.Next(aircraft.Count)];
                var when = nowUtc.Date.AddDays(-rng.Next(7, 180)).AddHours(9 + rng.Next(6));
                var kindIdx = i % 3;
                var kind = kindIdx == 0 ? "Routine" : (kindIdx == 1 ? "Inspection" : "Repair");
                maint.Add(new AircraftMaintenance
                {
                    AircraftId = ac.AircraftId,
                    MaintenanceDate = when,
                    Type = kind,
                    Note = $"{kind} check #{i + 1}"
                });
            }
            db.AircraftMaintenances.AddRange(maint);
            await db.SaveChangesAsync();

            // ---------------------------
            // 11) FlightCrews (assign 4–6 crew per flight)
            // ---------------------------
            var flightCrews = new List<FlightCrew>();
            foreach (var f in flights)
            {
                // Ensure at least 1 pilot, 1 copilot, and 2–4 FAs
                var pilots = crews.Where(c => c.Role == CrewRole.Pilot).OrderBy(_ => rng.Next()).Take(1).ToList();
                var cops = crews.Where(c => c.Role == CrewRole.CoPilot).OrderBy(_ => rng.Next()).Take(1).ToList();
                var fas = crews.Where(c => c.Role == CrewRole.FlightAttendant).OrderBy(_ => rng.Next()).Take(rng.Next(2, 5)).ToList();

                foreach (var c in pilots.Concat(cops).Concat(fas))
                {
                    flightCrews.Add(new FlightCrew
                    {
                        FlightId = f.FlightId,
                        CrewId = c.CrewId,
                        RoleOnFlight = c.Role.ToString()
                    });
                }
            }
            db.FlightCrews.AddRange(flightCrews);
            await db.SaveChangesAsync();
        }
    }
}
