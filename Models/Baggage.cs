using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace FlightManagementCompany_LINQ_EFCore.Models
{
    public class Baggage
    {
        [Key]
        public int BaggageId { get; set; }
        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal WeightKg { get; set; }
        [Required, StringLength(20)]
        public string TagNumber { get; set; }
       [Required]

        public int TicketId { get; set; } // Foreign key to Ticket table

        // navigation to ticket
        public Ticket Ticket { get; set; }

    }
}
