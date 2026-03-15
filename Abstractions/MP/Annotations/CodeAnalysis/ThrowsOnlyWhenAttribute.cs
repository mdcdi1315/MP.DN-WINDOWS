


using System;

namespace MP.Annotations.CodeAnalysis
{
    /// <summary>
    /// Specifies that a given exception type is not thrown unless the specified compilation symbol is defined.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor , AllowMultiple = true)]
    public sealed class ThrowsOnlyWhenAttribute : Attribute
    {
        private readonly System.Type type;
        private readonly System.String symbol;

        /// <summary>
        /// Creates a new instance of the attribute, specifying the exception type that is not thrown unless 
        /// the symbol defined in the <paramref name="symbol"/> parameter is defined as a compilation symbol.
        /// </summary>
        /// <param name="exceptiontype">The exception type that is thrown only when <paramref name="symbol"/> is defined.</param>
        /// <param name="symbol">The compilation symbol that must have been defined so that <paramref name="exceptiontype"/> has a chance to be thrown.</param>
        public ThrowsOnlyWhenAttribute(String symbol , Type exceptiontype)
        {
            type = exceptiontype;
            this.symbol = symbol;
        }

        /// <summary>
        /// The exception type that can be thrown when the symbol defined in the <see cref="RequiredCompilationSymbol"/> property is defined.
        /// </summary>
        public Type ExceptionType => type;

        /// <summary>
        /// The complation symbol that must be defined so that the exception type defined <see cref="ExceptionType"/> has a chance to be thrown.
        /// </summary>
        public String RequiredCompilationSymbol => symbol;
    }
}