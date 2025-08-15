using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightManagementCompany_LINQ_EFCore.Repositories
{
    public class FlightCrew
    {
        // database injection
        private readonly FlightDatabaseContext _context;
        public FlightCrew(FlightDatabaseContext context)
        {
            _context = context;
        }

        // Get all flight crews
        public IEnumerable<Models.FlightCrew> GetAllFlightCrews()
        {
            return _context.FlightCrews;
        }

        // Get flight crew by ID
        public Models.FlightCrew GetFlightCrewById(int CrewMemberId, int FlightId)
        {
            return _context.FlightCrews.FirstOrDefault(fc => fc.CrewId == CrewMemberId && fc.FlightId == FlightId);
        }

        // Add a new flight crew
        public void AddFlightCrew(Models.FlightCrew flightCrew)
        {
            
            _context.FlightCrews.Add(flightCrew);
            _context.SaveChanges();
        }

        // Update an existing flight crew
        public void UpdateFlightCrew(Models.FlightCrew flightCrew)
        {
         
            _context.FlightCrews.Update(flightCrew);
            _context.SaveChanges();
        }

        // Delete a flight crew
        public void DeleteFlightCrew(Models.FlightCrew flightCrew)
        {

            _context.FlightCrews.Remove(flightCrew);
            _context.SaveChanges();
        }

        /// // ================= Entity-Specific Helpers ===================
        // 6. GetFlightCrewsByFlightId(int flightId)
        public List<Models.FlightCrew> GetFlightCrewsByFlightId(int flightId)
        {
            return _context.FlightCrews.Where(fc => fc.FlightId == flightId).ToList();
        }

        // 7. GetFlightCrewsByCrewId(int crewId)
        public List<Models.FlightCrew> GetFlightCrewsByCrewId(int crewId)
        {
            return _context.FlightCrews.Where(fc => fc.CrewId == crewId).ToList();
        }

    }
}
