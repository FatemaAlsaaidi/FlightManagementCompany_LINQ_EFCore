using FlightManagementCompany_LINQ_EFCore.Models;

namespace FlightManagementCompany_LINQ_EFCore.Repositories
{
    public interface IAircraftRepo
    {
        void AddAircraft(Aircraft aircraft);
        void DeleteAircraft(Aircraft aircraft);
        Aircraft GetAircraftById(int id);
        List<Aircraft> GetAircraftDueForMaintenance(DateTime beforeDate);
        List<Aircraft> GetAllAircrafts();
        void UpdateAircraft(Aircraft aircraft);
    }
}