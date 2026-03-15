using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using MP.Annotations.CodeAnalysis;

namespace MP.ExceptionSystem
{
    /// <summary>
    /// Defines common extensions for all the Music Player exceptions.
    /// </summary>
    public static class ExceptionExtensions
    {
        private sealed class MusicPlayerExceptionWrappedException : Exception
        {
            private readonly BaseException origin;

            public MusicPlayerExceptionWrappedException(BaseException origin)
            {
                if (origin is null) { throw new System.ArgumentNullException(nameof(origin)); }
                this.origin = origin;
            }

            public override string Source
            {
                get {
                    string s = origin.Source;
                    if (s is null) {
                        return origin.GetType().FullName;
                    } else {
                        return s;
                    }
                }
            }

            public override IDictionary Data => origin.Data;

            public override string HelpLink => origin.HelpLink;

            public override string Message => $"Caught a Wrapped Music Player Exception.\nType: {origin.GetType().FullName}\nOriginal Message: {origin.Message}";

            public override string StackTrace => origin.StackTrace;

            public override System.Exception GetBaseException() => origin;
        }

        /// <summary>
        /// Returns a value whether the given exception is a Music Player exception. <br />
        /// Music Player exceptions are considered all these exceptions that derive from the special class
        /// <see cref="BaseException"/>. <br />
        /// If <paramref name="exception"/> is null , it returns false under no circumstance.
        /// </summary>
        /// <param name="exception">The exception to test against.</param>
        /// <returns>A value whether the <paramref name="exception"/> given derives from <see cref="BaseException"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)] // Fast call
        public static System.Boolean IsMusicPlayerException([AllowNull] this Exception exception) => exception is BaseException;

        /// <summary>
        /// Returns a value whether the given exception is a Music Player exception that comes translated from native interop. <br />
        /// Such exceptions are considered all of the exceptions that are deriving from <see cref="BaseException"/> and implement <see cref="INativeException"/>. <br />
        /// If <paramref name="exception"/> is null , it returns false under no circumstance.
        /// </summary>
        /// <param name="exception">The exception to test against.</param>
        /// <returns>A value whether the <paramref name="exception"/> given derives from <see cref="BaseException"/> and implements <see cref="INativeException"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)] // Fast call
        public static System.Boolean IsMarkedAsNativeException([AllowNull] this Exception exception) => exception is BaseException && exception is INativeException;

        /// <summary>
        /// Translates the given Music Player exception to a .NET - compatible exception. <br />
        /// If <paramref name="exception"/> is null , it returns null under no circumstance.
        /// </summary>
        /// <param name="exception">The exception to translate.</param>
        /// <returns>The translated exception object.</returns>
        [return: NotNullIfNotNull(nameof(exception))]
        public static Exception TranslateAsDotNetException([AllowNull] this BaseException exception) => exception is null ? null : new MusicPlayerExceptionWrappedException(exception);

        /// <summary>
        /// Adds all the held inner exceptions of this <see cref="AggregateException"/> object to the specified <see cref="ExceptionBuilder"/> instance.
        /// </summary>
        /// <param name="aggregate">The <see cref="AggregateException"/> object to hand the inner exceptions to.</param>
        /// <param name="builder">The <see cref="ExceptionBuilder"/> to store all the held inner exception instances that <paramref name="aggregate"/> contains.</param>
        /// <exception cref="ArgumentNullException"><paramref name="builder"/> is <see langword="null"/>.</exception>
        [Throws(typeof(ArgumentNullException))]
        public static void AppendToExceptionBuilder(this AggregateException aggregate, ExceptionBuilder builder)
        {
            ArgumentNullException.ThrowIfNull(builder);
            var ies = aggregate.InnerExceptions;
            Exception[] exceptions = new Exception[ies.Count];
            ies.CopyTo(exceptions , 0);
            builder.AddRange(exceptions);
        }
    }
}