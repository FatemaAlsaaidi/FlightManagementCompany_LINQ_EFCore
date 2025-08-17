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
        public string FlightNumber { get; set; } = null!;
        public DateTime DepartureUtc { get; set; }
        public DateTime ArrivalUtc { get; set; }
        public string Status { get; set; } = null!; // "Scheduled", etc.

        public int RouteId { get; set; }
        public int AircraftId { get; set; }

        public Route Route { get; set; } = null!;
        public Aircraft Aircraft { get; set; } = null!;

        public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
        public ICollection<FlightCrew> FlightCrews { get; set; } = new List<FlightCrew>();

    }
    

}
