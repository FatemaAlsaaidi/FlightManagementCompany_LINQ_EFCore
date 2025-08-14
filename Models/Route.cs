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
        [Key]
        public int RouteId { get; set; }
        [Required]
        public int DistanceKm { get; set; } // Distance in kilometers
        [Required]
       public int OriginAirportId { get; set; } // Foriegn key to airport
        [Required]
        public int DestinationAirportId { get; set; } // Foriegn key to airport

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
