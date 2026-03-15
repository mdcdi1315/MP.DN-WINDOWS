
using System;
using MP.Annotations.CodeAnalysis;
using System.Diagnostics.CodeAnalysis;

namespace MP
{
    /// <summary>
    /// Provides a layout over how object builders should be defined.
    /// </summary>
    /// <typeparam name="T">The type that will be built once <see cref="Build"/> is called.</typeparam>
    public interface IObjectBuilder<T>
    {
        /// <summary>
        /// Builds the object, returning an instance of type <typeparamref name="T"/>.
        /// </summary>
        /// <returns>The built object.</returns>
        /// <exception cref="InvalidOperationException">If the object builder allows the object to be created only once, this exception is thrown to indicate that object building for a second time is not allowed.</exception>
        [return: NotNull]
        [Throws(typeof(InvalidOperationException))]
        T Build();
    }
}
