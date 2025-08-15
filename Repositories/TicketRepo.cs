using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlightManagementCompany_LINQ_EFCore.Models;

namespace FlightManagementCompany_LINQ_EFCore.Repositories
{
    public class TicketRepo
    {

        // Database injection   
        private readonly FlightDatabaseContext _context;
        public TicketRepo(FlightDatabaseContext context)
        {
            _context = context;
        }

        // Get All Tickets
        public List<Ticket> GetAllTickets()
        {
            return _context.Tickets.ToList();
        }

        // Get Ticket by ID
        public Ticket GetTicketById(int id)
        {
            return _context.Tickets.FirstOrDefault(t => t.TicketId == id);
        }
        // Add a new Ticket
        public void AddTicket(Ticket ticket)
        {
            _context.Tickets.Add(ticket);
            _context.SaveChanges();
        }
        // Update an existing Ticket
        public void UpdateTicket(Ticket ticket)
        {
            _context.Tickets.Update(ticket);
            _context.SaveChanges();
        }
        // Delete a Ticket
        public void DeleteTicket(Ticket ticket)
        {
            _context.Tickets.Remove(ticket);
            _context.SaveChanges();
        }

        /// ====================== enetity Helpers methods 
        // 6. GetTicketsByBooking(string bookingRef)
        public IEnumerable<Ticket> GetTicketsByBooking(string bookingRef)
        {
            return _context.Tickets
                .Where(t => t.Booking != null && t.Booking.BookingRef == bookingRef)
                .ToList();

        }
        // 7. GetTicketsByPassenger(int passengerId)
        public IEnumerable<Ticket> GetTicketsByPassenger(int passengerId)
        {
            return _context.Tickets
                .Where(t => t.Booking != null && t.Booking.PassengerId == passengerId)
                .ToList();
        }



    }
}
