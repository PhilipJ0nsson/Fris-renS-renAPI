using FrisörenSörenAPI.Data;
using FrisörenSörenAPI.Interfaces;
using FrisörenSörenModels;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace FrisörenSörenAPI.Repositories
{
    public class CustomerService : ICustomerService
    {
        private readonly AppDbContext _context;

        public CustomerService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Customer> GetCustomerById(int customerId)
        {
            return await _context.Customers
                .Include(c => c.Bookings)
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);
        }

        public async Task<IEnumerable<Customer>> GetAllCustomers()
        {
            return await _context.Customers.ToListAsync();
        }

        public async Task<IEnumerable<Customer>> GetCustomersWithBookingsThisWeek()
        {
            var currentWeek = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(DateTime.Now, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

            // Hämta kunder inklusive deras bokningar från databasen
            var customersWithBookings = await _context.Customers
                .Include(c => c.Bookings)
                .ToListAsync();

            // Filtrera kunder baserat på bokningar för den aktuella veckan
            var customersThisWeek = customersWithBookings
                .Where(c => c.Bookings.Any(b => CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(b.StartTime, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday) == currentWeek))
                .ToList(); // Omvandla till lista för att filtrera lokalt på klienten

            // Om inga kunder hittades med bokningar för den aktuella veckan
            if (customersThisWeek.Count == 0)
            {
                return new List<Customer>(); // Returnera en tom lista
            }

            return customersThisWeek;
        }

        public async Task<int> GetBookingCountForCustomerInWeek(int customerId, int weekNumber)
        {
            var customer = await _context.Customers
                .Include(c => c.Bookings)
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);

            if (customer == null)
            {
                throw new ArgumentException($"Customer with ID {customerId} not found.");
            }

            return customer.Bookings.Count(b => CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(b.StartTime, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday) == weekNumber);
        }
    }
}

