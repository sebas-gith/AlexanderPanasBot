using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Alexander.Events;
using Alexander.Services;
using Discord.Interactions;
using Discord.Commands;
using Alexander.Repositories;
using Alexander.Data;
using Microsoft.EntityFrameworkCore;

namespace Alexander
{
    public class Program
    {
        static Task Main(string[] args) => new Program().MainAsync();

        public async Task MainAsync()
        {
            var config = new DiscordSocketConfig
            {
                GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent
            };

            var services = new ServiceCollection()
                            .AddSingleton(new DiscordSocketClient(config))
                            .AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>()))
                            .AddSingleton<InteractionHandlingService>()
                            .AddSingleton<CommandService>()
                            .AddSingleton<LoggingService>()
                            .AddSingleton<DiscordEventHandler>()
                            .AddDbContext<BotDbContext>(options =>
                                options.UseSqlite("Data Source=economia.db"), ServiceLifetime.Transient)
                            .AddSingleton<IEconomyRepository, EconomyRepository>()
                            .AddSingleton<IEconomyService, EconomyService>()
                            .AddTransient<IBettingRepository, BettingRepository>()
                            .AddTransient<IBettingService, BettingService>()
                            .AddHttpClient()
                            .AddSingleton<FactApiService>()
                            .AddSingleton<GhipyApiService>()
                            .AddSingleton<ConfigurationManager>()
                            .BuildServiceProvider();

            var configManager = ConfigurationManager.Instance;

            string token = configManager.GetDiscordToken();

            var client = services.GetRequiredService<DiscordSocketClient>();
            var eventHandler = services.GetRequiredService<DiscordEventHandler>();

            var interactionHandler = services.GetRequiredService<InteractionHandlingService>();

            eventHandler.Initialize();

            await interactionHandler.InitializeAsync();
            using (var scope = services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<Alexander.Data.BotDbContext>();
                db.Database.EnsureCreated();
            }
            await client.LoginAsync(Discord.TokenType.Bot, token);
            await client.StartAsync();
            await Task.Delay(-1);

        }

    }
}
