using Alexander.Models;

namespace Alexander.Repositories
{
    public interface IEconomyRepository
{
    Task<List<EconomyProfile>> LoadAllAsync();
    Task SaveAllAsync(List<EconomyProfile> profiles);
    Task<EconomyProfile> GetByUserIdAsync(ulong userId);
    Task AddAsync(EconomyProfile profile);
    Task UpdateAsync(EconomyProfile profile);
    Task DeleteAsync(ulong userId);
    bool Exists(ulong userId);
}
}
