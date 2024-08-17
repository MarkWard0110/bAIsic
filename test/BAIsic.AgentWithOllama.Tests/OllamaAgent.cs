using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BAIsic.Interlocutor;
using BAIsic.LlmApi.Ollama;
using BAIsic.LlmApi.Ollama.Tests;
using static BAIsic.LlmApi.Ollama.Tests.OllamaTestConsts;

namespace BAIsic.AgentWithOllama.Tests
{
    public class OllamaAgent : Agent
    {
        private readonly OllamaClient _ollamaClient;
        private readonly string _model;

        public OllamaAgent(string name, string model, string? systemPrompt = null) : base(name, systemPrompt)
        {
            _ollamaClient = OllamaClientExtensions.CreateOllamaClient();
            _model = model;

            _generateReplyHandlers.Add(GenerateReplyHandlerAsync);
        }

        private async Task<(bool isDone, BAIsic.Interlocutor.Message? message)> GenerateReplyHandlerAsync(IEnumerable<BAIsic.Interlocutor.Message> messages)
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
                    Temperature = 0.4f,
                    TopP = 0.4f,
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
