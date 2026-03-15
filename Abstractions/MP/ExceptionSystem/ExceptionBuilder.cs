
using System;
using MP.Collections;
using System.Diagnostics;
using MP.Annotations.CodeAnalysis;
using System.Diagnostics.CodeAnalysis;

namespace MP.ExceptionSystem
{
    /// <summary>
    /// Defines an abstract class for collecting a handful of exceptions, and then be thrown in a later time by packing them into a single exception object. <br />
    /// Be noted, however, that this class is NOT THREAD SAFE; you need to implement thread safety if you need this in cross-thread scenarios.
    /// </summary>
    public abstract class ExceptionBuilder
    {
        private long count;
        private string message;
        private Exception[] exceptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionBuilder"/> class.
        /// </summary>
        public ExceptionBuilder()
        {
            count = 0;
            message = null;
            exceptions = new Exception[10];
        }

        private void Grow(int by)
        {
            long new_cap = unchecked(count + by);
            if (new_cap < 0L) {
                throw new OverflowException("The exception builder has reached it's maximum capacity.");
            } else if (new_cap > exceptions.LongLength) {
                ArrayHelpers.Resize(ref exceptions, new_cap);
            }
        }

        /// <summary>Adds an exception to the builder.</summary>
        /// <param name="ex">The exception to add.</param>
        /// <exception cref="ArgumentNullException"><paramref name="ex"/> is <see langword="null"/>.</exception>
        [Throws(typeof(ArgumentNullException))]
        public void Add(Exception ex)
        {
            ArgumentNullException.ThrowIfNull(ex);
            Grow(1);
            exceptions[count++] = ex;
        }

        /// <summary>Adds the specified exceptions to the builder.</summary>
        /// <param name="exceptions">The exception(s) to add.</param>
        /// <exception cref="ArgumentNullException"><paramref name="exceptions"/> is <see langword="null"/>.</exception>
        [Throws(typeof(ArgumentNullException))]
        public void AddRange(params Exception[] exceptions)
        {
            ArgumentNullException.ThrowIfNull(exceptions);
            long ll = exceptions.LongLength;
            Grow(exceptions.Length);
            Array.Copy(exceptions, 0L , this.exceptions, count, ll);
            count += ll;
        }

        /// <summary>
        /// Gets/sets the exception message that will be passed to the exception 
        /// object when it will be thrown. <br />
        /// Can be <see langword="null"/>.
        /// </summary>
        public string Message
        {
            [MustNotReportException]
            get => message;
            [MustNotReportException]
            set => message = value;
        }

        /// <summary>
        /// Gets a value whether this exception builder has at least one exception registered.
        /// </summary>
        public bool HasExceptions => count > 0;

        /// <summary>
        /// If this exception builder has exceptions, throws an exception that the builder is associated with.
        /// </summary>
        [StackTraceHidden] // Must not be visible in the stack trace of the exception.
        public void ThrowIfHasExceptions()
        {
            if (count > 0) {
                Exception[] ex_t = new Exception[count];
                Array.Copy(exceptions, ex_t , count);
                ThrowException(message, ex_t);
            }
        }

        /// <summary>
        /// Throws the exception provided by this exception builder, with the specified message and the packed exceptions to additionally register to the target exception object.
        /// </summary>
        /// <param name="message">The message that describes the reason of throwing the exception. Can be <see langword="null"/>.</param>
        /// <param name="exceptions">The exceptions to additionally register to the target exception object. This can never be <see langword="null"/>, but it can be the empty array.</param>
        [DoesNotReturn]
        [StackTraceHidden] // Must not be visible in the stack trace of the exception.
        protected abstract void ThrowException([AllowNull] string message, [DisallowNull] Exception[] exceptions);
    }
}