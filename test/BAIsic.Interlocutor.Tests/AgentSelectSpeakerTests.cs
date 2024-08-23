using BAIsic.Tests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAIsic.Interlocutor.Tests
{
    public class AgentSelectSpeakerTests
    {
        public void AgentSelectSpeaker_SystemMessageChatMessage_WhenInitialize()
        {
            static IEnumerable<Message> mockInitialMessages(IConversableAgent speaker, Message message, IList<IConversableAgent> agents, IDictionary<string, List<string>> allowedTransitions)
            {
                return new List<Message> { new(AgentConsts.Roles.System, "Mock system message"), new(AgentConsts.Roles.User, "Mock message") };
            }

            static Message mockInitialChatMessage(IConversableAgent speaker, Message message, IList<IConversableAgent> agents, IDictionary<string, List<string>> allowedTransitions)
            {
                return new Message(AgentConsts.Roles.User, "Mock initial chat message");
            }

            var mockCheckSelectSpeakerAgent = new MockCheckSelectSpeakerAgent("mockCheck");
            var mockAgent = new MockSelectSpeakerAgent("MockAgent", mockInitialMessages, mockInitialChatMessage, mockCheckSelectSpeakerAgent);
        }

        public void AgentSelectSpeaker_SystemChatMessage_WhenInitialize()
        {
            static IEnumerable<Message> mockInitialMessages(IConversableAgent speaker, Message message, IList<IConversableAgent> agents, IDictionary<string, List<string>> allowedTransitions)
            {
                return new List<Message> { new(AgentConsts.Roles.System, "Mock system message") };
            }

            static Message mockInitialChatMessage(IConversableAgent speaker, Message message, IList<IConversableAgent> agents, IDictionary<string, List<string>> allowedTransitions)
            {
                return new Message(AgentConsts.Roles.User, "Mock message used as initial chat message");
            }

            var mockCheckSelectSpeakerAgent = new MockCheckSelectSpeakerAgent("mockCheck");
            var mockAgent = new MockSelectSpeakerAgent("MockAgent", mockInitialMessages, mockInitialChatMessage, mockCheckSelectSpeakerAgent);
        }
    }
}
