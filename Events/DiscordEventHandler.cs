using Discord.WebSocket;
using Alexander.Services;

namespace Alexander.Events
{
    public class DiscordEventHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly LoggingService _logger;

        public DiscordEventHandler(DiscordSocketClient client, LoggingService logger)
        {
            _client = client;
            _logger = logger;
        }

        public void Initialize()
        {
            _client.Log += _logger.LogAsync;
            _client.Ready += () => 
            {
                _logger.LogAsync(new Discord.LogMessage(Discord.LogSeverity.Info, "Gateway", "¡Bot conectado y listo!"));
                return System.Threading.Tasks.Task.CompletedTask;
            };
        }
    }
}