using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlightManagementCompany_LINQ_EFCore.Models; // Ensure this namespace matches your Booking model's namespace

namespace FlightManagementCompany_LINQ_EFCore.Repositories
{
    public class BookingRepo
    {
        // database injection
        private readonly FlightDatabaseContext _context;
        public BookingRepo(FlightDatabaseContext context)
        {
            _context = context;
        }

        // Get all bookings
        public List<Booking> GetAllBookings()
        {
            return _context.Bookings.ToList();
        }

        // Get booking by ID
        public Booking GetBookingById(int id)
        {
            return _context.Bookings.FirstOrDefault(b => b.BookingId == id);
        }

        // Add a new booking
        public void AddBooking(Booking booking)
        {
            _context.Bookings.Add(booking);
            _context.SaveChanges();
        }

        // Update an existing booking
        public void RemoveBooking(Booking booking)
        {
            _context.Bookings.Remove(booking);
            _context.SaveChanges();
        }

        // Delete a booking by ID
        public void DeleteBooking(Booking booking)
        {
            
                _context.Bookings.Remove(booking);
                _context.SaveChanges();
            
        }

    }
}
