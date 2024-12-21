using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BAIsic.LlmApi.AzureOpenAI
{
    public class RequestResponseFormat
    {
        [JsonPropertyName("type")]
        public string? Type { get; set; } = "json_object";
    }
}
