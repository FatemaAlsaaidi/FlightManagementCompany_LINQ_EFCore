using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// using data annotation


namespace FlightManagementCompany_LINQ_EFCore.Models
{
    public class AircraftMaintenance
    {
        [Key]
        public int MaintenanceId { get; set; }
        [Required]
        public DateTime MaintenanceDate { get; set; }
        [Required]
        public string Type { get; set; } // e.g., "Routine", "Repair", "Inspection"
        [Required]
        public string Note { get; set; } // Additional notes about the maintenance
        [Required]
        public int AircraftId { get; set; } // Foreign key to Aircraft table

        // navigation property to Aircraft
        public Aircraft Aircraft { get; set; } 
    }
}
