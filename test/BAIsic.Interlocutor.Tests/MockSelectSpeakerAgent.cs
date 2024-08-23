using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAIsic.Interlocutor.Tests
{
    public class MockSelectSpeakerAgent : Agent, ISelectSpeakerAgent
    {
        public delegate IEnumerable<Message> MockInitialMessages(IConversableAgent speaker, Message message, IList<IConversableAgent> agents, IDictionary<string, List<string>> allowedTransitions);
        public delegate Message MockInitialChatMessage(IConversableAgent speaker, Message message, IList<IConversableAgent> agents, IDictionary<string, List<string>> allowedTransitions);

        private readonly MockInitialMessages _mockInitialMessages;
        private readonly MockInitialChatMessage _mockInitialChatMessage;
        private readonly ICheckSelectSpeakerAgent _checkSelectSpeakerAgent;

        public ICheckSelectSpeakerAgent CheckSelectSpeakerAgent => throw new NotImplementedException();

        public MockSelectSpeakerAgent(string name, MockInitialMessages mockInitialMessages, MockInitialChatMessage mockInitialChatMessage, ICheckSelectSpeakerAgent checkSelectSpeakerAgent) : base(name, null)
        {
            _mockInitialMessages = mockInitialMessages;
            _mockInitialChatMessage = mockInitialChatMessage;
            _checkSelectSpeakerAgent = checkSelectSpeakerAgent;
        }

        public Message InitialChatMessage(IConversableAgent speaker, Message message, IList<IConversableAgent> agents, IDictionary<string, List<string>> allowedTransitions)
        {
            return _mockInitialChatMessage(speaker, message, agents, allowedTransitions);
        }

        public IEnumerable<Message> InitialMessages(IConversableAgent speaker, Message message, IList<IConversableAgent> agents, IDictionary<string, List<string>> allowedTransitions)
        {
            return _mockInitialMessages(speaker, message, agents, allowedTransitions);
        }
    }
}
