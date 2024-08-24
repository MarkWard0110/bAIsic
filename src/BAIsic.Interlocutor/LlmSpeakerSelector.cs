using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAIsic.Interlocutor
{
    public class LlmSpeakerSelector
    {
        private readonly ISelectSpeakerAgent _selectSpeakerAgent;
        private readonly ICheckSelectSpeakerAgent _checkSelectSpeakerAgent;
        public LlmSpeakerSelector(ISelectSpeakerAgent selectSpeakerAgent)
        {
            _selectSpeakerAgent = selectSpeakerAgent;
            _checkSelectSpeakerAgent = _selectSpeakerAgent.CheckSelectSpeakerAgent;
        }

        public async Task<IConversableAgent> SelectSpeakerAsync(IConversableAgent speaker, Message message, IList<IConversableAgent> agents, IDictionary<string, List<string>> allowedTransitions)
        {
            var initialMessages = _selectSpeakerAgent.InitialMessages(speaker, message, agents, allowedTransitions);
            var initialChatMessage = _selectSpeakerAgent.InitialChatMessage(speaker, message, agents, allowedTransitions);

            var dialogueConversation = new DialogueConversation();

            var result = await dialogueConversation.InitiateChat(
                _checkSelectSpeakerAgent, 
                initialChatMessage, 
                _selectSpeakerAgent, 
                terminateHandler: _checkSelectSpeakerAgent.ConversationTerminateHandlerAsync, 
                maximumTurnCount: 5,
                participantInitMessages: initialMessages);

            return GetSpeaker(result.Conversation[0].Messages.Last(), agents);
        }

        private IConversableAgent GetSpeaker(Message message, IList<IConversableAgent> agents)
        {
            if (message.Text.Contains(GroupConversationConsts.AgentSelected))
            {
                var agentName = message.Text.Replace(GroupConversationConsts.AgentSelected, "");
                return agents.First(a => a.Name == agentName);
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}
