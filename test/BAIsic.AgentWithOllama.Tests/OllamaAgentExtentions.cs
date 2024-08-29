using BAIsic.Interlocutor;
using BAIsic.LlmApi.Ollama.Tests;
using BAIsic.LlmApi.Ollama;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BAIsic.Interlocutor.Ollama;

namespace BAIsic.AgentWithOllama.Tests
{
    public static class OllamaAgentExtentions
    {
        public static T AddTestsOllamaGenerateReply<T>(this T agent, string model, RequestOptions? requestOptions = null) where T : IAgent
        {
            var ollamaClient = OllamaClientExtensions.CreateOllamaClient();
            var ollamaGenerateReplyHandler = new OllamaGenerateReplyHandler(model, ollamaClient, requestOptions);
            agent.GenerateReplyHandlers.Add(ollamaGenerateReplyHandler.GenerateReplyHandlerAsync);
            return agent;
        }
    }
}
