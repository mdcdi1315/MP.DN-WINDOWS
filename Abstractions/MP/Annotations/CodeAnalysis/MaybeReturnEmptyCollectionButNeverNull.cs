

using System;
using System.Collections;

namespace MP.Annotations.CodeAnalysis
{
    /// <summary>
    /// Specified to a method's return value to indicate that there is a possibility to return an empty collection, 
    /// but under no circumstance it will return <see langword="null"/> . <br />
    /// Should be applied to any collection type extending <see cref="IEnumerable"/>.
    /// </summary>
    [Preliminary]
    [AttributeUsage(AttributeTargets.ReturnValue, AllowMultiple = false)]
    public sealed class MaybeReturnEmptyCollectionButNeverNullAttribute : Attribute { }
}