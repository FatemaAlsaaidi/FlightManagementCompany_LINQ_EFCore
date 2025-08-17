using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightManagementCompany_LINQ_EFCore.DTOs
{
    public class OccupancyDto
    {
        public int FlightId { get; set; }
        public string FlightNumber { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public int Sold { get; set; }

        // Percentage (0–100). Defensive against divide-by-zero.
        public double Occupancy => Capacity == 0 ? 0 : (Sold * 100.0 / Capacity);
    }
}
