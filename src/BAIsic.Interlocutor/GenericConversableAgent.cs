using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAIsic.Interlocutor
{
    public class GenericConversableAgent : IConversableAgent
    {
        private readonly string _description;
        private readonly IAgent _agent;

        public GenericConversableAgent(IAgent agent, string description)
        {
            _description = description;
            _agent = agent;
        }
        public string Description => _description;

        public string Name => _agent.Name;

        public string? SystemPrompt { get => _agent.SystemPrompt; set => _agent.SystemPrompt = value; }

        public IList<PrepareHandlerAsync> PrepareSendHandlers => _agent.PrepareSendHandlers;

        public IList<PrepareHandlerAsync> PrepareReceiveHandlers => _agent.PrepareReceiveHandlers;

        public IList<GenerateReplyHandlerAsync> GenerateReplyHandlers => _agent.GenerateReplyHandlers;

        public Task<Message?> GenerateReplyAsync(IEnumerable<Message> messages)
        {
            return _agent.GenerateReplyAsync(messages);
        }

        public Task<Message?> PrepareReceiveMessageAsync(Message message)
        {
            return _agent.PrepareReceiveMessageAsync(message);
        }

        public Task<Message?> PrepareSendMessageAsync(Message message)
        {
            return _agent.PrepareSendMessageAsync(message);
        }
    }
}
