using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAIsic.Interlocutor
{
    public class LlmSelectSpeakerAgent : Agent, ISelectSpeakerAgent
    {
        private readonly ICheckSelectSpeakerAgent _checkSelectSpeakerAgent;
        private readonly LlmSelectSpeakerAgentConfig _config;

        public LlmSelectSpeakerAgent(string name, LlmSelectSpeakerAgentConfig config) : base(name, null)
        {
            _config = config;
            _checkSelectSpeakerAgent = new LlmCheckSelectSpeakerAgent("check_name", _config.NoneSelectedPrompt);
        }

        public ICheckSelectSpeakerAgent CheckSelectSpeakerAgent => _checkSelectSpeakerAgent;

        public Message InitialChatMessage(IConversableAgent speaker, Message message, IList<IConversableAgent> agents, IDictionary<string, List<string>> allowedTransitions)
        {
            var agentTransitions = GetTransitions(agents, allowedTransitions[speaker.Name]);
            var agentList = GetAgentList(agentTransitions);
            return new Message(AgentConsts.Roles.User, GetSelectPromptMessage(agentList));
        }

        public IEnumerable<Message> InitialMessages(IConversableAgent speaker, Message message, IList<IConversableAgent> agents, IDictionary<string, List<string>> allowedTransitions)
        {
            var agentTransitions = GetTransitions(agents, allowedTransitions[speaker.Name]);
            var roles = GetRoles(agentTransitions);
            var agentList = GetAgentList(agentTransitions);
            var systemMessage = AgentConventions.SystemMessage(GetSystemMessage(roles, agentList));

            var chatMessage = message.Role == AgentConsts.Roles.User ? message: message with { Role = AgentConsts.Roles.User };
            _checkSelectSpeakerAgent.Agents = agentTransitions;
            return [systemMessage, chatMessage];
        }

        private string GetSystemMessage(string roles, string agentList)
        {
            var systemStringBuilder = new StringBuilder(_config.SelectSpeakerMessageTemplate);
            systemStringBuilder.Replace("{roles}", roles);
            systemStringBuilder.Replace("{agentlist}", agentList);
            return systemStringBuilder.ToString();
        }

        private string GetSelectPromptMessage(string agentList)
        {
            return _config.SelectSpeakerPrompt.Replace("{agentlist}", agentList);
        }

        private IList<IConversableAgent> GetTransitions(IList<IConversableAgent> agents, List<string> transitions)
        {
            var result = new List<IConversableAgent>();
            foreach ( var transition in transitions) {
                result.Add(agents.First(a => a.Name == transition));
            }
            return result;
        }

        private string GetRoles(IList<IConversableAgent> agents)
        {
            var result = new StringBuilder();
            foreach (var agent in agents)
            {
                result.AppendLine($"{agent.Name}: {agent.Description}");
            }
            return result.ToString();
        }

        private string GetAgentList(IList<IConversableAgent> agents)
        {
            return string.Join(",", agents.Select(agent => agent.Name));
        }
    }
}
