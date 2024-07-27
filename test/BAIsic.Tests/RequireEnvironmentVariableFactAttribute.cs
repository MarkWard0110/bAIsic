using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAIsic.Tests
{
    public sealed class RequireEnvironmentVariableFactAttribute : IsEnvironmentSupportedFactAttribute
    {
        private readonly string[] _envVariableNames;
        public RequireEnvironmentVariableFactAttribute(params string[] envVariableNames) : base($"{string.Join(", ", envVariableNames)} environment variable(s) not found.")
        {
            _envVariableNames = envVariableNames;
        }

        protected override bool IsEnvironmentSupported()
        {
            return _envVariableNames.All(Environment.GetEnvironmentVariables().Contains);
        }
    }
}
