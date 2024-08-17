using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAIsic.Interlocutor.Tests
{
    public static class MockAgentBuilderGenerics
    {
        public static MockAgent AddStringLiteralGenerateReply(this MockAgent agent, string reply)
        {
            agent.GenerateReplyHandlers.Add( (messages) =>
            {
                return Task.FromResult<(bool, Message?)>((true, new Message(AgentConsts.Roles.Assistant, reply)));
            });
            return agent;
        }

        public static MockAgent AddLiteralGenerateReply(this MockAgent agent,bool isDone, string? reply)
        {
            agent.GenerateReplyHandlers.Add((messages) =>
            {
                if (reply == null)
                {
                    return Task.FromResult<(bool, Message?)>((isDone, null));
                }

                return Task.FromResult<(bool, Message?)>((isDone, new Message(AgentConsts.Roles.Assistant, reply)));
            });
            return agent;
        }

        public static MockAgent AddHugGenerateReply(this MockAgent agent, string hugName)
        {
            agent.GenerateReplyHandlers.Add((messages) =>
            {
                var lastMessage = messages.Last();
                return Task.FromResult<(bool, Message?)>((true, new Message(AgentConsts.Roles.Assistant, $"{hugName}_hug({lastMessage.Text})")));
            });
            return agent;
        }

        public static MockAgent AddToUpperPrepareSend(this MockAgent agent)
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

        public static MockAgent AddToLowerPrepareSend(this MockAgent agent)
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

        public static MockAgent AddToUpperPrepareReceive(this MockAgent agent)
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

        public static MockAgent AddToLowerPrepareReceive(this MockAgent agent)
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

        public static MockAgent AddNullPrepareSend(this MockAgent agent)
        {
            agent.PrepareSendHandlers.Add((message) =>
            {
                return Task.FromResult<Message?>(null);
            });
            return agent;
        }

        public static MockAgent AddNullPrepareReceive(this MockAgent agent)
        {
            agent.PrepareReceiveHandlers.Add((message) =>
            {
                return Task.FromResult<Message?>(null);
            });
            return agent;
        }
    }
}
