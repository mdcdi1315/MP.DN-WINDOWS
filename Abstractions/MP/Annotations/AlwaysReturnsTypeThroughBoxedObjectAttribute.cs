
using System;

namespace MP.Annotations
{
    /// <summary>
    /// Marked to a method return value which does always return a boxed object of the specified type, or null.
    /// </summary>
    [AttributeUsage(AttributeTargets.ReturnValue, AllowMultiple = false)]
    public sealed class AlwaysReturnsTypeThroughBoxedObjectAttribute : Attribute 
    {
        private Type unboxed;

        /// <summary>
        /// Creates a new instance of the <see cref="AlwaysReturnsTypeThroughBoxedObjectAttribute"/> 
        /// class with the specified type that is always returned boxed to an <see cref="System.Object"/>.
        /// </summary>
        /// <param name="unboxedtype">The type of the object that is always returned as it's boxed representation.</param>
        public AlwaysReturnsTypeThroughBoxedObjectAttribute(Type unboxedtype)
        {
            unboxed = unboxedtype;
        }

        /// <summary>Gets the type of the boxed object that is always returned.</summary>
        public Type ReturnType => unboxed;
    }
}