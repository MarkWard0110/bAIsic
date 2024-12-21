using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BAIsic.LlmApi.AzureOpenAI
{
    public class ChatRequest
    {
        /// <summary>
        /// (required) the model name
        /// </summary>
        [JsonPropertyName("model")]
        public string Model { get; set; } = string.Empty;

        /// <summary>
        /// the messages of the chat, this can be used to keep a chat memory
        /// </summary>
        [JsonPropertyName("messages")]
        public IList<Message> Messages { get; set; } = [];

        [JsonPropertyName("tools")]
        public Tool[]? Tools { get; set;} 

        /// <summary>
        /// if false the response will be returned as a single response object, rather than a stream of objects
        /// </summary>
        [JsonPropertyName("stream")]
        public bool Stream { get; set; }

        /// <summary>
        /// The temperature of the model. Increasing the temperature will make the model answer more creatively. (Default: 0.8)
        /// </summary>
        [JsonPropertyName("temperature")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public float? Temperature { get; set; }

        /// <summary>
        /// Sets the stop sequences to use. When this pattern is encountered the LLM will stop generating text and return.
        /// Multiple stop patterns may be set by specifying multiple separate stop parameters in a modelfile.
        /// </summary>
        [JsonPropertyName("stop")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string[]? Stop { get; set; }

        /// <summary>
        /// Works together with top-k. A higher value (e.g., 0.95) will lead to more diverse text, while a lower value (e.g., 0.5) will generate more focused and conservative text. (Default: 0.9)
        /// </summary>
        [JsonPropertyName("top_p")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public float? TopP { get; set; }
    }
}
