using Splat;
using System;
using System.Runtime.Serialization;

namespace Cirilla.MVVM.Extensions
{
    public class RequiredServiceMissingException : Exception
    {
        public RequiredServiceMissingException()
        {
        }

        public RequiredServiceMissingException(string message) : base(message)
        {
        }

        public RequiredServiceMissingException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected RequiredServiceMissingException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

    public static class SplatExtensions
    {
        public static T GetRequiredService<T>(this IReadonlyDependencyResolver resolver, string? contract = null)
        {
            return resolver.GetService<T>(contract) ?? throw new RequiredServiceMissingException($"Service '{typeof(T).FullName}' is not registered, but it is required.");
        }
    }
}
