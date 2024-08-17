using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAIsic.Tests
{
    public static class BAIsicTestConventions
    {
        public static class Agent
        {
            public static string RandomName() => "agent-" + Guid.NewGuid().ToString();
        }
    }
}
