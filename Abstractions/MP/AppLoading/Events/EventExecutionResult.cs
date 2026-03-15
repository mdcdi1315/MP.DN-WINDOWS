
using System;
using System.Diagnostics.CodeAnalysis;

namespace MP.AppLoading.Events
{
    /// <summary>
    /// Provides the result data regarding an event dispatch in a <see cref="IEventDispatcher"/> instance.
    /// </summary>
    public abstract class EventExecutionResult
    {
        /// <summary>
        /// Gets the execution status of the current event execution result.
        /// </summary>
        public abstract EventExecutionStatus Status { get; }

        /// <summary>
        /// If <see cref="Status"/> is <see cref="EventExecutionStatus.Exception"/>, gets the exception occurred during event dispatch.
        /// </summary>
        public abstract Exception Exception { get; }

        /// <summary>
        /// Gets the return value of the event, if the event implements the <see cref="IValueReturnableEvent{TV}"/> interface.
        /// </summary>
        [MaybeNull]
        public virtual Object ReturnValue { get => null; }
    }

    /// <summary>
    /// Gets the status of an execution of an event.
    /// </summary>
    public enum EventExecutionStatus : System.Byte
    {
        /// <summary>
        /// The event is still dispatching and is not completed.
        /// </summary>
        Running,
        /// <summary>
        /// The event was dispatched successfully.
        /// </summary>
        Completed,
        /// <summary>
        /// The event could not be completed because it was interrupted by an occurring exception.
        /// </summary>
        Exception
    }
}