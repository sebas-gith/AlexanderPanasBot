using System.Collections.Generic;
using System.Threading.Tasks;
using Alexander.Models;

namespace Alexander.Repositories
{
    public interface IRewardRepository
    {
        Task<Reward> AddAsync(Reward reward);
        Task<Reward> GetByIdAsync(int id);
        Task<List<Reward>> GetAllAsync();
        Task RemoveAsync(Reward reward);
    }
}