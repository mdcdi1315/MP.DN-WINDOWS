
using System;
using MP.Resources.Management;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace MP.Collections
{
    /// <summary>
    /// Defines an equality and order comparer for <see cref="PartialResourceIdentifier"/> instances. <br />
    /// This hybrid comparer should be used, where possible, when working with collection classes as it provides fast comparing and equality operations.
    /// </summary>
    public sealed class PartialResourceIdentifierComparer : IEqualityComparer<PartialResourceIdentifier>, IComparer<PartialResourceIdentifier>
    {
        private PartialResourceIdentifierComparer() { }

        /// <summary>
        /// Gets the single and only instance of the <see cref="PartialResourceIdentifierComparer"/> class.
        /// </summary>
        public static readonly PartialResourceIdentifierComparer Instance = new();

        /// <inheritdoc />
        public bool Equals(PartialResourceIdentifier x, PartialResourceIdentifier y) => Compare(x, y) == 0;

        /// <inheritdoc />
        public int GetHashCode([DisallowNull] PartialResourceIdentifier obj) {
            ArgumentNullException.ThrowIfNull(obj);
            return obj.GetHashCode();
        }

        /// <inheritdoc />
        public int Compare(PartialResourceIdentifier x, PartialResourceIdentifier y)
        {
            System.Boolean n1 = x is null, n2 = y is null;
            if (n1) {
                return n2 ? 0 : 1;
            } else if (n2) {
                return n1 ? 0 : -1;
            } else {
                return x.GetHashCode() - y.GetHashCode();
            }
        }
    }
}