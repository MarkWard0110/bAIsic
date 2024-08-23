using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAIsic.Interlocutor
{
    public interface ICheckSelectSpeakerAgent: IAgent
    {
        public IList<IConversableAgent>? Agents { get; set; }
        public Task<bool> ConversationTerminateHandlerAsync(bool isInitialMessage, IAgent agent, Message message);
    }
}
