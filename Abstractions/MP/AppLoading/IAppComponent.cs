
using System;
using MP.Resources;
using MP.AppLoading.Events;
using System.Diagnostics.CodeAnalysis;

namespace MP.AppLoading
{
    /// <summary>
    /// A declaration to an app's component. <br />
    /// Additional attributes about the component are described through the implementation of the <see cref="IAttributeable"/> interface. <br />
    /// The <see cref="IDisposable.Dispose"/> method that it must be implemented must destroy all the data associated with the component.
    /// </summary>
    public interface IAppComponent : IAttributeable, IDisposable
    {
        /// <summary>
        /// Gets the ID of this app component.
        /// </summary>
        public string ID { get; }

        /// <summary>
        /// Gets the version of this app component.
        /// </summary>
        public Version Version { get; }

        /// <summary>
        /// Gets the event dispatcher of this app component. <br />
        /// If <see langword="null"/>, it means that the component does not use events.
        /// </summary>
        [MaybeNull]
        public IEventDispatcher Dispatcher { get; }

        /// <summary>
        /// Gets an <see cref="IResourceReader"/> object for providing additional files relative to the location of the component library.
        /// </summary>
        public IResourceReader AdditionalFiles { get; }
    }
}