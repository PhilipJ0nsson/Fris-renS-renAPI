
using FrisörenSörenModels;

namespace FrisörenSörenAPI.Interfaces
{
    public interface IChangeLogService
    {
        Task LogChangeAsync(string action, int customerId, int companyId, int bookingId);
        Task<IEnumerable<ChangeLog>> GetChangelogAsync();
    }
}
