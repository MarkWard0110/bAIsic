﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAIsic.Interlocutor
{
    public delegate Task<bool> ConversationTerminateHandlerAsync(IAgent agent, Message message);
}
