using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAIsic.Interlocutor
{
    public static class AgentGenerics
    {
        public static Agent AddStringLiteralGenerateReply(this Agent agent, string reply)
        {
            agent.GenerateReplyHandlers.Add((messages) =>
            {
                return Task.FromResult<(bool, Message?)>((true, new Message(AgentConsts.Roles.Assistant, reply)));
            });
            return agent;
        }

        public static Agent AddLiteralGenerateReply(this Agent agent, bool isDone, string? reply)
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
    }
}
