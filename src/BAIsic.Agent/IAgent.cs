using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAIsic.Agent
{
    public interface IAgent
    {
        public string Name { get; }
        public string? SystemPrompt { get; }

        public Task<Message> PrepareSendMessageAsync(Message message);
        public IList<PrepareHandlerAsync> PrepareSendHandlers { get; }
        
        public Task<Message> PrepareReceiveMessageAsync(Message message);
        public IList<PrepareHandlerAsync> PrepareReceiveHandlers { get; }

        public IList<GenerateReplyHandlerAsync> GenerateReplyHandlers { get; }
        public Task<Message?> GenerateReplyAsync(IEnumerable<Message> messages);
    }
}
