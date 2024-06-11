using FrisörenSörenModels;

namespace FrisörenSörenAPI.Interfaces
{
    public interface ILogInService
    {
        Task<Customer> AuthenticateCustomer(string email, string password);
        Task<Company> AuthenticateCompany(string email, string password);


        bool IsCompany(HttpContext context);
        bool IsCustomer(HttpContext context);

        Task<Customer> GetLoggedInCustomer(HttpContext context);
    }
}
