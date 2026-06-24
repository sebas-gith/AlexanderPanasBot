using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alexander.Models;
using Alexander.Repositories;

namespace Alexander.Services
{
    public class EconomyService : IEconomyService
    {
        private readonly IEconomyRepository _repository;

        public EconomyService(IEconomyRepository repository)
        {
            _repository = repository;
        }

        public async Task<AccountResult> OpenAccountAsync(ulong userId, float initialCoins = 1000)
        {
            if (userId == 0) return AccountResult.Failure("UserId no puede ser 0");
            if (initialCoins < 0) return AccountResult.Failure("Las monedas iniciales no pueden ser negativas");

            var existingProfile = await _repository.GetByUserIdAsync(userId);
            if (existingProfile != null)
                return AccountResult.Failure($"El usuario {userId} ya tiene una cuenta");

            var profile = new EconomyProfile { UserId = userId, SmashCoins = initialCoins };
            await _repository.AddAsync(profile);

            return AccountResult.Ok($"Cuenta creada exitosamente con {initialCoins} monedas", profile);
        }

        public async Task<float> GetBalanceAsync(ulong userId)
        {
            var profile = await _repository.GetByUserIdAsync(userId);
            return profile != null ? profile.SmashCoins : 0;
        }

        public async Task<bool> AddCoinsAsync(ulong userId, float amount)
        {
            if (amount <= 0) return false;

            var profile = await _repository.GetByUserIdAsync(userId);
            if (profile == null) return false;

            profile.SmashCoins += amount;
            await _repository.UpdateAsync(profile);
            return true;
        }

        public async Task<bool> RemoveCoinsAsync(ulong userId, float amount)
        {
            if (amount <= 0) return false;

            var profile = await _repository.GetByUserIdAsync(userId);
            if (profile == null || profile.SmashCoins < amount) return false;

            profile.SmashCoins -= amount;
            await _repository.UpdateAsync(profile);
            return true;
        }

        public async Task<bool> TransferCoinsAsync(ulong fromUserId, ulong toUserId, float amount)
        {
            if (amount <= 0 || fromUserId == toUserId) return false;

            var fromProfile = await _repository.GetByUserIdAsync(fromUserId);
            var toProfile = await _repository.GetByUserIdAsync(toUserId);

            if (fromProfile == null || toProfile == null || fromProfile.SmashCoins < amount)
                return false;

            fromProfile.SmashCoins -= amount;
            toProfile.SmashCoins += amount;

            await _repository.UpdateAsync(fromProfile);
            await _repository.UpdateAsync(toProfile);

            return true;
        }

        public async Task<Dictionary<ulong, float>> GetAllBalancesAsync()
        {
            var profiles = await _repository.LoadAllAsync();
            return profiles.ToDictionary(p => p.UserId, p => p.SmashCoins);
        }

        public async Task<bool> AccountExistsAsync(ulong userId)
        {
            var profile = await _repository.GetByUserIdAsync(userId);
            return profile != null;
        }
        public async Task<(bool Success, string Message, float NewBalance)> ClaimDailyAsync(ulong userId)
        {
            var profile = await _repository.GetByUserIdAsync(userId);

            if (profile == null)
            {
                return (false, "¡Aún no tienes dónde guardar tus monedas! Usa `/economy openaccount` para crear tu cuenta antes de reclamar recompensas.", 0);
            }

            if (DateTime.UtcNow.Date <= profile.LastDailyClaim.Date)
            {
                var nextClaim = profile.LastDailyClaim.Date.AddDays(1);
                var timeRemaining = nextClaim - DateTime.UtcNow;

                return (false, $"Ya has reclamado tu recompensa hoy. Vuelve en **{timeRemaining.Hours}h y {timeRemaining.Minutes}m**.", profile.SmashCoins);
            }

            profile.SmashCoins += 70;
            profile.LastDailyClaim = DateTime.UtcNow;

            await _repository.UpdateAsync(profile);

            return (true, string.Empty, profile.SmashCoins);
        }
    }
}