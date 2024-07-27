using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BAIsic.LlmApi.Ollama
{
    public class ListLocalModelsResponse
    {
        [JsonPropertyName("models")]
        public Model[] Models { get; set; } = [];
    }

    [DebuggerDisplay("{Name}")]
    public class Model
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("modified_at")]
        public DateTime ModifiedAt { get; set; }

        [JsonPropertyName("size")]
        public long Size { get; set; }

        [JsonPropertyName("digest")]
        public string Digest { get; set; } = string.Empty;

        [JsonPropertyName("details")]
        public Details? Details { get; set; }
    }

    public class Details
    {
        [JsonPropertyName("parent_model")]
        public string ParentModel { get; set; } = string.Empty;

        [JsonPropertyName("format")]
        public string Format { get; set; }  = string.Empty;

        [JsonPropertyName("family")]
        public string Family { get; set; } = string.Empty;

        [JsonPropertyName("families")]
        public string[]? Families { get; set; }

        [JsonPropertyName("parameter_size")]
        public string ParameterSize { get; set; } = string.Empty;

        [JsonPropertyName("quantization_level")]
        public string QuantizationLevel { get; set; } = string.Empty;
    }
}
