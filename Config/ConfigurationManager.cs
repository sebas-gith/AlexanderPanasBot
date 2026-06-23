using Microsoft.Extensions.Configuration;
using System.IO;

namespace Alexander
{
    public class ConfigurationManager
    {
        private readonly IConfiguration _configuration;
        private static ConfigurationManager? _instance;
        private ConfigurationManager()
        {
            _configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory() + "/Config")
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
        }
        public static ConfigurationManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new ConfigurationManager();
                return _instance;
            }
        }
        public string GetValue(string key)
        {
            var value = _configuration[key];
            
            if (string.IsNullOrEmpty(value))
                throw new InvalidOperationException($"No se encontró la configuración: {key} en appsettings.json");
            
            return value;
        }
        public T GetValue<T>(string key)
        {
            var value = _configuration[key];
            
            if (string.IsNullOrEmpty(value))
                throw new InvalidOperationException($"No se encontró la configuración: {key} en appsettings.json");
            
            return (T)Convert.ChangeType(value, typeof(T));
        }

        public string GetDiscordToken() => GetValue("Discord:Token");
        public string GetDiscordClientId() => GetValue("Discord:ClientId");
        public string GetApiKey(string apiName) => GetValue($"Apis:{apiName}:ApiKey");
        public string GetApiBaseUrl(string apiName) => GetValue($"Apis:{apiName}:BaseUrl");
        public int GetApiTimeout(string apiName) => GetValue<int>($"Apis:{apiName}:TimeoutSeconds");
    }
}