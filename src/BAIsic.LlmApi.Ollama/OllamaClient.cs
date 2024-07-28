using System.Net;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;

namespace BAIsic.LlmApi.Ollama
{
    public class OllamaClient(HttpClient httpClient)
    {
        private readonly HttpClient _httpClient = httpClient;

        public async Task<Model[]> ListLocalModelsAsync(CancellationToken cancellationToken = default)
        {
            using var response = await _httpClient.GetAsync(OllamaConsts.Endpoints.ListLocalModelsEndpoint, cancellationToken);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
            var data = JsonSerializer.Deserialize<ListLocalModelsResponse>(responseBody);

            if (data == null)
            {
                return [];
            }

            return data.Models;
        }

        public async Task<ChatResponse> InvokeChatCompletionAsync(ChatRequest chatRequest, TextWriter? outputStream = null, CancellationToken cancellationToken = default)
        {
            bool isStreaming = chatRequest.Stream;

            if (isStreaming && chatRequest.Tools != null)
            {
                // Ollama v0.3.0 does not support streaming with tools
                // TODO: support streaming with tools
                isStreaming = false;
                chatRequest.Stream = false;
            }

            var httpRequest = BuildHttpRequest(chatRequest);

            if (isStreaming)
            {
                using (HttpResponseMessage? response = await _httpClient.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead, cancellationToken))
                {
                    response.EnsureSuccessStatusCode();
                    using var streamResponse = await response.Content.ReadAsStreamAsync(cancellationToken);
                    using var reader = new StreamReader(streamResponse);

                    string? responseRole = null;
                    var responseContent = new StringBuilder();

                    string? line;

                    // using ReadLineAsync check for null to avoid additional code to check for EOF and rare case of null line
                    while ((line = await reader.ReadLineAsync(cancellationToken)) != null && !cancellationToken.IsCancellationRequested)
                    {
                        ChatResponse chatResponse = JsonSerializer.Deserialize<ChatResponse>(line) ?? throw new Exception("Failed to deserialize response");

                        responseRole ??= chatResponse?.Message?.Role;
                        responseContent.Append(chatResponse?.Message?.Content);

                        // write to feedback stream here
                        if (outputStream != null)
                        {
                            await outputStream.WriteAsync(chatResponse?.Message?.Content);
                            outputStream.Flush();
                        }

                        if (chatResponse?.Done ?? false)
                        {
                            responseRole ??= string.Empty;
                            chatResponse.Message = new Message(){
                                Role = responseRole, 
                                Content = responseContent.ToString()
                            };
                            return chatResponse;
                        }
                    }
                }
                throw new NotImplementedException();
            }
            else
            {
                using HttpResponseMessage? response = await _httpClient.SendAsync(httpRequest, HttpCompletionOption.ResponseContentRead, cancellationToken);
                response.EnsureSuccessStatusCode();
                using Stream? streamResponse = await response.Content.ReadAsStreamAsync(cancellationToken);
                ChatResponse chatResponse = await JsonSerializer.DeserializeAsync<ChatResponse>(streamResponse, cancellationToken: cancellationToken) ?? throw new Exception("Failed to deserialize response");

                //TODO: write to feedback stream here

                return chatResponse;
            }
        }

        private static HttpRequestMessage BuildHttpRequest(ChatRequest request)
        {
            string serialized = JsonSerializer.Serialize(request);
            return new HttpRequestMessage(HttpMethod.Post, OllamaConsts.Endpoints.ChatCompletionEndpoint)
            {
                Content = new StringContent(serialized, Encoding.UTF8, OllamaConsts.JsonMediaType)
            };
        }
    }
}
