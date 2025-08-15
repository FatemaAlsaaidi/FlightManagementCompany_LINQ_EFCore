using FlightManagementCompany_LINQ_EFCore.Models;

namespace FlightManagementCompany_LINQ_EFCore.Repositories
{
    public interface IFlightRepo
    {
        void AddFlight(Flight flight);
        void DeleteFlight(Flight flight);
        List<Flight> GetAllFlights();
        Flight GetFlightById(int id);
        IEnumerable<Flight> GetFlightsByDateRange(DateTime from, DateTime to);
        IEnumerable<Flight> GetFlightsByRoute(int routeId);
        void UpdateFlight(Flight flight);
    }
}