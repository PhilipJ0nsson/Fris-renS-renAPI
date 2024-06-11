using FrisörenSörenAPI.Data;
using FrisörenSörenAPI.Dto;
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

        public async Task<IEnumerable<CustomerDto>> GetCustomersWithBookingsThisWeek()
        {
            var currentWeek = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(DateTime.Now, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

            // Hämta kunder inklusive deras bokningar från databasen
            var customersWithBookings = await _context.Customers
                .Include(c => c.Bookings)
                .ToListAsync();

            // Filtrera kunder baserat på bokningar för den aktuella veckan och skapa en ny lista med filtrerade bokningar
            var customersThisWeek = customersWithBookings
                .Select(c => new CustomerDto
                {
                    CustomerId = c.CustomerId,
                    CustomerName = c.CustomerName,
                    Email = c.Email,
                    Bookings = c.Bookings
                        .Where(b => CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(b.StartTime, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday) == currentWeek)
                        .Select(b => new BookingDto
                        {
                            BookingId = b.BookingId,
                            StartTime = b.StartTime,
                            EndTime = b.EndTime
                        })
                        .ToList()
                })
                .Where(c => c.Bookings.Any()) // Endast inkludera kunder som har bokningar denna veckan
                .ToList();

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

