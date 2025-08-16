
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace MP.Collections
{
    /// <summary>
    /// Represents an equality comparer that can precisely compare two deleagtes. <br />
    /// If the user requires to, he can override this class and override/extend the provided interface method implementations.
    /// </summary>
    /// <typeparam name="TD">The type of the delegate to be compared for equality.</typeparam>
    public class DelegateEqualityComparer<TD> : IEqualityComparer<TD>
        where TD : System.Delegate
    {
        /// <summary>
        /// Determines whether the specified delegates do represent the same delegate.
        /// </summary>
        /// <param name="x">The first delegate to test.</param>
        /// <param name="y">The second delegate to test.</param>
        /// <returns><see langword="true"/> if the specified delegates are the same delegate; otherwise, <see langword="false"/>.</returns>
        public System.Boolean Equals(TD x, TD y) => x == y;

        /// <summary>
        /// Gets a hash code that uniquely identifies the provided delegate instance.
        /// </summary>
        /// <param name="obj">The delegate instance to get it's hash code.</param>
        /// <returns>The unique hash code identifier for <paramref name="obj"/>.</returns>
        public System.Int32 GetHashCode([DisallowNull] TD obj) => obj.Method.MethodHandle.GetHashCode();
    }
}