using FlightManagementCompany_LINQ_EFCore.DTOs;
using FlightManagementCompany_LINQ_EFCore.Models;
using FlightManagementCompany_LINQ_EFCore.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
        public List<FlightManifestDto> DailyFlightManifest(DateTime dayUtcOrLocal)
        {


            // 1.)Pre-filter flights to the day window to keep joins small
            var flightsOfDay = _flightRepo.GetAllFlights()
                .Where(f => f.DepartureUtc.Date == dayUtcOrLocal.Date);

            // 2) LINQ chain equivalent to your SQL
            var manifest =
                flightsOfDay
                // f + r
                .Join(_routeRepo.GetAllRoutes(),
                      f => f.RouteId,
                      r => r.RouteId,
                      (f, r) => new { f, r })

                // + origin airport (ao)
                .Join(_airportRepo.GetAllAirports(),
                      fr => fr.r.OriginAirportId,
                      ao => ao.AirportId,
                      (fr, ao) => new { fr.f, fr.r, ao })

                // + destination airport (ad)
                .Join(_airportRepo.GetAllAirports(),
                      frao => frao.r.DestinationAirportId,
                      ad => ad.AirportId,
                      (frao, ad) => new { frao.f, frao.r, frao.ao, ad })

                // + aircraft
                .Join(_aircraftRepo.GetAllAircrafts(),
                      frA => frA.f.AircraftId,
                      ac => ac.AircraftId,
                      (frA, ac) => new { frA.f, frA.r, frA.ao, frA.ad, ac })

                // + tickets
                .Join(_ticketRepo.GetAllTickets(),
                      x => x.f.FlightId,
                      t => t.FlightId,
                      (x, t) => new { x.f, x.r, x.ao, x.ad, x.ac, t })

                // + bookings
                .Join(_bookingRepo.GetAllBookings(),
                      xt => xt.t.BookingId,
                      b => b.BookingId,
                      (xt, b) => new { xt.f, xt.r, xt.ao, xt.ad, xt.ac, xt.t, b })

                // + passengers (for DISTINCT passenger count)
                .Join(_passengerRepo.GetAllPassengers(),
                      xtb => xtb.b.PassengerId,
                      p => p.PassengerId,
                      (xtb, p) => new { xtb.f, xtb.r, xtb.ao, xtb.ad, xtb.ac, xtb.t, xtb.b, p })

                // + flight crew links
                .Join(_flightCrewRepo.GetAllFlightCrews(),
                      x => x.f.FlightId,
                      fc => fc.FlightId,
                      (x, fc) => new { x.f, x.r, x.ao, x.ad, x.ac, x.t, x.b, x.p, fc })

                // + crew members
                .Join(_crewMemberRepo.GetAllCrewMembers(),
                      xfc => xfc.fc.CrewId,
                      cm => cm.CrewId,
                      (xfc, cm) => new { xfc.f, xfc.r, xfc.ao, xfc.ad, xfc.ac, xfc.t, xfc.b, xfc.p, xfc.fc, cm })

                // LEFT JOIN Baggages
                .GroupJoin(_baggageRepo.GetAllBaggages(),
                           x => x.t.TicketId,
                           bag => bag.TicketId,
                           (x, bags) => new { x.f, x.r, x.ao, x.ad, x.ac, x.b, x.p, x.cm, x.fc, bags })

                // GROUP BY flight key (like your SQL GROUP BY)
                .GroupBy(x => new
                {
                    x.f.FlightId,
                    x.f.FlightNumber,
                    x.f.DepartureUtc,
                    x.f.ArrivalUtc,
                    OriginIATA = x.ao.IATA,
                    DestIATA = x.ad.IATA,
                    x.ac.TailNumber
                })

                // Project to DTO
                .Select(g => new FlightManifestDto
                {
                    FlightNumber = g.Key.FlightNumber,
                    DepUtc = g.Key.DepartureUtc,
                    ArrUtc = g.Key.ArrivalUtc,
                    OriginIATA = g.Key.OriginIATA,
                    DestinationIATA = g.Key.DestIATA,
                    AircraftTail = g.Key.TailNumber,

                    // DISTINCT passenger count
                    PassengerCount = g.Select(x => x.p.PassengerId).Distinct().Count(),

                    // SUM baggage weights (LEFT JOIN semantics)
                    TotalBaggageKg = g.SelectMany(x => x.bags.DefaultIfEmpty())
                                      .Sum(b => b == null ? 0 : b.WeightKg),

                    // Crew list: DISTINCT by Name+Role then materialize to CrewDto
                    Crew = g.Select(x => new { Name = $"{x.cm.Fname} {x.cm.Lname}", Role = x.cm.Role.ToString() })
                            .GroupBy(c => new { c.Name, c.Role })
                            .Select(cg => new CrewDto
                            {
                                Name = cg.Key.Name,
                                Role = cg.Key.Role
                            })
                            .ToList()
                })
                .OrderBy(m => m.DepUtc)
                .ToList();

            return manifest;
        }


        // 2. Top Routes by Revenue 
        public List<RouteRevenueDto> GetTopRoutesByRevenue(DateTime date, int topN)
        {
            var flightsInRange = _flightRepo.GetAllFlights()
                .Where(f => f.DepartureUtc.Date >= date.Date);

            var query =
        from f in flightsInRange
        join r in _routeRepo.GetAllRoutes() on f.RouteId equals r.RouteId
        join ao in _airportRepo.GetAllAirports() on r.OriginAirportId equals ao.AirportId
        join ad in _airportRepo.GetAllAirports() on r.DestinationAirportId equals ad.AirportId
        join t in _ticketRepo.GetAllTickets() on f.FlightId equals t.FlightId
        select new
        {
            r.RouteId,
            OriginIATA = ao.IATA,
            DestIATA = ad.IATA,
            ao.Country,
            DestCountry = ad.Country,
            r.DistanceKm,
            t.TicketId,
            t.Fare
        };

            // 3) GroupBy route + projection to DTO
            var byRoute = query
                .GroupBy(x => new
                {
                    x.RouteId,
                    x.OriginIATA,
                    x.DestIATA,
                    x.Country,
                    x.DestCountry,
                    x.DistanceKm
                })
                .Select(g => new RouteRevenueDto
                {
                    RouteId = g.Key.RouteId,
                    OriginIATA = g.Key.OriginIATA,
                    DestinationIATA = g.Key.DestIATA,
                    DistanceKm = g.Key.DistanceKm,
                    RouteCountries = g.Key.Country + " : " + g.Key.DestCountry,

                    Revenue = g.Sum(x => x.Fare),
                    // Seats sold = Number of tickets (Distinct for safety if duplicate join occurs)
                    SeatsSold = g.Select(x => x.TicketId).Distinct().Count(), // projection part

                    AvgFare = g.Average(x => x.Fare)
                })
                .OrderByDescending(x => x.Revenue)
                .Take(topN)
                .ToList();

            return byRoute;
        }
        //3. On-Time Performance 
        public List<OnTimePerformanceDto> GetOnTimePerformanceByRoute(
    DateTime fromUtc, DateTime toUtc, int thresholdMinutes)
        {
            var flightsInRange = _flightRepo.GetAllFlights()
                .Where(f => f.DepartureUtc >= fromUtc && f.DepartureUtc < toUtc);

            var q =
                from f in flightsInRange
                join r in _routeRepo.GetAllRoutes() on f.RouteId equals r.RouteId
                join ao in _airportRepo.GetAllAirports() on r.OriginAirportId equals ao.AirportId
                join ad in _airportRepo.GetAllAirports() on r.DestinationAirportId equals ad.AirportId
                select new
                {
                    r.RouteId,
                    OriginIATA = ao.IATA,
                    DestIATA = ad.IATA,
                    ScheduledArrival = f.ArrivalUtc,        // المجدول
                    ActualArrival = f.ActualArrivalUtc   // الفعلي (nullable)
                };

            var result =
                q.GroupBy(x => new { x.RouteId, x.OriginIATA, x.DestIATA })
                 .Select(g =>
                 {
                     var flightsCount = g.Count();

                     var onTimeCount = g.Count(x =>
                         x.ActualArrival != null &&
                         (x.ActualArrival.Value - x.ScheduledArrival).TotalMinutes <= thresholdMinutes
                     );

                     return new OnTimePerformanceDto
                     {
                         Route = new RouteDto
                         {
                             OriginIATA = g.Key.OriginIATA,
                             DestinationIATA = g.Key.DestIATA
                         },
                         Flights = flightsCount,
                         OnTime = onTimeCount
                        
                     };
                 })
                 .OrderByDescending(x => x.OnTimePercent)  
                 .ToList();

            return result;
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
                OriginIATA = ap[routes[f.RouteId].OriginAirportId].IATA,
                DestinationIATA = ap[routes[f.RouteId].DestinationAirportId].IATA,
                AircraftTail = ac[f.AircraftId].TailNumber,
                PassengerCount = 0,
                TotalBaggageKg = 0
            }).ToList();

            return (union, intersect, except, page);
        }

        // 12.  Conversion Operators Demonstration

        public (Dictionary<string, FlightManifestDto> byNumber, SimpleRouteDto[] topRoutes,
                IEnumerable<FlightManifestDto> enumerable, IEnumerable<object> onlyTickets)
            ConversionOperatorsDemo(DateTime dayUtcOrLocal)
        {
            var manifest = DailyFlightManifest(dayUtcOrLocal);
            var dict = manifest.ToDictionary(x => x.FlightNumber, x => x, StringComparer.OrdinalIgnoreCase);

            var flights = _flightRepo.GetAllFlights();
            var routes = _routeRepo.GetAllRoutes();
            var ap = _airportRepo.GetAllAirports().ToDictionary(a => a.AirportId);

            var topRoutesArray = flights
                .GroupBy(f => f.RouteId)
                .OrderByDescending(g => g.Count())
                .Take(10)
                .Select(g =>
                {
                    var r = routes.First(x => x.RouteId == g.Key);
                    return new SimpleRouteDto
                    {
                        RouteId = r.RouteId,
                        Origin = ap[r.OriginAirportId].IATA,
                        Destination = ap[r.DestinationAirportId].IATA
                    };
                })
                .ToArray();

            var asEnum = manifest.AsEnumerable();

            var mixed = new object[] { "example", 42, new FlightManifestDto(), new RouteRevenueDto(), new Ticket() };
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
