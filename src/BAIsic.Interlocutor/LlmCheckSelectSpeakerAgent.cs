using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BAIsic.Interlocutor
{
    public class LlmCheckSelectSpeakerAgent : Agent, ICheckSelectSpeakerAgent
    {

        private readonly string _noneTemplate;
        private readonly string _manyTemplate;
        private IList<IConversableAgent>? _agents = null;
        public LlmCheckSelectSpeakerAgent(string name, string noneTemplate, string manyTemplate) : base(name, null)
        {
            _generateReplyHandlers.Add(GenerateReplyHandlerAsync);
            _noneTemplate = noneTemplate;
            _manyTemplate = manyTemplate;
        }

        public IList<IConversableAgent>? Agents { get => _agents; set => _agents = value; }

        public Task<bool> ConversationTerminateHandlerAsync(bool isInitialMessage, IAgent agent, Message message)
        {
            if (message.Text.Contains(GroupConversationConsts.AgentSelected))
            {
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        private async Task<(bool isDone, Message? message)> GenerateReplyHandlerAsync(IEnumerable<Message> messages)
        {
            if (_agents == null)
            {
                throw new NotImplementedException();
            }

            var mentions = MentionedAgents(messages.Last());
            if (mentions.Count == 1)
            {
                var agent = _agents?.FirstOrDefault(a => mentions.ContainsKey(a.Name));
                if (agent != null)
                {
                    var message = new Message(AgentConsts.Roles.Assistant, $"{GroupConversationConsts.AgentSelected}{agent.Name}");
                    return (true, message);
                }
            }
            else if (mentions.Count == 0)
            {
                var agentList = GetAgentList(_agents);
                var message = new Message(AgentConsts.Roles.Assistant, _noneTemplate.Replace("{agentlist}", agentList));
                return (true, message);
            }
            else 
            {
                var agentList = GetAgentList(_agents);
                var message = new Message(AgentConsts.Roles.Assistant, _manyTemplate.Replace("{agentlist}", agentList));
                return (true, message);
            }

            throw new NotImplementedException();
        }

        private string GetAgentList(IList<IConversableAgent> agents)
        {
            return string.Join(",", agents.Select(agent => agent.Name));
        }

        private IDictionary<string, int> MentionedAgents(Message message)
        {

            var mentions = new Dictionary<string, int>();
            if (_agents == null)
            {
                return mentions;
            }
            foreach (var agent in _agents)
            {
                // Create a regex pattern to match the agent name, accommodating underscores and escaping
                var regexPattern = $@"(?<=\W)({Regex.Escape(agent.Name)}|{Regex.Escape(agent.Name.Replace("_", " "))}|{Regex.Escape(agent.Name.Replace("_", @"\_"))})(?=\W)";
                var matches = Regex.Matches($" {message.Text} ", regexPattern); // Pad the message to help with matching

                if (matches.Count > 0)
                {
                    mentions[agent.Name] = matches.Count;
                }
            }

            return mentions;
        }
    }
}
