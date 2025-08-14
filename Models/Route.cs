using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlightManagementCompany_LINQ_EFCore.Models
{
    public class Route
    {
        public int RouteId { get; set; }
        public int DistanceKm { get; set; } // Distance in kilometers
        public string OriginOriginAirportId { get; set; } // Foriegn key to airport 
        public string DestinationAirportId { get; set; } // Foriegn key to airport

        // navigation to origin airport
        [InverseProperty("OriginRoute")]
        public Airport OriginAirport { get; set; } 
        // navigation to Destination airport
        [InverseProperty("DistenationRoute")]
        public Airport DistenationAirport { get; set; } 
        // navigation to flight
        public ICollection<Flight> Flights { get; set; } = new List<Flight>();


    }
}
