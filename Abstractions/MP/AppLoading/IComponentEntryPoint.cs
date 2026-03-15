

using System;
using System.Diagnostics.CodeAnalysis;

namespace MP.AppLoading
{
    /// <summary>
    /// Implemented by the components themselves to start initializing. <br />
    /// The <see cref="IDisposable.Dispose"/> method that you need to also implement is for to run finalization tasks before the app is closed.
    /// </summary>
    public interface IComponentEntryPoint : IDisposable
    {
        /// <summary>
        /// Provides the signature for the component's initializer.
        /// </summary>
        /// <param name="component">A reference to the instance of THIS component.</param>
        public void Run([DisallowNull] IAppComponent component);
    }
}