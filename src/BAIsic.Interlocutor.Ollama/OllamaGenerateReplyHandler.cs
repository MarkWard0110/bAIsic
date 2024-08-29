using BAIsic.Interlocutor;
using BAIsic.LlmApi.Ollama;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAIsic.Interlocutor.Ollama
{
    public class OllamaGenerateReplyHandler
    {
        private readonly OllamaClient _ollamaClient;
        private readonly string _model;
        private readonly RequestOptions? _requestOptions;

        public OllamaGenerateReplyHandler(string model, OllamaClient ollamaClient, RequestOptions? requestOptions = null)
        {
            _ollamaClient = ollamaClient;
            _model = model;
            _requestOptions = requestOptions;
        }

        public OllamaGenerateReplyHandler(string model, HttpClient httpClient, RequestOptions? requestOptions = null) : this(model, new OllamaClient(httpClient), requestOptions)
        {

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
                Stream = false,
                Options = _requestOptions,
                KeepAlive = -1,
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

