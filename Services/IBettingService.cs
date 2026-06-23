using System.Collections.Generic;
using System.Threading.Tasks;
using Alexander.Models;

namespace Alexander.Services
{
    public interface IBettingService
    {
        Task<int> CreateEventAsync(string description, ulong creatorId);
        Task<(bool Success, string Message)> LockEventAsync(int eventId, ulong userId, bool isAdmin);
        Task<List<BetEvent>> GetActiveEventsAsync();
        Task<(bool Success, string Message)> LockEventAsync(int eventId);
        Task<(bool Success, string Message, string EventDesc, int EstimatedPayout)> PlaceBetAsync(ulong userId, int eventId, int amount, bool inFavor);
        Task<List<string>> ResolveEventAsync(int eventId, bool wonInFavor);
    }
}