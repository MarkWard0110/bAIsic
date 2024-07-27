using System.Net;
using System.Reflection;
using System.Text.Json;

namespace BAIsic.LlmApi.Ollama
{
    public class OllamaClient
    {
        private readonly HttpClient _httpClient;
        public OllamaClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Model[]> ListLocalModelsAsync(CancellationToken cancellationToken = default)
        {
            using var response = await _httpClient.GetAsync(OllamaConsts.Endpoints.ListLocalModelsEndpoint, cancellationToken);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<ListLocalModelsResponse>(responseBody);

            if (data == null)
            {
                return [];
            }

            return data.Models;
        }
    }
}
