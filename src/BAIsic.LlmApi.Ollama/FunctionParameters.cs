using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BAIsic.LlmApi.Ollama
{
    public class FunctionParameters
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = "object";

        [JsonPropertyName("required")]
        public List<string>? Required { get; set; }

        [JsonPropertyName("properties")]
        public Dictionary<string, FunctionProperty>? Properties { get; set; }
    }
}
