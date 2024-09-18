using BAIsic.LlmApi.Ollama;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAIsic.Interlocutor.Ollama
{
    public class OllamaOptions
    {
        public RequestOptions? RequestOptions { get; set; } = null;
        public string? ResponseFormat { get; set; } = null;
        public int KeepAlive { get; set; } = -1;
    }
}
