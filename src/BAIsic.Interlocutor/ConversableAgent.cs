using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAIsic.Interlocutor
{
    public class ConversableAgent : Agent, IConversableAgent
    {
        private readonly string _description;
        public ConversableAgent(string name, string description = "", string? systemPrompt = null) : base(name, systemPrompt)
        {
            _description = description;
        }

        public string Description => _description;
    }
}
