using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightManagementCompany_LINQ_EFCore.DTOs
{
    public class BaggageOverweightDto
    {
        public int TicketId { get; set; }
        public string FlightNumber { get; set; } = string.Empty;
        public string PassengerName { get; set; } = string.Empty;
        public decimal TotalBaggageKg { get; set; }
    }
}
