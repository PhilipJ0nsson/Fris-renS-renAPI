using FrisörenSörenAPI.Dto;
using FrisörenSörenModels;

namespace FrisörenSörenAPI.Interfaces
{
    public interface IBookingService
    {
        Task<BookingDto> CustomerAddBooking(int customerId, BookingDto bookingDto);
        Task<Booking> CompanyAddBooking(int customerId, BookingDto bookingDto);
        Task<BookingDto> CustomerUpdateBooking(int bookingId, BookingDto bookingDto);
        Task<Booking> CompanyUpdateBooking(int bookingId, BookingDto bookingDto);
        Task<IEnumerable<Booking>> GetAllBookings();
        Task<IEnumerable<Booking>> GetBookingsForCustomer(int customerId);
        Task<IEnumerable<Booking>> GetBookingsForCustomerThisWeek(int customerId);
        Task<Booking> GetBookingById(int bookingId);
        Task<IEnumerable<Booking>> GetBookingsByWeek(int weekNumber);
        Task<bool> CompanyDeleteBooking(int bookingId);
        Task<bool> CustomerDeleteBooking(int bookingId);
    }
}
