using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAIsic.LlmApi.AzureOpenAI.Tests
{
    public static class AzureOpenAITestConsts
    {
        public static class  EnvironmentVariable
        {
            public const string ChatCompletionEndpoint = "AZUREOPENAI_CHATCOMPLETION_ENDPOINT";
            public const string ChatCompletionApiKey = "AZUREOPENAI_CHATCOMPLETION_APIKEY";
        }

        public static class Model
        {
            public const string Llama3_1_8b = "Meta-Llama-3.1-8B-Instruct";
            public const string Llama3_3_11b_Vision = "Llama-3.2-11B-Vision-Instruct";
        }
    }
}
