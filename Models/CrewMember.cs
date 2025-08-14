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

        public CrewRole Role { get; set; }  // use enum here
                                            
        public string LicenseNo { get; set; } // For pilots, the license number
    }
    // Enum for crew roles

    public enum CrewRole
    {
        Pilot,
        CoPilot,
        FlightAttendant
    }
}
