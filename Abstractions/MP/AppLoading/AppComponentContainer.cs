
using System;
using MP.ExceptionSystem;
using System.Diagnostics.CodeAnalysis;

namespace MP.AppLoading
{
    /// <summary>
    /// Defines a container for an app component. <br />
    /// This container is subsequently used in initializing the app component.
    /// </summary>
    public sealed class AppComponentContainer : IDisposable
    {
        private readonly IAppComponent component;
        private readonly IComponentEntryPoint entry_point;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppComponentContainer"/> class.
        /// </summary>
        /// <param name="component">The component loaded from metadata.</param>
        /// <param name="entry_point">The component entry point.</param>
        public AppComponentContainer(IAppComponent component, IComponentEntryPoint entry_point)
        {
            ArgumentNullException.ThrowIfNull(component);
            ArgumentNullException.ThrowIfNull(entry_point);
            this.component = component;
            this.entry_point = entry_point;
        }

        /// <summary>
        /// Gets the component associated with this container.
        /// </summary>
        [NotNull]
        public IAppComponent Component => component;

        /// <summary>
        /// Gets the entry point that will be invoked by the <see cref="IComponentEntryPoint"/> interface.
        /// </summary>
        [NotNull]
        public IComponentEntryPoint EntryPoint => entry_point;

        /// <summary>
        /// Disposes this component container.
        /// </summary>
        public void Dispose()
        {
            AggregateExceptionBuilder builder = new();
            try {
                entry_point.Dispose();
            } catch (Exception e) {
                builder.Add(e);
            }
            try {
                component.Dispose();
            } catch (Exception e) {
                builder.Add(e);
            }
            builder.ThrowIfHasExceptions();
        }
    }
}