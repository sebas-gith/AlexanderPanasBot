using Microsoft.EntityFrameworkCore;
using Alexander.Models;

namespace Alexander.Data
{
    public class BotDbContext : DbContext
    {
        public DbSet<EconomyProfile> Perfiles { get; set; }
        public DbSet<BetEvent> BetEvents { get; set; }
        public DbSet<BetTicket> BetTickets { get; set; }
        public DbSet<CountingState> CountingStates { get; set; }
        public DbSet<Reward> Rewards { get; set; }
        public BotDbContext(DbContextOptions<BotDbContext> options) : base(options)
        {
        }
    }
}