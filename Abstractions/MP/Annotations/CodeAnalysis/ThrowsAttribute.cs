

using System;

namespace MP.Annotations.CodeAnalysis
{
    /// <summary>
    /// Specified to any executable code (methods, properties, constructors and events) and specifies the exception types
    /// that are guaranteed to be thrown on known code path failures. <br />
    /// Additional exceptions may be thrown, but this is used to know the most common ones at reflection.
    /// </summary>
    [AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Event, AllowMultiple = false, Inherited = false)]
    public sealed class ThrowsAttribute : Attribute
    {
        private Type[] exceptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="ThrowsAttribute"/> class with the specified types that are exceptions that might be thrown by the attributed code.
        /// </summary>
        /// <param name="types">The exception types known to be thrown by the specified executable code.</param>
        public ThrowsAttribute(params Type[] types) => exceptions = types;

        /// <summary>
        /// Gets an array of types that do specify the exceptions that are known and occurable in the specified executable code.
        /// </summary>
        public Type[] Exceptions => exceptions;
    }
}