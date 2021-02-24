using System;

namespace Cirilla.Core.Exceptions
{
    public class VanillaSaveGameException : Exception
    {
        public VanillaSaveGameException(string message) : base(message)
        {
        }
    }
}
