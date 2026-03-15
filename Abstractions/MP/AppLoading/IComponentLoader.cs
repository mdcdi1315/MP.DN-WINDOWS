
using System;
using MP.AppLoading.Events;
using System.Collections.Generic;
using MP.Annotations.CodeAnalysis;
using System.Diagnostics.CodeAnalysis;

namespace MP.AppLoading
{
    /// <summary>
    /// Defines the public interface for the Music Player component loader. <br />
    /// Typically instances of this are the app's bootstrappers and they do manage the entire launching process.
    /// </summary>
    public interface IComponentLoader : IEnumerable<AppComponentContainer>, IDisposable
    {
        /// <summary>
        /// Adds a component to the loader to be processed and loaded.
        /// </summary>
        /// <param name="component">The app component to be loaded.</param>
        /// <exception cref="ArgumentNullException"><paramref name="component"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvalidOperationException">No more components can be registered after <see cref="Load"/> has been called.</exception>
        [Throws(typeof(ArgumentNullException), typeof(InvalidOperationException))]
        public void AddComponentContainer(AppComponentContainer component);

        /// <summary>
        /// Gets the program arguments once the <see cref="Load"/> method executes.
        /// </summary>
        public System.String[] ProgramArguments {
            [return: MaybeReturnEmptyCollectionButNeverNull]
            get; 
        }

        /// <summary>
        /// Begins the loading process as mandated by the loader. <br />
        /// All the components registered through the <see cref="AddComponentContainer"/> method will be now loaded.
        /// </summary>
        /// <param name="arguments">The process arguments. Can be <see langword="null"/>.</param>
        public void Load([AllowNull] System.String[] arguments);

        /// <summary>
        /// Returns the version of the in-use component loader. <br />
        /// This version will be used by the components to realize the environment.
        /// </summary>
        public Version Version { [return: NotNull] get; }

        /// <summary>
        /// Returns the version of the main application. Also useful for the components.
        /// </summary>
        public Version AppVersion { [return: NotNull] get; }

        /// <summary>
        /// Dispatches the specified event instance to all the components that provide a valid instance of the <see cref="IEventDispatcher"/> interface.
        /// </summary>
        /// <param name="evt">The <see cref="IEvent"/> instance to dispatch.</param>
        /// <exception cref="ArgumentNullException"><paramref name="evt"/> is <see langword="null"/>.</exception>
        /// <exception cref="NotSupportedException">The current component loader does not support dispatching events through this way. This is used to enforce specific ways to dispatch events to components, if needed so.</exception>
        [Throws(typeof(NotSupportedException), typeof(ArgumentNullException), typeof(EventDispatchFailedException))]
        public virtual void DispatchEvent(IEvent evt)
        {
            ArgumentNullException.ThrowIfNull(evt);
            EventDispatchFailedExceptionBuilder builder = new();
            builder.ExecutingEvent = evt;
            EventExecutionResult result;
            foreach (AppComponentContainer container in this)
            {
                result = container.Component.Dispatcher?.DispatchEvent(evt);
                if (result is not null && result.Status == EventExecutionStatus.Exception)
                {
                    builder.Add(result.Exception);
                }
            }
            builder.ThrowIfHasExceptions();
        }
    }
}