using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;


namespace FlightManagementCompany_LINQ_EFCore.Models
{
    public class Passenger
    {
        [Key]
        public int PassengerId { get; set; }
        [Required, StringLength(20)]
        public string Fname { get; set; } // First name of the passenger
        [Required, StringLength(20)]
        public string Lname { get; set; } // Last name of the passenger
        [Required, StringLength(15)]
        public string PassportNo { get; set; } // Unique passport number
        [Required, StringLength(15)]
        public string Nationality { get; set; } 
        [Required]
        public DateOnly DOB { get; set; } // Date of birth of the passenger

        // Navigation to Booking
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    }
}
