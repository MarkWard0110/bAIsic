using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAIsic.Interlocutor.AmazonBedrock
{
    public class AmazonBedrockOptions
    {
        public RequestOptions? RequestOptions { get; set; } = null;
        public string? ResponseFormat { get; set; } = null;
    }
}
