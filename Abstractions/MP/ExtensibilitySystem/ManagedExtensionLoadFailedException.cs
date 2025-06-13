
using System;

namespace MP.ExtensibilitySystem
{
    /// <summary>
    /// Thrown when the <see cref="ExtensibilitySystemExtension.OnLoad"/> or <see cref="ExtensibilitySystemExtension.OnUnload"/> methods threw back an exception.
    /// </summary>
    public sealed class ManagedExtensionLoadFailedException : ExtensibilitySystemException
    {
        /// <summary>
        /// Creates a new instance of the <see cref="ManagedExtensionLoadFailedException"/> class from the exception that was thrown by the aforementioned methods.
        /// </summary>
        /// <param name="basex">The <see cref="Exception"/> that was thrown.</param>
        public ManagedExtensionLoadFailedException(Exception basex)
            : base("Load code from an extension threw back an exception.", basex) { }
    }
}