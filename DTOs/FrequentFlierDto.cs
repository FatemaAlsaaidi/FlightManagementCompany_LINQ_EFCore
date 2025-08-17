using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightManagementCompany_LINQ_EFCore.DTOs
{
    public class FrequentFlierDto
    {
        public int PassengerId { get; set; }
        public string PassengerName { get; set; } = string.Empty;
        public int FlightsCount { get; set; }
        public int TotalDistanceKm { get; set; }   // when byDistance = true
    }
}
