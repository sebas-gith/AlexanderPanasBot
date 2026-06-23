using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Alexander.Services
{
    public class FactApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ConfigurationManager _config;

        public FactApiService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _config = ConfigurationManager.Instance;
        }

        public async Task<string> GetFactAsync()
        {
            try
            {
                string url = _config.GetApiBaseUrl("RandomFact");
                string responseJson = await _httpClient.GetStringAsync(url);

                using JsonDocument doc = JsonDocument.Parse(responseJson);
                string? dato = doc.RootElement.GetProperty("fact").GetString();

                return dato;
            }catch(Exception ex)
            {
                Console.WriteLine($"Error {ex.Message}");
                return "Hubo un problemita mio";
            }
        }
    }
}