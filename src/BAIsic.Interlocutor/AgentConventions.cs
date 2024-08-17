using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAIsic.Interlocutor
{
    public static class AgentConventions
    {
        public static Message SystemMessage(string systemMessage) => new(Role: AgentConsts.Roles.System, Text: systemMessage);

    }
}
