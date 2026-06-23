using System;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Alexander.Services
{
    public class GhipyApiService
    {
        private readonly HttpClient _httpClient;

        private readonly ConfigurationManager _config;
        public GhipyApiService(IHttpClientFactory clientFactory)
        {
            _httpClient = clientFactory.CreateClient();
            _config = ConfigurationManager.Instance;
        }

        public async Task<string> GetGifAsync(string keyword)
        {
            try
            {
                string cleanKeyword = Uri.EscapeDataString(keyword);
                string baseUrl = _config.GetApiBaseUrl("Giphy");
                string apiKey = _config.GetApiKey("Giphy");
                string finalUrl = $"{baseUrl}?api_key={apiKey}&q={keyword}&rating=g";

                string responseJson = await _httpClient.GetStringAsync(finalUrl);

                using JsonDocument doc = JsonDocument.Parse(responseJson);

                var data = doc.RootElement.GetProperty("data");

                if (data.GetArrayLength() == 0)
                {
                    return null;
                }

                string? gifUrl = data[0]
                    .GetProperty("images")
                    .GetProperty("original")
                    .GetProperty("url")
                    .GetString();

                return gifUrl;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error consumiendo Giphy: {ex.Message}");
                return null;
            }
        }
    }
}