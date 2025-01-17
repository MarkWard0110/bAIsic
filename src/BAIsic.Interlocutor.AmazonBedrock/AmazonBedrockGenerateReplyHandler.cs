using Amazon.BedrockRuntime;
using Amazon.BedrockRuntime.Model;
using BAIsic.Interlocutor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAIsic.Interlocutor.AmazonBedrock
{
    public class AmazonBedrockGenerateReplyHandler
    {
        private readonly AmazonBedrockRuntimeClient _client;
        private readonly string _model;
        private readonly AmazonBedrockOptions _options;

        public AmazonBedrockGenerateReplyHandler(string model, AmazonBedrockRuntimeClient client, AmazonBedrockOptions? options = null)
        {
            _client = client;
            _model = model;
            _options = options ?? new AmazonBedrockOptions();
        }

        //public AmazonBedrockGenerateReplyHandler(string model, HttpClient httpClient, AmazonBedrockOptions? options = null) : this(model, new AzureOpenAIClient(httpClient), options)
        //{

        //}

        public async Task<(bool isDone, BAIsic.Interlocutor.Message? message)> GenerateReplyHandlerAsync(IEnumerable<BAIsic.Interlocutor.Message> messages)
        {
            var combinedMessages = new List<Amazon.BedrockRuntime.Model.Message>();
            Amazon.BedrockRuntime.Model.Message? lastMessage = null;

            foreach (var message in messages.Where(m => m.Role != AgentConsts.Roles.System))
            {
                if (lastMessage != null && lastMessage.Role == message.Role)
                {
                    lastMessage.Content[0].Text += "\n" + message.Text;
                }
                else
                {
                    lastMessage = new Amazon.BedrockRuntime.Model.Message
                    {
                        Role = message.Role,
                        Content = new List<ContentBlock> { new ContentBlock { Text = message.Text } }
                    };
                    combinedMessages.Add(lastMessage);
                }
            }

            string? systemRole = messages.FirstOrDefault(m => m.Role == AgentConsts.Roles.System)?.Text;
            var inferenceConfig = new InferenceConfiguration();
            if (_options.RequestOptions != null)
            {
                if (_options.RequestOptions.Temperature != null)
                {
                    inferenceConfig.Temperature = _options.RequestOptions.Temperature.Value;
                }

                if (_options.RequestOptions.TopP != null)
                {
                    inferenceConfig.TopP = _options.RequestOptions.TopP.Value;
                }
            }

            var chatRequest = new ConverseRequest()
            {
                ModelId = _model,
                System = string.IsNullOrEmpty(systemRole) ? null : new List<SystemContentBlock> { new SystemContentBlock { Text = systemRole } },
                Messages = combinedMessages,
                InferenceConfig = inferenceConfig
            };

            var chatResponse = await _client.ConverseAsync(chatRequest);

            return (true, new BAIsic.Interlocutor.Message(AgentConsts.Roles.Assistant, chatResponse?.Output?.Message?.Content?[0]?.Text ?? ""));
        }
    }
}

