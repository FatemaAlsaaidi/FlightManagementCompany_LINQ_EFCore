using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlightManagementCompany_LINQ_EFCore.Models;

namespace FlightManagementCompany_LINQ_EFCore.Repositories
{
    public class FlightRepo
    {
        // Database injection

        private readonly FlightDatabaseContext _context;
        public FlightRepo(FlightDatabaseContext context)
        {
            _context = context;
        }

        // Get All Flights

        public List<Flight> GetAllFlights()
        {
            return _context.Flights.ToList();
        }

        // Get Flight by ID
        public Flight GetFlightById(int id)
        {
            return _context.Flights.FirstOrDefault(f => f.FlightId == id);
        }
        // Add a new Flight
        public void AddFlight(Flight flight)
        {
            _context.Flights.Add(flight);
            _context.SaveChanges();
        }
        // Update an existing Flight
        public void UpdateFlight(Flight flight)
        {
            _context.Flights.Update(flight);
            _context.SaveChanges();
        }

        // Delete a Flight
        public void DeleteFlight(Flight flight)
        {
           
                _context.Flights.Remove(flight);
                _context.SaveChanges();
            
        }

        /// ====================== entity Helpers methods ================
        // 6. GetFlightsByDateRange(DateTime from, DateTime to)
        public IEnumerable<Flight> GetFlightsByDateRange(DateTime from, DateTime to)
        {
            return _context.Flights
                .Where(f => f.DepartureUtc >= from && f.ArrivalUtc <= to)
                .ToList();
        }
        // 7. GetFlightsByRoute(int routeId)
        public IEnumerable<Flight> GetFlightsByRoute(int routeId)
        {
            return _context.Flights
                .Where(f => f.RouteId == routeId)
                .ToList();
        }

    }
}
