using FlightManagementCompany_LINQ_EFCore.Models;

namespace FlightManagementCompany_LINQ_EFCore.Repositories
{
    public interface IBaggageRepo
    {
        void AddBaggage(Baggage baggage);
        void DeleteBaggage(Baggage baggage);
        List<Baggage> GetAllBaggages();
        Baggage GetBaggageById(int id);
        List<Baggage> GetBaggagesByTicketId(int ticketId);
        void UpdateBaggage(Baggage baggage);
    }
}