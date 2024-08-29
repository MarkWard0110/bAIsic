using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAIsic.Interlocutor.Tests
{
    public class MockCheckSelectSpeakerAgent : Agent, ICheckSelectSpeakerAgent
    {

        private IList<IConversableAgent>? _agents;

        public MockCheckSelectSpeakerAgent(string name) : base(name, null)
        {
        }

        public IList<IConversableAgent>? Agents { get => _agents; set => _agents = value; }

        public Task<bool> ConversationTerminateHandlerAsync(bool isInitialMessage, IAgent agent, Message message)
        {
            if (message.Text.Contains(GroupConversationConsts.AgentSelected))
            {
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }
    }
}
