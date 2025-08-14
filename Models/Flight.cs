using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace FlightManagementCompany_LINQ_EFCore.Models
{
    public class Flight
    {
        [Key]
        public int FlightId { get; set; }
        [Required , StringLength(10)]
        public string FlightNumber { get; set; } // Unique flight number  e.g., "FM101"
        [Required]
        public DateTime DepartureUtc { get; set; }
        [Required]
        public DateTime ArrivalUtc { get; set; } // UTC time of arrival
        [Required, StringLength(20)]
        public string Status { get; set; } // e.g., "Scheduled", "Delayed", "Cancelled"
        [Required]
        public int RouteId { get; set; } // Foreign key to Route table
        [Required]
        public int AircraftId { get; set; } // Foreign key to Aircraft table

        
        // collection navigation to aircraft
        public Aircraft Aircraft { get; set; }

        // navigation property to route
        public Route Route { get; set; } // Navigation property to Route

        // navigation to Ticket
        public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
        // navigation property to FlightCrew

        public ICollection<FlightCrew> FlightCrews { get; set; } = new List<FlightCrew>();

    }
}
