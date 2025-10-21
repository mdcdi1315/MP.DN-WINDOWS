
using System;

namespace MP.ExceptionSystem
{
    /// <summary>
    /// Exception that is thrown or returned when the RCU engine has failed initialization. <br />
    /// This exception only occurs if another exception is thrown on the user code.
    /// </summary>
    public sealed class RCUEngineInitializationException : BaseException
    {
        /// <summary>
        /// Constructs a new instance of the <see cref="RCUEngineInitializationException"/> class, providing the exception that caused this exception to be thrown.
        /// </summary>
        /// <param name="any">The exception that caused this exception to be constructed.</param>
        public RCUEngineInitializationException(Exception any) : base("RCU Engine failed initialization due to an exception." , any) { }
    }
}