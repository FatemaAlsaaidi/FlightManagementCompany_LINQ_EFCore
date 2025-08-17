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

        public int AircraftId { get; set; }
        public string TailNumber { get; set; } = null!;
        public string Model { get; set; } = null!;
        public int Capacity { get; set; }

        public ICollection<Flight> Flights { get; set; } = new List<Flight>();
        public ICollection<AircraftMaintenance> AircraftMaintenances { get; set; } = new List<AircraftMaintenance>();

    }
}
