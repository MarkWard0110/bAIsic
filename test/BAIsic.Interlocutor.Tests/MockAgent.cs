using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAIsic.Interlocutor.Tests
{
    public class MockAgent(string name, string? systemPrompt = null) : Agent(name, systemPrompt)
    {
    }
}
