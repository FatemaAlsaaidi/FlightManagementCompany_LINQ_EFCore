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
                b.HasKey(r => r.RouteId);
                b.Property(r => r.RouteId).ValueGeneratedOnAdd(); // Auto-increment primary key  
                b.Property(r => r.DistanceKm).IsRequired();
                b.Property(r => r.OriginOriginAirportId).IsRequired();
                b.Property(r => r.DestinationAirportId).IsRequired();

                // Origin FK
                b.HasOne<Airport>()                          // or .HasOne(r => r.Origin) if you have a nav property
                 .WithMany()                                 // or .WithMany(a => a.OriginRoutes)
                 .HasForeignKey(r => r.OriginOriginAirportId)           // shadow FK if not in your class
                 .OnDelete(DeleteBehavior.Restrict);         // avoid cascade cycles

                // Destination FK
                b.HasOne<Airport>()                          // or .HasOne(r => r.Destination)
                 .WithMany()                                 // or .WithMany(a => a.DestinationRoutes)
                 .HasForeignKey(r => r.DestinationAirportId)
                 .OnDelete(DeleteBehavior.Restrict);

                // Helpful indexes on FKs
                b.HasIndex(r => r.OriginOriginAirportId);
                b.HasIndex(r => r.DestinationAirportId);
            });

            // ===================== 5. Flight =====================
            modelBuilder.Entity<Flight>(b =>
            {
                b.HasKey(f => f.FlightId);
                b.Property(f => f.FlightId).ValueGeneratedOnAdd(); // Auto-increment primary key  
                b.Property(f => f.FlightNumber).IsRequired().HasMaxLength(10);
                b.Property(f => f.DepartureUtc).IsRequired();
                b.Property(f => f.ArrivalUtc).IsRequired();
                b.Property(f => f.RouteId).IsRequired(); // Foreign key to Route table
                b.Property(f => f.AircraftId).IsRequired(); // Foreign key to Aircraft table

                b.Property(f => f.Status)
                    .HasConversion<string>()    // converts enum to string
                    .IsRequired()
                    .HasMaxLength(20);
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

                b.HasIndex(b => b.RouteId);


                b.HasOne<Aircraft>()
                 .WithMany()                                 
                 .HasForeignKey(b => b.AircraftId)
                 .OnDelete(DeleteBehavior.Restrict);

                b.HasIndex(b => b.AircraftId);
            });

            // ===================== 6. Passenger =====================
            modelBuilder.Entity<Passenger>(b =>
            {
                b.HasKey("PassengerId");
                b.Property(p => p.PassengerId).ValueGeneratedOnAdd(); // Auto-increment primary key  
                b.Property(p => p.Fname).IsRequired().HasMaxLength(50);
                b.Property(p => p.Lname).IsRequired().HasMaxLength(50);
                b.Property(p => p.PassportNo).IsRequired().HasMaxLength(100);
                b.HasIndex(a => a.PassportNo)
               .IsUnique();
                b.Property(p => p.Nationality).HasMaxLength(15); // Optional
                b.Property(p => p.DOB).HasMaxLength(50); 
            });

            // ===================== 7. Booking =====================
            modelBuilder.Entity<Booking>(b =>
            {
                b.HasKey(bk => bk.BookingId);
                b.Property(bk => bk.BookingId).ValueGeneratedOnAdd(); // Auto-increment primary key  
                b.Property(bk => bk.BookingRef).IsRequired();
                b.Property(bk => bk.BookingDate).IsRequired();
                b.Property(bk => bk.status).HasConversion<string>()    // converts enum to string
                    .IsRequired()
                    .HasMaxLength(20);
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

                b.HasIndex(bk => bk.PassengerId);

            });

            // ===================== 8. Ticket =====================
            modelBuilder.Entity<Ticket>(b =>
            {
                b.HasKey(t => t.TicketId);
                b.Property(t => t.TicketId).ValueGeneratedOnAdd(); // Auto-increment primary key  
                b.Property(t => t.SeatNumber).IsRequired().HasMaxLength(10);
                b.Property(t => t.Fare).IsRequired().HasColumnType("decimal(18,2)");
                b.Property(t => t.CheckedIn).IsRequired();
                b.ToTable(tb =>
                {
                    tb.HasCheckConstraint(
                        "CK_Ticket_CheckedIn",
                        "[CheckedIn] IN (0,1)"); // 0 for false, 1 for true
                });
                b.Property(t=> t.BookingId).IsRequired();
                b.Property(t=> t.FlightId).IsRequired();

                b.HasOne<Booking>()
                 .WithMany()                                 // or .WithMany(bk => bk.Tickets)
                 .HasForeignKey(t => t.BookingId)
                 .OnDelete(DeleteBehavior.Cascade);

                b.HasIndex(t => t.BookingId);


                b.HasOne<Flight>()
                 .WithMany()                                 // or .WithMany(f => f.Tickets)
                 .HasForeignKey(t => t.FlightId)
                 .OnDelete(DeleteBehavior.Restrict);

                b.HasIndex(t => t.FlightId);
            });

            // ===================== 9. Baggage =====================
            modelBuilder.Entity<Baggage>(b =>
            {
                b.HasKey("BaggageId");
                b.Property(b => b.BaggageId).ValueGeneratedOnAdd(); // Auto-increment primary key  
                b.Property(b => b.WeightKg).IsRequired();
                b.Property(b => b.TagNumber).IsRequired().HasMaxLength(50);
                b.Property(b=> b.TicketId).IsRequired();

                b.HasOne<Ticket>()
                 .WithMany()                                 // or .WithMany(t => t.BaggageItems)
                 .HasForeignKey(b => b.TicketId)
                 .OnDelete(DeleteBehavior.Cascade);

                b.HasIndex(b => b.TicketId);

            });

            // ===================== 10. AircraftMaintenance =====================
            modelBuilder.Entity<AircraftMaintenance>(AM =>
            {
                AM.HasKey("MaintenanceId");
                AM.Property(am => am.MaintenanceId).ValueGeneratedOnAdd(); // Auto-increment primary key  
                AM.Property(am => am.MaintenanceDate).IsRequired();
                AM.Property(am => am.Type).IsRequired().HasMaxLength(200);
                AM.Property(am => am.Note).IsRequired().HasMaxLength(500);
                AM.Property(am => am.AircraftId).IsRequired();

                AM.HasOne<Aircraft>()
                 .WithMany()                                 // or .WithMany(a => a.Maintenances)
                 .HasForeignKey(am => am.AircraftId)
                 .OnDelete(DeleteBehavior.Cascade);

                AM.HasIndex(am => am.AircraftId);

            });

            // ===================== 11. FlightCrew =====================
            modelBuilder.Entity<FlightCrew>(b =>
            {
                b.Property(fc => fc.FlightId).IsRequired();
                b.Property(fc => fc.CrewId).IsRequired();
                b.Property(fc=> fc.RoleOnFlight)
                 .HasConversion<string>()    // converts enum to string
                 .IsRequired()
                 .HasMaxLength(20);

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
