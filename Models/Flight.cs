using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightManagementCompany_LINQ_EFCore.Models
{
    public class Flight
    {
        public int FlightId { get; set; }
        public string FlightNumber { get; set; } // Unique flight number  e.g., "FM101"
        public DateTime DepartureUtc { get; set; }
        public DateTime ArrivalUtc { get; set; } // UTC time of arrival
        public string Status { get; set; } // e.g., "Scheduled", "Delayed", "Cancelled"
        // coolection navigation to route 
    }
}
