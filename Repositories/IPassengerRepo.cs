using FlightManagementCompany_LINQ_EFCore.Models;

namespace FlightManagementCompany_LINQ_EFCore.Repositories
{
    public interface IPassengerRepo
    {
        void AddPassenger(Passenger passenger);
        void DeletePassenger(Passenger passenger);
        List<Passenger> GetAllPassengers();
        Passenger GetPassengerById(int id);
        void UpdatePassenger(Passenger passenger);
    }
}