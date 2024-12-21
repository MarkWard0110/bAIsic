using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BAIsic.LlmApi.AzureOpenAI.Tests
{
    public static class AzureOpenAIClientExtensions
    {
        public static AzureOpenAIClient CreateAzureOpenAIClient()
        {
            var endpoint = Environment.GetEnvironmentVariable(AzureOpenAITestConsts.EnvironmentVariable.ChatCompletionEndpoint);
            if (string.IsNullOrWhiteSpace(endpoint))
            {
                throw new InvalidOperationException($"{AzureOpenAITestConsts.EnvironmentVariable.ChatCompletionEndpoint} environment variable not found.");
            }

            var apiKey = Environment.GetEnvironmentVariable(AzureOpenAITestConsts.EnvironmentVariable.ChatCompletionApiKey);
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                throw new InvalidOperationException($"{AzureOpenAITestConsts.EnvironmentVariable.ChatCompletionApiKey} environment variable not found.");
            }

            var httpClient = new HttpClient
            {
                BaseAddress = new Uri(endpoint)
            };
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);
            httpClient.DefaultRequestHeaders.Add("api-key", apiKey);

            return new AzureOpenAIClient(httpClient);
        }
    }
}
