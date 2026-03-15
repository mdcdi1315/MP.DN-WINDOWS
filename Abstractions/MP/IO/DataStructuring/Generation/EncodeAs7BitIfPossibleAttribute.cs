
using System;

namespace MP.IO.DataStructuring.Generation
{
    /// <summary>Applied to a numeric field only. Indicate to the source generator that the field must be encoded as a 7-bit encoded integer, if possible.</summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public sealed class EncodeAs7BitIfPossibleAttribute : Attribute { }
}