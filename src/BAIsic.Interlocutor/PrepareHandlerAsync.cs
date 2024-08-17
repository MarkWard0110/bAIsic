using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAIsic.Interlocutor
{
    public delegate Task<Message?> PrepareHandlerAsync(Message? message);
}
