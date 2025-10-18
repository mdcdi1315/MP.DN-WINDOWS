
using System;

namespace MP.Annotations.CodeAnalysis
{
    /// <summary>
    /// Defined to a delegate type, indicating that the caller
    /// must handle all the exceptions thrown by the invocation
    /// of the attributed delegate.
    /// </summary>
    [AttributeUsage(AttributeTargets.Delegate , AllowMultiple = false , Inherited = false)]
    public sealed class CallerMustHandleExceptionsAttribute : Attribute { }
}