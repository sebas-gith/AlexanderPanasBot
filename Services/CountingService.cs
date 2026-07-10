using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Alexander.Data;

namespace Alexander.Services
{
    public class CountingService
    {
        private readonly DiscordSocketClient _client;
        private readonly IServiceProvider _services;

        public CountingService(DiscordSocketClient client, IServiceProvider services)
        {
            _client = client;
            _services = services;
        }

        public void Initialize()
        {
            _client.MessageReceived += HandleCountingAsync;
        }

        private async Task HandleCountingAsync(SocketMessage message)
        {
            if (message is not SocketUserMessage userMessage || message.Author.IsBot) return;

            if (!int.TryParse(message.Content.Trim(), out int newNumber)) return;

            using (var scope = _services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<BotDbContext>();

                var state = await db.CountingStates.FirstOrDefaultAsync();
                if (state == null)
                {
                    state = new Models.CountingState();
                    db.CountingStates.Add(state);
                    await db.SaveChangesAsync();
                }

                if (state.ChannelId == 0 || message.Channel.Id != state.ChannelId) return;

                if (newNumber == state.CurrentCount + 1)
                {
                    state.CurrentCount++;
                    state.LastUserId = message.Author.Id;
                    await db.SaveChangesAsync();

                    await message.AddReactionAsync(new Emoji("✅"));
                }
                else
                {
                    int lostStreak = state.CurrentCount;

                    state.CurrentCount = 0;
                    state.LastUserId = 0;
                    await db.SaveChangesAsync();

                    await message.AddReactionAsync(new Emoji("❌"));

                    string bullshit = $"🤡 ¡ATENCIÓN, DISCORD! **{message.Author.Username}** acaba de hacer el ridículo. \n" +
                  $"Ha escrito **{newNumber}** en lugar de {lostStreak + 1}. ¿Te duele la cabeza o es que no tienes neuronas? \n\n" +
                  $"El Guerrero Jarro te da una segunda oportunidad. Desde el **1**. No la cagues otra vez.";

                    await message.Channel.SendMessageAsync(bullshit);
                }
            }
        }
    }
}