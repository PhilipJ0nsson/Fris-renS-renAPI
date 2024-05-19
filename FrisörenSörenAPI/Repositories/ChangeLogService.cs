using FrisörenSörenAPI.Data;
using FrisörenSörenAPI.Interfaces;
using FrisörenSörenModels;
using Microsoft.EntityFrameworkCore;

namespace FrisörenSörenAPI.Repositories
{
    public class ChangeLogService : IChangeLogService
    {
        private readonly AppDbContext _context;

        public ChangeLogService(AppDbContext context)
        {
            _context = context;
        }

        public async Task LogChangeAsync(string action, int customerId, int companyId, int bookingId)
        {
            string timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm");
            string details;

            if (customerId > 0)
            {
                details = $"Customer {customerId} {action} booking id {bookingId} at {timestamp}";
            }
            else
            {
                details = $"Company {companyId} {action} booking id {bookingId} at {timestamp}";
            }

            var changelog = new ChangeLog
            {
                Timestamp = DateTime.UtcNow,
                Action = action,
                CustomerId = customerId,
                CompanyId = companyId,
                Details = details
            };

            _context.ChangeLogs.Add(changelog);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<ChangeLog>> GetChangelogAsync()
        {
            return await _context.ChangeLogs.OrderBy(c => c.Timestamp).ToListAsync();
        }

    }
}
