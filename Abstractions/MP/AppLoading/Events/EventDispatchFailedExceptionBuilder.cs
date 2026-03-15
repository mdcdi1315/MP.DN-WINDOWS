
using System;
using MP.ExceptionSystem;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace MP.AppLoading.Events
{
    /// <summary>
    /// Provides a <see cref="ExceptionBuilder"/> specialization for the <see cref="EventDispatchFailedException"/> class.
    /// </summary>
    public sealed class EventDispatchFailedExceptionBuilder : ExceptionBuilder
    {
        private sealed class ExceptionDispatchCustom : EventDispatchFailedException
        {
            private readonly Exception[] exceptions;

            public ExceptionDispatchCustom(IEvent eventimplementation, string msg, Exception[] exceptions) : base(eventimplementation, msg) => this.exceptions = exceptions;

            public override IEnumerable<Exception> Exceptions => exceptions;
        }

        private IEvent event_executing;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventDispatchFailedExceptionBuilder"/> class.
        /// </summary>
        public EventDispatchFailedExceptionBuilder() => event_executing = null;

        /// <summary>Gets or sets the event that is executing.</summary>
        /// <exception cref="ArgumentNullException">Value cannot be <see langword="null"/> while setting.</exception>
        [MaybeNull]
        public IEvent ExecutingEvent
        {
            get => event_executing;
            set {
                ArgumentNullException.ThrowIfNull(value);
                event_executing = value;
            }
        }

        /// <inheritdoc />
        protected override void ThrowException([AllowNull] string message, [DisallowNull] Exception[] exceptions) => throw new ExceptionDispatchCustom(event_executing, message, exceptions);
    }
}