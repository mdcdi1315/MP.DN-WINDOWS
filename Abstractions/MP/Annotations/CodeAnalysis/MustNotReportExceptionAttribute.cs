

using System;

namespace MP.Annotations.CodeAnalysis
{
    /// <summary>
    /// Specified to any method declaration to say that the attributed method must or does actually not throw any exceptions under any circumstance.
    /// </summary>
    [Preliminary]
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class MustNotReportExceptionAttribute : Attribute { }
}