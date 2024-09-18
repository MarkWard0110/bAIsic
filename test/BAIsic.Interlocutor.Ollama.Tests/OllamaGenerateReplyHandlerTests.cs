using BAIsic.LlmApi.Ollama.Tests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BAIsic.Interlocutor.Ollama.Tests
{
    public class OllamaGenerateReplyHandlerTests
    {
        [Theory]
        [RequireModelData(OllamaTestConsts.Model.Llama3_1_8b)]
        public async Task OllamaGenerateReplyHandler_ReplysWithJson_WhenOptionFormatSet(string model)
        {
            var initiatorAgent = new Agent("initiator");
            var participantAgent = new Agent("llm", @"The user has provided a JSON-formatted answer.
Output only the answer provided by the user.

Repeat the exact input provided without making any changes or assumptions.

Without a greeting or additional information.")
                .AddTestsOllamaGenerateReply(model, new OllamaOptions() { ResponseFormat = "json"});

            var initialMessage = new Message(AgentConsts.Roles.User, @"A0 here, I have 4 chocolates.
My chocolate count is even.

Here's the combined JSON format from all teams:
{
""odd"": [""A2"", ""B1""],
""even"": [""A0"", ""A1"", ""B0"", ""C0"", ""C1""]
}
");
            var conversation = new DialogueConversation();
            var conversationResult = await conversation.InitiateChat(initiatorAgent, initialMessage, participantAgent, maximumTurnCount: 1);
            Assert.True(IsValidJson(conversationResult.Conversation[1].Messages.Last().Text));
            Assert.Equal(1, conversationResult.TurnCount);
        }

        private static bool IsValidJson(string input)
        {
            try
            {
                JsonDocument.Parse(input);
                return true;
            }
            catch (JsonException)
            {
                return false;
            }
            catch (ArgumentNullException)
            {
                return false;
            }
        }
    }
}
