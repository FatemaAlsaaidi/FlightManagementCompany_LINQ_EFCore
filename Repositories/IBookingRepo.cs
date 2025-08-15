using FlightManagementCompany_LINQ_EFCore.Models;

namespace FlightManagementCompany_LINQ_EFCore.Repositories
{
    public interface IBookingRepo
    {
        void AddBooking(Booking booking);
        void DeleteBooking(Booking booking);
        List<Booking> GetAllBookings();
        Booking GetBookingById(int id);
        List<Booking> GetBookingsByDateRange(DateTime from, DateTime to);
        List<Booking> GetBookingsByPassengerId(int passengerId);
        void RemoveBooking(Booking booking);
    }
}