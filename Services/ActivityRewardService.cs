using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Alexander.Repositories;

namespace Alexander.Services
{
    public class ActivityRewardService
    {
        private readonly DiscordSocketClient _client;
        private readonly IServiceProvider _services;

        private readonly ConcurrentDictionary<ulong, DateTime> _chatCooldowns = new();
        private readonly ConcurrentDictionary<ulong, DateTime> _voiceSessions = new();

        public ActivityRewardService(DiscordSocketClient client, IServiceProvider services)
        {
            _client = client;
            _services = services;
        }

        public void Initialize()
        {
            _client.MessageReceived += HandleMessageRewardAsync;
            _client.UserVoiceStateUpdated += HandleVoiceRewardAsync;
        }

        private async Task HandleMessageRewardAsync(SocketMessage message)
        {
            if (message is not SocketUserMessage userMessage || message.Author.IsBot) return;

            ulong userId = message.Author.Id;
            DateTime now = DateTime.UtcNow;

            if (_chatCooldowns.TryGetValue(userId, out DateTime lastReward))
            {
                if ((now - lastReward).TotalMinutes < 1) return;
            }

            _chatCooldowns[userId] = now;

            using (var scope = _services.CreateScope())
            {
                var economyRepo = scope.ServiceProvider.GetRequiredService<IEconomyRepository>();
                var profile = await economyRepo.GetByUserIdAsync(userId);

                if (profile != null)
                {
                    profile.SmashCoins += 1;
                    await economyRepo.UpdateAsync(profile);
                }
            }
        }

        private async Task HandleVoiceRewardAsync(SocketUser user, SocketVoiceState oldState, SocketVoiceState newState)
        {
            if (user.IsBot) return;
            ulong userId = user.Id;

            if (oldState.VoiceChannel == null && newState.VoiceChannel != null)
            {
                _voiceSessions[userId] = DateTime.UtcNow;
            }
            else if (oldState.VoiceChannel != null && newState.VoiceChannel == null)
            {
                if (_voiceSessions.TryRemove(userId, out DateTime joinTime))
                {
                    TimeSpan timeSpent = DateTime.UtcNow - joinTime;
                    int coinsToReward = (int)Math.Floor(timeSpent.TotalMinutes / 60.0 * 10);

                    if (coinsToReward > 0)
                    {
                        using (var scope = _services.CreateScope())
                        {
                            var economyRepo = scope.ServiceProvider.GetRequiredService<IEconomyRepository>();
                            var profile = await economyRepo.GetByUserIdAsync(userId);

                            if (profile != null)
                            {
                                profile.SmashCoins += coinsToReward;
                                await economyRepo.UpdateAsync(profile);
                                Console.WriteLine("Se le dieron 2 pesos");
                            }

                        }
                    }
                    Console.WriteLine("No se le dieron na");
                }
            }
        }
    }
}