using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


namespace FlightManagementCompany_LINQ_EFCore.Models
{
    public class Route
    {
        public int RouteId { get; set; }
        public int DistanceKm { get; set; }
        public int OriginAirportId { get; set; }
        public int DestinationAirportId { get; set; }

        public Airport OriginAirport { get; set; } = null!;
        public Airport DistenationAirport { get; set; } = null!; 

        public ICollection<Flight> Flights { get; set; } = new List<Flight>();



    }
}
