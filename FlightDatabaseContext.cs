using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace FlightManagementCompany_LINQ_EFCore
{
    public class FlightDatabaseContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=DESKTOP-B1EOQP1 ;Initial Catalog=FlightManagementDB;Integrated Security=True;TrustServerCertificate=True");
        }
    }
}
