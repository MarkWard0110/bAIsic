using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAIsic.Interlocutor
{
    public record ConversationHistory(IAgent Agent, ImmutableList<Message> Messages);
}
