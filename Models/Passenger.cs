using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightManagementCompany_LINQ_EFCore.Models
{
    public class Passenger
    {
        public int PassengerId { get; set; }
        public string Fname { get; set; } // First name of the passenger
        public string Lname { get; set; } // Last name of the passenger
        public string PassportNo { get; set; } // Unique passport number
        public string Nationality { get; set; } 
        public DateOnly DOB { get; set; } // Date of birth of the passenger
    }
}
