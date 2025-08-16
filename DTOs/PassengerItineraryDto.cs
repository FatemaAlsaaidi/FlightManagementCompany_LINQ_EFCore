using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightManagementCompany_LINQ_EFCore.DTOs
{
    public class PassengerItineraryDto
    {

        public int PassengerId { get; set; }
        public string PassengerName { get; set; } = string.Empty;
        public List<ItinSegmentDto> Segments { get; set; } = new();
        

       
    }

    public class ItinSegmentDto
    {
        public int FlightId { get; set; }
        public string FlightNumber { get; set; } = string.Empty;
        public string OriginIata { get; set; } = string.Empty;
        public string DestinationIata { get; set; } = string.Empty;
        public DateTime DepUtc { get; set; }
        public DateTime ArrUtc { get; set; }
    }
}
