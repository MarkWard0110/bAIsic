using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAIsic.Interlocutor
{
    public interface ISelectSpeakerAgent: IAgent
    {
        public ICheckSelectSpeakerAgent CheckSelectSpeakerAgent { get; }
        public IEnumerable<Message> InitialMessages(IConversableAgent speaker, Message message, IList<IConversableAgent> agents, IDictionary<string, List<string>> allowedTransitions);
        public Message InitialChatMessage(IConversableAgent speaker, Message message, IList<IConversableAgent> agents, IDictionary<string, List<string>> allowedTransitions);
    }
}
