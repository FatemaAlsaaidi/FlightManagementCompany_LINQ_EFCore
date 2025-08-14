using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// using annotion 
using System.ComponentModel.DataAnnotations;

namespace FlightManagementCompany_LINQ_EFCore.Models
{
    public class Aircraft
    {
        // Primary key
        [Key]
        public int AircraftId { get; set; }
        // Properties
        [Required, StringLength(10)]
        public string TailNumber { get; set; }
        [Required, StringLength(20)]
        public string Model { get; set; }
        [Required]
        public int Capacity { get; set; }

        // navigation to flight 
        public ICollection<Flight> Flights { get; set; } = new List<Flight>();

        // navigation to AircraftMaintenance
        public ICollection<AircraftMaintenance> AircraftMaintenances { get; set; } = new List<AircraftMaintenance>();

    }
}
