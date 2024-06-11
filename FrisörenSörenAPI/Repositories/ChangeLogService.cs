using FrisörenSörenAPI.Data;
using FrisörenSörenAPI.Dto;
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

        public async Task<ChangeLogDto> LogChangeAsync(string action, int customerId, int companyId, int bookingId)
        {
            string timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm");
            string details;

            if (companyId > 0)
            {
                details = $"Company id {companyId} {action} booking id {bookingId} for Customer id {customerId} at {timestamp}";
            }
            else
            {
                details = $"Customer id {customerId} {action} booking id {bookingId} at {timestamp}";
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

            return new ChangeLogDto { Details = details };
        }

        public async Task<IEnumerable<ChangeLog>> GetChangelogAsync()
        {
            return await _context.ChangeLogs.OrderBy(c => c.Timestamp).ToListAsync();
        }

    }
}
