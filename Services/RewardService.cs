using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord.WebSocket;
using Alexander.Models;
using Alexander.Repositories;

namespace Alexander.Services
{
    public class RewardService
    {
        private readonly IRewardRepository _rewardRepo;
        private readonly IEconomyRepository _economyRepo;

        public RewardService(IRewardRepository rewardRepo, IEconomyRepository economyRepo)
        {
            _rewardRepo = rewardRepo;
            _economyRepo = economyRepo;
        }

        public async Task<Reward> CreateRoleRewardAsync(string name, float price, ulong roleId, string description)
        {
            var reward = new Reward
            {
                Name = name,
                Price = price,
                Description = description,
                ActionType = RewardActionType.GiveRole,
                ActionData = roleId.ToString()
            };

            return await _rewardRepo.AddAsync(reward);
        }

        public async Task<List<Reward>> GetAvailableRewardsAsync()
        {
            return await _rewardRepo.GetAllAsync();
        }

        public async Task<(bool Success, string Message)> BuyRewardAsync(int rewardId, SocketGuildUser user)
        {
            var reward = await _rewardRepo.GetByIdAsync(rewardId);
            if (reward == null)
                return (false, "❌ Este artículo ya no existe en el catálogo.");

            var profile = await _economyRepo.GetByUserIdAsync(user.Id);
            if (profile == null || profile.SmashCoins < reward.Price)
                return (false, $"❌ Fondos insuficientes. Necesitas {reward.Price} 🪙.");

            if (reward.ActionType == RewardActionType.GiveRole)
            {
                if (ulong.TryParse(reward.ActionData, out ulong roleId))
                {
                    var role = user.Guild.GetRole(roleId);
                    if (role == null)
                        return (false, "❌ El rol vinculado a esta recompensa fue borrado del servidor.");

                    try
                    {
                        await user.AddRoleAsync(role);
                    }
                    catch (Exception)
                    {
                        return (false, "❌ Error de jerarquía: No tengo permisos suficientes para otorgar ese rol.");
                    }
                }
            }

            profile.SmashCoins -= reward.Price;
            await _economyRepo.UpdateAsync(profile);

            return (true, $"🎉 ¡Compra exitosa! Has adquirido **{reward.Name}** y se ha aplicado a tu cuenta.");
        }
    }
}