using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAIsic.LlmApi.Ollama.Tests
{
    public static class OllamaTestConsts
    {
        public static class  EnvironmentVariable
        {
            public const string OllamaHost = "OLLAMA_HOST";
        }

        public static class Model
        {
            public const string Llama3_1_8b = "llama3.1:8b-instruct-q4_0";
            public const string Llava_7b = "llava:7b-v1.6-mistral-q4_0";
        }
    }
}
