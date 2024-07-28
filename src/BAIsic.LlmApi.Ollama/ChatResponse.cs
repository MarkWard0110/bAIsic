using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BAIsic.LlmApi.Ollama
{
    public class ChatResponse
    {
        [JsonPropertyName("model")]
        public string Model { get; set; } = string.Empty;

        [JsonPropertyName("created_at")]
        public string CreatedAt { get; set; } = string.Empty;

        [JsonPropertyName("message")]
        public Message? Message { get; set; }

        [JsonPropertyName("done")]
        public bool Done { get; set; } = false;

        [JsonPropertyName("done_reason")]
        public string? DoneReason { get; set; } 

        #region Final Response
        /// <summary>
        /// time spent generating the response
        /// </summary>
        [JsonPropertyName("total_duration")]
        public long? TotalDuration { get; set; }

        /// <summary>
        /// time spent in nanoseconds loading the model
        /// </summary>
        [JsonPropertyName("load_duration")]
        public long? LoadDuration { get; set; }

        /// <summary>
        /// number of tokens in the prompt
        /// </summary>
        [JsonPropertyName("prompt_eval_count")]
        public int? PromptEvalCount { get; set; }
        
        /// <summary>
        /// time spent in nanoseconds evaluating the prompt
        /// </summary>
        [JsonPropertyName("prompt_eval_duration")]
        public long? PromptEvalDuration { get; set; }

        /// <summary>
        /// number of tokens the response
        /// </summary>
        [JsonPropertyName("eval_count")]
        public int? EvalCount { get; set; }

        /// <summary>
        /// time in nanoseconds spent generating the response
        /// </summary>
        [JsonPropertyName("eval_duration")]
        public long? EvalDuration { get; set; }
        #endregion
    }
}
