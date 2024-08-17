using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAIsic.Interlocutor
{

    /*
     * A reason the function returns a tuple with isDone is when the generate reply handler wants to end the conversation.
     * The isDone flag indicates the handler intended to return a null reply.
     */

    /// <summary>
    /// Process a list of messages and generate a reply.
    /// isDone is true when the handler wants to end the generate reply process.  Any following generate reply handlers will not be called.
    /// If isDone is true and the message is null, the conversation will end. (allowing an agent to end the conversation without a message)
    /// 
    /// If the generate reply returns a message that is intended to end the conversation, the isDone flag should be set to true.  
    /// The conversation will check for end conversation messages and end the conversation if one is found.
    /// </summary>
    /// <param name="messages"></param>
    /// <returns></returns>
    public delegate Task<(bool isDone, Message? message)> GenerateReplyHandlerAsync(IEnumerable<Message> messages);

}
