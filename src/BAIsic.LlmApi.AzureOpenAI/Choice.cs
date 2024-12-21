using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BAIsic.LlmApi.AzureOpenAI
{
    public class Choice
    {
        [JsonPropertyName("index")]
        public int Index { get; set; }

        [JsonPropertyName("finish_reason")]
        public string FinishReason { get; set; } = string.Empty;

        [JsonPropertyName("message")]
        public Message? Message { get; set; }
    }
}
