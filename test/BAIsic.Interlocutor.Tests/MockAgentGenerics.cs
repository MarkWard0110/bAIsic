using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAIsic.Interlocutor.Tests
{
    public static class MockAgentBuilderGenerics
    {
        public static Agent AddHugGenerateReply(this Agent agent, string hugName)
        {
            agent.GenerateReplyHandlers.Add((messages) =>
            {
                var lastMessage = messages.Last();
                return Task.FromResult<(bool, Message?)>((true, new Message(AgentConsts.Roles.Assistant, $"{hugName}_hug({lastMessage.Text})")));
            });
            return agent;
        }

        public static Agent AddToUpperPrepareSend(this Agent agent)
        {
            agent.PrepareSendHandlers.Add((message) =>
            {
                if (message == null)
                {
                    return Task.FromResult<Message?>(null);
                }
                return Task.FromResult<Message?>( message with { Text = message.Text.ToUpper()});
            });
            return agent;
        }

        public static Agent AddToLowerPrepareSend(this Agent agent)
        {
            agent.PrepareSendHandlers.Add((message) =>
            {
                if (message == null)
                {
                    return Task.FromResult<Message?>(null);
                }
                return Task.FromResult<Message?>(message with { Text = message.Text.ToLower() });
            });
            return agent;
        }

        public static Agent AddToUpperPrepareReceive(this Agent agent)
        {
            agent.PrepareReceiveHandlers.Add((message) =>
            {
                if (message == null)
                {
                    return Task.FromResult<Message?>(null);
                }
                return Task.FromResult<Message?>(message with { Text = message.Text.ToUpper() });
            });
            return agent;
        }

        public static Agent AddToLowerPrepareReceive(this Agent agent)
        {
            agent.PrepareReceiveHandlers.Add((message) =>
            {
                if (message == null)
                {
                    return Task.FromResult<Message?>(null);
                }
                return Task.FromResult<Message?>(message with { Text = message.Text.ToLower() });
            });
            return agent;
        }

        public static Agent AddNullPrepareSend(this Agent agent)
        {
            agent.PrepareSendHandlers.Add((message) =>
            {
                return Task.FromResult<Message?>(null);
            });
            return agent;
        }

        public static Agent AddNullPrepareReceive(this Agent agent)
        {
            agent.PrepareReceiveHandlers.Add((message) =>
            {
                return Task.FromResult<Message?>(null);
            });
            return agent;
        }
    }
}
