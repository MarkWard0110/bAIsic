using BAIsic.Interlocutor;
using BAIsic.LlmApi.Ollama.Tests;
using BAIsic.Tests;
using Microsoft.VisualBasic;

namespace BAIsic.AgentWithOllama.Tests
{
    public class TellAJokeTests
    {
        [Theory]
        [RequireModelData(OllamaTestConsts.Model.Llama3_1_8b)]
        public async Task DialogueConveration_OneTurn_WhenCalled(string model)
        {
            var initiatorAgent = new OllamaAgent("initiator", model, "Your name is Cathy and you are a part of a duo of comedians.");
            var participantAgent = new OllamaAgent("participant", model, "Your name is Joe and you are a part of a duo of comedians.");

            var initialMessage = new Message(AgentConsts.Roles.User, "Cathy, tell me a joke.");
            var conversation = new DialogueConversation();
            var conversationResult = await conversation.InitiateChat(initiatorAgent, initialMessage, participantAgent, maximumTurnCount: 1);
            Assert.Equal(1, conversationResult.TurnCount);
        }

        [Theory]
        [RequireModelData(OllamaTestConsts.Model.Llama3_1_8b)]
        public async Task DialogueConveration_TwoTurn_WhenCalled(string model)
        {
            var initiatorAgent = new OllamaAgent("initiator", model, "Your name is Cathy and you are a part of a duo of comedians.");
            var participantAgent = new OllamaAgent("participant", model, "Your name is Joe and you are a part of a duo of comedians.");

            var initialMessage = new Message(AgentConsts.Roles.User, "Cathy, tell me a joke.");
            var conversation = new DialogueConversation();
            var conversationResult = await conversation.InitiateChat(initiatorAgent, initialMessage, participantAgent, maximumTurnCount: 2);
            Assert.Equal(2, conversationResult.TurnCount);
        }

        [Theory]
        [RequireModelData(OllamaTestConsts.Model.Llama3_1_8b)]
        public async Task DialogueConveration_ThreeTurn_WhenCalled(string model)
        {
            var initiatorAgent = new OllamaAgent("initiator", model, "Your name is Cathy and you are a part of a duo of comedians.");
            var participantAgent = new OllamaAgent("participant", model, "Your name is Joe and you are a part of a duo of comedians.");

            var initialMessage = new Message(AgentConsts.Roles.User, "Cathy, tell me a joke.");
            var conversation = new DialogueConversation();
            var conversationResult = await conversation.InitiateChat(initiatorAgent, initialMessage, participantAgent, maximumTurnCount: 3);
            Assert.Equal(3, conversationResult.TurnCount);
        }

    }
}