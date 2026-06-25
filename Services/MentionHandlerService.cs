using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Alexander.Utils;

namespace Alexander.Services
{
    public class MentionHandlerService
    {
        private readonly DiscordSocketClient _client;

        public MentionHandlerService(DiscordSocketClient client)
        {
            _client = client;
        }

        public void Initialize()
        {
            _client.MessageReceived += HandleMessageAsync;
        }


        private async Task HandleMessageAsync(SocketMessage message)
        {
            var phrase = new AlexanderPhrases();
            if (message is not SocketUserMessage userMessage || message.Author.IsBot) return;

            if (userMessage.MentionedUsers.Any(u => u.Id == _client.CurrentUser.Id))
            {
                using (message.Channel.EnterTypingState())
                {
                    await Task.Delay(1000);

                    string respuesta = string.Format(phrase.GetRandomFrase(), message.Author.Mention);

                    await message.Channel.SendMessageAsync(respuesta);
                }
            }
        }
    }
}