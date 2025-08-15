using FlightManagementCompany_LINQ_EFCore.Models;

namespace FlightManagementCompany_LINQ_EFCore.Repositories
{
    public interface IRouteRepo
    {
        void AddRoute(Route route);
        void DeleteRoute(Route route);
        List<Route> GetAllRoutes();
        Route GetRouteById(int id);
        List<Route> GetRoutesByOriginAndDestinationCity(string originCity, string destinationCity);
        void UpdateRoute(Route route);
    }
}