using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightManagementCompany_LINQ_EFCore.Models
{
    public class FlightCrew
    {
        public string RoleOnFlight { get; set; } // Role of the crew member on the flight (e.g., Pilot, Flight Attendant)
        public int CrewId { get; set; } // Unique identifier for the crew member
        public int FlightId { get; set; } // Foreign key to the Flight table

        public Flight Flight { get; set; } // Navigation property to Flight

        public CrewMember CrewMember { get; set; } // Navigation property to CrewMember
    }
}
