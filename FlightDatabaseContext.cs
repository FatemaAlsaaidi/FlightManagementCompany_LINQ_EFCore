using System;
using Microsoft.EntityFrameworkCore;
using FlightManagementCompany_LINQ_EFCore.Models;

namespace FlightManagementCompany_LINQ_EFCore
{
    public class FlightDatabaseContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=DESKTOP-J26O8DP\\SQLEXPRESS01 ;Initial Catalog=FlightManagementDB;Integrated Security=True;TrustServerCertificate=True");
        }

        // ===================== DbSets =====================
        public DbSet<Airport> Airports { get; set; }
        public DbSet<Aircraft> Aircraft { get; set; }
        public DbSet<Route> Routes { get; set; }
        public DbSet<Flight> Flights { get; set; }
        public DbSet<Passenger> Passengers { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Baggage> Baggages { get; set; }
        public DbSet<AircraftMaintenance> AircraftMaintenances { get; set; }
        public DbSet<CrewMember> CrewMembers { get; set; }
        public DbSet<FlightCrew> FlightCrews { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ===================== AIRCRAFT =====================
            modelBuilder.Entity<Aircraft>(b =>
            {
                b.HasKey(a => a.AircraftId);
                b.Property(a => a.TailNumber).IsRequired().HasMaxLength(10);
                b.Property(a => a.Model).IsRequired().HasMaxLength(20);
                b.Property(a => a.Capacity).IsRequired();
                b.HasIndex(a => a.TailNumber).IsUnique();
            });

            // ===================== AIRPORT =====================
            modelBuilder.Entity<Airport>(b =>
            {
                b.HasKey(a => a.AirportId);
                b.Property(a => a.Name).IsRequired().HasMaxLength(20);
                b.Property(a => a.IATA).IsRequired().HasMaxLength(3);
                b.Property(a => a.City).IsRequired().HasMaxLength(20);
                b.Property(a => a.Country).IsRequired().HasMaxLength(20);
                b.Property(a => a.TimeZone).IsRequired().HasMaxLength(20);
                b.HasIndex(a => a.IATA).IsUnique();
            });

            // ===================== CREWMEMBER =====================
            modelBuilder.Entity<CrewMember>(b =>
            {
                b.HasKey(c => c.CrewId);
                b.Property(c => c.Fname).IsRequired().HasMaxLength(20);
                b.Property(c => c.Lname).IsRequired().HasMaxLength(20);

                // If Role is enum, convert to string for the DB (required for your check constraint)
                b.Property(c => c.Role)
                 .IsRequired()
                 .HasMaxLength(20)
                 .HasConversion<string>();

                b.Property(c => c.LicenseNo).IsRequired().HasMaxLength(10);

                b.ToTable(tb =>
                {
                    tb.HasCheckConstraint(
                        "CK_CrewMember_Role",
                        "[Role] IN ('Pilot','CoPilot','FlightAttendant')");
                });


            });


            // ===================== PASSENGER =====================
            modelBuilder.Entity<Passenger>(b =>
            {
                b.HasKey(p => p.PassengerId);
                b.Property(p => p.Fname).IsRequired().HasMaxLength(20);
                b.Property(p => p.Lname).IsRequired().HasMaxLength(20);
                b.Property(p => p.PassportNo).IsRequired().HasMaxLength(15);
                b.Property(p => p.Nationality).IsRequired().HasMaxLength(15);
                b.Property(p => p.DOB).HasColumnType("date").IsRequired();
                b.HasIndex(p => p.PassportNo).IsUnique();
            });

            // ===================== ROUTE =====================
            modelBuilder.Entity<Route>(b =>
            {
                b.HasKey(r => r.RouteId);
                b.Property(r => r.DistanceKm).IsRequired();
                b.Property(r => r.OriginAirportId).IsRequired();
                b.Property(r => r.DestinationAirportId).IsRequired();

                // Map BOTH airport relationships explicitly (current property names kept)
                b.HasOne(r => r.OriginAirport)
                 .WithMany(a => a.OriginRoute)
                 .HasForeignKey(r => r.OriginAirportId)
                 .OnDelete(DeleteBehavior.Restrict)
                 .HasConstraintName("FK_Routes_Airports_OriginAirportId");

                b.HasOne(r => r.DistenationAirport)
                 .WithMany(a => a.DistenationRoute)
                 .HasForeignKey(r => r.DestinationAirportId)
                 .OnDelete(DeleteBehavior.Restrict)
                 .HasConstraintName("FK_Routes_Airports_DestinationAirportId");

                b.HasIndex(r => r.OriginAirportId);
                b.HasIndex(r => r.DestinationAirportId);
            });

            // ===================== FLIGHT =====================
            modelBuilder.Entity<Flight>(b =>
            {
                b.HasKey(f => f.FlightId);
                b.Property(f => f.FlightNumber).IsRequired().HasMaxLength(10);
                b.Property(f => f.DepartureUtc).IsRequired();
                b.Property(f => f.ArrivalUtc).IsRequired();
                b.Property(f => f.Status).IsRequired().HasMaxLength(20);
                b.Property(f => f.RouteId).IsRequired();
                b.Property(f => f.AircraftId).IsRequired();

                // Status check
                b.ToTable(tb =>
                {
                    tb.HasCheckConstraint(
                        "CK_Flight_Status",
                        "[Status] IN ('Scheduled','Delayed','Cancelled','Completed')");
                });

                // CRITICAL: bind nav pair to avoid RouteId1
                b.HasOne(f => f.Route)
                 .WithMany(r => r.Flights)
                 .HasForeignKey(f => f.RouteId)
                 .OnDelete(DeleteBehavior.Restrict)
                 .HasConstraintName("FK_Flights_Routes_RouteId");

                b.HasOne(f => f.Aircraft)
                 .WithMany(a => a.Flights)
                 .HasForeignKey(f => f.AircraftId)
                 .OnDelete(DeleteBehavior.Cascade)
                 .HasConstraintName("FK_Flights_Aircraft_AircraftId");

                // Optional indexes
                b.HasIndex(f => f.AircraftId);
                b.HasIndex(f => f.RouteId);
            });

            // ===================== BOOKING =====================
            modelBuilder.Entity<Booking>(b =>
            {
                b.HasKey(x => x.BookingId);
                b.Property(x => x.BookingRef).IsRequired().HasMaxLength(20);
                b.Property(x => x.BookingDate).IsRequired();
                // If your property is "status" (lowercase), map to column "Status" for constraint compatibility
                b.Property(x => x.status).IsRequired().HasMaxLength(20).HasColumnName("Status");
                b.Property(x => x.PassengerId).IsRequired();

                b.HasIndex(x => x.BookingRef).IsUnique();
                b.HasIndex(x => x.PassengerId);

                b.ToTable(tb =>
                {
                    tb.HasCheckConstraint(
                        "CK_Booking_Status",
                        "[Status] IN ('Confirmed','Cancelled','Pending')");
                });

                b.HasOne(bk => bk.Passenger)
                    .WithMany(p => p.Bookings)
                    .HasForeignKey(bk => bk.PassengerId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Bookings_Passengers_PassengerId");
            });

            // ===================== TICKET =====================
            modelBuilder.Entity<Ticket>(b =>
            {
                b.HasKey(t => t.TicketId);
                b.Property(t => t.SeatNumber).IsRequired().HasMaxLength(20);
                b.Property(t => t.Fare).HasColumnType("decimal(18,2)").IsRequired();
                b.Property(t => t.CheckedIn).IsRequired();
                b.Property(t => t.BookingId).IsRequired();
                b.Property(t => t.FlightId).IsRequired();

                b.ToTable(tb =>
                {
                    tb.HasCheckConstraint(
                        "CK_Ticket_CheckedIn",
                        "[CheckedIn] IN (0,1)");
                });

                b.HasOne(t => t.Booking)
                  .WithMany(bk => bk.Tickets)
                  .HasForeignKey(t => t.BookingId)
                  .OnDelete(DeleteBehavior.Restrict)
                  .HasConstraintName("FK_Tickets_Bookings_BookingId");

                b.HasOne(t => t.Flight)
                 .WithMany(f => f.Tickets)
                 .HasForeignKey(t => t.FlightId)
                 .OnDelete(DeleteBehavior.Restrict)
                 .HasConstraintName("FK_Tickets_Flights_FlightId");

                b.HasIndex(t => t.BookingId);
                b.HasIndex(t => t.FlightId);
            });

            // ===================== BAGGAGE =====================
            modelBuilder.Entity<Baggage>(b =>
            {
                b.HasKey(x => x.BaggageId);

                b.Property(x => x.WeightKg).HasColumnType("decimal(18,2)").IsRequired();
                b.Property(x => x.TagNumber).HasMaxLength(20).IsRequired();
                b.Property(x => x.TicketId).IsRequired();

                b.HasOne(x => x.Ticket)
                  .WithMany()                       // <-- no collection at all
                  .HasForeignKey(x => x.TicketId)
                  .OnDelete(DeleteBehavior.Cascade)
                  .HasConstraintName("FK_Baggages_Tickets_TicketId");
            });


            // ===================== AIRCRAFT MAINTENANCE =====================
            modelBuilder.Entity<AircraftMaintenance>(e =>
            {
                e.HasKey(x => x.MaintenanceId);
                e.Property(x => x.MaintenanceDate).IsRequired();
                e.Property(x => x.Type).IsRequired();
                e.Property(x => x.Note).IsRequired();
                e.Property(x => x.AircraftId).IsRequired();

                e.HasOne(m => m.Aircraft)
                 .WithMany(a => a.AircraftMaintenances)
                 .HasForeignKey(m => m.AircraftId)
                 .OnDelete(DeleteBehavior.Restrict)
                 .HasConstraintName("FK_AircraftMaintenances_Aircraft_AircraftId");

                e.HasIndex(x => x.AircraftId);
            });

            // ===================== FLIGHT CREW =====================
            modelBuilder.Entity<FlightCrew>(b =>
            {
                b.HasKey(fc => new { fc.FlightId, fc.CrewId });
                b.Property(fc => fc.RoleOnFlight).IsRequired().HasMaxLength(20);

                b.ToTable(tb =>
                {
                    tb.HasCheckConstraint(
                        "CK_FlightCrew_RoleOnFlight",
                        "[RoleOnFlight] IN ('Pilot','CoPilot','FlightAttendant')");
                });

                b.HasOne(fc => fc.Flight)
                 .WithMany(f => f.FlightCrews)
                 .HasForeignKey(fc => fc.FlightId)
                 .OnDelete(DeleteBehavior.Cascade)
                 .HasConstraintName("FK_FlightCrews_Flights_FlightId");

                b.HasOne(fc => fc.CrewMember)
                 .WithMany(c => c.FlightCrews)
                 .HasForeignKey(fc => fc.CrewId)
                 .OnDelete(DeleteBehavior.Restrict)
                 .HasConstraintName("FK_FlightCrews_CrewMembers_CrewId");

                b.HasIndex(fc => fc.FlightId);
                b.HasIndex(fc => fc.CrewId);
            });
        }
    }
}
