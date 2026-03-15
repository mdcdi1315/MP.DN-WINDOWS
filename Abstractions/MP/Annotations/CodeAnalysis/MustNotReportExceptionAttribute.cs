

using System;

namespace MP.Annotations.CodeAnalysis
{
    /// <summary>
    /// Specified to any method or constructor declaration to indicate that the attributed method must or does actually not throw any exceptions under any circumstance.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor, AllowMultiple = false, Inherited = false)]
    public sealed class MustNotReportExceptionAttribute : Attribute { }
}