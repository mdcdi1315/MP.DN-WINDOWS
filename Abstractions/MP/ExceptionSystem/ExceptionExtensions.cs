namespace MP.ExceptionSystem
{
    /// <summary>
    /// Defines common extensions for all the Music Player exceptions.
    /// </summary>
    public static class ExceptionExtensions
    {
        private sealed class MusicPlayerExceptionWrappedException : System.Exception
        {
            private BaseException origin;

            public MusicPlayerExceptionWrappedException(BaseException origin)
            {
                if (origin is null) { throw new System.ArgumentNullException(nameof(origin)); }
                this.origin = origin;
            }

            public override string Source => origin.GetType().FullName;

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
        public static System.Boolean IsMusicPlayerException(this System.Exception exception)
        {
            if (exception is null) { return false; }
            return exception is BaseException;
        }

        /// <summary>
        /// Returns a value whether the given exception is a Music Player exception that comes translated from native interop. <br />
        /// Such exceptions are considered all of the exceptions that are deriving from <see cref="BaseException"/> and implement <see cref="INativeException"/>. <br />
        /// If <paramref name="exception"/> is null , it returns false under no circumstance.
        /// </summary>
        /// <param name="exception">The exception to test against.</param>
        /// <returns>A value whether the <paramref name="exception"/> given derives from <see cref="BaseException"/> and implements <see cref="INativeException"/>.</returns>
        public static System.Boolean IsMarkedAsNativeException(this System.Exception exception)
        {
            if (exception is null) { return false; }
            return exception is BaseException && exception is INativeException;
        }

        /// <summary>
        /// Translates the given Music Player exception to a .NET - compatible exception. <br />
        /// If <paramref name="exception"/> is null , it returns null under no circumstance.
        /// </summary>
        /// <param name="exception">The exception to translate.</param>
        /// <returns>The translated exception object.</returns>
        public static System.Exception TranslateAsDotNetException(this BaseException exception) 
        {
            if (exception is null) { return null; }
            return new MusicPlayerExceptionWrappedException(exception);
        }
    }
}