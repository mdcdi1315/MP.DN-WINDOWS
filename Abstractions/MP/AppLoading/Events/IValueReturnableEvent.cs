namespace MP.AppLoading.Events
{
    /// <summary>
    /// Provides a way for returning back a result from dispatching an event that needs to return a value to the caller.
    /// </summary>
    /// <typeparam name="TV">The type of the value to be returned from the event dispatch once the event is completed.</typeparam>
    public interface IValueReturnableEvent<TV> : IEvent
    {
        /// <summary>
        /// Gets/sets the value to return.
        /// </summary>
        public TV ReturnValue { get; set; }
    }
}
