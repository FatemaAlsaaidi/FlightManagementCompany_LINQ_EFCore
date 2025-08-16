using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightManagementCompany_LINQ_EFCore.DTOs
{
    public class RouteRevenueDto
    {
        public int RouteId { get; set; }
        public string Origin { get; set; } = string.Empty;      // Origin IATA
        public string Destination { get; set; } = string.Empty;      // Destination IATA
        public decimal Revenue { get; set; }                      // Sum of fares
        public int SeatsSold { get; set; }                      // Count of tickets
        public decimal AvgFare { get; set; }                      // Revenue / SeatsSold
    }
}
