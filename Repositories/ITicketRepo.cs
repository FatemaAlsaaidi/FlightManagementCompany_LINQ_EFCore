using FlightManagementCompany_LINQ_EFCore.Models;

namespace FlightManagementCompany_LINQ_EFCore.Repositories
{
    public interface ITicketRepo
    {
        void AddTicket(Ticket ticket);
        void DeleteTicket(Ticket ticket);
        List<Ticket> GetAllTickets();
        Ticket GetTicketById(int id);
        IEnumerable<Ticket> GetTicketsByBooking(string bookingRef);
        IEnumerable<Ticket> GetTicketsByPassenger(int passengerId);
        void UpdateTicket(Ticket ticket);
    }
}