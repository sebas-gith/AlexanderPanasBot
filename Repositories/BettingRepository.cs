using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Alexander.Data;
using Alexander.Models;

namespace Alexander.Repositories
{
    public class BettingRepository : IBettingRepository
    {
        private readonly BotDbContext _context;

        public BettingRepository(BotDbContext context)
        {
            _context = context;
        }

        public async Task<List<BetEvent>> GetActiveEventsAsync()
        {
            return await _context.BetEvents
                .Include(e => e.Tickets)
                .Where(e => !e.IsClosed)
                .ToListAsync();
        }

        public async Task<BetEvent> GetActiveEventByIdAsync(int eventId)
        {
            return await _context.BetEvents
                .Include(e => e.Tickets)
                .FirstOrDefaultAsync(e => e.Id == eventId && !e.IsClosed);
        }

        public async Task<int> CreateEventAsync(string description, ulong creatorId)
        {
            var newEvent = new BetEvent { Description = description, CreatorId = creatorId };
            await _context.BetEvents.AddAsync(newEvent);
            await _context.SaveChangesAsync();
            return newEvent.Id;
        }

        public async Task AddTicketAsync(BetTicket ticket)
        {
            await _context.BetTickets.AddAsync(ticket);
            await _context.SaveChangesAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}