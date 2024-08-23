using BAIsic.Interlocutor;
using BAIsic.LlmApi.Ollama.Tests;
using BAIsic.LlmApi.Ollama;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAIsic.AgentWithOllama.Tests
{
    public class OllamaGenerateReplyHandler
    {
        private readonly OllamaClient _ollamaClient;
        private readonly string _model;

        public OllamaGenerateReplyHandler(string model)
        {
            _ollamaClient = OllamaClientExtensions.CreateOllamaClient();
            _model = model;
        }


        public async Task<(bool isDone, BAIsic.Interlocutor.Message? message)> GenerateReplyHandlerAsync(IEnumerable<BAIsic.Interlocutor.Message> messages)
        {
            IList<LlmApi.Ollama.Message> ollamaMessages = messages.Select(m => new LlmApi.Ollama.Message
            {
                Role = m.Role,
                Content = m.Text,
            }).ToList();


            var chatRequest = new ChatRequest()
            {
                Model = _model,
                Stream = true,
                Options = new RequestOptions()
                {
                    Temperature = 0.1f,
                    TopP = 0.1f,
                },
                Messages = ollamaMessages
            };

            var chatResponse = await _ollamaClient.InvokeChatCompletionAsync(chatRequest);

            if (chatResponse.Message == null)
            {
                return (true, null);
            }

            return (true, new BAIsic.Interlocutor.Message(AgentConsts.Roles.Assistant, chatResponse.Message.Content));
        }
    }
}

