using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAIsic.Interlocutor
{
    public class SelectSpeakerAgent : Agent, ISelectSpeakerAgent
    {
        const string SelectSpeakerMessageTemplate = @"You are in a role play game. The following roles are available:
{roles}.
Read the following conversation.
Then select the next role from {agentlist} to play. Only return the role.";
        const string SelectSpeakerPrompt = "Read the above conversation. Then select the next role from {agentlist} to play. Only return the role.";
        private const string NoneSelectedPrompt =
@"You didn't choose a speaker. As a reminder, to determine the speaker use these prioritised rules:
1. If the context refers to themselves as a speaker e.g. ""As the..."" , choose that speaker's name
2. If it refers to the ""next"" speaker name, choose that name
3. Otherwise, choose the first provided speaker's name in the context
The names are case-sensitive and should not be abbreviated or changed.
The only names that are accepted are {agentlist}.
Respond with ONLY the name of the speaker and DO NOT provide a reason.";

        private readonly ICheckSelectSpeakerAgent _checkSelectSpeakerAgent;

        public SelectSpeakerAgent(string name) : base(name, null)
        {
            _checkSelectSpeakerAgent = new CheckSelectSpeakerAgent("check_name", NoneSelectedPrompt);
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
            var systemStringBuilder = new StringBuilder(SelectSpeakerMessageTemplate);
            systemStringBuilder.Replace("{roles}", roles);
            systemStringBuilder.Replace("{agentlist}", agentList);
            return systemStringBuilder.ToString();
        }

        private string GetSelectPromptMessage(string agentList)
        {
            return SelectSpeakerPrompt.Replace("{agentlist}", agentList);
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
