using BAIsic.Tests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BAIsic.LlmApi.Ollama.Tests
{
    public class RequireModelDataAttribute(string model) : IsEnvironmentSupportedDataAttribute($"Model {model} is not found.")
    {
        private readonly string _model = model;

        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            return [[_model]];
        }

        protected override bool IsEnvironmentSupported()
        {
            if (!Environment.GetEnvironmentVariables().Contains(OllamaTestConsts.EnvironmentVariable.OllamaHost))
            {
                return false;
            }

            var ollamaClient = OllamaClientExtensions.CreateOllamaClient();
            var models = ollamaClient.ListLocalModelsAsync().Result;

            return models.Any(m => string.Compare(m.Name, _model, StringComparison.OrdinalIgnoreCase) == 0);
        }
    }
}
