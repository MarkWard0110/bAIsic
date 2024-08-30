using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAIsic.Interlocutor
{
    public class GroupConversation
    {
        private readonly IList<IConversableAgent> _agents;
        private readonly IDictionary<string, List<string>> _allowedTransitions;
        private readonly SelectSpeakerHandlerAsync _selectSpeakerHandler;
        private readonly ConversationTerminateHandlerAsync? _terminationHandler;
        private readonly int _maximumTurnCount;

        public GroupConversation(IList<IConversableAgent> agents, SelectSpeakerHandlerAsync selectSpeakerHandler, IDictionary<string, List<string>>? allowedTransitions=null, int maxTurnCount = 1, ConversationTerminateHandlerAsync? terminationHandler=null)
        {
            _agents = agents;
            _selectSpeakerHandler = selectSpeakerHandler;
            _maximumTurnCount = maxTurnCount;
            _terminationHandler = terminationHandler;

            if (allowedTransitions == null)
            {
                _allowedTransitions = new Dictionary<string, List<string>>();
                foreach (var agent in agents)
                {
                    _allowedTransitions[agent.Name] = agents.Select(a => a.Name).ToList();
                }
            }
            else
            {
                _allowedTransitions = allowedTransitions;
            }
        }

        public async Task<ConversationResult> InitiateChatAsync(IConversableAgent initiator, Message initialMessage)
        {
            Dictionary<string, List<Message>> agentMessages = [];

            // initialize agent messages
            foreach (var agent in _agents)
            {
                var messages = new List<Message>();
                if (!string.IsNullOrEmpty(agent.SystemPrompt))
                {
                    messages.Add(AgentConventions.SystemMessage(agent.SystemPrompt));
                }
                agentMessages[agent.Name] = messages;
            }

            var speaker = initiator;
            var groupMessage = initialMessage;
            var turnCount = 0;

            while (true)
            {
                // speaker sends message to "group"
                var speakerSendMessage = await speaker.PrepareSendMessageAsync(groupMessage);
                if (speakerSendMessage == null)
                {
                    // terminate conversation (speaker send message is null)
                    break;
                }

                agentMessages[speaker.Name].Add(speakerSendMessage);

                // broadcast message to all agents
                bool broadcastExit = false;
                foreach (var agent in _agents)
                {
                    if (ReferenceEquals(agent, speaker))
                    {
                        continue;
                    }

                    var agentReceiveMessage = await agent.PrepareReceiveMessageAsync(speakerSendMessage);
                    if (agentReceiveMessage == null)
                    {
                        // terminate conversation (agent receive message is null)
                        broadcastExit = true;
                        break;
                    }

                    agentMessages[agent.Name].Add(agentReceiveMessage);
                }

                if (broadcastExit)
                {
                    // terminate conversation (agent receive message is null)
                    break;
                }

                // terminate and turn count check
                if (_terminationHandler != null)
                {
                    if (await _terminationHandler(0==turnCount, speaker, speakerSendMessage))
                    {
                        // terminate conversation (terminate handler on speaker send)
                        break;
                    }
                }

                // turn count check
                if (turnCount >= _maximumTurnCount)
                {
                    // terminate conversation (maximum turn count reached)
                    break;
                }

                // select next speaker
                speaker = await _selectSpeakerHandler(speaker, speakerSendMessage, _agents, allowedTransitions: _allowedTransitions);

                if (speaker == null)
                {
                    // terminate conversation (select speaker is null)
                    break;
                }

                // speaker generates reply
                var speakerGenerateReply = await speaker.GenerateReplyAsync(agentMessages[speaker.Name]);
                if (speakerGenerateReply == null)
                {
                    // terminate conversation (speaker generate reply is null)
                    break;
                }

                // prepare for next turn
                groupMessage = speakerGenerateReply;

                turnCount++;
            }

            List<ConversationHistory> conversationHistories = [];
            foreach (var agent in _agents)
            {
                conversationHistories.Add(new ConversationHistory(agent, [.. agentMessages[agent.Name]]));
            }

            return new ConversationResult([.. conversationHistories], turnCount);
        }
    }
}
