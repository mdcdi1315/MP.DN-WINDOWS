
using System;

namespace MP.ExceptionSystem
{
    /// <summary>
    /// Exception that is thrown when the RCU engine has failed uninitialization. <br />
    /// This exception only occurs if another exception is thrown on the user code. <br />
    /// Similar to how the <see cref="RCUEngineInitializationException"/> class is used.
    /// </summary>
    public sealed class RCUEngineUninitializationException : BaseException
    {
        /// <summary>
        /// Constructs a new instance of the <see cref="RCUEngineUninitializationException"/> class, providing the exception that caused this exception to be thrown.
        /// </summary>
        /// <param name="any">The exception that caused this exception to be constructed.</param>
        public RCUEngineUninitializationException(Exception any) : base("RCU Engine failed uninitialization due to an exception.", any) { }
    }
}