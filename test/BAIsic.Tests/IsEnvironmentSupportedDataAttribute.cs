using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Sdk;

namespace BAIsic.Tests
{
    public abstract class IsEnvironmentSupportedDataAttribute(string skipMessage) : DataAttribute
    {
        private readonly string _skipMessage = skipMessage ?? throw new ArgumentNullException(nameof(skipMessage));

        public sealed override string Skip => IsEnvironmentSupported() ? string.Empty : _skipMessage;

        protected abstract bool IsEnvironmentSupported();
    }
}
