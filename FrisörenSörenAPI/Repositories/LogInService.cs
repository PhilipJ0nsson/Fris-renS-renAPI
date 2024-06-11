using FrisörenSörenAPI.Data;
using FrisörenSörenAPI.Interfaces;
using FrisörenSörenModels;
using Microsoft.EntityFrameworkCore;

namespace FrisörenSörenAPI.Repositories
{
    public class LogInService : ILogInService
    {
        private readonly AppDbContext _context;

        public LogInService(AppDbContext context)
        {
            _context = context;
        }


        public async Task<Company> AuthenticateCompany(string email, string password)
        {
            return await _context.Companies.FirstOrDefaultAsync(c => c.Email == email && c.Password == password);
        }

        public async Task<Customer> AuthenticateCustomer(string email, string password)
        {
            return await _context.Customers.FirstOrDefaultAsync(c => c.Email == email && c.Password == password);
        }


        public bool IsCompany(HttpContext context)
        {
            var userRole = context.Session.GetString("UserRole");
            return userRole == "Company";
        }

        public bool IsCustomer(HttpContext context)
        {
            var userRole = context.Session.GetString("UserRole");
            return userRole == "Customer";
        }
        public async Task<Customer> GetLoggedInCustomer(HttpContext context)
        {
            var userEmail = context.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(userEmail))
            {
                return null;
            }
            return await _context.Customers.FirstOrDefaultAsync(c => c.Email == userEmail);
        }
    }
}
