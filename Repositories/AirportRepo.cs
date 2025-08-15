using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlightManagementCompany_LINQ_EFCore.Models; // Ensure this namespace matches your Airport model's namespace

namespace FlightManagementCompany_LINQ_EFCore.Repositories
{
    public class AirportRepo
    {
        // database injection 
        private readonly FlightDatabaseContext _context;
        public AirportRepo(FlightDatabaseContext context)
        {
            _context = context;
        }

        // 1. GetAll
        public List<Airport> GetAll()
        {
            return _context.Airports.ToList();
        }
        // 2. GetById
        public Airport GetById(int id)
        {
            return _context.Airports.FirstOrDefault(a => a.AirportId == id);
        }
        // 3. Add
        public void Add(Airport airport)
        {
            _context.Airports.Add(airport);
            _context.SaveChanges();
        }
        // 4. Update
        public void Update(Airport airport)
        {
            var existingAirport = _context.Airports.FirstOrDefault(a => a.AirportId == airport.AirportId);
            if (existingAirport != null)
            {
                existingAirport.Name = airport.Name;
                existingAirport.City = airport.City;
                existingAirport.Country = airport.Country;
                existingAirport.IATA = airport.IATA;
                _context.SaveChanges();
            }
        }
        // 5. Delete
        public void Delete(Airport airport)
        {
           
            _context.Airports.Remove(airport);
            _context.SaveChanges();
            
        }
    }
}
