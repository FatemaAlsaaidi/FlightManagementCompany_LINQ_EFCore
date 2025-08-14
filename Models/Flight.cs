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

        public int RouteId { get; set; } // Foreign key to Route table
        public int AircraftId { get; set; } // Foreign key to Aircraft table

        // coolection navigation to route 
        public ICollection<Route> Routes { get; set; } = new List<Route>();

        // collection navigation to aircraft
        public Aircraft Aircraft { get; set; }

        // navigation property to FlightCrew
        public Route Route { get; set; } // Navigation property to Route

        // navigation to Ticket
        public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    }
}
