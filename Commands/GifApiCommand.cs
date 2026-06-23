using Discord;
using Discord.Interactions;
using System.Threading.Tasks;
using Alexander.Services;
using System.Text.RegularExpressions;

namespace Alexander.Commands
{
    public class GifApiCommand : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly GhipyApiService _giphyApiService;

        public GifApiCommand(GhipyApiService apiService)
        {
            _giphyApiService = apiService;
        }

        [SlashCommand("gif", "busca y envia un gif")]
        public async Task SendGifAsync([Summary("Busqueda", "Lo que quieres buscar")] string keyword)
        {
            await DeferAsync();

            string result = await _giphyApiService.GetGifAsync(keyword);

            if (result == null)
            {
                await FollowupAsync($"No encontré ningún GIF para: **{keyword}** 😕");
            }
            else
            {
                await FollowupAsync(result);
            }
        }

        [SlashCommand("gifword", "envia una palabra con gifs")]
        public async Task SendAWordMadeofGifs([Summary("Palabra", "la palabra que quieres enviar")] string keyword)
        {
            await DeferAsync();
            string pattern = @"[^a-zA-ZáéíóúÁÉÍÓÚñÑ]";
            string cleanedKyword = Regex.Replace(keyword, pattern, "");
            Console.WriteLine(cleanedKyword);
            List<string> gifsUrls = new List<string>();
            foreach(var letter in cleanedKyword)
            {
                string template = $"rega marketing {letter}";
                gifsUrls.Add(await _giphyApiService.GetGifAsync(template));
            }
            foreach(var currGif in gifsUrls)
            {
                await FollowupAsync(currGif);
            }
        }
    }
}