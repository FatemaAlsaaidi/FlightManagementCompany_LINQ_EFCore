using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightManagementCompany_LINQ_EFCore.Models
{
    public class AircraftMaintenance
    {
        public int MaintenanceId { get; set; }
        public DateTime MaintenanceDate { get; set; }
        public string Type { get; set; } // e.g., "Routine", "Repair", "Inspection"
        public string Note { get; set; } // Additional notes about the maintenance
        public int AircraftId { get; set; } // Foreign key to Aircraft table

        // navigation property to Aircraft
        public Aircraft Aircraft { get; set; } 
    }
}
