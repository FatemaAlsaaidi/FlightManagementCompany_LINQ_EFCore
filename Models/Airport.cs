using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;


namespace FlightManagementCompany_LINQ_EFCore.Models
{
    public class Airport
    {
        public int AirportId { get; set; }
        public string Name { get; set; } = null!;
        public string IATA { get; set; } = null!;
        public string City { get; set; } = null!;
        public string Country { get; set; } = null!;
        public string TimeZone { get; set; } = null!;

        // Keep collection names you already used in mappings
        public ICollection<Route> OriginRoute { get; set; } = new List<Route>();
        public ICollection<Route> DistenationRoute { get; set; } = new List<Route>();





    }
}
