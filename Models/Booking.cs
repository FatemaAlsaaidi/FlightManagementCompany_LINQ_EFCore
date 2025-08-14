using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightManagementCompany_LINQ_EFCore.Models
{
    public class Booking
    {
        public int BookingId { get; set; }
        public string BookingRef { get; set; }
        public DateTime BookingDate { get; set; }
        public string status { get; set; }
        public int PassengerId { get; set; } // Foreign key to Passenger table

        // navigation to  passenger
        public Passenger Passenger { get; set; }

        // navigation to tickets
        public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();

    }
}
