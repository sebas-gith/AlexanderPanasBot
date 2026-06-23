using Discord;
using Discord.Interactions;
using Alexander.Services;
using System.Threading.Tasks;

namespace Alexander.Commands
{
    public class FactApiCommand : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly FactApiService _apiService;

        public FactApiCommand(FactApiService apiService)
        {
            _apiService = apiService;
        }

        [SlashCommand("randomfact", "Un dato random en ingles")]
        public async Task DatoCuriosoAsync()
        {
            await DeferAsync();

            string result = await _apiService.GetFactAsync();

            var embed = new EmbedBuilder()
                .WithTitle("💡 ¡Dato Interesante De Los gatos!")
                .WithDescription(result)
                .WithColor(Color.Blue)
                .WithFooter("Tu sabia eso?")
                .Build();

            await FollowupAsync(embed: embed);
        }
    }
}