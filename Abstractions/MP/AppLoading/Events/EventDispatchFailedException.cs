
using System;
using MP.Collections;
using MP.ExceptionSystem;
using System.Collections.Generic;
using MP.Annotations.CodeAnalysis;
using System.Diagnostics.CodeAnalysis;

namespace MP.AppLoading.Events
{
    /// <summary>
    /// Provides a base class that can be thrown when dispatching of a specific event has been failed. <br />
    /// Typically used in the app's component loader implementations.
    /// </summary>
    public class EventDispatchFailedException : BaseException
    {
        private readonly IEvent event_impl;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventDispatchFailedException"/> class with the specified <see cref="IEvent"/> instance that dispatch was failed for it.
        /// </summary>
        /// <param name="eventimplementation">The event implementation that was dispatched and caused this exception object to be created.</param>
        [MustNotReportException]
        public EventDispatchFailedException(IEvent eventimplementation) : base() => event_impl = eventimplementation;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventDispatchFailedException"/> class with the specified <see cref="IEvent"/> instance that dispatch was failed for it,
        /// along with the specified error message that explains the reason why dispatch was failed for the specified event.
        /// </summary>
        /// <param name="message">The error message that explains the reason why dispatch was failed for the specified event.</param>
        /// <param name="eventimplementation">The event implementation that was dispatched and caused this exception object to be created.</param>
        [MustNotReportException]
        public EventDispatchFailedException(IEvent eventimplementation, [AllowNull] string message) : base(message) => event_impl = eventimplementation;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventDispatchFailedException"/> class with the specified <see cref="IEvent"/> instance that dispatch was failed for it,
        /// along with the specified error message that explains the reason why dispatch was failed for the specified event. <br />
        /// Additionally, an optional exception object can be provided that can be the cause of this exception object to be created.
        /// </summary>
        /// <param name="inner">The optional exception object that can be the cause of the current exception object to be created.</param>
        /// <param name="message">The error message that explains the reason why dispatch was failed for the specified event.</param>
        /// <param name="eventimplementation">The event implementation that was dispatched and caused this exception object to be created.</param>
        [MustNotReportException]
        public EventDispatchFailedException(IEvent eventimplementation, [AllowNull] string message, [AllowNull] Exception inner) : base(message, inner) => event_impl = eventimplementation;

        /// <summary>
        /// Gets an enumerable that provides the exceptions, if any, that were thrown during event dispatch.
        /// </summary>
        public virtual IEnumerable<Exception> Exceptions { 
            [return: MaybeReturnEmptyCollectionButNeverNull]
            get => new EmptyEnumerable<Exception>(); 
        }

        /// <summary>
        /// Gets the <see cref="IEvent"/> object that was dispatched and it's dispatch caused this exception to be thrown.
        /// </summary>
        public IEvent DispatchedEvent => event_impl;
    }
}