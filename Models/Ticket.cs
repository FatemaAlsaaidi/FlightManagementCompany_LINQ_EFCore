using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace FlightManagementCompany_LINQ_EFCore.Models
{
    public class Ticket
    {
        [Key]
        public int TicketId { get; set; }
        [Required, StringLength(20)]
        public string SeatNumber { get; set; }
        [Required]
        public decimal Fare { get; set; }
        [Required]
        public bool CheckedIn { get; set; }
        [Required]
        public int BookingId { get; set; } // Foreign key to Booking table
        [Required]
        public int FlightId { get; set; } // Foreign key to Flight table

        // navigation to booking 
        public Booking Booking { get; set; }

        // navigation to flight
        public Flight Flight { get; set; }

        // navigation to Baggage
        public ICollection<Baggage> Baggages { get; set; } = new List<Baggage>();
    }
}
