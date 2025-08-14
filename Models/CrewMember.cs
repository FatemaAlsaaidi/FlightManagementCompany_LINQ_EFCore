using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// using data annotation 
using System.ComponentModel.DataAnnotations;

namespace FlightManagementCompany_LINQ_EFCore.Models
{
    public class CrewMember
    {
        [Key]
        public int CrewId { get; set; }
        [Required, StringLength(20)]
        public string Fname { get; set; }
        [Required, StringLength(20)]

        public string Lname { get; set; }
        [Required, StringLength(10)]

        public CrewRole Role { get; set; }  // use enum here
        [Required, StringLength(10)]
                                            
        public string LicenseNo { get; set; } // For pilots, the license number

        // navigation property to FlightCrew
        public ICollection<FlightCrew> FlightCrews { get; set; } = new List<FlightCrew>();
    }
    // Enum for crew roles

    public enum CrewRole
    {
        Pilot,
        CoPilot,
        FlightAttendant
    }
}
