using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Alexander.Data;
using Alexander.Models;

namespace Alexander.Repositories
{
    public class RewardRepository : IRewardRepository
    {
        private readonly BotDbContext _db;

        public RewardRepository(BotDbContext db)
        {
            _db = db;
        }

        public async Task<Reward> AddAsync(Reward reward)
        {
            _db.Rewards.Add(reward);
            await _db.SaveChangesAsync();
            return reward;
        }

        public async Task<Reward> GetByIdAsync(int id)
        {
            return await _db.Rewards.FindAsync(id);
        }

        public async Task<List<Reward>> GetAllAsync()
        {
            return await _db.Rewards.ToListAsync();
        }

        public async Task RemoveAsync(Reward reward)
        {
            _db.Rewards.Remove(reward);
            await _db.SaveChangesAsync();
        }
    }
}