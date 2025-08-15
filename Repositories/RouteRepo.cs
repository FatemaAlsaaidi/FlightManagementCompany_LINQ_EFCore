using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlightManagementCompany_LINQ_EFCore.Models;
using Microsoft.EntityFrameworkCore;

namespace FlightManagementCompany_LINQ_EFCore.Repositories
{
    public class RouteRepo
    {
        // Database injection
        private readonly FlightDatabaseContext _context;
        public RouteRepo(FlightDatabaseContext context)
        {
            _context = context;
        }

        // Get All Routes
        public List<Route> GetAllRoutes()
        {
            return _context.Routes.ToList();
        }

        // Get Route by ID
        public Route GetRouteById(int id)
        {
            return _context.Routes.FirstOrDefault(r => r.RouteId == id);
        }

        // Add a new Route
        public void AddRoute(Route route)
        {
            _context.Routes.Add(route);
            _context.SaveChanges();
        }
        // Update an existing Route
        public void UpdateRoute(Route route)
        {
            _context.Routes.Update(route);
            _context.SaveChanges();
        }
        // Delete a Route
        public void DeleteRoute(Route route)
        {
            _context.Routes.Remove(route);
            _context.SaveChanges();
        }

        /// // ================= Entity-Specific Helpers ===================
        // 6. GetRoutesByOriginAndDestination(string origin, string destination)
        public List<Route> GetRoutesByOriginAndDestinationCity(string originCity, string destinationCity)
        {
            originCity = originCity?.Trim();
            destinationCity = destinationCity?.Trim();

            return _context.Routes.AsNoTracking()
                .Where(r => r.OriginAirport.City == originCity
                         && r.DistenationAirport.City == destinationCity)
                .ToList();
        }

    }
}
