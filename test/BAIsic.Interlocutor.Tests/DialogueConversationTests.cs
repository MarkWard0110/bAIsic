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
            var initiatorAgent = new Agent(BAIsicTestConventions.Agent.RandomName())
                .AddHugGenerateReply("initiator");
            var participantAgent = new Agent(BAIsicTestConventions.Agent.RandomName())
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
            var initiatorAgent = new Agent(BAIsicTestConventions.Agent.RandomName(), "you are a helpful assistant")
                .AddHugGenerateReply("initiator");
            var participantAgent = new Agent(BAIsicTestConventions.Agent.RandomName(), "the assistant is helpful")
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

            Assert.Equal(new Message(AgentConsts.Roles.System, "you are a helpful assistant"), conversationResult.Conversation[0].Messages[0]);
            Assert.Equal(new Message(AgentConsts.Roles.System, "the assistant is helpful"), conversationResult.Conversation[1].Messages[0]);
        }

        [Fact]
        public async Task DialogConversation_ExecutesThreeTurns_WhenCalled()
        {
            // Arrange
            var initiatorAgent = new Agent(BAIsicTestConventions.Agent.RandomName())
                .AddHugGenerateReply("initiator");
            var participantAgent = new Agent(BAIsicTestConventions.Agent.RandomName())
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

            Assert.Equal(new Message(AgentConsts.Roles.Assistant, "hello"), conversationResult.Conversation[0].Messages[0]);
            Assert.Equal(new Message(AgentConsts.Roles.User, "participant_hug(initiator_hug(participant_hug(initiator_hug(participant_hug(hello)))))"), conversationResult.Conversation[0].Messages.Last());


            Assert.Equal(new Message(AgentConsts.Roles.User, "hello"), conversationResult.Conversation[1].Messages[0]);
            Assert.Equal(new Message(AgentConsts.Roles.Assistant, "participant_hug(initiator_hug(participant_hug(initiator_hug(participant_hug(hello)))))"), conversationResult.Conversation[1].Messages.Last());
        }

        [Fact]
        public async Task DialogConversation_TerminateInitiatorSendPrepare_WhenCalled()
        {
            // Arrange
            var initiatorAgent = new Agent(BAIsicTestConventions.Agent.RandomName())
                .AddNullPrepareSend()
                .AddHugGenerateReply("initiator");
            var participantAgent = new Agent(BAIsicTestConventions.Agent.RandomName())
                .AddHugGenerateReply("participant");
            var initialMessage = new Message(AgentConsts.Roles.User, "hello");
            var conversation = new DialogueConversation();
            int turnCount = 3;

            // Act
            var conversationResult = await conversation.InitiateChat(initiatorAgent, initialMessage, participantAgent, turnCount);

            // Assert
            Assert.Equal(0, conversationResult.TurnCount);
            Assert.Equal(2, conversationResult.Conversation.Count);
            Assert.Empty(conversationResult.Conversation[0].Messages);
            Assert.Empty(conversationResult.Conversation[1].Messages);
        }

        [Fact]
        public async Task DialogConversation_TerminateInitiatorSend_WhenCalled()
        {
            // Arrange
            var initiatorAgent = new Agent(BAIsicTestConventions.Agent.RandomName())
                .AddHugGenerateReply("initiator");
            var participantAgent = new Agent(BAIsicTestConventions.Agent.RandomName())
                .AddLiteralGenerateReply(true, "goodbye");

            var initialMessage = new Message(AgentConsts.Roles.User, "hello and goodbye");
            var conversation = new DialogueConversation();
            int turnCount = 3;

            static async Task<bool> goodbyeTerminate(bool isInitialMessage, IAgent agent, Message message)
            {
                return message.Text.Contains("goodbye", StringComparison.OrdinalIgnoreCase);
            }

            // Act
            var conversationResult = await conversation.InitiateChat(initiatorAgent, initialMessage, participantAgent, turnCount, goodbyeTerminate);

            // Assert
            Assert.Equal(0, conversationResult.TurnCount);
            Assert.Equal(2, conversationResult.Conversation.Count);
            Assert.Single(conversationResult.Conversation[0].Messages);
            Assert.Empty(conversationResult.Conversation[1].Messages);

            Assert.Equal(new Message(AgentConsts.Roles.Assistant, "hello and goodbye"), conversationResult.Conversation[0].Messages[0]);
        }

        [Fact]
        public async Task DialogConversation_TerminateParticipantReceivePrepare_WhenCalled()
        {
            // Arrange
            var initiatorAgent = new Agent(BAIsicTestConventions.Agent.RandomName())
                .AddHugGenerateReply("initiator");
            var participantAgent = new Agent(BAIsicTestConventions.Agent.RandomName())
                .AddNullPrepareReceive()
                .AddLiteralGenerateReply(true, "goodbye");

            var initialMessage = new Message(AgentConsts.Roles.User, "hello and goodbye");
            var conversation = new DialogueConversation();
            int turnCount = 3;

            // Act
            var conversationResult = await conversation.InitiateChat(initiatorAgent, initialMessage, participantAgent, turnCount);

            // Assert
            Assert.Equal(0, conversationResult.TurnCount);
            Assert.Equal(2, conversationResult.Conversation.Count);
            Assert.Single(conversationResult.Conversation[0].Messages);
            Assert.Empty(conversationResult.Conversation[1].Messages);

            Assert.Equal(new Message(AgentConsts.Roles.Assistant, "hello and goodbye"), conversationResult.Conversation[0].Messages[0]);
        }

        [Fact]
        public async Task DialogConversation_TerminateParticipantGenerateReply_WhenCalled()
        {
            // Arrange
            var initiatorAgent = new Agent(BAIsicTestConventions.Agent.RandomName())
                .AddHugGenerateReply("initiator");
            var participantAgent = new Agent(BAIsicTestConventions.Agent.RandomName())
                .AddLiteralGenerateReply(true, null);

            var initialMessage = new Message(AgentConsts.Roles.User, "hello and goodbye");
            var conversation = new DialogueConversation();
            int turnCount = 3;

            // Act
            var conversationResult = await conversation.InitiateChat(initiatorAgent, initialMessage, participantAgent, turnCount);

            // Assert
            Assert.Equal(0, conversationResult.TurnCount);
            Assert.Equal(2, conversationResult.Conversation.Count);
            Assert.Single(conversationResult.Conversation[0].Messages);
            Assert.Single(conversationResult.Conversation[1].Messages);

            Assert.Equal(new Message(AgentConsts.Roles.Assistant, "hello and goodbye"), conversationResult.Conversation[0].Messages[0]);
        }

        [Fact]
        public async Task DialogConversation_TerminateParticipantSendPrepare_WhenCalled()
        {
            // Arrange
            var initiatorAgent = new Agent(BAIsicTestConventions.Agent.RandomName())
                .AddHugGenerateReply("initiator");
            var participantAgent = new Agent(BAIsicTestConventions.Agent.RandomName())
                .AddNullPrepareSend()
                .AddLiteralGenerateReply(true, "goodbye");

            var initialMessage = new Message(AgentConsts.Roles.User, "hello and goodbye");
            var conversation = new DialogueConversation();
            int turnCount = 3;

            // Act
            var conversationResult = await conversation.InitiateChat(initiatorAgent, initialMessage, participantAgent, turnCount);

            // Assert
            Assert.Equal(0, conversationResult.TurnCount);
            Assert.Equal(2, conversationResult.Conversation.Count);
            Assert.Single(conversationResult.Conversation[0].Messages);
            Assert.Single(conversationResult.Conversation[1].Messages);

            Assert.Equal(new Message(AgentConsts.Roles.Assistant, "hello and goodbye"), conversationResult.Conversation[0].Messages[0]);
        }

        [Fact]
        public async Task DialogConversation_TerminateInitiatorReceivePrepare_WhenCalled()
        {
            // Arrange
            var initiatorAgent = new Agent(BAIsicTestConventions.Agent.RandomName())
                .AddNullPrepareReceive()
                .AddHugGenerateReply("initiator");
            var participantAgent = new Agent(BAIsicTestConventions.Agent.RandomName())
                .AddLiteralGenerateReply(true, "goodbye");

            var initialMessage = new Message(AgentConsts.Roles.User, "hello and goodbye");
            var conversation = new DialogueConversation();
            int turnCount = 3;

            // Act
            var conversationResult = await conversation.InitiateChat(initiatorAgent, initialMessage, participantAgent, turnCount);

            // Assert
            Assert.Equal(0, conversationResult.TurnCount);
            Assert.Equal(2, conversationResult.Conversation.Count);
            Assert.Single(conversationResult.Conversation[0].Messages);
            Assert.Equal(2, conversationResult.Conversation[1].Messages.Count);

            Assert.Equal(new Message(AgentConsts.Roles.Assistant, "hello and goodbye"), conversationResult.Conversation[0].Messages[0]);
        }

        [Fact]
        public async Task DialogConversation_TerminateInitiatorGenerateReply_WhenCalled()
        {
            // Arrange
            var initiatorAgent = new Agent(BAIsicTestConventions.Agent.RandomName())
                .AddLiteralGenerateReply(true, null);
            var participantAgent = new Agent(BAIsicTestConventions.Agent.RandomName())
                .AddLiteralGenerateReply(true, "goodbye");

            var initialMessage = new Message(AgentConsts.Roles.User, "hello and goodbye");
            var conversation = new DialogueConversation();
            int turnCount = 3;

            // Act
            var conversationResult = await conversation.InitiateChat(initiatorAgent, initialMessage, participantAgent, turnCount);

            // Assert
            Assert.Equal(1, conversationResult.TurnCount);
            Assert.Equal(2, conversationResult.Conversation.Count);
            Assert.Equal(2, conversationResult.Conversation[0].Messages.Count);
            Assert.Equal(2, conversationResult.Conversation[1].Messages.Count);

            Assert.Equal(new Message(AgentConsts.Roles.Assistant, "hello and goodbye"), conversationResult.Conversation[0].Messages[0]);
        }

        [Fact]
        public async Task DialogConversation_TerminateParticipantSendCheckingBool_WhenCalled()
        {
            // Arrange
            var initiatorAgent = new Agent(BAIsicTestConventions.Agent.RandomName())
                .AddHugGenerateReply("initiator");
            var participantAgent = new Agent(BAIsicTestConventions.Agent.RandomName())
                .AddLiteralGenerateReply(true, "goodbye");
            var initialMessage = new Message(AgentConsts.Roles.User, "hello and goodbye");
            var conversation = new DialogueConversation();
            int turnCount = 3;

            async Task<bool> goodbyeTerminate(bool isInitialMessage, IAgent agent, Message message)
            {
                if (isInitialMessage) return false;
                if (ReferenceEquals(participantAgent, agent)) return false;

                return message.Text.Contains("goodbye", StringComparison.OrdinalIgnoreCase);
            }

            // Act
            var conversationResult = await conversation.InitiateChat(initiatorAgent, initialMessage, participantAgent, turnCount, goodbyeTerminate);

            // Assert
            Assert.Equal(1, conversationResult.TurnCount);
            Assert.Equal(2, conversationResult.Conversation.Count);
            Assert.Equal(2+1, conversationResult.Conversation[0].Messages.Count);
            Assert.Equal(2, conversationResult.Conversation[1].Messages.Count);

            Assert.Equal(new Message(AgentConsts.Roles.Assistant, "hello and goodbye"), conversationResult.Conversation[0].Messages[0]);
            Assert.Equal(new Message(AgentConsts.Roles.User, "goodbye"), conversationResult.Conversation[0].Messages[1]);
            Assert.Equal(new Message(AgentConsts.Roles.Assistant, "initiator_hug(goodbye)"), conversationResult.Conversation[0].Messages[2]);

            Assert.Equal(new Message(AgentConsts.Roles.User, "hello and goodbye"), conversationResult.Conversation[1].Messages[0]);
            Assert.Equal(new Message(AgentConsts.Roles.Assistant, "goodbye"), conversationResult.Conversation[1].Messages.Last());
        }

        [Fact]
        public async Task DialogConversation_TerminateInitiatorSendCheckingBool_WhenCalled()
        {
            // Arrange
            var initiatorAgent = new Agent(BAIsicTestConventions.Agent.RandomName())
                .AddHugGenerateReply("initiator");
            var participantAgent = new Agent(BAIsicTestConventions.Agent.RandomName())
                .AddLiteralGenerateReply(true, "goodbye");
            var initialMessage = new Message(AgentConsts.Roles.User, "hello and goodbye");
            var conversation = new DialogueConversation();
            int turnCount = int.MaxValue;

            static async Task<bool> goodbyeTerminate(bool isInitialMessage, IAgent agent, Message message)
            {
                if (isInitialMessage) return false;

                return message.Text.Contains("goodbye", StringComparison.OrdinalIgnoreCase);
            }

            // Act
            var conversationResult = await conversation.InitiateChat(initiatorAgent, initialMessage, participantAgent, turnCount, goodbyeTerminate);

            // Assert
            Assert.Equal(1, conversationResult.TurnCount);
            Assert.Equal(2, conversationResult.Conversation.Count);
            Assert.Equal(2, conversationResult.Conversation[0].Messages.Count);
            Assert.Equal(2, conversationResult.Conversation[1].Messages.Count);

            Assert.Equal(new Message(AgentConsts.Roles.Assistant, "hello and goodbye"), conversationResult.Conversation[0].Messages[0]);
            Assert.Equal(new Message(AgentConsts.Roles.User, "goodbye"), conversationResult.Conversation[0].Messages.Last());

            Assert.Equal(new Message(AgentConsts.Roles.User, "hello and goodbye"), conversationResult.Conversation[1].Messages[0]);
            Assert.Equal(new Message(AgentConsts.Roles.Assistant, "goodbye"), conversationResult.Conversation[1].Messages.Last());
        }

        [Fact]
        public async Task DialogConversation_TerminateParticipantSendCheckingAgent_WhenCalled()
        {
            // Arrange
            var initiatorAgent = new Agent(BAIsicTestConventions.Agent.RandomName())
                .AddHugGenerateReply("initiator");
            var participantAgent = new Agent(BAIsicTestConventions.Agent.RandomName())
                .AddLiteralGenerateReply(true, "goodbye")
                .AddHugGenerateReply("participant");
            var initialMessage = new Message(AgentConsts.Roles.User, "hello and goodbye");
            var conversation = new DialogueConversation();
            int turnCount = int.MaxValue;

            async Task<bool> goodbyeTerminate(bool isInitialMessage, IAgent agent, Message message)
            {
                if (!object.ReferenceEquals(agent, participantAgent)) return false;

                return message.Text.Contains("goodbye", StringComparison.OrdinalIgnoreCase);
            }

            // Act
            var conversationResult = await conversation.InitiateChat(initiatorAgent, initialMessage, participantAgent, turnCount, goodbyeTerminate);

            // Assert
            Assert.Equal(1, conversationResult.TurnCount);
            Assert.Equal(2, conversationResult.Conversation.Count);
            Assert.Equal(2, conversationResult.Conversation[0].Messages.Count);
            Assert.Equal(2, conversationResult.Conversation[1].Messages.Count);

            Assert.Equal(new Message(AgentConsts.Roles.Assistant, "hello and goodbye"), conversationResult.Conversation[0].Messages[0]);
            Assert.Equal(new Message(AgentConsts.Roles.User, "goodbye"), conversationResult.Conversation[0].Messages.Last());

            Assert.Equal(new Message(AgentConsts.Roles.User, "hello and goodbye"), conversationResult.Conversation[1].Messages[0]);
            Assert.Equal(new Message(AgentConsts.Roles.Assistant, "goodbye"), conversationResult.Conversation[1].Messages.Last());
        }
    }
}
