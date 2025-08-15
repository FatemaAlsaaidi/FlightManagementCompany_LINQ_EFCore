using FlightManagementCompany_LINQ_EFCore.Models;

namespace FlightManagementCompany_LINQ_EFCore.Repositories
{
    public interface IAirportRepo
    {
        void AddAirport(Airport airport);
        void DeleteAirport(Airport airport);
        Airport GetAirportById(int id);
        List<Airport> GetAirportsByDestinationCity(string destinationCity);
        List<Airport> GetAirportsByOriginCity(string Origincity);
        List<Airport> GetAllAirports();
        void UpdateAirport(Airport airport);
    }
}