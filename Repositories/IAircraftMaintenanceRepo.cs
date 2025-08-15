using FlightManagementCompany_LINQ_EFCore.Models;

namespace FlightManagementCompany_LINQ_EFCore.Repositories
{
    public interface IAircraftMaintenanceRepo
    {
        void AddAircraftMaintenance(AircraftMaintenance maintenance);
        void DeleteAircraftMaintenance(AircraftMaintenance maintenance);
        AircraftMaintenance GetAircraftMaintenanceById(int id);
        List<AircraftMaintenance> GetAllAircrafMaintenances();
        List<AircraftMaintenance> GetMaintenancesByAircraftId(int aircraftId);
        void UpdateAircraftMaintenance(AircraftMaintenance maintenance);
    }
}