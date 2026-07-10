using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Microsoft.EntityFrameworkCore;
using Alexander.Data;

namespace Alexander.Commands
{
    [Group("counting", "Configura el canal de conteo infinito")]
    [DefaultMemberPermissions(GuildPermission.Administrator)]
    public class CountingCommand : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly BotDbContext _db;

        public CountingCommand(BotDbContext db)
        {
            _db = db;
        }

        [SlashCommand("setchannel", "Establece este canal como la zona oficial para contar")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task SetChannel()
        {
            var state = await _db.CountingStates.FirstOrDefaultAsync();
            if (state == null)
            {
                state = new Models.CountingState();
                _db.CountingStates.Add(state);
            }

            state.ChannelId = Context.Channel.Id;
            state.CurrentCount = 0;
            state.LastUserId = 0;
            
            await _db.SaveChangesAsync();

            await RespondAsync($"🏆 ¡Este canal ahora es el coliseo de los números! Empecemos la cadena. El primer valiente debe escribir **1**.");
        }
    }
}