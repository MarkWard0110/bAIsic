using Moq.Protected;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BAIsic.LlmApi.Ollama.Tests
{
    public static class OllamaClientExtensions
    {
        public static OllamaClient CreateOllamaClient()
        {
            var host = Environment.GetEnvironmentVariable(OllamaTestConsts.EnvironmentVariable.OllamaHost);
            if (string.IsNullOrWhiteSpace(host))
            {
                throw new InvalidOperationException($"{OllamaTestConsts.EnvironmentVariable.OllamaHost} environment variable not found.");
            }

            var httpClient = new HttpClient
            {
                BaseAddress = new Uri(host)
            };

            return new OllamaClient(httpClient);
        }
    }
}
