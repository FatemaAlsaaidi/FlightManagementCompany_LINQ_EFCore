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
        [Key]
        public int AirportId { get; set; }
        [Required, StringLength (20)]
        public string Name { get; set; }
        [Required, StringLength(3)]
        public string IATA { get; set; }
        [Required, StringLength(20)]
        public string City { get; set; }
        [Required, StringLength(20)]
        public string Country { get; set; }
        [Required, StringLength(20)]
        public string TimeZone { get; set; }

        


        [InverseProperty("OriginAirport")]
        public ICollection<Route> OriginRoute { get; set; } = new List<Route>();

        [InverseProperty("DistenationAirport")]
        public ICollection<Route> DistenationRoute { get; set; } = new List<Route>();





    }
}
