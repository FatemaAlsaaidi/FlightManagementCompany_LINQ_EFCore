using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlightManagementCompany_LINQ_EFCore.Models;

namespace FlightManagementCompany_LINQ_EFCore.Repositories
{
    public class PassengerRepo
    {
        // Database injection
        private readonly FlightDatabaseContext _context;
        public PassengerRepo(FlightDatabaseContext context)
        {
            _context = context;
        }

        // Get All Passengers
        public List<Passenger> GetAllPassengers()
        {
            return _context.Passengers.ToList();
        }

        // Get Passenger by ID
        public Passenger GetPassengerById(int id)
        {
            return _context.Passengers.FirstOrDefault(p => p.PassengerId == id);
        }

        // Add a new Passenger
        public void AddPassenger(Passenger passenger)
        {
            _context.Passengers.Add(passenger);
            _context.SaveChanges();
        }

        // Update an existing Passenger
        public void UpdatePassenger(Passenger passenger)
        {
            _context.Passengers.Update(passenger);
            _context.SaveChanges();
        }

        // Delete a Passenger
        public void DeletePassenger(Passenger passenger)
        {
            _context.Passengers.Remove(passenger);
            _context.SaveChanges();
        }
    }
}
