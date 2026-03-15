

using System.Diagnostics.CodeAnalysis;

namespace MP.AppLoading.Events
{
    /// <summary>
    /// Provides a way for listening to an event dispatched via the <see cref="IEventDispatcher"/> interface.
    /// </summary>
    /// <typeparam name="T">The type of the event to listen on.</typeparam>
    public interface IEventListener<T>
        where T : IEvent
    {
        /// <summary>Consumes an event dispatch. This is called only by the <see cref="IEventDispatcher"/>-implementing class.</summary>
        /// <param name="evt">The event data. Must not be <see langword="null"/>.</param>
        public void Consume([DisallowNull] T evt);
    }
}