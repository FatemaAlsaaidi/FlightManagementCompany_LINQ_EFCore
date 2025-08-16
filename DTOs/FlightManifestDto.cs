using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightManagementCompany_LINQ_EFCore.DTOs
{
    public class FlightManifestDto
    {
        public string FlightNumber { get; set; } = string.Empty;
        public string Origin { get; set; } = string.Empty;      // IATA of origin
        public string Destination { get; set; } = string.Empty;      // IATA of destination
        public DateTime DepUtc { get; set; }
        public DateTime ArrUtc { get; set; }
        public string AircraftTail { get; set; } = string.Empty;
        public int PassengerCount { get; set; }
        public decimal TotalBaggageKg { get; set; }
        public List<CrewDto> Crew { get; set; } = new();

        public class CrewDto
        {
            public string Name { get; set; } = string.Empty; // e.g., "Ali Hamed"
            public string Role { get; set; } = string.Empty; // e.g., "Pilot", "CoPilot", "FlightAttendant"
        }
    }
}
