using BAIsic.Interlocutor;
using BAIsic.LlmApi.AzureOpenAI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAIsic.Interlocutor.AzureOpenAI
{
    public static class AzureOpenAIAgentExtentions
    {
        public static T AddAzureOpenAIGenerateReply<T>(this T agent, string model, AzureOpenAIClient azureOpenAIClient, AzureOpenAIOptions azureOpenAIOptions) where T : IAgent
        {
            var azureOpenAIGenerateReplyHandler = new AzureOpenAIGenerateReplyHandler(model, azureOpenAIClient, azureOpenAIOptions);
            agent.GenerateReplyHandlers.Add(azureOpenAIGenerateReplyHandler.GenerateReplyHandlerAsync);
            return agent;
        }

        public static T AddAzureOpenAIGenerateReply<T>(this T agent, string model, HttpClient httpClient, AzureOpenAIOptions azureOpenAIOptions) where T : IAgent
        {
            var azureOpenAIGenerateReplyHandler = new AzureOpenAIGenerateReplyHandler(model, httpClient, azureOpenAIOptions);
            agent.GenerateReplyHandlers.Add(azureOpenAIGenerateReplyHandler.GenerateReplyHandlerAsync);
            return agent;
        }

        public static T AddAzureOpenAIGenerateReply<T>(this T agent, string model, AzureOpenAIClient azureOpenAIClient) where T : IAgent
        {
            return AddAzureOpenAIGenerateReply(agent, model, azureOpenAIClient, new AzureOpenAIOptions());
        }

        public static T AddAzureOpenAIGenerateReply<T>(this T agent, string model, HttpClient httpClient) where T : IAgent
        {
            return AddAzureOpenAIGenerateReply(agent, model, httpClient, new AzureOpenAIOptions());
        }

        public static T AddAzureOpenAIGenerateReply<T>(this T agent, string model, AzureOpenAIClient azureOpenAIClient, RequestOptions requestOptions) where T : IAgent
        {
            return AddAzureOpenAIGenerateReply(agent, model, azureOpenAIClient, new AzureOpenAIOptions() { RequestOptions = requestOptions });
        }

        public static T AddAzureOpenAIGenerateReply<T>(this T agent, string model, HttpClient httpClient, RequestOptions requestOptions) where T : IAgent
        {
            return AddAzureOpenAIGenerateReply(agent, model, httpClient, new AzureOpenAIOptions() { RequestOptions = requestOptions });
        }
    }
}
