
using System;
using System.Diagnostics.CodeAnalysis;

namespace MP.AppLoading.Events
{
    /// <summary>
    /// Provides the event dispatcher, a service for global, per-component dispatched events. <br />
    /// Because it can be a service created from a component, this interface implements the <see cref="IService"/> interface.
    /// </summary>
    public interface IEventDispatcher : IService
    {
        private sealed class WrappedEventListener<T> : IEventListener<T>
            where T : IEvent
        {
            private readonly Action<T> action;

            public WrappedEventListener(Action<T> act) => action = act;

            public void Consume([DisallowNull] T evt) => action.Invoke(evt);
        }

        /// <summary>
        /// Adds a listener for the specified event.
        /// </summary>
        /// <typeparam name="T">The exact type of the event to be invoked.</typeparam>
        /// <param name="listener">The listener to register.</param>
        /// <exception cref="ArgumentNullException"><paramref name="listener"/> is <see langword="null"/>.</exception>
        public void AddListener<T>(Action<T> listener) where T : IEvent
        {
            ArgumentNullException.ThrowIfNull(listener);
            AddListener(new WrappedEventListener<T>(listener));
        }

        /// <summary>
        /// Adds a listener for the specified event.
        /// </summary>
        /// <typeparam name="T">The exact type of the event to be invoked.</typeparam>
        /// <param name="listener">The listener to register.</param>
        /// <exception cref="ArgumentNullException"><paramref name="listener"/> is <see langword="null"/>.</exception>
        public void AddListener<T>(IEventListener<T> listener) where T : IEvent;

        /// <summary>
        /// Dispatches the specified event to all the registered listeners for the event.
        /// </summary>
        /// <param name="event_data">The actual event instance that dispatches the event.</param>
        /// <returns>An event execution result providing information on how the event is executing.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="event_data"/> is <see langword="null"/>.</exception>
        public EventExecutionResult DispatchEvent(IEvent event_data);
    }
}