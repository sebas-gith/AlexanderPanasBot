using Discord;
using Discord.Interactions;
using System.Text;
using System.Threading.Tasks;

namespace Alexander.Commands
{
    public class SpamCommand : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("spamuser", "Te gustaria Spammear a alguien?")]
        public async Task SpamAsync(
            [Summary("user", "Selecciona al usuario")] IUser targetUser,
            [Summary("text", "Texto que aparecera al lado del usuario")] string text,
            [Summary("amount", "Cuantas veces quieres repetir (limite = 15)")] int amount)
        {

            ulong spamChannelId = 1503220497125609532u;

            if (Context.Channel.Id != spamChannelId)
            {
                await RespondAsync("Sol puedes usar este comando el canal de spam");
            }
            else
            {
                string message = $"{text} {targetUser.Mention}";
                int finalAmount = amount > 15 ? 15 : amount;

                var messageFinal = new StringBuilder();
                for (int i = 0; i < finalAmount; ++i)
                {
                    messageFinal.Append(message + "\n");
                }

                await RespondAsync(messageFinal.ToString());
            }

        }
    }
}