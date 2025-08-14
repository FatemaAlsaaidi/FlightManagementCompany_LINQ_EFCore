using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightManagementCompany_LINQ_EFCore.Models
{
    public class Ticket
    {
        public int TicketId { get; set; }
        public string SeatNumber { get; set; }
        public decimal Fare { get; set; }
        public bool CheckedIn { get; set; }

        public int BookingId { get; set; } // Foreign key to Booking table
        public int FlightId { get; set; } // Foreign key to Flight table

        // navigation to booking 
        public Booking Booking { get; set; }

        // navigation to flight
        public Flight Flight { get; set; }

        // navigation to passengers
        public ICollection<Passenger> Passengers { get; set; } = new List<Passenger>();
    }
}
