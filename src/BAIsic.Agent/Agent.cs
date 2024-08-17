﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAIsic.Agent
{
    public abstract class Agent : IAgent
    {
        protected string? _systemPrompt = null;
        protected readonly string _name;
        protected readonly IList<PrepareHandlerAsync> _prepareSendHandlers = [];
        protected readonly IList<PrepareHandlerAsync> _prepareReceiveHandlers = [];
        protected readonly IList<GenerateReplyHandlerAsync> _generateReplyHandlers = [];

        public Agent(string name)
        {
            _name = name;
            _prepareSendHandlers.Add(PrepareSendRoleAsync);
            _prepareReceiveHandlers.Add(PrepareReceiveRoleAsync);
        }

        public string Name => _name;
        public string? SystemPrompt => _systemPrompt;

        public IList<PrepareHandlerAsync> PrepareSendHandlers => _prepareSendHandlers;
        public IList<PrepareHandlerAsync> PrepareReceiveHandlers => _prepareReceiveHandlers;
        public IList<GenerateReplyHandlerAsync> GenerateReplyHandlers => _generateReplyHandlers;

        public async Task<Message?> GenerateReplyAsync(IEnumerable<Message> messages)
        {
            foreach (var handler in _generateReplyHandlers) 
            {
                var (isDone, message) = await handler(messages);
                if (isDone)
                {
                    return message;
                }
            }

            return null;
        }

        public async Task<Message> PrepareReceiveMessageAsync(Message message)
        {
            foreach (var handler in _prepareReceiveHandlers)
            {
                message = await handler(message);
            }

            return message;
        }

        public async Task<Message> PrepareSendMessageAsync(Message message)
        {
            foreach (var handler in _prepareSendHandlers)
            {
                message = await handler(message);
            }

            return message;
        }

        private Task<Message> PrepareSendRoleAsync(Message message)
        {
            if (!string.Equals(message.Role, AgentConsts.Roles.Assistant, StringComparison.Ordinal))
            {
                return Task.FromResult(message with { Role = AgentConsts.Roles.Assistant });
            }

            return Task.FromResult(message);
        }

        private Task<Message> PrepareReceiveRoleAsync(Message message)
        {
            if (!string.Equals(message.Role, AgentConsts.Roles.User, StringComparison.Ordinal))
            {
                return Task.FromResult(message with { Role = AgentConsts.Roles.User });
            }

            return Task.FromResult(message);
        }
    }
}
