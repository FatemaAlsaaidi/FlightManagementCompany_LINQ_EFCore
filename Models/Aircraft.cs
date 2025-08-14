using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightManagementCompany_LINQ_EFCore.Models
{
    public class Aircraft
    {
        public int AircraftId { get; set; }
        public string TailNumber { get; set; }
        public string Model { get; set; }
        public int Capacity { get; set; }

        // navigation to flight 
        public ICollection<Flight> Flights { get; set; } = new List<Flight>();

        // navigation to AircraftMaintenance
        public ICollection<AircraftMaintenance> AircraftMaintenances { get; set; } = new List<AircraftMaintenance>();

    }
}
