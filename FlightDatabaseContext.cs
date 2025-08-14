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
            optionsBuilder.UseSqlServer("Data Source=DESKTOP-J26O8DP\\SQLEXPRESS01 ;Initial Catalog=FlightManagementDB;Integrated Security=True;TrustServerCertificate=True");
        }

        // DbSet properties for your entities
        public DbSet<Flight> Flights { get; set; } // Assuming you have a Flight model
        public DbSet<Passenger> Passengers { get; set; } // Assuming you have a Passenger model
        public DbSet<Booking> Bookings { get; set; } // Assuming you have a Booking model
        public DbSet<Airport> Airports { get; set; } // Assuming you have an Airport model
        public DbSet<Aircraft> Aircraft { get; set; } // Assuming you have an Aircraft model
        public DbSet<AircraftMaintenance> AircraftMaintenances { get; set; } // Assuming you have an AircraftMaintenance model
        public DbSet<Baggage> Baggages { get; set; } // Assuming you have a Baggage model
        public DbSet<CrewMember> CrewMembers { get; set; } // Assuming you have a CrewMember model
        public DbSet<FlightCrew> FlightCrews { get; set; } // Assuming you have a FlightCrew model
        public DbSet<Ticket> Tickets { get; set; } // Assuming you have a Ticket model
        public DbSet<Route> Routes { get; set; } // Assuming you have a Route model


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            // ===================== 1. Aircraft ===================== 
            modelBuilder.Entity<Aircraft>(b =>
            {
                b.HasIndex(a => a.TailNumber).IsUnique();
                
            });

            // ===================== 2. Airport =====================
            modelBuilder.Entity<Airport>(b =>
            {
                b.HasIndex(a => a.IATA)
                .IsUnique();
            });

            // ===================== 3. CrewMember =====================
            modelBuilder.Entity<CrewMember>(b =>
            {

                b.Property(c => c.Role)
                    .HasConversion<string>();   // converts enum to string
                    
                //enforce only allowed values at the DB level
                b.ToTable(tb =>
                {
                    tb.HasCheckConstraint(
                        "CK_CrewMember_Role",
                        "[Role] IN ('Pilot','CoPilot','FlightAttendant')");
                });

            });

            // ===================== 4. Rout =====================
            modelBuilder.Entity<Route>(b =>
            {
                b.HasKey(r => r.RouteId);

                // Origin FK (NO CASCADE)
                b.HasOne(r => r.OriginAirport)
                 .WithMany(a => a.OriginRoute)          
                 .HasForeignKey(r => r.OriginAirportId)
                 .OnDelete(DeleteBehavior.Restrict);

                // Destination FK (NO CASCADE)
                b.HasOne(r => r.DistenationAirport)      // keep the current spelling used in Airport.cs
                 .WithMany(a => a.DistenationRoute)      // or .WithMany() if you removed the attributes/collections
                 .HasForeignKey(r => r.DestinationAirportId)
                 .OnDelete(DeleteBehavior.Restrict);

                // Helpful indexes (optional)
                b.HasIndex(r => r.OriginAirportId);
                b.HasIndex(r => r.DestinationAirportId);
            });

            // ===================== 5. Flight =====================
            modelBuilder.Entity<Flight>(b =>
            {
               
                //enforce only allowed values at the DB level
                b.ToTable(tb =>
                {
                    tb.HasCheckConstraint(
                        "CK_Flight_Status",
                        "[Status] IN ('Scheduled','Delayed','Cancelled','Completed')");
                });

                b.HasOne<Route>()
                 .WithMany()                                 
                 .HasForeignKey(b => b.RouteId)
                 .OnDelete(DeleteBehavior.Restrict);



                b.HasOne<Aircraft>()
                 .WithMany()                                 
                 .HasForeignKey(b => b.AircraftId)
                 .OnDelete(DeleteBehavior.Restrict);

            });

            // ===================== 6. Passenger =====================
            modelBuilder.Entity<Passenger>(b =>
            {
                
                b.HasIndex(a => a.PassportNo)
               .IsUnique();
              
            });

            // ===================== 7. Booking =====================
            modelBuilder.Entity<Booking>(b =>
            {
               
                b.HasIndex(bk => bk.BookingRef).IsUnique();
               
                b.ToTable(tb =>
                {
                    tb.HasCheckConstraint(
                        "CK_Booking_Status",
                        "[Status] IN ('Confirmed','Cancelled','Pending')");
                });
                b.Property(bk => bk.PassengerId).IsRequired();

                b.HasOne<Passenger>()
                 .WithMany()                                 
                 .HasForeignKey(bk => bk.PassengerId)
                 .OnDelete(DeleteBehavior.Cascade);


            });

            // ===================== 8. Ticket =====================
            modelBuilder.Entity<Ticket>(b =>
            {
               
                b.ToTable(tb =>
                {
                    tb.HasCheckConstraint(
                        "CK_Ticket_CheckedIn",
                        "[CheckedIn] IN (0,1)"); // 0 for false, 1 for true
                });
               
                b.HasOne<Booking>()
                 .WithMany()                                 // or .WithMany(bk => bk.Tickets)
                 .HasForeignKey(t => t.BookingId)
                 .OnDelete(DeleteBehavior.Cascade);



                b.HasOne<Flight>()
                 .WithMany()                                 // or .WithMany(f => f.Tickets)
                 .HasForeignKey(t => t.FlightId)
                 .OnDelete(DeleteBehavior.Restrict);

            });

            // ===================== 9. Baggage =====================
            modelBuilder.Entity<Baggage>(b =>
            {
               

                b.HasOne<Ticket>()
                 .WithMany()                                
                 .HasForeignKey(b => b.TicketId)
                 .OnDelete(DeleteBehavior.Cascade);


            });

            // ===================== 10. AircraftMaintenance =====================
            modelBuilder.Entity<AircraftMaintenance>(e =>
            {

                e.HasOne(m => m.Aircraft)
                 .WithMany(a => a.AircraftMaintenances)
                 .HasForeignKey(m => m.AircraftId)
                 .OnDelete(DeleteBehavior.Restrict);

            });

            // ===================== 11. FlightCrew =====================
            modelBuilder.Entity<FlightCrew>(b =>
            {
               
                //enforce only allowed values at the DB level
                b.ToTable(tb =>
                {
                    tb.HasCheckConstraint(
                        "CK_FlightCrew_RoleOnFlight",
                        "[RoleOnFlight] IN ('Pilot','CoPilot','FlightAttendant')");
                });

                // Composite PK to prevent duplicates
                b.HasKey(fc => new { fc.FlightId, fc.CrewId });

                b.HasOne<Flight>()
                 .WithMany()                                 
                 .HasForeignKey(fc => fc.FlightId)
                 .OnDelete(DeleteBehavior.Cascade);
                b.HasIndex(fc => fc.FlightId);

                b.HasOne<CrewMember>()
                 .WithMany()                                
                 .HasForeignKey(fc => fc.CrewId)
                 .OnDelete(DeleteBehavior.Cascade);

                b.HasIndex(fc => fc.CrewId);
            });



        }

    }
}
