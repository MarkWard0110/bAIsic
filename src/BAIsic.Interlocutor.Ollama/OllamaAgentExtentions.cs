using BAIsic.Interlocutor;
using BAIsic.LlmApi.Ollama;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAIsic.Interlocutor.Ollama
{
    public static class OllamaAgentExtentions
    {
        public static T AddOllamaGenerateReply<T>(this T agent, string model, OllamaClient ollamaClient, RequestOptions? requestOptions = null) where T : IAgent
        {
            var ollamaGenerateReplyHandler = new OllamaGenerateReplyHandler(model, ollamaClient, requestOptions);
            agent.GenerateReplyHandlers.Add(ollamaGenerateReplyHandler.GenerateReplyHandlerAsync);
            return agent;
        }

        public static T AddOllamaGenerateReply<T>(this T agent, string model, HttpClient httpClient, RequestOptions? requestOptions = null) where T : IAgent
        {
            var ollamaGenerateReplyHandler = new OllamaGenerateReplyHandler(model, httpClient, requestOptions);
            agent.GenerateReplyHandlers.Add(ollamaGenerateReplyHandler.GenerateReplyHandlerAsync);
            return agent;
        }
    }
}
