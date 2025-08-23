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
        public string OriginIATA { get; set; } = string.Empty;      // Origin IATA
        public string DestinationIATA { get; set; } = string.Empty;      // Destination IATA
        public decimal Revenue { get; set; }                      // Sum of fares
        public decimal DistanceKm { get; set; }                     // Distance in kilometers
        public int SeatsSold { get; set; }                      // Count of tickets
        public string RouteCountries { get; set; } = string.Empty; // Comma-separated list of countries for the route
        public decimal AvgFare { get; set; }                      // Revenue / SeatsSold
    }
}
