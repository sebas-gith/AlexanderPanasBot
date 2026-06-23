using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Alexander.Services
{
    public class InteractionHandlingService
    {
        private readonly DiscordSocketClient _client;
        private readonly InteractionService _interactions;
        private readonly IServiceProvider _services;

        public InteractionHandlingService(DiscordSocketClient client, InteractionService interactions, IServiceProvider services)
        {
            _client = client;
            _interactions = interactions;
            _services = services;
        }

        public async Task InitializeAsync()
        {
            await _interactions.AddModulesAsync(Assembly.GetEntryAssembly(), _services);

            _client.InteractionCreated += HandleInteractionAsync;
            _client.Ready += OnReadyAsync;
        }

        private async Task OnReadyAsync()
        {
            //Id del server LosPanasdelSmash
            ulong serverId = 708733220031692801u;

            await _interactions.RegisterCommandsToGuildAsync(serverId);
            Console.WriteLine("Comandos de barra registrados en Discord.");
        }

        private async Task HandleInteractionAsync(SocketInteraction interaction)
        {
            try
            {
                var context = new SocketInteractionContext(_client, interaction);
                var result = await _interactions.ExecuteCommandAsync(context, _services);

                if (!result.IsSuccess)
                {
                    Console.WriteLine($"[Error de Interacción] ID: {interaction.Type} - Motivo: {result.ErrorReason}");
                    
                    if (interaction.Type == InteractionType.MessageComponent || interaction.Type == InteractionType.ModalSubmit)
                    {
                        await interaction.RespondAsync($"Ocurrió un error interno: {result.ErrorReason}", ephemeral: true);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al ejecutar la interacción: {ex.Message}");
                
                if (interaction.Type == InteractionType.ApplicationCommand)
                {
                    await interaction.RespondAsync("Ocurrió un error al ejecutar este comando.", ephemeral: true);
                }
            }
        }
    }
}