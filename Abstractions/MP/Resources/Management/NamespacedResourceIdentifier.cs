
using System;
using MP.Annotations.CodeAnalysis;
using System.Diagnostics.CodeAnalysis;

namespace MP.Resources.Management
{
    /// <summary>
    /// Provides a fully qualified resource identifier by assigning a namespace to the identifier itself.
    /// </summary>
    public sealed class NamespacedResourceIdentifier :
        IEquatable<NamespacedResourceIdentifier>,
        // Provided by explicit interface implementation, typically not meant to be used since is provided through the Path property.
        ITruncatable<PartialResourceIdentifier>,
        ISyncronized, // Immutable
        ICloneable
    {
        private const string NS_SEPARATOR = "://";

        private readonly int hash_code;
        private readonly string _namespace_;
        private readonly PartialResourceIdentifier path;

        /// <summary>
        /// Initializes a new instance of the <see cref="NamespacedResourceIdentifier"/> class from the specified namespace and path.
        /// </summary>
        /// <param name="ns">The namespace of the current identifier.</param>
        /// <param name="path">The path component of the identifier.</param>
        /// <exception cref="ArgumentNullException"><paramref name="ns"/> and/or <paramref name="path"/> are <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="ns"/> is the empty string (&quot;&quot;) <br /> <br />
        /// 
        /// -or- <br /> <br />
        /// 
        /// <paramref name="path"/> is equal to the <see cref="PartialResourceIdentifier.Empty"/> instance.
        /// </exception>
        [Throws(typeof(ArgumentNullException), typeof(ArgumentException))]
        public NamespacedResourceIdentifier(String ns, PartialResourceIdentifier path)
        {
            ArgumentNullException.ThrowIfNull(path);
            ArgumentNullException.ThrowIfNullOrEmpty(ns);
            if (path.Equals(PartialResourceIdentifier.Empty)) {
                throw new ArgumentException("The path component of the namespaced resource identifier cannot be empty.", nameof(path));
            } else {
                HashCode hc = new();
                hc.Add(_namespace_ = ns);
                hc.Add(this.path = path);
                hash_code = hc.ToHashCode();
            }
        }

        /// <summary>
        /// Parses a <see cref="NamespacedResourceIdentifier"/> from the specified string. 
        /// Typically, the input for this method is the value returned from the <see cref="FullName"/> property.
        /// </summary>
        /// <param name="value">The string to parse.</param>
        /// <returns>A new instance of the <see cref="NamespacedResourceIdentifier"/> class representing the parsed string.</returns>
        /// <exception cref="ArgumentException">The specified string did not contain a namespace.</exception>
        [Throws(typeof(ArgumentNullException), typeof(ArgumentException))]
        public static NamespacedResourceIdentifier Parse(String value)
        {
            ArgumentNullException.ThrowIfNull(value);

            int sep;

            if ((sep = value.IndexOf(NS_SEPARATOR)) == -1) {
                throw new ArgumentException($"The specified string is not a namespaced resource identifier: \"{value}\"", nameof(value));
            } else {
                return new NamespacedResourceIdentifier(value.Remove(sep) , PartialResourceIdentifier.Parse(value.Substring(sep+NS_SEPARATOR.Length)));
            }
        }

        /// <summary>
        /// Builds a <see cref="NamespacedResourceIdentifier"/> from the specified namespace and raw path components.
        /// </summary>
        /// <param name="ns">The namespace of the new identifier.</param>
        /// <param name="path_components">The components that comprise the path of the new identifier. This cannot be an empty array.</param>
        /// <returns>The built <see cref="NamespacedResourceIdentifier"/> instance.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="ns"/> and/or <paramref name="path_components"/> are <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="ns"/> is the empty string (&quot;&quot;) <br /> <br />
        /// 
        /// -or- <br /> <br />
        /// 
        /// <paramref name="path_components"/> is the empty array.
        /// </exception>
        [Throws(typeof(ArgumentNullException), typeof(ArgumentException))]
        public static NamespacedResourceIdentifier Build(String ns, params String[] path_components)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(ns);
            ArgumentNullException.ThrowIfNull(path_components);
            return new(ns, PartialResourceIdentifier.Build(path_components));
        }

        /// <summary>
        /// Gets the namespace portion of this namespaced resource identifier.
        /// </summary>
        public String Namespace => _namespace_;

        /// <summary>
        /// Gets the absolute and namespaced path of this <see cref="NamespacedResourceIdentifier"/> object.
        /// </summary>
        public String FullName => String.Concat(_namespace_, NS_SEPARATOR, path.FullName);

        /// <summary>
        /// Gets the path component of this namespaced resource identifier.
        /// </summary>
        public PartialResourceIdentifier Path => path;

        /// <summary>
        /// Returns a string describing the current namespaced resource identifier.
        /// </summary>
        /// <returns>A string describing the current namespaced resource identifier.</returns>
        public override string ToString() => $"Namespaced Resource Identifier: {FullName}";

        /// <summary>
        /// Gets a hash code for this <see cref="NamespacedResourceIdentifier"/>, that is suitable to be used in collection classes and hash tables.
        /// </summary>
        /// <returns>The pre-computed hash code for this <see cref="NamespacedResourceIdentifier"/>.</returns>
        public override int GetHashCode() => hash_code;

        /// <summary>
        /// Clones this <see cref="NamespacedResourceIdentifier"/> to a new object instance.
        /// </summary>
        /// <returns>The cloned <see cref="NamespacedResourceIdentifier"/> instance.</returns>
        public NamespacedResourceIdentifier Clone() => new(_namespace_, path.Clone());

        /// <summary>
        /// Gets a value whether the current and the specified <see cref="NamespacedResourceIdentifier"/> are considered equal.
        /// </summary>
        /// <param name="other">The other partial resource identifier to match against.</param>
        /// <returns>A value whether both instances have the same namespaced and have declared the same path components and are the same order as well.</returns>
        public bool Equals([AllowNull] NamespacedResourceIdentifier other) => other is not null && _namespace_.Equals(other._namespace_) && path.Equals(other.path);

        /// <summary>
        /// Gets a value whether the current <see cref="NamespacedResourceIdentifier"/> and the specified object are equal.
        /// </summary>
        /// <param name="obj">The object to compare this instance against.</param>
        /// <returns>A value whether <paramref name="obj"/> is an object of type <see cref="PartialResourceIdentifier"/> and is equal to the current instance as described in the <see cref="Equals(NamespacedResourceIdentifier)"/> implementation.</returns>
        public override bool Equals(object obj) => obj is NamespacedResourceIdentifier other && Equals(other);

        #region Explicit interface implementations

        System.Object ICloneable.Clone() => Clone();

        PartialResourceIdentifier ITruncatable<PartialResourceIdentifier>.Truncate() => path.Clone();

        #endregion
    }
}
