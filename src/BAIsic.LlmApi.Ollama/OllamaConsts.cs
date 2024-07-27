using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAIsic.LlmApi.Ollama
{
    public static class OllamaConsts
    {
        public const string JsonFormatType = "json";
        public const string JsonMediaType = "application/json";

        public static class Endpoints
        {
            public const string ListLocalModelsEndpoint = "api/tags";
            public const string ChatCompletionEndpoint = "/api/chat";
            public const string EmbeddingsEndpoint = "/api/embeddings";
        }
    }
}
