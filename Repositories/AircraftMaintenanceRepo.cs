using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlightManagementCompany_LINQ_EFCore.Models; // Ensure this namespace matches your AircraftMaintenance model's namespace


namespace FlightManagementCompany_LINQ_EFCore.Repositories
{
    public class AircraftMaintenanceRepo
    {
        // Constructor database Injection 
        private readonly FlightDatabaseContext _context;
        public AircraftMaintenanceRepo(FlightDatabaseContext context)
        {
            _context = context;
        }

        // 1. GetAll()
        public void GetAllAircrafMaintenances() 
        {
            var aircraftMaintenances = _context.AircraftMaintenances.ToList();
            foreach (var maintenance in aircraftMaintenances)
            {
                Console.WriteLine($"Maintenance ID: {maintenance.MaintenanceId}, Aircraft ID: {maintenance.AircraftId}, Maintenance Date: {maintenance.MaintenanceDate}, Description: {maintenance.Note}");
            }
        }

        // 2. GetById(int id)
        public void GetAircraftMaintenanceById(int id)
        {
            var maintenance = _context.AircraftMaintenances.Find(id);
            if (maintenance != null)
            {
                Console.WriteLine($"Maintenance ID: {maintenance.MaintenanceId}, Aircraft ID: {maintenance.AircraftId}, Maintenance Date: {maintenance.MaintenanceDate}, Description: {maintenance.Note}");
            }
            else
            {
                Console.WriteLine($"No maintenance found with ID: {id}");
            }
        }

        // 3. Add(entity)
        public void AddAircraftMaintenance(AircraftMaintenance maintenance)
        {
            if (maintenance != null)
            {
                _context.AircraftMaintenances.Add(maintenance);
                _context.SaveChanges();
                Console.WriteLine($"Added Maintenance ID: {maintenance.MaintenanceId}, Aircraft ID: {maintenance.AircraftId}");
            }
            else
            {
                Console.WriteLine("Cannot add null maintenance.");
            }
        }

        // 4. Update(entity)
        public void UpdateAircraftMaintenance(AircraftMaintenance maintenance)
        {
            if (maintenance != null)
            {
                _context.AircraftMaintenances.Update(maintenance);
                _context.SaveChanges();
                Console.WriteLine($"Updated Maintenance ID: {maintenance.MaintenanceId}, Aircraft ID: {maintenance.AircraftId}");
            }
            else
            {
                Console.WriteLine("Cannot update null maintenance.");
            }
        }

        // 5. Delete(int id)
        public void DeleteAircraftMaintenance(int id)
        {
            var maintenance = _context.AircraftMaintenances.Find(id);
            if (maintenance != null)
            {
                _context.AircraftMaintenances.Remove(maintenance);
                _context.SaveChanges();
                Console.WriteLine($"Deleted Maintenance ID: {id}");
            }
            else
            {
                Console.WriteLine($"No maintenance found with ID: {id}");
            }
        }

    }
}
