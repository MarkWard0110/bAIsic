using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BAIsic.LlmApi.AzureOpenAI
{
    public class AzureOpenAIClient(HttpClient httpClient)
    {
        private readonly HttpClient _httpClient = httpClient;

        public async Task<ChatResponse> InvokeChatCompletionAsync(ChatRequest chatRequest, TextWriter? outputStream = null, CancellationToken cancellationToken = default)
        {
            bool isStreaming = chatRequest.Stream;

            if (isStreaming && chatRequest.Tools != null)
            {
                // TODO: support streaming with tools
                isStreaming = false;
                chatRequest.Stream = false;
            }

            var httpRequest = BuildHttpRequest(chatRequest);

            if (isStreaming)
            {
                //using (HttpResponseMessage? response = await _httpClient.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead, cancellationToken))
                //{
                //    response.EnsureSuccessStatusCode();
                //    using var streamResponse = await response.Content.ReadAsStreamAsync(cancellationToken);
                //    using var reader = new StreamReader(streamResponse);

                //    string? responseRole = null;
                //    var responseContent = new StringBuilder();

                //    string? line;

                //    // using ReadLineAsync check for null to avoid additional code to check for EOF and rare case of null line
                //    while ((line = await reader.ReadLineAsync(cancellationToken)) != null && !cancellationToken.IsCancellationRequested)
                //    {
                //        ChatResponse chatResponse = JsonSerializer.Deserialize<ChatResponse>(line) ?? throw new Exception("Failed to deserialize response");

                //        responseRole ??= chatResponse?.Message?.Role;
                //        responseContent.Append(chatResponse?.Message?.Content);

                //        // write to feedback stream here
                //        if (outputStream != null)
                //        {
                //            await outputStream.WriteAsync(chatResponse?.Message?.Content);
                //            outputStream.Flush();
                //        }

                //        if (chatResponse?.Done ?? false)
                //        {
                //            responseRole ??= string.Empty;
                //            chatResponse.Message = new Message()
                //            {
                //                Role = responseRole,
                //                Content = responseContent.ToString()
                //            };
                //            return chatResponse;
                //        }
                //    }
                //}
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
            return new HttpRequestMessage(HttpMethod.Post, string.Empty)
            {
                Content = new StringContent(serialized, Encoding.UTF8, AzureOpenAIConsts.JsonMediaType)
            };
        }
    }
}
