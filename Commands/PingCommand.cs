using Discord.Interactions;
using System.Threading.Tasks;

namespace Alexander.Commands
{
    public class PingCommand : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("ping", "Responde con un pong")]
        public async Task PingAsync()
        {
            await RespondAsync("¡Pong!");
        }
    }
}