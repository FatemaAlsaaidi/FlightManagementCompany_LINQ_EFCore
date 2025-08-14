using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace FlightManagementCompany_LINQ_EFCore.Models
{
    public class Booking
    {
        [Key]
        public int BookingId { get; set; }
        [Required, StringLength(20)]
        public string BookingRef { get; set; }
        [Required]
        public DateTime BookingDate { get; set; }
        [Required, StringLength(20)]
        public string status { get; set; }

        [Required]
        public int PassengerId { get; set; } // Foreign key to Passenger table

        // navigation to  passenger
        public Passenger Passenger { get; set; }

        // navigation to tickets
        public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();

    }
}
