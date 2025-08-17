using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightManagementCompany_LINQ_EFCore.DTOs
{
    public class SimpleRouteDto
    {
        public int RouteId { get; set; }
        public string Origin { get; set; } = string.Empty;      // IATA
        public string Destination { get; set; } = string.Empty;      // IATA
    }
}
