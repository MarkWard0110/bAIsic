using BAIsic.Interlocutor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAIsic.AgentWithOllama.Tests
{
    public static class OllamaAgentExtentions
    {
        public static T AddOllamaGenerateReply<T>(this T agent, string model) where T : IAgent
        {
            var ollamaGenerateReplyHandler = new OllamaGenerateReplyHandler(model);
            agent.GenerateReplyHandlers.Add(ollamaGenerateReplyHandler.GenerateReplyHandlerAsync);
            return agent;
        }
    }
}
