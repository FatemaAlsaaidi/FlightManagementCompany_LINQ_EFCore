using FlightManagementCompany_LINQ_EFCore.DTOs;
using FlightManagementCompany_LINQ_EFCore.Models;
using FlightManagementCompany_LINQ_EFCore.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightManagementCompany_LINQ_EFCore.Services
{
    public class FlightAnalyticsService 
    {
      
            private readonly FlightRepo _flightRepo;
            private readonly RouteRepo _routeRepo;
            private readonly AirportRepo _airportRepo;
            private readonly AircraftRepo _aircraftRepo;
            private readonly TicketRepo _ticketRepo;
            private readonly BaggageRepo _baggageRepo;
            private readonly CrewMemberRepo _crewMemberRepo;
            private readonly FlightCrewRepo _flightCrewRepo;
            private readonly BookingRepo _bookingRepo;
            private readonly PassengerRepo _passengerRepo;
            private readonly AircraftMaintenanceRepo _maintenanceRepo;

        //to create a constructor to initialize the repositories ...
        public FlightAnalyticsService(FlightRepo flightRepo,
                             RouteRepo routeRepo,
                             AirportRepo airportRepo,
                             AircraftRepo aircraftRepo,
                             TicketRepo ticketRepo,
                             BaggageRepo baggageRepo,
                             CrewMemberRepo crewMemberRepo,
                             FlightCrewRepo flightCrewRepo,
                             BookingRepo bookingRepo,
                             PassengerRepo passengerRepo,
                             AircraftMaintenanceRepo aircraftMaintenanceRepo)
        {
            _flightRepo = flightRepo;
            _routeRepo = routeRepo;
            _airportRepo = airportRepo;
            _aircraftRepo = aircraftRepo;
            _ticketRepo = ticketRepo;
            _baggageRepo = baggageRepo;
            _crewMemberRepo = crewMemberRepo;
            _flightCrewRepo = flightCrewRepo;
            _bookingRepo = bookingRepo;
            _passengerRepo = passengerRepo;
            _maintenanceRepo = aircraftMaintenanceRepo;

        }




        // 1. Daily Flight Manifest 
        public List<FlightManifestDto> DailyFlightManifest(DateTime FromDate, DateTime ToDate)
        {
            //// 1) Compute the day's UTC window [fromUtc, toUtc)
            //var fromUtc = (dateUtcOrLocal.Kind == DateTimeKind.Utc)
            //    ? dateUtcOrLocal.Date
            //    : dateUtcOrLocal.ToUniversalTime().Date;
            //var toUtc = fromUtc.AddDays(1);

            // 2) Pull flights that depart within the day
            var flights = _flightRepo.GetAllFlights()
                .Where(f => f.DepartureUtc >= FromDate && f.DepartureUtc < ToDate)
                .ToList();

            if (flights.Count == 0) return new();

            // Keys to selectively load related data
            var routeIds = flights.Select(f => f.RouteId).Distinct().ToList();
            var aircraftIds = flights.Select(f => f.AircraftId).Distinct().ToList();
            var flightIds = flights.Select(f => f.FlightId).ToList();

            // 3) Supporting data (routes, airports, aircraft)
            var routes = _routeRepo.GetAllRoutes().Where(r => routeIds.Contains(r.RouteId)).ToList();
            var airports = _airportRepo.GetAllAirports(); // we only need IATA here
            var aircraft = _aircraftRepo.GetAllAircrafts().Where(a => aircraftIds.Contains(a.AircraftId)).ToList();

            // Dictionaries for fast lookups
            var routeById = routes.ToDictionary(r => r.RouteId);
            var apById = airports.ToDictionary(a => a.AirportId);
            var aircraftById = aircraft.ToDictionary(a => a.AircraftId);

            // 4) Tickets → passenger count per flight
            var tickets = _ticketRepo.GetAllTickets()
                .Where(t => flightIds.Contains(t.FlightId))
                .ToList();

            var paxByFlight = tickets
                .GroupBy(t => t.FlightId)
                .ToDictionary(g => g.Key, g => g.Count());

            // 5) Baggage → total weight per flight (Join on TicketId)
            var baggage = _baggageRepo.GetAllBaggages()
                .Where(b => tickets.Select(t => t.TicketId).Contains(b.TicketId))
                .ToList();

            var bagByFlight = baggage
                .Join(tickets, b => b.TicketId, t => t.TicketId, (b, t) => new { t.FlightId, b.WeightKg })
                .GroupBy(x => x.FlightId)
                .ToDictionary(g => g.Key, g => g.Sum(x => x.WeightKg));

            // 6) Crew: GroupJoin (flight -> flightCrews), then join with crewMembers to get name/role
            var flightCrews = _flightCrewRepo.GetAllFlightCrews()
                .Where(fc => fc.FlightId != 0 && flightIds.Contains(fc.FlightId))
                .ToList();

            var crewIds = flightCrews.Select(fc => fc.CrewId).Distinct().ToList();
            var crewMembers = _crewMemberRepo.GetAllCrewMembers()
                .Where(c => crewIds.Contains(c.CrewId))
                .ToList();

            // 7) Project to DTO with GroupJoin for crew and left-joins for pax/baggage
            var result =
                (from f in flights
                 let r = routeById[f.RouteId]
                 let originIata = apById[r.OriginAirportId].IATA
                 let destIata = apById[r.DestinationAirportId].IATA
                 let tail = aircraftById[f.AircraftId].TailNumber

                 // GroupJoin: each flight → its crew assignment links
                 join fc in flightCrews on f.FlightId equals fc.FlightId into crewGroup

                 // LEFT join for passengers (may not exist → default(KeyValuePair) where .Value = 0)
                 join pax in paxByFlight on f.FlightId equals pax.Key into paxg
                 from pax in paxg.DefaultIfEmpty()

                     // LEFT join for baggage (may not exist → default(KeyValuePair) where .Value = 0)
                 join bag in bagByFlight on f.FlightId equals bag.Key into bagg
                 from bag in bagg.DefaultIfEmpty()

                 orderby f.DepartureUtc
                 select new FlightManifestDto
                 {
                     FlightNumber = f.FlightNumber,
                     DepUtc = f.DepartureUtc,
                     ArrUtc = f.ArrivalUtc,
                     Origin = originIata,
                     Destination = destIata,
                     AircraftTail = tail,
                     PassengerCount = pax.Value,          // default(KeyValuePair) => 0 if missing
                     TotalBaggageKg = bag.Value,          // default(KeyValuePair) => 0 if missing

                     // Build crew list from the crewGroup links
                     Crew = (from link in crewGroup
                             join cm in crewMembers on link.CrewId equals cm.CrewId
                             select new CrewDto
                             {
                                 Name = $"{cm.Fname} {cm.Lname}",
                                 Role = cm.Role.ToString()
                             }).ToList()
                 })
                .ToList();

            return result;
        }

        // 2. Top Routes by Revenue 
        public List<RouteRevenueDto> GetTopRoutesByRevenue(DateTime fromUtc, DateTime toUtc, int? topN = null)
        {
            var flights = _flightRepo.GetAllFlights()
                .Where(f => f.DepartureUtc >= fromUtc && f.DepartureUtc < toUtc)
                .ToList();

            if (flights.Count == 0) return new();

            var routes = _routeRepo.GetAllRoutes().ToDictionary(r => r.RouteId);
            var airports = _airportRepo.GetAllAirports().ToDictionary(a => a.AirportId);
            var flightIds = flights.Select(f => f.FlightId).ToHashSet();
            var tickets = _ticketRepo.GetAllTickets().Where(t => flightIds.Contains(t.FlightId)).ToList();

            var q = tickets
                .GroupBy(t => flights.First(f => f.FlightId == t.FlightId).RouteId)
                .Select(g =>
                {
                    var r = routes[g.Key];
                    var origin = airports[r.OriginAirportId].IATA;
                    var dest = airports[r.DestinationAirportId].IATA;
                    var seats = g.Count();
                    var revenue = g.Sum(x => x.Fare);
                    var avg = seats == 0 ? 0 : g.Average(x => x.Fare);

                    return new RouteRevenueDto
                    {
                        RouteId = r.RouteId,
                        Origin = origin,
                        Destination = dest,
                        SeatsSold = seats,
                        Revenue = revenue,
                        AvgFare = avg
                    };
                })
                .OrderByDescending(x => x.Revenue);

            return topN is int n ? q.Take(n).ToList() : q.ToList();
        }

        //3. On-Time Performance 
        public List<OnTimePerformanceDto> GetOnTimePerformanceByRoute(
            DateTime fromUtc, DateTime toUtc, int thresholdMinutes, Func<Flight, DateTime?> actualArrivalProvider = null)
        {
            var flights = _flightRepo.GetAllFlights()
                .Where(f => f.DepartureUtc >= fromUtc && f.DepartureUtc < toUtc)
                .ToList();

            if (flights.Count == 0) return new();

            var routes = _routeRepo.GetAllRoutes().ToDictionary(r => r.RouteId);
            var airports = _airportRepo.GetAllAirports().ToDictionary(a => a.AirportId);

            var keyed = flights.Select(f =>
            {
                var r = routes[f.RouteId];
                var key = $"{airports[r.OriginAirportId].IATA}->{airports[r.DestinationAirportId].IATA}";
                var sched = f.ArrivalUtc;
                var actual = actualArrivalProvider?.Invoke(f) ?? sched;
                var diff = Math.Abs((actual - sched).TotalMinutes);
                var onTime = diff <= thresholdMinutes;
                return new { key, onTime };
            });

            var res = keyed
                .GroupBy(x => x.key)
                .Select(g => new OnTimePerformanceDto
                {
                    Key = g.Key,
                    Flights = g.Count(),
                    OnTime = g.Count(z => z.onTime)
                })
                .OrderByDescending(x => x.OnTimePercent)
                .ToList();

            return res;
        }

        // 4. Seat Occupancy Heatmap
        // occupancy = tickets sold / capacity; return > min% or topN
        public List<OccupancyDto> GetHighOccupancyFlights(DateTime fromUtc, DateTime toUtc, double minPercent = 80, int? topN = null)
        {
            var flights = _flightRepo.GetAllFlights()
                .Where(f => f.DepartureUtc >= fromUtc && f.DepartureUtc < toUtc)
                .ToList();

            if (flights.Count == 0) return new();

            var aircraft = _aircraftRepo.GetAllAircrafts().ToDictionary(a => a.AircraftId);
            var flightIds = flights.Select(f => f.FlightId).ToHashSet();
            var tickets = _ticketRepo.GetAllTickets().Where(t => flightIds.Contains(t.FlightId)).ToList();

            var q = flights.Select(f =>
            {
                var cap = aircraft[f.AircraftId].Capacity;
                var sold = tickets.Count(t => t.FlightId == f.FlightId);
                return new OccupancyDto
                {
                    FlightId = f.FlightId,
                    FlightNumber = f.FlightNumber,
                    Capacity = cap,
                    Sold = sold
                };
            })
            .Where(x => x.Capacity > 0 && x.Occupancy >= minPercent)
            .OrderByDescending(x => x.Occupancy);

            return topN is int n ? q.Take(n).ToList() : q.ToList();
        }

        // 5. Find Available Seats for a Flight 
        // Build seat map from capacity (A..F columns), then Except booked seats.
        public List<string> GetAvailableSeats(int flightId)
        {
            var f = _flightRepo.GetAllFlights().FirstOrDefault(x => x.FlightId == flightId);
            if (f is null) return new List<string>();

            var cap = _aircraftRepo.GetAllAircrafts().First(a => a.AircraftId == f.AircraftId).Capacity;
            var cols = new[] { "A", "B", "C", "D", "E", "F" };
            var rows = (int)Math.Ceiling(cap / 6.0);
            var all = Enumerable.Range(1, rows)
                                .SelectMany(r => cols.Select(c => c + r))
                                .Take(cap)
                                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var taken = _ticketRepo.GetAllTickets()
                .Where(t => t.FlightId == flightId)
                .Select(t => t.SeatNumber);

            return all.Except(taken, StringComparer.OrdinalIgnoreCase)
                      .OrderBy(s => s)
                      .ToList();
        }

        // 6. Crew Scheduling Conflicts 
        public List<CrewConflictDto> GetCrewSchedulingConflicts(DateTime fromUtc, DateTime toUtc)
        {
            var flights = _flightRepo.GetAllFlights()
                .Where(f => f.DepartureUtc >= fromUtc && f.DepartureUtc < toUtc)
                .ToList();

            if (flights.Count == 0) return new();

            var flightById = flights.ToDictionary(f => f.FlightId);
            var flightIds = flightById.Keys.ToHashSet();
            var assigns = _flightCrewRepo.GetAllFlightCrews().Where(fc => flightIds.Contains(fc.FlightId)).ToList();
            var crew = _crewMemberRepo.GetAllCrewMembers().ToDictionary(c => c.CrewId);

            var conflicts = new List<CrewConflictDto>();

            foreach (var g in assigns.GroupBy(a => a.CrewId))
            {
                var list = g.Select(x => new
                {
                    CrewId = x.CrewId,
                    CrewName = $"{crew[x.CrewId].Fname} {crew[x.CrewId].Lname}",
                    F = flightById[x.FlightId]
                })
                .OrderBy(x => x.F.DepartureUtc)
                .ToList();

                for (int i = 0; i < list.Count; i++)
                    for (int j = i + 1; j < list.Count; j++)
                    {
                        var a = list[i].F;
                        var b = list[j].F;
                        var overlap = a.DepartureUtc < b.ArrivalUtc && b.DepartureUtc < a.ArrivalUtc;
                        if (overlap)
                        {
                            conflicts.Add(new CrewConflictDto
                            {
                                CrewId = g.Key,
                                CrewName = list[i].CrewName,
                                FlightAId = a.FlightId,
                                FlightBId = b.FlightId,
                                FlightADep = a.DepartureUtc,
                                FlightBDep = b.DepartureUtc
                            });
                        }
                    }
            }

            return conflicts;
        }

        // 7. Passengers with Connections 
        // Same booking, sequential flights within X hours, and matching airport (Dest of A == Origin of B)
        public List<PassengerItineraryDto> GetPassengersWithConnections(int withinHours)
        {
            var hrs = TimeSpan.FromHours(withinHours);

            var tickets = _ticketRepo.GetAllTickets();
            var bookings = _bookingRepo.GetAllBookings().ToDictionary(b => b.BookingId);
            var pax = _passengerRepo.GetAllPassengers().ToDictionary(p => p.PassengerId);
            var flights = _flightRepo.GetAllFlights().ToDictionary(f => f.FlightId);
            var routes = _routeRepo.GetAllRoutes().ToDictionary(r => r.RouteId);
            var ap = _airportRepo.GetAllAirports().ToDictionary(a => a.AirportId);

            var result = new List<PassengerItineraryDto>();

            foreach (var g in tickets.GroupBy(t => t.BookingId))
            {
                var b = bookings[g.Key];
                var p = pax[b.PassengerId];

                var segs = g.Select(t =>
                {
                    var f = flights[t.FlightId];
                    var r = routes[f.RouteId];
                    return new
                    {
                        f.FlightId,
                        f.FlightNumber,
                        Dep = f.DepartureUtc,
                        Arr = f.ArrivalUtc,
                        OrigIata = ap[r.OriginAirportId].IATA,
                        DestIata = ap[r.DestinationAirportId].IATA
                    };
                })
                .OrderBy(x => x.Dep)
                .ToList();

                var conn = new List<ItinSegmentDto>();
                for (int i = 0; i < segs.Count - 1; i++)
                {
                    var a = segs[i]; var c = segs[i + 1];
                    if (c.Dep - a.Arr <= hrs && a.DestIata == c.OrigIata)
                    {
                        conn.Add(new ItinSegmentDto { FlightId = a.FlightId, FlightNumber = a.FlightNumber, OriginIata = a.OrigIata, DestinationIata = a.DestIata, DepUtc = a.Dep, ArrUtc = a.Arr });
                        conn.Add(new ItinSegmentDto { FlightId = c.FlightId, FlightNumber = c.FlightNumber, OriginIata = c.OrigIata, DestinationIata = c.DestIata, DepUtc = c.Dep, ArrUtc = c.Arr });
                    }
                }

                if (conn.Count > 0)
                {
                    result.Add(new PassengerItineraryDto
                    {
                        PassengerId = p.PassengerId,
                        PassengerName = $"{p.Fname} {p.Lname}",
                        Segments = conn
                    });
                }
            }

            return result;
        }

        // 8. Frequent Fliers
        // Top N by number of flights OR by total distance
        public List<FrequentFlierDto> GetFrequentFliers(int topN, bool byDistance = false)
        {
            var tickets = _ticketRepo.GetAllTickets();
            var bookings = _bookingRepo.GetAllBookings().ToDictionary(b => b.BookingId);
            var pax = _passengerRepo.GetAllPassengers().ToDictionary(p => p.PassengerId);
            var flights = _flightRepo.GetAllFlights().ToDictionary(f => f.FlightId);
            var routes = _routeRepo.GetAllRoutes().ToDictionary(r => r.RouteId);

            IEnumerable<FrequentFlierDto> res;

            if (!byDistance)
            {
                res = tickets.GroupBy(t => bookings[t.BookingId].PassengerId)
                    .Select(g => new FrequentFlierDto
                    {
                        PassengerId = g.Key,
                        PassengerName = $"{pax[g.Key].Fname} {pax[g.Key].Lname}",
                        FlightsCount = g.Select(x => x.FlightId).Distinct().Count(),
                        TotalDistanceKm = 0
                    })
                    .OrderByDescending(x => x.FlightsCount)
                    .ThenBy(x => x.PassengerName)
                    .Take(topN);
            }
            else
            {
                res = tickets.GroupBy(t => bookings[t.BookingId].PassengerId)
                    .Select(g => new FrequentFlierDto
                    {
                        PassengerId = g.Key,
                        PassengerName = $"{pax[g.Key].Fname} {pax[g.Key].Lname}",
                        FlightsCount = g.Select(x => x.FlightId).Distinct().Count(),
                        TotalDistanceKm = g.Sum(x => routes[flights[x.FlightId].RouteId].DistanceKm)
                    })
                    .OrderByDescending(x => x.TotalDistanceKm)
                    .ThenBy(x => x.PassengerName)
                    .Take(topN);
            }

            return res.ToList();
        }

        // 9. Maintenance Alert 
        // Aircraft with hours > threshold or last maintenance older than Y days.
        public List<MaintenanceAlertDto> GetMaintenanceAlerts(double hoursThreshold, int daysSinceLastMaint)
        {
            const double AvgSpeedKmPerHour = 800.0;

            var tickets = _ticketRepo.GetAllTickets();
            var flights = _flightRepo.GetAllFlights().ToDictionary(f => f.FlightId);
            var routes = _routeRepo.GetAllRoutes().ToDictionary(r => r.RouteId);
            var aircraft = _aircraftRepo.GetAllAircrafts().ToDictionary(a => a.AircraftId);
            var maint = _maintenanceRepo.GetAllAircrafMaintenances();

            var hoursByAc = tickets.GroupBy(t => flights[t.FlightId].AircraftId)
                .Select(g => new
                {
                    AircraftId = g.Key,
                    Tail = aircraft[g.Key].TailNumber,
                    Hours = g.Sum(x => routes[flights[x.FlightId].RouteId].DistanceKm) / AvgSpeedKmPerHour
                })
                .ToDictionary(x => x.AircraftId, x => x);

            var lastMaintByAc = maint.GroupBy(m => m.AircraftId)
                .ToDictionary(g => g.Key, g => g.Max(x => x.MaintenanceDate));

            var now = DateTime.UtcNow;
            var alerts = new List<MaintenanceAlertDto>();

            foreach (var kv in hoursByAc)
            {
                var acId = kv.Key;
                var hrs = kv.Value.Hours;
                lastMaintByAc.TryGetValue(acId, out var last);
                var days = last == default ? int.MaxValue : (int)Math.Floor((now - last).TotalDays);

                var reasons = new List<string>();
                if (hrs > hoursThreshold) reasons.Add("Hours>Threshold");
                if (days > daysSinceLastMaint) reasons.Add("LastMaint>Y days");
                if (reasons.Count == 0) continue;

                alerts.Add(new MaintenanceAlertDto
                {
                    AircraftId = acId,
                    TailNumber = kv.Value.Tail,
                    TotalHours = Math.Round(hrs, 1),
                    DaysSinceLastMaintenance = days == int.MaxValue ? 99999 : days,
                    Reason = string.Join(" & ", reasons)
                });
            }

            return alerts.OrderByDescending(a => a.TotalHours).ToList();
        }

        // 10.  Baggage Overweight Alerts 
        // Tickets with total baggage weight > threshold
        public List<BaggageOverweightDto> GetBaggageOverweightAlerts(decimal kgThreshold)
        {
            var baggage = _baggageRepo.GetAllBaggages();
            var tickets = _ticketRepo.GetAllTickets().ToDictionary(t => t.TicketId);
            var flights = _flightRepo.GetAllFlights().ToDictionary(f => f.FlightId);
            var bookings = _bookingRepo.GetAllBookings().ToDictionary(b => b.BookingId);
            var pax = _passengerRepo.GetAllPassengers().ToDictionary(p => p.PassengerId);

            var q = baggage.GroupBy(b => b.TicketId)
                .Select(g =>
                {
                    var t = tickets[g.Key];
                    var p = pax[bookings[t.BookingId].PassengerId];
                    return new BaggageOverweightDto
                    {
                        TicketId = g.Key,
                        FlightNumber = flights[t.FlightId].FlightNumber,
                        PassengerName = $"{p.Fname} {p.Lname}",
                        TotalBaggageKg = g.Sum(x => x.WeightKg)
                    };
                })
                .Where(x => x.TotalBaggageKg > kgThreshold)
                .OrderByDescending(x => x.TotalBaggageKg)
                .ToList();

            return q;
        }

        // 11.  Complex Set/Partitioning Examples
        // Set ops: Union / Intersect / Except; plus paging with Skip/Take for flights
        public (List<int> unionIds, List<int> intersectIds, List<int> exceptIds, List<FlightManifestDto> page) ComplexSetAndPaging(IEnumerable<int> vipPassengerIds, IEnumerable<int> frequentFlierIds, IEnumerable<int> canceledPassengerIds, int pageIndex, int pageSize, DateTime dayUtcOrLocal)
        {
            var vip = vipPassengerIds?.ToHashSet() ?? new HashSet<int>();
            var ff = frequentFlierIds?.ToHashSet() ?? new HashSet<int>();
            var canceled = canceledPassengerIds?.ToHashSet() ?? new HashSet<int>();

            var union = vip.Union(ff).ToList();
            var intersect = vip.Intersect(ff).ToList();
            var except = union.Except(canceled).ToList();

            var fromUtc = (dayUtcOrLocal.Kind == DateTimeKind.Utc)
            ? dayUtcOrLocal.Date
            : dayUtcOrLocal.ToUniversalTime().Date;

            var toUtc = fromUtc.AddDays(1);
            var flights = _flightRepo.GetAllFlights()
                .Where(f => f.DepartureUtc >= fromUtc && f.DepartureUtc < toUtc)
                .OrderBy(f => f.DepartureUtc)
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToList();

            // Minimal manifest page (no crew/baggage for speed)
            var routes = _routeRepo.GetAllRoutes().ToDictionary(r => r.RouteId);
            var ap = _airportRepo.GetAllAirports().ToDictionary(a => a.AirportId);
            var ac = _aircraftRepo.GetAllAircrafts().ToDictionary(a => a.AircraftId);

            var page = flights.Select(f => new FlightManifestDto
            {
                FlightNumber = f.FlightNumber,
                DepUtc = f.DepartureUtc,
                ArrUtc = f.ArrivalUtc,
                Origin = ap[routes[f.RouteId].OriginAirportId].IATA,
                Destination = ap[routes[f.RouteId].DestinationAirportId].IATA,
                AircraftTail = ac[f.AircraftId].TailNumber,
                PassengerCount = 0,
                TotalBaggageKg = 0
            }).ToList();

            return (union, intersect, except, page);
        }

        // 12.  Conversion Operators Demonstration

        public (Dictionary<string, FlightManifestDto> byNumber,
                SimpleRouteDto[] topRoutes,
                IEnumerable<FlightManifestDto> enumerable,
                IEnumerable<object> onlyTickets)
            ConversionOperatorsDemo(DateTime fromDate, DateTime toDate)
        {
            // Normalize input to UTC (so comparisons against DepartureUtc are correct)
            var fromUtc = fromDate.Kind == DateTimeKind.Utc ? fromDate : fromDate.ToUniversalTime();
            var toUtc = toDate.Kind == DateTimeKind.Utc ? toDate : toDate.ToUniversalTime();

            // Pull the manifest for the requested window
            // If you don't already have this overload, add the simple one shown below.
            var manifest = DailyFlightManifest(fromUtc, toUtc);

            // Defensive: empty manifest => return empty shapes with correct types
            if (manifest.Count == 0)
            {
                return (
                    new Dictionary<string, FlightManifestDto>(StringComparer.OrdinalIgnoreCase),
                    Array.Empty<SimpleRouteDto>(),
                    Enumerable.Empty<FlightManifestDto>(),
                    Enumerable.Empty<object>()
                );
            }

            // 1) Map FlightNumber -> Manifest row (case-insensitive keys)
            var dict = manifest.ToDictionary(x => x.FlightNumber, x => x, StringComparer.OrdinalIgnoreCase);

            // 2) Top routes in the SAME window
            var flightsInWindow = _flightRepo.GetAllFlights()
                .Where(f => f.DepartureUtc >= fromUtc && f.DepartureUtc < toUtc)
                .ToList();

            var routes = _routeRepo.GetAllRoutes().ToList();
            var airportsById = _airportRepo.GetAllAirports().ToDictionary(a => a.AirportId);

            var topRoutesArray = flightsInWindow
                .GroupBy(f => f.RouteId)
                .OrderByDescending(g => g.Count())
                .Take(10)
                .Select(g =>
                {
                    var r = routes.FirstOrDefault(x => x.RouteId == g.Key);
                    if (r is null) return null; // defensive

                    airportsById.TryGetValue(r.OriginAirportId, out var originAp);
                    airportsById.TryGetValue(r.DestinationAirportId, out var destAp);

                    return new SimpleRouteDto
                    {
                        RouteId = r.RouteId,
                        Origin = originAp?.IATA ?? "???",
                        Destination = destAp?.IATA ?? "???"
                    };
                })
                .Where(x => x != null)!
                .Cast<SimpleRouteDto>()
                .ToArray();

            // 3) Conversion operators demos
            var asEnum = manifest.AsEnumerable();

            var mixed = new object[]
            {
        "example",
        42,
        new FlightManifestDto(),
        new RouteRevenueDto(),
        new Ticket()
            };

            var onlyTickets = mixed.OfType<Ticket>().Cast<object>();

            return (dict, topRoutesArray, asEnum, onlyTickets);
        }

        // 13. Window-like Operation (running totals) 
        // For each day in range, compute cumulative revenue
        public List<(DateTime dayUtc, decimal runningRevenue)> GetDailyRevenueRunningTotal(DateTime fromUtc, DateTime toUtc)
        {
            var flights = _flightRepo.GetAllFlights()
                .Where(f => f.DepartureUtc >= fromUtc && f.DepartureUtc < toUtc)
                .ToList();

            if (flights.Count == 0) return new();

            var tickets = _ticketRepo.GetAllTickets()
                .Where(t => flights.Select(f => f.FlightId).Contains(t.FlightId))
                .ToList();

            var daily = tickets
                .GroupBy(t => flights.First(f => f.FlightId == t.FlightId).DepartureUtc.Date)
                .Select(g => new { Day = g.Key, Revenue = g.Sum(x => x.Fare) })
                .OrderBy(x => x.Day)
                .ToList();

            decimal acc = 0;
            return daily.Select(x => { acc += x.Revenue; return (x.Day, acc); }).ToList();
        }

        // 14. Forecasting (simple) 
        // Avg bookings per day over lookback → project next 7 days
        public decimal ForecastNextWeekBookings(int lookbackDays = 28)
        {
            var from = DateTime.UtcNow.Date.AddDays(-lookbackDays);
            var to = DateTime.UtcNow.Date;

            var daily = _bookingRepo.GetAllBookings()
                .Where(b => b.BookingDate >= from && b.BookingDate < to)
                .GroupBy(b => b.BookingDate.Date)
                .Select(g => g.Count())
                .ToList();

            if (daily.Count == 0) return 0m;
            var avgPerDay = daily.Average();
            return (decimal)(avgPerDay * 7.0);
        }

    }
}
