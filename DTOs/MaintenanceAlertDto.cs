using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightManagementCompany_LINQ_EFCore.DTOs
{
    public class MaintenanceAlertDto
    {
        public int AircraftId { get; set; }
        public string TailNumber { get; set; } = string.Empty;
        public double TotalHours { get; set; }
        public int DaysSinceLastMaintenance { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}
