using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlightManagementCompany_LINQ_EFCore.Models; // Ensure this namespace matches your AircraftMaintenance model's namespace


namespace FlightManagementCompany_LINQ_EFCore.Repositories
{
    public class AircraftMaintenanceRepo : IAircraftMaintenanceRepo
    {
        // Constructor database Injection 
        private readonly FlightDatabaseContext _context;
        public AircraftMaintenanceRepo(FlightDatabaseContext context)
        {
            _context = context;
        }

        // 1. GetAll()
        public List<AircraftMaintenance> GetAllAircrafMaintenances()
        {
            return _context.AircraftMaintenances.ToList();

        }

        // 2. GetById(int id)
        public AircraftMaintenance GetAircraftMaintenanceById(int id)
        {
            return _context.AircraftMaintenances.Find(id);
        }

        // 3. Add(entity)
        public void AddAircraftMaintenance(AircraftMaintenance maintenance)
        {
            _context.AircraftMaintenances.Add(maintenance);
            _context.SaveChanges();

        }

        // 4. Update(entity)
        public void UpdateAircraftMaintenance(AircraftMaintenance maintenance)
        {

            _context.AircraftMaintenances.Update(maintenance);
            _context.SaveChanges();

        }

        // 5. Delete
        public void DeleteAircraftMaintenance(AircraftMaintenance maintenance)
        {

            _context.AircraftMaintenances.Remove(maintenance);
            _context.SaveChanges();

        }

        /// // ================= Entity-Specific Helpers ===================
        // 6. GetMaintenancesByAircraftId(int aircraftId)
        public List<AircraftMaintenance> GetMaintenancesByAircraftId(int aircraftId)
        {
            return _context.AircraftMaintenances
                           .Where(m => m.AircraftId == aircraftId)
                           .OrderByDescending(m => m.MaintenanceDate)
                           .ToList();
        }










    }
}
