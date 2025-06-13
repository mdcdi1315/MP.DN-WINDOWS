

using System.Collections.Generic;

namespace MP.Collections
{
    /// <summary>
    /// Provides a default comparer for comparing delegate instances.
    /// </summary>
    /// <typeparam name="TD">The type of the delegate to be compared.</typeparam>
    public class DelegateComparer<TD> : IComparer<TD>
        where TD : System.Delegate
    {
        /// <summary>
        /// Compares two <typeparamref name="TD"/> delegate objects and returns their comparison result. <br />
        /// Note that the following truth table is applied: <br />
        /// <list type="table">
        ///     <listheader>
        ///         <term><paramref name="x"/> is <see langword="null"/>?</term>
        ///         <term><paramref name="y"/> is <see langword="null"/>?</term>
        ///         <description>Result value outcome</description>
        ///     </listheader>
        ///     <item>
        ///         <term>Yes</term>
        ///         <term>Yes</term>
        ///         <description>Both objects are <see langword="null"/> - so they are equal. 0 is returned.</description>
        ///     </item>
        ///     <item>
        ///         <term>Yes</term>
        ///         <term>No</term>
        ///         <description><paramref name="x"/> is greater than <paramref name="y"/>. 1 is returned.</description>
        ///     </item>
        ///     <item>
        ///         <term>No</term>
        ///         <term>Yes</term>
        ///         <description><paramref name="x"/> is less than <paramref name="y"/>. -1 is returned.</description>
        ///     </item>
        ///     <item>
        ///         <term>No</term>
        ///         <term>No</term>
        ///         <description>Both objects are not null, and their method handles are compared.</description>
        ///     </item>
        /// </list>
        /// </summary>
        /// <param name="x">The first delegate object to be compared.</param>
        /// <param name="y">The second delegate object to be compared.</param>
        /// <returns>The comparison result.</returns>
        public System.Int32 Compare(TD x, TD y)
        {
            System.Boolean n1 = x is null;
            System.Boolean n2 = y is null;
            if (n1 && n2) { return 0; }
            if (n1 == false && n2) { return -1; }
            if (n1 && n2 == false) { return 1; }
            return (x.Method.MethodHandle.Value - y.Method.MethodHandle.Value).ToInt64().ToInt32();
        }
    }
}