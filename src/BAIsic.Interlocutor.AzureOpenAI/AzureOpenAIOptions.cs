using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAIsic.Interlocutor.AzureOpenAI
{
    public class AzureOpenAIOptions
    {
        public RequestOptions? RequestOptions { get; set; } = null;
        public string? ResponseFormat { get; set; } = null;
    }
}
