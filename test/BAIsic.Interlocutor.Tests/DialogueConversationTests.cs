using BAIsic.Tests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAIsic.Interlocutor.Tests
{
    public class DialogueConversationTests
    {
        [Fact]
        public async Task DialogConversation_ExecutesOneTurn_WhenCalledWithDefaultTurn()
        {
            // Arrange
            var initiatorAgent = new MockAgent(BAIsicTestConventions.Agent.RandomName())
                .AddHugGenerateReply("initiator");
            var participantAgent = new MockAgent(BAIsicTestConventions.Agent.RandomName())
                .AddHugGenerateReply("participant");

            var initialMessage = new Message(AgentConsts.Roles.User, "hello");
            var conversation = new DialogueConversation();

            // Act
            var conversationResult = await conversation.InitiateChat(initiatorAgent, initialMessage, participantAgent);

            // Assert
            Assert.Equal(1, conversationResult.TurnCount);
            Assert.Equal(2, conversationResult.Conversation.Count);
            Assert.Equal(2, conversationResult.Conversation[0].Messages.Count);
            Assert.Equal(2, conversationResult.Conversation[1].Messages.Count);
        }

        [Fact]
        public async Task DialogConversation_HasSystemMessage_WhenCalled()
        {
            // Arrange
            var initiatorAgent = new MockAgent(BAIsicTestConventions.Agent.RandomName(), "you are a helpful assistant")
                .AddHugGenerateReply("initiator");
            var participantAgent = new MockAgent(BAIsicTestConventions.Agent.RandomName(), "the assistant is helpful")
                .AddHugGenerateReply("participant");

            var initialMessage = new Message(AgentConsts.Roles.User, "hello");
            var conversation = new DialogueConversation();

            // Act
            var conversationResult = await conversation.InitiateChat(initiatorAgent, initialMessage, participantAgent);

            // Assert
            Assert.Equal(1, conversationResult.TurnCount);
            Assert.Equal(2, conversationResult.Conversation.Count);
            Assert.Equal(2 + 1, conversationResult.Conversation[0].Messages.Count);
            Assert.Equal(2 + 1, conversationResult.Conversation[1].Messages.Count);

            Assert.Equal(new Message("system", "you are a helpful assistant"), conversationResult.Conversation[0].Messages[0]);
            Assert.Equal(new Message("system", "the assistant is helpful"), conversationResult.Conversation[1].Messages[0]);
        }

        [Fact]
        public async Task DialogConversation_ExecutesThreeTurns_WhenCalled()
        {
            // Arrange
            var initiatorAgent = new MockAgent(BAIsicTestConventions.Agent.RandomName())
                .AddHugGenerateReply("initiator");
            var participantAgent = new MockAgent(BAIsicTestConventions.Agent.RandomName())
                .AddHugGenerateReply("participant");

            var initialMessage = new Message(AgentConsts.Roles.User, "hello");
            var conversation = new DialogueConversation();
            int turnCount = 3;
            // Act
            var conversationResult = await conversation.InitiateChat(initiatorAgent, initialMessage, participantAgent, turnCount);

            // Assert
            Assert.Equal(3, conversationResult.TurnCount);
            Assert.Equal(2, conversationResult.Conversation.Count);
            Assert.Equal(2*turnCount, conversationResult.Conversation[0].Messages.Count);
            Assert.Equal(2*turnCount, conversationResult.Conversation[1].Messages.Count);

            Assert.Equal(new Message("assistant", "hello"), conversationResult.Conversation[0].Messages[0]);
            Assert.Equal(new Message("user", "participant_hug(initiator_hug(participant_hug(initiator_hug(participant_hug(hello)))))"), conversationResult.Conversation[0].Messages.Last());


            Assert.Equal(new Message("user", "hello"), conversationResult.Conversation[1].Messages[0]);
            Assert.Equal(new Message("assistant", "participant_hug(initiator_hug(participant_hug(initiator_hug(participant_hug(hello)))))"), conversationResult.Conversation[1].Messages.Last());
        }

        [Fact]
        public async Task DialogConversation_TerminateInitiatorSend_WhenCalled()
        {
            // Arrange
            var initiatorAgent = new MockAgent(BAIsicTestConventions.Agent.RandomName())
                .AddNullPrepareSend()
                .AddHugGenerateReply("initiator");
            var participantAgent = new MockAgent(BAIsicTestConventions.Agent.RandomName())
                .AddHugGenerateReply("participant");
            var initialMessage = new Message(AgentConsts.Roles.User, "hello");
            var conversation = new DialogueConversation();
            int turnCount = int.MaxValue;

            // Act
            var conversationResult = await conversation.InitiateChat(initiatorAgent, initialMessage, participantAgent, turnCount);

            // Assert
            Assert.Equal(0, conversationResult.TurnCount);
            Assert.Equal(2, conversationResult.Conversation.Count);
            Assert.Empty(conversationResult.Conversation[0].Messages);
            Assert.Empty(conversationResult.Conversation[1].Messages);
        }
    }
}
