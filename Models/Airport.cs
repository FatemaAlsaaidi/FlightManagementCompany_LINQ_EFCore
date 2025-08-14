using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightManagementCompany_LINQ_EFCore.Models
{
    public class Airport
    {
        public int AirportId { get; set; }
        public string Name { get; set; }
        public string IATA { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string TimeZone { get; set; }

        [InverseProperty("OriginAirport")]
        public ICollection<Route> OriginRoute { get; set; } = new List<Route>();// navigation to Origin Route

        [InverseProperty("DistenationAirport")]
        public ICollection<Route> DistenationRoute { get; set; } = new List<Route>();// navigation to Distenation Route




    }
}
