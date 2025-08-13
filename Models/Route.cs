using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightManagementCompany_LINQ_EFCore.Models
{
    public class Route
    {
        public int RouteId { get; set; }
        public int DistanceKm { get; set; } // Distance in kilometers
        public string OriginCity { get; set; } // City of origin
        public string DestinationCity { get; set; } // City of destination
    }
}
