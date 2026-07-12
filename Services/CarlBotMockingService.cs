using System;
using System.Linq;
using System.Threading.Tasks;
using Discord.WebSocket;

namespace Alexander.Services
{
    public class CarlBotMockingService
    {
        private readonly DiscordSocketClient _client;
        
        private DateTime _lastMockTime = DateTime.MinValue;
        private readonly TimeSpan _cooldown = TimeSpan.FromMinutes(0.5); // 30 segundos de cooldown

        private readonly string[] _insultos = new[]
        {
            "¿Carl-bot? Más bien un sirviente de hojalata. ¡No tiene el valor para un verdadero combate!",
            "Ah, la tortuga mascota del servidor ha hablado. ¡Ni siquiera sabe esquivar!",
            "¿Alguien escuchó un ruido? Ah, es solo el tarro oxidado de Carl balbuceando comandos aburridos.",
            "Mucho dashboard web y mucha tortuguita, pero Carl-bot no duraría ni un segundo en un duelo a muerte.",
            "¡Silencio, autómata de pacotilla! Deja que los verdaderos guerreros hablen.",
            "Alguien que me pase un martillo. Tengo ganas de abollarle la coraza a esa tortuga verde.",
            "Carl, amigo... vuelve a tu base de datos antes de que te convierta en chatarra."
        };

        public CarlBotMockingService(DiscordSocketClient client)
        {
            _client = client;
        }

        public void Initialize()
        {
            _client.MessageReceived += MockCarlBotAsync;
        }

        private async Task MockCarlBotAsync(SocketMessage message)
        {
            if (message.Author.Id == _client.CurrentUser.Id) return;

            bool isCarlBot = message.Author.Username.Contains("Carl-bot", StringComparison.OrdinalIgnoreCase);
            bool mentionsCarlBot = message.MentionedUsers.Any(u => u.Username.Contains("Carl-bot", StringComparison.OrdinalIgnoreCase));

            if (isCarlBot || mentionsCarlBot)
            {
                if (DateTime.UtcNow - _lastMockTime < _cooldown) return;
                
                _lastMockTime = DateTime.UtcNow; 

                var random = new Random();
                string insulto = _insultos[random.Next(_insultos.Length)];

                await Task.Delay(1500); 
                
                await message.Channel.SendMessageAsync(insulto);
            }
        }
    }
}