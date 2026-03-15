
using System;
using MP.Utilities;

namespace MP.Serialization
{
    /// <summary>
    /// Defines that this field definition can accept and return and derived types of it, when de/serializing it. <br />
    /// The binding is done on the fly during that field lookup.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field , AllowMultiple = false)]
    public sealed class DerivedTypeBindingAttribute : Attribute
    {
        private readonly Type[] types;

        /// <summary>
        /// Constructs a new instance of the <see cref="DerivedTypeBindingAttribute"/> class defining the types that must be also tested whether they make a match.
        /// </summary>
        /// <param name="types">A variable array of statically defined types.</param>
        public DerivedTypeBindingAttribute(params Type[] types) => this.types = types;

        /// <summary>
        /// Defines the types that the serialization manager should also look up.
        /// </summary>
        public Type[] DerivedTypes => types;

        /// <summary>
        /// Gets a value whether all the specified types in the current attribute instance are types that somehow are deriving from <paramref name="type"/>. <br />
        /// NOTE: This should not be used by your code. It is an internal API used by the serialization manager services.
        /// </summary>
        /// <param name="type">The type to test for derivability.</param>
        /// <returns>A value whether the defined types are all deriving from the specified type; otherwise , <see langword="false"/>.</returns>
        public System.Boolean AreAllDerivingFrom(Type type)
        {
            ArgumentNullException.ThrowIfNull(type);
            foreach (var t in types) {
                if (!t.DerivesFrom(type)) {
                    return false;
                }
            }
            return true;
        }
    }
}