using System;
using Cirilla.Core.Logging;

namespace Cirilla.Core.Test
{
    class LogProvider : ILogProvider
    {
        public Logger GetLogger(string name)
        {
            return (logLevel, messageFunc, exception, formatParameters) =>
            {
                if (messageFunc == null) return true;

                Console.WriteLine("{0} | {1} | {2} | {3}", DateTime.UtcNow, logLevel, name, messageFunc());
                return true;
            };
        }

        public IDisposable OpenMappedContext(string key, object value, bool destructure = false)
        {
            throw new NotImplementedException();
        }

        public IDisposable OpenNestedContext(string message)
        {
            throw new NotImplementedException();
        }
    }
}
