using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAIsic.Interlocutor
{
    public class DialogueConversation : IConversation
    {
        public async Task<ConversationResult> InitiateChat(IAgent initiator, Message initialMessage, IAgent participant, int maximumTurnCount = 1, ConversationTerminateHandlerAsync? terminateHandler = null, IEnumerable<Message>? initiatorInitMessages=null, IEnumerable<Message>? participantInitMessages=null)
        {
            // initialize the conversation
            List<Message> initiatorMessages = [];
            List<Message> participantMessages = [];
            int turnCount = 0;
            bool isInitialMessage = true;

            // initialize initiator
            if (initiatorInitMessages != null)
            {
                initiatorMessages.AddRange(initiatorInitMessages);
            }
            else
            {
                if (!string.IsNullOrEmpty(initiator.SystemPrompt))
                {
                    initiatorMessages.Add(AgentConventions.SystemMessage(initiator.SystemPrompt));
                }
            }

            // initialize participant
            if (participantInitMessages != null)
            {
                participantMessages.AddRange(participantInitMessages);
            }
            else
            {
                if (!string.IsNullOrEmpty(participant.SystemPrompt))
                {
                    participantMessages.Add(AgentConventions.SystemMessage(participant.SystemPrompt));
                }
            }

            var initiatorMessage = initialMessage;

            // dialogue loop
            while (true)
            {
                // initiator send
                var initiatorSendMessage = await initiator.PrepareSendMessageAsync(initiatorMessage);
                
                if (initiatorSendMessage == null)
                {
                    // terminate conversation (initiator send message is null)
                    break;
                }

                initiatorMessages.Add(initiatorSendMessage);

                // termination handler check
                if (terminateHandler != null)
                {
                    if (await terminateHandler(isInitialMessage, initiator, initiatorSendMessage))
                    {
                        // terminate conversation (terminate handler on initiator send)
                        break;
                    }
                }
                isInitialMessage = false;

                // participant receive
                var participantReceiveMessage = await participant.PrepareReceiveMessageAsync(initiatorSendMessage);
                
                if (participantReceiveMessage == null)
                {
                    // terminate conversation (participant receive message is null)
                    break;
                }
                
                participantMessages.Add(participantReceiveMessage);

                // participant generate reply
                var participantGenerateReply = await participant.GenerateReplyAsync(participantMessages);
                
                if (participantGenerateReply == null)
                {
                    // terminate conversation (participant generate reply is null)
                    break;
                }

                // participant send
                var participantSendMessage = await participant.PrepareSendMessageAsync(participantGenerateReply);
                
                if (participantSendMessage == null)
                {
                    // terminate conversation (participant send message is null)
                    break;
                }
                
                participantMessages.Add(participantSendMessage);

                // initiator receive
                var initiatorReceiveMessage = await initiator.PrepareReceiveMessageAsync(participantSendMessage);
                
                if (initiatorReceiveMessage == null)
                {
                    // terminate conversation (initiator receive message is null)
                    break;
                }

                initiatorMessages.Add(initiatorReceiveMessage);

                turnCount++; // indicate a turn has completed

                // turn count check
                if (turnCount >= maximumTurnCount)
                {
                    // terminate conversation (maximum turn count reached)
                    break;
                }

                // terminate handler check
                if (terminateHandler != null)
                {
                    if (await terminateHandler(isInitialMessage, participant, participantSendMessage))
                    {
                        // terminate conversation (terminate handler on participant send)
                        break;
                    }
                }

                // prepare for next turn
                var initiatorReply = await initiator.GenerateReplyAsync(initiatorMessages);

                if (initiatorReply == null) {
                    // terminate conversation (initiator generate reply is null)
                    break;
                }

                initiatorMessage = initiatorReply;
            }

            // create a conversation history
            var initiatorConversationHistory = new ConversationHistory(initiator, [.. initiatorMessages]);
            var participantConversationHistory = new ConversationHistory(participant, [.. participantMessages]);

            var conversationResult = new ConversationResult([initiatorConversationHistory, participantConversationHistory], turnCount);
            return conversationResult;
        }
    }
}
