

using System;

namespace MP.Annotations.CodeAnalysis
{
    /// <summary>
    /// Specified to any executable code (methods and constructors) and specifies the exception types
    /// that are guaranteed to be thrown on known code path failures. <br />
    /// Additional exceptions may be thrown, but this is used to know the most common ones at reflection. <br />
    /// It can be also specified without any types to indicate that the code throws any kind of exceptions. <br />
    /// If you want to specify this attribute on a property, specify it instead to it's accessor method. <br />
    /// </summary>
    [AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class ThrowsAttribute : Attribute
    {
        private readonly Type[] exceptions;

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