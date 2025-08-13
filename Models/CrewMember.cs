using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightManagementCompany_LINQ_EFCore.Models
{
    public class CrewMember
    {
        public int CrewId { get; set; }
        public string Fname { get; set; }

        public string Lname { get; set; }

        public string Role { get; set; } // e.g., Pilot, Co-Pilot, Flight Attendant
        public string LicenseNo { get; set; } // For pilots, the license number
    }
}
