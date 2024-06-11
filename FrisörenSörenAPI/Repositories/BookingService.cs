using FrisörenSörenAPI.Data;
using FrisörenSörenAPI.Dto;
using FrisörenSörenAPI.Interfaces;
using FrisörenSörenModels;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.Design;
using System.Globalization;

namespace FrisörenSörenAPI.Repositories
{
    public class BookingService : IBookingService
    {
        private readonly AppDbContext _context;
        private readonly IChangeLogService _changeLogService;

        public BookingService(AppDbContext context, IChangeLogService changeLogService)
        {
            _context = context;
            _changeLogService = changeLogService;
        }

        public async Task<BookingDto> CustomerAddBooking(int customerId, BookingDto bookingDto, string userEmail)
        {
            var customer = await _context.Customers.Include(c => c.Bookings).FirstOrDefaultAsync(c => c.CustomerId == customerId && c.Email == userEmail);
            if (customer == null)
            {
                throw new ArgumentException($"Customer with ID {customerId} not found or not authorized.");
            }

            var companyId = 1; // Assuming a default company ID

            var company = await _context.Companies.FindAsync(companyId);
            if (company == null)
            {
                throw new ArgumentException($"Company with ID {companyId} not found.");
            }

            var booking = new Booking
            {
                StartTime = bookingDto.StartTime,
                EndTime = bookingDto.EndTime,
                CustomerId = customerId,
                CompanyId = companyId
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            await _changeLogService.LogChangeAsync("created", booking.CustomerId, 0, booking.BookingId);

            return new BookingDto
            {
                BookingId = booking.BookingId,
                StartTime = booking.StartTime,
                EndTime = booking.EndTime
            };

        }
        public async Task<Booking> CompanyAddBooking(int customerId, BookingDto bookingDto, string userEmail)
        {
            var company = await _context.Companies.FirstOrDefaultAsync(c => c.Email == userEmail);
            if (company == null)
            {
                throw new UnauthorizedAccessException("You are not authorized to add a booking.");
            }

            var customer = await _context.Customers.Include(c => c.Bookings).FirstOrDefaultAsync(c => c.CustomerId == customerId);
            if (customer == null)
            {
                throw new ArgumentException($"Customer with ID {customerId} not found.");
            }

            var booking = new Booking
            {
                StartTime = bookingDto.StartTime,
                EndTime = bookingDto.EndTime,
                CustomerId = customerId,
                CompanyId = company.CompanyId
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            await _changeLogService.LogChangeAsync("created", customerId, company.CompanyId, booking.BookingId);



            return new Booking
            {
                BookingId = booking.BookingId,
                StartTime = booking.StartTime,
                EndTime = booking.EndTime,
                CustomerId = booking.CustomerId,
                CompanyId = booking.CompanyId
            };
        }

        public async Task<BookingDto> CustomerUpdateBooking(int bookingId, BookingDto bookingDto, string userEmail)
        {
            var booking = await _context.Bookings.Include(b => b.Customer).FirstOrDefaultAsync(b => b.BookingId == bookingId);
            if (booking == null)
            {
                throw new ArgumentException($"Booking with ID {bookingId} not found.");
            }

            if (booking.Customer.Email != userEmail)
            {
                throw new UnauthorizedAccessException("You are not authorized to update this booking.");
            }

            booking.StartTime = bookingDto.StartTime;
            booking.EndTime = bookingDto.EndTime;
            _context.Entry(booking).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            await _changeLogService.LogChangeAsync("updated", booking.CustomerId, 0, booking.BookingId);

            return new BookingDto
            {
                BookingId = booking.BookingId,
                StartTime = booking.StartTime,
                EndTime = booking.EndTime
            };
        }
        public async Task<Booking> CompanyUpdateBooking(int bookingId, BookingDto bookingDto, string userEmail)
        {
            var company = await _context.Companies.FirstOrDefaultAsync(c => c.Email == userEmail);
            if (company == null)
            {
                throw new UnauthorizedAccessException("You are not authorized to update a booking.");
            }

            var existingBooking = await _context.Bookings
                .Include(b => b.Customer) // Include related Customer
                .FirstOrDefaultAsync(b => b.BookingId == bookingId);

            if (existingBooking == null)
            {
                throw new ArgumentException($"Booking with ID {bookingId} not found.");
            }

            existingBooking.StartTime = bookingDto.StartTime;
            existingBooking.EndTime = bookingDto.EndTime;

            _context.Entry(existingBooking).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            await _changeLogService.LogChangeAsync("updated", existingBooking.CustomerId, existingBooking.CompanyId, existingBooking.BookingId);

            return existingBooking;
        }

        public async Task<IEnumerable<Booking>> GetAllBookings()
        {
            return await _context.Bookings.ToListAsync();
        }

        public async Task<IEnumerable<Booking>> GetBookingsForCustomer(int customerId)
        {
            return await _context.Bookings
                .Where(b => b.CustomerId == customerId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Booking>> GetBookingsForCustomerThisWeek(int customerId)
        {
            var currentWeek = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(DateTime.Now, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
            return await _context.Bookings
                .Where(b => b.CustomerId == customerId && CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(b.StartTime, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday) == currentWeek)
                .ToListAsync();
        }

        public async Task<Booking> GetBookingById(int bookingId)
        {
            return await _context.Bookings.FindAsync(bookingId);
        }

        public async Task<IEnumerable<Booking>> GetBookingsByWeek(int weekNumber)
        {
            var firstDayOfWeek = DayOfWeek.Monday; // Ange önskad första veckodag för att beräkna veckonummer

            var bookings = await _context.Bookings
                .ToListAsync(); // Hämta alla bokningar från databasen

            var filteredBookings = bookings
                .AsEnumerable() // Gör resten av operationen i minnet
                .Where(b => GetWeekOfYear(b.StartTime, firstDayOfWeek) == weekNumber) // Filtera resultaten i minnet
                .ToList(); // Konvertera till en lista

            if (!filteredBookings.Any())
            {
                return null; // Returnera null om inga bokningar hittades för veckonumret
            }

            return filteredBookings; // Returnera bokningarna för den angivna veckan
        }

        // Hjälpmetod för att beräkna veckonumret för ett datum med en given första veckodag
        private int GetWeekOfYear(DateTime date, DayOfWeek firstDayOfWeek)
        {
            var calendar = CultureInfo.CurrentCulture.Calendar;
            return calendar.GetWeekOfYear(date, CalendarWeekRule.FirstFourDayWeek, firstDayOfWeek);
        }

        public async Task<bool> CompanyDeleteBooking(int bookingId, string userEmail)
        {
            var company = await _context.Companies.FirstOrDefaultAsync(c => c.Email == userEmail);
            if (company == null)
            {
                throw new UnauthorizedAccessException("You are not authorized to delete a booking.");
            }

            var booking = await _context.Bookings.FindAsync(bookingId);
            if (booking == null)
            {
                return false;
            }

            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();

            await _changeLogService.LogChangeAsync("deleted", booking.CustomerId, booking.CompanyId, booking.BookingId);

            return true;
        }
        public async Task<bool> CustomerDeleteBooking(int bookingId, string userEmail)
        {
            var booking = await _context.Bookings.Include(b => b.Customer).FirstOrDefaultAsync(b => b.BookingId == bookingId);
            if (booking == null)
            {
                return false;
            }

            if (booking.Customer.Email != userEmail)
            {
                throw new UnauthorizedAccessException("You are not authorized to delete this booking.");
            }

            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();

            await _changeLogService.LogChangeAsync("deleted", booking.CustomerId, 0, booking.BookingId);

            return true;
        }
    }
}
