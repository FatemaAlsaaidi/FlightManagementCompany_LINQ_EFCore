using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FlightManagementCompany_LINQ_EFCore.Models; // Ensure this namespace matches your Flight model's namespace

namespace FlightManagementCompany_LINQ_EFCore
{
    public class FlightDatabaseContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=DESKTOP-B1EOQP1 ;Initial Catalog=FlightManagementDB;Integrated Security=True;TrustServerCertificate=True");
        }

        // DbSet properties for your entities
        public DbSet<Flight> Flights { get; set; } // Assuming you have a Flight model
        public DbSet<Passenger> Passengers { get; set; } // Assuming you have a Passenger model
        public DbSet<Booking> Bookings { get; set; } // Assuming you have a Booking model
        public DbSet<Airport> Airports { get; set; } // Assuming you have an Airport model
        public DbSet<Aircraft> Airlines { get; set; } // Assuming you have an Aircraft model
        public DbSet<AircraftMaintenance> AircraftMaintenances { get; set; } // Assuming you have an AircraftMaintenance model
        public DbSet<Baggage> Baggages { get; set; } // Assuming you have a Baggage model
        public DbSet<CrewMember> CrewMembers { get; set; } // Assuming you have a CrewMember model
        public DbSet<FlightCrew> FlightCrews { get; set; } // Assuming you have a FlightCrew model
        public DbSet<Route> FlightStatuses { get; set; } // Assuming you have a Route model
        public DbSet<Ticket> Tickets { get; set; } // Assuming you have a Ticket model


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            // ===================== 1. Aircraft ===================== 
            modelBuilder.Entity<Aircraft>(b =>
            {
                b.HasKey("AircraftId");
                b.Property(a => a.AircraftId).ValueGeneratedOnAdd(); // Auto-increment primary key  

                b.HasIndex(a => a.TailNumber).IsUnique();
                b.Property(a => a.Model).IsRequired().HasMaxLength(50);
                b.Property(a => a.Capacity).IsRequired();
            });

            // ===================== 2. Airport =====================
            modelBuilder.Entity<Airport>(b =>
            {
                b.HasKey("AirportId");
                b.Property(a => a.AirportId).ValueGeneratedOnAdd(); // Auto-increment primary key  
                b.Property(a => a.IATA)
                .IsRequired()
                .HasMaxLength(3)      // <= THIS prevents NVARCHAR(MAX) so indexing works
                .IsFixedLength();     // maps to CHAR(3) on SQL Server
                b.HasIndex(a => a.IATA)
                .IsUnique();
                b.Property(a => a.Name).IsRequired().HasMaxLength(100);
                b.Property(a => a.City).IsRequired().HasMaxLength(50);
                b.Property(a => a.Country).IsRequired().HasMaxLength(50);
                b.Property(a => a.TimeZone).IsRequired().HasMaxLength(50);
            });

            // ===================== 3. CrewMember =====================
            modelBuilder.Entity<CrewMember>(b =>
            {
                b.HasKey("CrewId");
                b.Property(c => c.CrewId).ValueGeneratedOnAdd(); // Auto-increment primary key  
                b.Property(c => c.Fname).IsRequired().HasMaxLength(50);
                b.Property(c => c.Lname).IsRequired().HasMaxLength(50);
                b.Property(c => c.Role)
                    .HasConversion<string>()    // converts enum to string
                    .IsRequired()
                    .HasMaxLength(20);
                //enforce only allowed values at the DB level
                b.ToTable(tb =>
                {
                    tb.HasCheckConstraint(
                        "CK_CrewMember_Role",
                        "[Role] IN ('Pilot','CoPilot','FlightAttendant')");
                });

                b.Property(c => c.LicenseNo).HasMaxLength(50); // Optional for non-pilot crew members
            });

            // ===================== 4. Rout =====================
            modelBuilder.Entity<Route>(b =>
            {
                b.HasKey("RouteId");
                b.Property(r => r.RouteId).ValueGeneratedOnAdd(); // Auto-increment primary key  
                b.Property(r => r.DistanceKm).IsRequired();
                b.Property(r => r.OriginCity).IsRequired();
                b.Property(r => r.DestinationCity).IsRequired();
            });

           


        }

    }
}
