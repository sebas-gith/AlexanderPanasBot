using Discord;
using Discord.Interactions;
using System.Threading.Tasks;

namespace Alexander.Commands
{
    [Group("admin", "Comando para los admins, los más duros del server")]
    [DefaultMemberPermissions(GuildPermission.Administrator)]
    public class AdminCommand : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("announcement", "Haz un anuncio al servidor")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task Announcement(
            [Summary("title", "el titulo del anuncio")] string title,
            [Summary("description", "la descripcion del anuncio")] string description, 
            [Summary("footer", "el footer del anuncio")] string footer)
        {   
               var embed = new EmbedBuilder()
                .WithTitle($"📢 {title}")
                .WithDescription($"***{description}***")
                .WithAuthor(Context.Client.CurrentUser)
                .WithColor(Color.Purple)
                .WithFooter($"{footer}")
                .Build();

            await Context.Channel.SendMessageAsync(embed: embed);
        }

    }
}