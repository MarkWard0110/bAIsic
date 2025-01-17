using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BAIsic.Interlocutor.AmazonBedrock
{
    public class RequestOptions
    {
        /// <summary>
        /// Sets the size of the context window used to generate the next token. (Default: 2048)
        /// </summary>
        public int? NumCtx { get; set; }

        /// <summary>
        /// The temperature of the model. Increasing the temperature will make the model answer more creatively. (Default: 0.8)
        /// </summary>
        public float? Temperature { get; set; }

        /// <summary>
        /// Sets the random number seed to use for generation.
        /// Setting this to a specific number will make the model generate the same text for the same prompt. (Default: 0)
        /// </summary>
        public int? Seed { get; set; }

        /// <summary>
        /// Sets the stop sequences to use. When this pattern is encountered the LLM will stop generating text and return.
        /// Multiple stop patterns may be set by specifying multiple separate stop parameters in a modelfile.
        /// </summary>
        public string[]? Stop { get; set; }


        /// <summary>
        /// Maximum number of tokens to predict when generating text. (Default: 2048, -1 = infinite generation, -2 = fill context)
        /// </summary>
        public int? NumPredict { get; set; }

        /// <summary>
        /// Reduces the probability of generating nonsense. A higher value (e.g. 100) will give more diverse answers, while a lower value (e.g. 10) will be more conservative. (Default: 40)
        /// </summary>
        public int? TopK { get; set; }

        /// <summary>
        /// Works together with top-k. A higher value (e.g., 0.95) will lead to more diverse text, while a lower value (e.g., 0.5) will generate more focused and conservative text. (Default: 0.9)
        /// </summary>
        public float? TopP { get; set; }
    }
}
