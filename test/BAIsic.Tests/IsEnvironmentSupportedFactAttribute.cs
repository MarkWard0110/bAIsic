﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAIsic.Tests
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public abstract class IsEnvironmentSupportedFactAttribute(string skipMessage) : FactAttribute
    {
        private readonly string _skipMessage = skipMessage ?? throw new ArgumentNullException(nameof(skipMessage));

        public sealed override string Skip => IsEnvironmentSupported() ? string.Empty : _skipMessage;

        protected abstract bool IsEnvironmentSupported();
    }
}
