using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlightManagementCompany_LINQ_EFCore.Models; // Ensure this namespace matches your Baggage model's namespace

namespace FlightManagementCompany_LINQ_EFCore.Repositories
{
    public class BaggageRepo
    {
        // database injection
        private readonly FlightDatabaseContext _context;
        public BaggageRepo(FlightDatabaseContext context)
        {
            _context = context;
        }

        // GetAll
        public List<Baggage> GetAll()
        {
            return _context.Baggages.ToList();
        }
        // GetById
        public Baggage GetById(int id)
        {
            return _context.Baggages.FirstOrDefault(b => b.BaggageId == id);
        }
        // Add
        public void Add(Baggage baggage)
        {
            _context.Baggages.Add(baggage);
            _context.SaveChanges();
        }
        // Update
        public void Update(Baggage baggage)
        {
            var existingBaggage = _context.Baggages.FirstOrDefault(b => b.BaggageId == baggage.BaggageId);
            if (existingBaggage != null)
            {
                existingBaggage.WeightKg = baggage.WeightKg;
                existingBaggage.TagNumber = baggage.TagNumber;
                existingBaggage.TicketId = baggage.TicketId; // Assuming TicketId is a foreign key
                _context.SaveChanges();
            }
        }

        // Delete
        public void Delete(Baggage baggage)
        {
           
            _context.Baggages.Remove(baggage);
            _context.SaveChanges();
            
        }
    }
}
