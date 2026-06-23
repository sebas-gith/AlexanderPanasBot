using Alexander.Models;

namespace Alexander.Services
{
    public interface IEconomyService
{
    Task<AccountResult> OpenAccountAsync(ulong userId, float initialCoins = 1000);
    Task<float> GetBalanceAsync(ulong userId);
    Task<bool> AddCoinsAsync(ulong userId, float amount);
    Task<bool> RemoveCoinsAsync(ulong userId, float amount);
    Task<bool> TransferCoinsAsync(ulong fromUserId, ulong toUserId, float amount);
    Task<Dictionary<ulong, float>> GetAllBalancesAsync();
    Task<bool> AccountExistsAsync(ulong userId);
}
}