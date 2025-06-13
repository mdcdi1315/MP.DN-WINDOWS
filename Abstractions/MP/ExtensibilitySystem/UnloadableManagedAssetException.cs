

using System;

namespace MP.ExtensibilitySystem
{
    /// <summary>
    /// Thrown when a managed asset cannot be loaded into context. <br />
    /// This is usually caused by .NET, and thus will always contain the exception caused this one.
    /// </summary>
    public sealed class UnloadableManagedAssetException : ExtensibilitySystemException
    {
        /// <summary>
        /// Creates a new <see cref="UnloadableManagedAssetException"/> class instance with the specified asset name that causes this exception and the original exception that caused this exception.
        /// </summary>
        /// <param name="name">The name of the asset throwing this exception</param>
        /// <param name="exc">The exception occured when this asset was loaded.</param>
        public UnloadableManagedAssetException(System.String name, Exception exc)
            : base($"The managed asset with name {name} cannot be loaded by the engine." , exc) { }
    }
}