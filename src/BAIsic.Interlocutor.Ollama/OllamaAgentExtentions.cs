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
        public static T AddOllamaGenerateReply<T>(this T agent, string model, OllamaClient ollamaClient, OllamaOptions ollamaOptions) where T : IAgent
        {
            var ollamaGenerateReplyHandler = new OllamaGenerateReplyHandler(model, ollamaClient, ollamaOptions);
            agent.GenerateReplyHandlers.Add(ollamaGenerateReplyHandler.GenerateReplyHandlerAsync);
            return agent;
        }

        public static T AddOllamaGenerateReply<T>(this T agent, string model, HttpClient httpClient, OllamaOptions ollamaOptions) where T : IAgent
        {
            var ollamaGenerateReplyHandler = new OllamaGenerateReplyHandler(model, httpClient, ollamaOptions);
            agent.GenerateReplyHandlers.Add(ollamaGenerateReplyHandler.GenerateReplyHandlerAsync);
            return agent;
        }

        public static T AddOllamaGenerateReply<T>(this T agent, string model, OllamaClient ollamaClient) where T : IAgent
        {
            return AddOllamaGenerateReply(agent, model, ollamaClient, new OllamaOptions());
        }

        public static T AddOllamaGenerateReply<T>(this T agent, string model, HttpClient httpClient) where T : IAgent
        {
            return AddOllamaGenerateReply(agent, model, httpClient, new OllamaOptions());
        }

        public static T AddOllamaGenerateReply<T>(this T agent, string model, OllamaClient ollamaClient, RequestOptions requestOptions) where T : IAgent
        {
            return AddOllamaGenerateReply(agent, model, ollamaClient, new OllamaOptions() { RequestOptions = requestOptions });
        }

        public static T AddOllamaGenerateReply<T>(this T agent, string model, HttpClient httpClient, RequestOptions requestOptions) where T : IAgent
        {
            return AddOllamaGenerateReply(agent, model, httpClient, new OllamaOptions() { RequestOptions = requestOptions });
        }
    }
}
