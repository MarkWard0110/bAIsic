using BAIsic.Interlocutor;
using BAIsic.LlmApi.Ollama.Tests;
using BAIsic.LlmApi.Ollama;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BAIsic.Interlocutor.Ollama;

namespace BAIsic.Interlocutor.Ollama.Tests
{
    public static class OllamaAgentExtentions
    {
        public static T AddTestsOllamaGenerateReply<T>(this T agent, string model) where T : IAgent
        {
            var ollamaClient = OllamaClientExtensions.CreateOllamaClient();
            return agent.AddOllamaGenerateReply(model, ollamaClient);
        }

        public static T AddTestsOllamaGenerateReply<T>(this T agent, string model, RequestOptions requestOptions) where T : IAgent
        {
            var ollamaClient = OllamaClientExtensions.CreateOllamaClient();
            return agent.AddOllamaGenerateReply(model, ollamaClient, requestOptions);
        }

        public static T AddTestsOllamaGenerateReply<T>(this T agent, string model, OllamaOptions ollamaOptions) where T : IAgent
        {
            var ollamaClient = OllamaClientExtensions.CreateOllamaClient();
            return agent.AddOllamaGenerateReply(model, ollamaClient, ollamaOptions);
        }
    }
}
