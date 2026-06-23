using System.Collections.Generic;
using System.Threading.Tasks;
using Alexander.Models;

namespace Alexander.Repositories
{
    public interface IBettingRepository
    {
        Task<List<BetEvent>> GetActiveEventsAsync();
        Task<BetEvent> GetActiveEventByIdAsync(int eventId);
        Task<int> CreateEventAsync(string description, ulong creatorId);
        Task AddTicketAsync(BetTicket ticket);
        Task SaveChangesAsync();

    }
}