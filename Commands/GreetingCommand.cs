using Discord;
using Discord.Interactions;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Alexander.Commands
{
    public class GreetingCommand : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("saludar", "responde con dos botones")]
        public async Task GreetingAsync()
        {
            var builder = new ComponentBuilder()
                .WithButton("Yes", "btn_aceptar", ButtonStyle.Success)
                .WithButton("No", "btn_rechazar", ButtonStyle.Danger);

            await RespondAsync("Te gusta el ajedrez?", components: builder.Build());
    
        }

        [ComponentInteraction("btn_aceptar")]
        public async Task BtnAceptar()
        {
            await RespondAsync("Aceptaste");
        }

        [ComponentInteraction("btn_rechazar")]
        public async Task BtnRechazar()
        {
            await RespondAsync("Rechazaste");
        }
    }
}