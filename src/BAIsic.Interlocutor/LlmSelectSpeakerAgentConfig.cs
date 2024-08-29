using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAIsic.Interlocutor
{
    public class LlmSelectSpeakerAgentConfig
    {
        public string SelectSpeakerSystemMessageTemplate { get; set; } = @"You are in a role play game. The following roles are available:
{roles}.
Read the following conversation.
Then select the next role from {agentlist} to play. Only return the role.";

        public string SelectSpeakerPrompt { get; set; } = "Read the above conversation. Then select the next role from {agentlist} to play. Only return the role.";
        public string NoneSelectedPrompt { get; set; } =
@"You didn't choose a speaker. As a reminder, to determine the speaker use these prioritised rules:
1. If the context refers to themselves as a speaker e.g. ""As the..."" , choose that speaker's name
2. If it refers to the ""next"" speaker name, choose that name
3. Otherwise, choose the first provided speaker's name in the context
The names are case-sensitive and should not be abbreviated or changed.
The only names that are accepted are {agentlist}.
Respond with ONLY the name of the speaker and DO NOT provide a reason.";

        public string ManySelectedPrompt { get; set; } = string.Empty;
    }
}
