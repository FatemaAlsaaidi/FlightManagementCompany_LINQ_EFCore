# FlightManagementCompany_LINQ_EFCore
# Project Summary 
Build a Flight Management System (FMS) backend for a company that manages flights, aircraft, crews, bookings, 
passengers and airports — implemented using layered architecture, EF Core, Repository pattern, and LINQ.

# Learning Goals 
- Design and document an ERD and relational schema. 
- Implement EF Core models with annotations and relationships. 
- Implement layered architecture: Presentation / Service / Repository / Data (DbContext). 
- Implement per-entity repositories exposing essential CRUD + query methods. 
- Write LINQ queries: joins, groupings, aggregations, partitioning, projection to DTOs, hierarchical queries

# Business Domain & ERD (entities + relationships)

### Entities (core): 
**Airport**
• AirportId (int PK) 
• IATA (string, 3, unique) 
• Name (string) 
• City (string) 
• Country (string) 
• TimeZone (string) 
**Aircraft**
• AircraftId (int PK) 
• TailNumber (string, unique) 
• Model (string) 
• Capacity (int) 
**CrewMember**
• CrewId (int PK) 
• FullName (string) 
• Role (enum/string) — Pilot/CoPilot/FlightAttendant 
• LicenseNo (string, nullable) 
**Route** 
• RouteId (int PK) 
• DistanceKm (int) 
**Flight** 
• FlightId (int PK) 
• FlightNumber (string) — e.g., "FM101" 
• DepartureUtc (DateTime) 
• ArrivalUtc (DateTime) 
• Status (string/enum) 
• Note: add unique constraint on (FlightNumber, DepartureUtc.Date) 
**Passenger** 
• PassengerId (int PK) 
• FullName (string) 
• PassportNo (string, unique) 
• Nationality (string) 
• DOB (DateTime) 
**Booking**
• BookingId (int PK) 
• BookingRef (string, unique) 
• BookingDate (DateTime) 
• Status (string) 
**Ticket** 
• TicketId (int PK) 
• SeatNumber (string) 
• Fare (decimal) 
• CheckedIn (bool) 
**FlightCrew ➔ relationship attributes on many to many** 
• RoleOnFlight (string) 
• Primary Key (FlightId, CrewId) 
**Baggage** 
• BaggageId (int PK) 
• TicketId (FK → Ticket) 
• WeightKg (decimal) 
• TagNumber (string) 
**AircraftMaintenance** 
• MaintenanceId (int PK) 
• MaintenanceDate (DateTime) 
• Type (string) 
• Notes (string) 

### Relationships (high level): 
- Airport 1..* Route (as origin and destination) — Route has two FKs to Airport. 
- Route 1..* Flight 
- Aircraft 1..* Flight 
- Flight .. CrewMember via FlightCrew (many-to-many) 
- Passenger 1..* Booking 
- Booking 1..* Ticket (one booking could book multiple segments/flights) 
- Flight 1..* Ticket 
- Ticket 1..* Baggage 
- Aircraft 1..* AircraftMaintenance
