using Microsoft.EntityFrameworkCore;
using FlightManagementCompany_LINQ_EFCore.SeedData;
using FlightManagementCompany_LINQ_EFCore.Repositories;
using FlightManagementCompany_LINQ_EFCore.Services;
using FlightManagementCompany_LINQ_EFCore.DTOs;
using FlightManagementCompany_LINQ_EFCore.Models;
using System;

using System.Globalization;


namespace FlightManagementCompany_LINQ_EFCore
{
    internal class Program
    {
        static async Task Main()
        {
            await using var db = new FlightDatabaseContext();

            // DEV reset (optional)
            // await db.Database.EnsureDeletedAsync();

            //await db.Database.MigrateAsync();

            //// ---- Repositories wired to the same DbContext ----
            //var airportRepo = new AirportRepo(db);
            //var routeRepo = new RouteRepo(db);
            //var aircraftRepo = new AircraftRepo(db);
            //var flightRepo = new FlightRepo(db);
            //var passengerRepo = new PassengerRepo(db);
            //var bookingRepo = new BookingRepo(db);
            //var ticketRepo = new TicketRepo(db);
            //var baggageRepo = new BaggageRepo(db);
            //var crewRepo = new CrewMemberRepo(db);
            //var flightCrewRepo = new FlightCrewRepo(db);
            //var maintenanceRepo = new AircraftMaintenanceRepo(db);

            //try
            //{


            //    DatabaseSeeder.CreateSampleData(
            //         db,                     
            //         airportRepo,
            //         routeRepo,
            //         aircraftRepo,
            //         flightRepo,
            //         passengerRepo,
            //         bookingRepo,
            //         ticketRepo,
            //         baggageRepo,
            //         crewRepo,
            //         flightCrewRepo,
            //         maintenanceRepo
            //        );


            //    Console.ForegroundColor = ConsoleColor.Green;
            //    Console.WriteLine("Seed done");
            //    Console.ResetColor();





            //}
            //catch (Exception ex)
            //{
            //    Console.ForegroundColor = ConsoleColor.Red;
            //    Console.WriteLine($"Seeding failed: {ex.Message}");
            //    Console.ResetColor();
            //    Console.WriteLine(ex); // full stack for debugging
            //}

 /// =====================================================================
            // 1) One context for everything in this console run
            using var context = new FlightDatabaseContext();

            // 2) Apply migrations (creates DB if missing). Prefer this over EnsureCreated().
            await context.Database.MigrateAsync();

            // 3) (Optional) Seed once
            // await SeedData.DatabaseSeeder.SeedAsync(context);

            // 4) Build repositories (concrete classes, not interfaces)
            var airportRepo = new AirportRepo(context);
            var routeRepo = new RouteRepo(context);
            var aircraftRepo = new AircraftRepo(context);
            var flightRepo = new FlightRepo(context);
            var passengerRepo = new PassengerRepo(context);
            var bookingRepo = new BookingRepo(context);
            var ticketRepo = new TicketRepo(context);
            var baggageRepo = new BaggageRepo(context);
            var crewRepo = new CrewMemberRepo(context);
            var flightCrewRepo = new FlightCrewRepo(context);
            var maintenanceRepo = new AircraftMaintenanceRepo(context);

            // 5) Create the analytics service (constructor arg order must match your class!)
             var svc = new FlightAnalyticsService(
                flightRepo, routeRepo, airportRepo, aircraftRepo,
                ticketRepo, baggageRepo, crewRepo, flightCrewRepo,
                bookingRepo, passengerRepo, maintenanceRepo
            );



            while (await ShowMenuAsync(svc))
            {
                Console.Write("\nto return to the menu press 'Enter'");
                Console.ReadLine();
            }

            Console.WriteLine("Bye!");



            /// ========================================================================




            static async Task<bool> ShowMenuAsync(FlightAnalyticsService svc)
            {
                Console.Clear();
                Console.WriteLine("================== MENU ==================");
                Console.WriteLine("1) Daily Flight Manifest");
                Console.WriteLine("2) Top Routes by Revenue");
                Console.WriteLine("3) On-Time Performance (per route)");
                Console.WriteLine("4) Seat Occupancy (> %) / Top N");
                Console.WriteLine("5) Find Available Seats for a Flight");
                Console.WriteLine("6) Crew Scheduling Conflicts");
                Console.WriteLine("7) Passengers with Connections");
                Console.WriteLine("8) Frequent Fliers (count/distance)");
                Console.WriteLine("9) Maintenance Alerts");
                Console.WriteLine("10) Baggage Overweight Alerts");
                Console.WriteLine("11) Complex Set + Paging");
                Console.WriteLine("12) Conversion Operators Demo");
                Console.WriteLine("13) Running Daily Revenue");
                Console.WriteLine("14) Forecast Next Week Bookings");
                Console.WriteLine("X) Exit");
                Console.Write("Select: ");
                var choice = Console.ReadLine()?.Trim();

                switch (choice?.ToUpperInvariant())
                {
                    case "1":
                        {
                            var Date1 = ReadDate("Enter 'From' date (YYYY-MM-DD) [UTC/local]: ");
                            var Date2 = ReadDate("Enter 'To' date (YYYY-MM-DD) [UTC/local]: ");


                            var list = svc.DailyFlightManifest(Date1, Date2);
                            PrintTitle($"Daily Flight Manifest from ({Date1:yyyy-MM-dd}) To ({Date2:yyyy-MM-dd}) ");
                            PrintManifest(list);
                            return true;
                        }

                    case "2":
                        {
                            var (fromUtc, toUtc) = ReadDateRange();
                            var topN = ReadInt("Top N (or 0 for all): ", allowZero: true);
                            var list = svc.GetTopRoutesByRevenue(fromUtc, toUtc, topN == 0 ? (int?)null : topN);
                            PrintTitle($"Top Routes by Revenue ({fromUtc:yyyy-MM-dd} → {toUtc:yyyy-MM-dd})");
                            PrintRouteRevenue(list);
                            return true;
                        }

                    case "3":
                        {
                            var (fromUtc, toUtc) = ReadDateRange();
                            var threshold = ReadInt("Threshold minutes (e.g., 15): ");
                            // Example: no actual-arrival provider supplied (assumes schedule==actual)
                            var list = svc.GetOnTimePerformanceByRoute(fromUtc, toUtc, threshold);
                            PrintTitle($"On-Time Performance ±{threshold} min");
                            foreach (var r in list)
                                Console.WriteLine($"{r.Key,-12}  Flights={r.Flights,3}  OnTime={r.OnTime,3}  {r.OnTimePercent,6:N1}%");
                            Console.WriteLine();
                            return true;
                        }

                    case "4":
                        {
                            var (fromUtc, toUtc) = ReadDateRange();
                            var min = ReadDouble("Min occupancy % (e.g., 80): ");
                            var topN = ReadInt("Top N (or 0 for all): ", allowZero: true);
                            var list = svc.GetHighOccupancyFlights(fromUtc, toUtc, min, topN == 0 ? (int?)null : topN);
                            PrintTitle($"Seat Occupancy > {min:N1}%");
                            foreach (var o in list)
                                Console.WriteLine($"{o.FlightNumber,-8}  {o.Sold,3}/{o.Capacity,-3}  {o.Occupancy,6:N1}%");
                            Console.WriteLine();
                            return true;
                        }

                    case "5":
                        {
                            Console.Write("Enter FlightId: ");
                            if (int.TryParse(Console.ReadLine(), out var flightId))
                            {
                                var seats = svc.GetAvailableSeats(flightId);
                                PrintTitle($"Available Seats (FlightId {flightId})");
                                Console.WriteLine(seats.Count == 0 ? "(none)" : string.Join(", ", seats.Take(80)) + (seats.Count > 80 ? " ..." : ""));
                                Console.WriteLine();
                            }
                            else Console.WriteLine("Invalid FlightId.");
                            return true;
                        }

                    case "6":
                        {
                            var (fromUtc, toUtc) = ReadDateRange();
                            var list = svc.GetCrewSchedulingConflicts(fromUtc, toUtc);
                            PrintTitle("Crew Scheduling Conflicts");
                            foreach (var c in list)
                                Console.WriteLine($"Crew {c.CrewId} {c.CrewName,-20}  FLT {c.FlightAId} vs {c.FlightBId}   {c.FlightADep:MM-dd HH:mm} / {c.FlightBDep:MM-dd HH:mm}");
                            Console.WriteLine(list.Count == 0 ? "(no conflicts)\n" : "\n");
                            return true;
                        }

                    case "7":
                        {
                            var hrs = ReadInt("Connection window (hours), e.g., 4: ");
                            var list = svc.GetPassengersWithConnections(hrs);
                            PrintTitle($"Passengers with Connections (≤{hrs}h)");
                            foreach (var it in list)
                            {
                                Console.WriteLine($"{it.PassengerName} (#{it.PassengerId})");
                                foreach (var s in it.Segments)
                                    Console.WriteLine($"  {s.FlightNumber,-8}  {s.OriginIata}->{s.DestinationIata}  {s.DepUtc:MM-dd HH:mm}→{s.ArrUtc:HH:mm}");
                            }
                            Console.WriteLine(list.Count == 0 ? "(none)\n" : "\n");
                            return true;
                        }

                    case "8":
                        {
                            var topN = ReadInt("Top N: ");
                            Console.Write("By distance? (y/N): ");
                            var byDist = (Console.ReadLine()?.Trim().Equals("y", StringComparison.OrdinalIgnoreCase) ?? false);
                            var list = svc.GetFrequentFliers(topN, byDist);
                            PrintTitle(byDist ? "Frequent Fliers (by distance)" : "Frequent Fliers (by flights)");
                            foreach (var f in list)
                                Console.WriteLine(byDist
                                    ? $"{f.PassengerName,-20}  Dist={f.TotalDistanceKm,6} km"
                                    : $"{f.PassengerName,-20}  Flights={f.FlightsCount,3}");
                            Console.WriteLine();
                            return true;
                        }

                    case "9":
                        {
                            var hours = ReadDouble("Hours threshold (e.g., 500): ");
                            var days = ReadInt("Days since last maintenance (e.g., 60): ");
                            var list = svc.GetMaintenanceAlerts(hours, days);
                            PrintTitle("Maintenance Alerts");
                            foreach (var m in list)
                                Console.WriteLine($"{m.TailNumber,-10}  Hours={m.TotalHours,6:N1}  SinceMaint={m.DaysSinceLastMaintenance,4}d  [{m.Reason}]");
                            Console.WriteLine(list.Count == 0 ? "(none)\n" : "\n");
                            return true;
                        }

                    case "10":
                        {
                            var th = ReadDecimal("Baggage threshold kg (e.g., 30): ");
                            var list = svc.GetBaggageOverweightAlerts(th);
                            PrintTitle($"Baggage Overweight Alerts (>{th} kg)");
                            foreach (var b in list)
                                Console.WriteLine($"Ticket {b.TicketId,-5}  {b.FlightNumber,-8}  {b.PassengerName,-20}  {b.TotalBaggageKg,6:N1} kg");
                            Console.WriteLine(list.Count == 0 ? "(none)\n" : "\n");
                            return true;
                        }

                    case "11":
                        {
                            var day = ReadDate("Day for paging (YYYY-MM-DD): ");
                            var pageIndex = ReadInt("Page index (0..): ");
                            var pageSize = ReadInt("Page size: ");

                            // sample VIP/FF/canceled input; customize as needed
                            Console.Write("VIP IDs (comma separated or empty): ");
                            var vip = ParseIntList(Console.ReadLine());
                            Console.Write("Frequent Fliers IDs (comma separated or empty): ");
                            var ff = ParseIntList(Console.ReadLine());
                            Console.Write("Canceled IDs (comma separated or empty): ");
                            var cx = ParseIntList(Console.ReadLine());

                            var (union, intersect, except, page) = svc.ComplexSetAndPaging(vip, ff, cx, pageIndex, pageSize, day);
                            PrintTitle("Set Ops");
                            Console.WriteLine($"Union:     [{string.Join(", ", union)}]");
                            Console.WriteLine($"Intersect: [{string.Join(", ", intersect)}]");
                            Console.WriteLine($"Except:    [{string.Join(", ", except)}]");
                            Console.WriteLine("Page:");
                            foreach (var x in page)
                                Console.WriteLine($"  {x.FlightNumber,-8} {x.Origin}->{x.Destination}  {x.DepUtc:HH:mm}→{x.ArrUtc:HH:mm}");
                            Console.WriteLine();
                            return true;
                        }

                    case "12":
                        {
                            Console.Write("Enter from date (YYYY-MM-DD): ");
                            if (!DateTime.TryParseExact(Console.ReadLine(), "yyyy-MM-dd",
                                CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out var fromDate))
                            {
                                Console.WriteLine("Invalid date format.");
                                return true;
                            }

                            Console.Write("Enter to date (YYYY-MM-DD): ");
                            if (!DateTime.TryParseExact(Console.ReadLine(), "yyyy-MM-dd",
                                CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out var toDate))
                            {
                                Console.WriteLine("Invalid date format.");
                                return true;
                            }

                            // Optional: ensure from <= to
                            if (toDate < fromDate) (fromDate, toDate) = (toDate, fromDate);

                            var (byNumber, topRoutes, enumerable, onlyTickets) = svc.ConversionOperatorsDemo(fromDate, toDate);

                            PrintTitle("Conversion Operators");
                            Console.WriteLine($"ToDictionary(by number): {byNumber.Count}");
                            Console.WriteLine($"ToArray(top routes): {topRoutes.Length}");
                            Console.WriteLine($"AsEnumerable(manifest): {enumerable.Count()}");
                            Console.WriteLine($"OfType<Ticket> from mixed: {onlyTickets.Count()}");
                            Console.WriteLine();
                        }
                        return true; // exit menu loop

                    case "13":
                        {
                            var (fromUtc, toUtc) = ReadDateRange();
                            var list = svc.GetDailyRevenueRunningTotal(fromUtc, toUtc);
                            PrintTitle("Running Daily Revenue");
                            foreach (var (dayUtc, sum) in list)
                                Console.WriteLine($"{dayUtc:yyyy-MM-dd}  →  {sum,10:C}");
                            Console.WriteLine();
                            return true;
                        }

                    case "14":
                        {
                            var lookback = ReadInt("Lookback days (e.g., 28): ");
                            var forecast = svc.ForecastNextWeekBookings(lookback);
                            PrintTitle("Forecast (next 7 days bookings)");
                            Console.WriteLine($"Expected bookings: ~{forecast:N1}\n");
                            return true;
                        }

                    case "X":
                        return false;

                    default:
                        Console.WriteLine("Unknown option.");
                        return true;
                }
            }

            // ---------- helpers ----------
            static DateTime ReadDate(string prompt)
            {
                while (true)
                {
                    Console.Write(prompt);
                    var s = Console.ReadLine()?.Trim();
                    if (DateTime.TryParseExact(s, "yyyy-MM-dd", CultureInfo.InvariantCulture,
                                               DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out var d))
                        return d;
                    Console.WriteLine("Invalid date format. Try again (YYYY-MM-DD).");
                }
            }

            static (DateTime fromUtc, DateTime toUtc) ReadDateRange()
            {
                var from = ReadDate("From (YYYY-MM-DD): ");
                var to = ReadDate("To   (YYYY-MM-DD): ").AddDays(1); // make end exclusive
                return (from, to);
            }

            static int ReadInt(string prompt, bool allowZero = false)
            {
                while (true)
                {
                    Console.Write(prompt);
                    if (int.TryParse(Console.ReadLine(), out var x) && (allowZero ? x >= 0 : x > 0))
                        return x;
                    Console.WriteLine("Invalid integer. Try again.");
                }
            }

            static double ReadDouble(string prompt)
            {
                while (true)
                {
                    Console.Write(prompt);
                    if (double.TryParse(Console.ReadLine(), NumberStyles.Float, CultureInfo.InvariantCulture, out var x))
                        return x;
                    Console.WriteLine("Invalid number. Try again.");
                }
            }

            static decimal ReadDecimal(string prompt)
            {
                while (true)
                {
                    Console.Write(prompt);
                    if (decimal.TryParse(Console.ReadLine(), NumberStyles.Float, CultureInfo.InvariantCulture, out var x))
                        return x;
                    Console.WriteLine("Invalid decimal. Try again.");
                }
            }

            static List<int> ParseIntList(string? input)
            {
                if (string.IsNullOrWhiteSpace(input)) return new List<int>();
                return input.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                            .Where(s => int.TryParse(s, out _))
                            .Select(int.Parse)
                            .ToList();
            }

            // Printers (reuse from earlier)
            static void PrintTitle(string title)
            {
                Console.WriteLine(new string('=', title.Length));
                Console.WriteLine(title);
                Console.WriteLine(new string('=', title.Length));
            }

            static void PrintManifest(List<FlightManifestDto> list)
            {
                if (list.Count == 0) { Console.WriteLine("(no flights)\n"); return; }

                foreach (var f in list)
                {
                    Console.WriteLine($"{f.FlightNumber,-8}  {f.Origin}->{f.Destination}  {f.DepUtc:HH:mm}→{f.ArrUtc:HH:mm}  Tail:{f.AircraftTail}");
                    Console.WriteLine($"  Pax: {f.PassengerCount,3} | Bags: {f.TotalBaggageKg,6:N1} kg");
                    if (f.Crew?.Count > 0)
                        Console.WriteLine("  Crew: " + string.Join(", ", f.Crew.Select(c => $"{c.Name} ({c.Role})")));
                    Console.WriteLine();
                }
            }

            static void PrintRouteRevenue(List<RouteRevenueDto> list)
            {
                if (list.Count == 0) { Console.WriteLine("(no data)\n"); return; }
                foreach (var r in list)
                    Console.WriteLine($"{r.Origin}->{r.Destination}  Seats:{r.SeatsSold,3}  Rev:{r.Revenue,10:C}  Avg:{r.AvgFare,8:C}");
                Console.WriteLine();
            }

        }


    }
}
