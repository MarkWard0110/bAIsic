using BAIsic.Interlocutor;
using BAIsic.LlmApi.AzureOpenAI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAIsic.Interlocutor.AzureOpenAI
{
    public class AzureOpenAIGenerateReplyHandler
    {
        private readonly AzureOpenAIClient _azureOpenAIClient;
        private readonly string _model;
        private readonly AzureOpenAIOptions _azureOpenAIOptions;

        public AzureOpenAIGenerateReplyHandler(string model, AzureOpenAIClient azureOpenAIClient, AzureOpenAIOptions? options = null)
        {
            _azureOpenAIClient = azureOpenAIClient;
            _model = model;
            _azureOpenAIOptions = options ?? new AzureOpenAIOptions();
        }

        public AzureOpenAIGenerateReplyHandler(string model, HttpClient httpClient, AzureOpenAIOptions? options = null) : this(model, new AzureOpenAIClient(httpClient), options)
        {

        }

        public async Task<(bool isDone, BAIsic.Interlocutor.Message? message)> GenerateReplyHandlerAsync(IEnumerable<BAIsic.Interlocutor.Message> messages)
        {
            IList<LlmApi.AzureOpenAI.Message> ollamaMessages = messages.Select(m => new LlmApi.AzureOpenAI.Message
            {
                Role = m.Role,
                Content = m.Text,
            }).ToList();

            var chatRequest = new ChatRequest()
            {
                Model = _model,
                Stream = false,
                Messages = ollamaMessages,
                ResponseFormat = string.IsNullOrEmpty(_azureOpenAIOptions.ResponseFormat) ? null : new RequestResponseFormat { Type = _azureOpenAIOptions.ResponseFormat }
            };

            var chatResponse = await _azureOpenAIClient.InvokeChatCompletionAsync(chatRequest);

            if (chatResponse.Choices == null)
            {
                return (true, null);
            }

            // TODO: handle multiple choices
            return (true, new BAIsic.Interlocutor.Message(AgentConsts.Roles.Assistant, chatResponse.Choices[0].Message!.Content));
        }
    }
}

