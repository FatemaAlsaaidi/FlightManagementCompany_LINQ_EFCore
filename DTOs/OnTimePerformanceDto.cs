using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightManagementCompany_LINQ_EFCore.DTOs
{
    public class OnTimePerformanceDto
    {
        // Grouping key, e.g. "MCT->DXB" (per route) or any label you choose
        public string Key { get; set; } = string.Empty;

        // How many flights in the group
        public int Flights { get; set; }

        // How many were on time (within threshold minutes)
        public int OnTime { get; set; }

        // Convenience computed property
        public double OnTimePercent => Flights == 0 ? 0 : (OnTime * 100.0) / Flights;

        public RouteDto Route { get; set; } = new(); // Route information
    }
    public class RouteDto
    {
        public string OriginIATA { get; set; } = string.Empty;      // Origin IATA
        public string DestinationIATA { get; set; } = string.Empty;      // Destination IATA


    }
}
