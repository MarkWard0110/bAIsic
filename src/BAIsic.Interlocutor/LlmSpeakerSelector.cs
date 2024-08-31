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
        private readonly int _maximumTurnCount;

        public LlmSpeakerSelector(ISelectSpeakerAgent selectSpeakerAgent, int maximumTurnCount=5)
        {
            _selectSpeakerAgent = selectSpeakerAgent;
            _checkSelectSpeakerAgent = _selectSpeakerAgent.CheckSelectSpeakerAgent;
            _maximumTurnCount = maximumTurnCount;
        }

        public async Task<(IConversableAgent? Speaker, ConversationResult? SelectSpeakerConversationResult)> SelectSpeakerAsync(IConversableAgent speaker, Message message, IList<IConversableAgent> agents, IDictionary<string, List<string>> allowedTransitions)
        {
            var initialMessages = _selectSpeakerAgent.InitialMessages(speaker, message, agents, allowedTransitions);
            var initialChatMessage = _selectSpeakerAgent.InitialChatMessage(speaker, message, agents, allowedTransitions);

            var dialogueConversation = new DialogueConversation();

            var result = await dialogueConversation.InitiateChat(
                _checkSelectSpeakerAgent, 
                initialChatMessage, 
                _selectSpeakerAgent, 
                terminateHandler: _checkSelectSpeakerAgent.ConversationTerminateHandlerAsync, 
                maximumTurnCount: _maximumTurnCount,
                participantInitMessages: initialMessages);

            return (GetSpeaker(result.Conversation[0].Messages.Last(), agents), result);
        }

        private IConversableAgent? GetSpeaker(Message message, IList<IConversableAgent> agents)
        {
            if (message.Text.Contains(GroupConversationConsts.AgentSelected))
            {
                var agentName = message.Text.Replace(GroupConversationConsts.AgentSelected, "");
                return agents.First(a => a.Name == agentName);
            }
            else
            {
                return null;
            }
        }
    }
}
