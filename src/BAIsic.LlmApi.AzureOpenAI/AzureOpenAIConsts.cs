using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAIsic.LlmApi.AzureOpenAI
{
    public class AzureOpenAIConsts
    {
        public const string JsonMediaType = "application/json";

        public static class ChatRequest
        {
            public const string ToolFunctionType = "function"; 
        }
    }
}
