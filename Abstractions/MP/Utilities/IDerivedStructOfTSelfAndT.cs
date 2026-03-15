
using System.Diagnostics.CodeAnalysis;

namespace MP.Utilities
{
    /// <summary>
    /// Provides an interface implementation for structures that do contain another structure that is based on it.
    /// </summary>
    /// <typeparam name="TSelf">The structure type itself.</typeparam>
    /// <typeparam name="T">The type of structure that is 'derived'.</typeparam>
    public interface IDerivedStruct<TSelf, T>
        where TSelf : struct, IDerivedStruct<TSelf, T>
        where T : struct
    {
        /// <summary>Implicitly casts this <typeparamref name="TSelf"/> structure to a structure of type <typeparamref name="T"/>.</summary>
        /// <param name="t">The structure instance that is a 'derived' form of <typeparamref name="T"/>.</param>
        public static abstract implicit operator T([DisallowNull] TSelf t);
    }
}