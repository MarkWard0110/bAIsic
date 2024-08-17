using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAIsic.Agent
{
    public record Message(string Role, string Text, string[]? Images = null);

}
