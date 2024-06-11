using FrisörenSörenAPI.Dto;
using FrisörenSörenModels;

namespace FrisörenSörenAPI.Interfaces
{
    public interface ICustomerService
    {
        Task<Customer> GetCustomerById(int customerId);
        Task<IEnumerable<Customer>> GetAllCustomers();
        Task<IEnumerable<CustomerDto>> GetCustomersWithBookingsThisWeek();
        Task<int> GetBookingCountForCustomerInWeek(int customerId, int weekNumber);
    }
}
