using Amazon.BedrockRuntime;
using BAIsic.Interlocutor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAIsic.Interlocutor.AmazonBedrock
{
    public static class AmazonBedrockAgentExtentions
    {
        public static T AddAmazonBedrockGenerateReply<T>(this T agent, string model, AmazonBedrockRuntimeClient client, AmazonBedrockOptions options) where T : IAgent
        {
            var generateReplyHandler = new AmazonBedrockGenerateReplyHandler(model, client, options);
            agent.GenerateReplyHandlers.Add(generateReplyHandler.GenerateReplyHandlerAsync);
            return agent;
        }

        //public static T AddAmazonBedrockGenerateReply<T>(this T agent, string model, HttpClient httpClient, AmazonBedrockOptions options) where T : IAgent
        //{
        //    var azureOpenAIGenerateReplyHandler = new AmazonBedrockGenerateReplyHandler(model, httpClient, options);
        //    agent.GenerateReplyHandlers.Add(azureOpenAIGenerateReplyHandler.GenerateReplyHandlerAsync);
        //    return agent;
        //}

        public static T AddAmazonBedrockGenerateReply<T>(this T agent, string model, AmazonBedrockRuntimeClient client) where T : IAgent
        {
            return AddAmazonBedrockGenerateReply(agent, model, client, new AmazonBedrockOptions());
        }

        //public static T AddAmazonBedrockGenerateReply<T>(this T agent, string model, HttpClient httpClient) where T : IAgent
        //{
        //    return AddAmazonBedrockGenerateReply(agent, model, httpClient, new AmazonBedrockOptions());
        //}

        public static T AddAmazonBedrockGenerateReply<T>(this T agent, string model, AmazonBedrockRuntimeClient client, RequestOptions requestOptions) where T : IAgent
        {
            return AddAmazonBedrockGenerateReply(agent, model, client, new AmazonBedrockOptions() { RequestOptions = requestOptions });
        }

        //public static T AddAmazonBedrockGenerateReply<T>(this T agent, string model, HttpClient httpClient, RequestOptions requestOptions) where T : IAgent
        //{
        //    return AddAmazonBedrockGenerateReply(agent, model, httpClient, new AmazonBedrockOptions() { RequestOptions = requestOptions });
        //}
    }
}
