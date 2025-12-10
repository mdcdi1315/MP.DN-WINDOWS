

using System;
using System.Text;
using System.Collections.Generic;
using MP.Annotations.CodeAnalysis;
using System.Diagnostics.CodeAnalysis;

namespace MP.Resources.Management
{
    /// <summary>
    /// Provides the definition of a 'partial resource identifier', that is an identifier identifying a resource in a resource file. <br />
    /// These identifiers are also file-system structured, that means that can have path separators, and file names with extensions are supported as well.
    /// </summary>
    public sealed class PartialResourceIdentifier : ICloneable, IEquatable<PartialResourceIdentifier>
    {
        /// <summary>
        /// Defines the character that identifies the path separation.
        /// </summary>
        public const System.Char PATH_SEPARATOR = '/';

        /// <summary>
        /// Provides the only and default instance of the <see cref="PartialResourceIdentifier"/> class that has empty path components.
        /// </summary>
        public static readonly PartialResourceIdentifier Empty = new(Array.Empty<String>());

        private readonly String[] path_components;
        private readonly int hash_code;

        private PartialResourceIdentifier(String[] components)
        {
            // Precompute the hash code and save it to a field to gain the advantage of accessing it quickly -
            // will be very useful for using this with collection classes and sorting algorithms.
            HashCode hc = new();
            foreach (var component in path_components = components) {
                hc.Add(component);
            }
            int pcs = path_components.Length - 1;
            for (int I = 0; I < pcs; I++) { hc.Add(PATH_SEPARATOR); }
            hash_code = hc.ToHashCode();
        }

        private PartialResourceIdentifier(String[] components , int hash)
        {
            path_components = components;
            hash_code = hash;
        }

        /// <summary>
        /// Parses a <see cref="PartialResourceIdentifier"/> from a string.
        /// </summary>
        /// <param name="path">The string to parse.</param>
        /// <returns>A <see cref="PartialResourceIdentifier"/> identifying the parsed partial path.</returns>
        [Throws(typeof(ArgumentNullException))]
        public static PartialResourceIdentifier Parse(String path)
        {
            ArgumentNullException.ThrowIfNull(path);

            List<String> components = new(10); // Typically, a path should not have more than 10 components.

            StringBuilder builder = new(path.Length);

            foreach (System.Char c in path) 
            {
                if (c == PATH_SEPARATOR) {
                    // Only add a component if the path represents an actual file or folder (that is, patterns like // are avoided).
                    if (builder.Length > 0) { components.Add(builder.ToString()); }
                    builder.Clear();
                } else {
                    builder.Append(c);
                }
            }

            // If there are leftovers on the builder, make them a new component and append it to the list.
            if (builder.Length > 0) {
                components.Add(builder.ToString());
                builder.Clear();
            }
            // Unreference the builder because ToArray can take some time to complete.
            builder = null;

            return (components.Count == 0) ? Empty : new PartialResourceIdentifier(components.ToArray());
        }

        /// <summary>
        /// Builds a <see cref="PartialResourceIdentifier"/> from the specified components.
        /// </summary>
        /// <param name="components">The strings defining valid path components. These should not contain any path separators. If they do, <see cref="ArgumentException"/> will be thrown.</param>
        /// <returns>A <see cref="PartialResourceIdentifier"/> from the specified raw path components.</returns>
        [Throws(typeof(ArgumentNullException), typeof(ArgumentException))]
        public static PartialResourceIdentifier Build(params String[] components)
        {
            ArgumentNullException.ThrowIfNull(components);
            return BuildIdentifier(null , components);
        }

        private static PartialResourceIdentifier BuildIdentifier([MaybeNull] PartialResourceIdentifier base_id, String[] components)
        {
            foreach (String component in components)
            {
                if (component is null) {
                    throw new ArgumentException("One of the specified path components is null.", nameof(components));
                } else if (component.Contains(PATH_SEPARATOR)) {
                    throw new ArgumentException($"One of the specified path components contains path separators.\nComponent: {component}", nameof(components));
                }
            }
            String[] final_components = (base_id is null) ? components : JoinArrays(base_id.path_components , components);
            return (final_components.Length == 0) ? Empty : new PartialResourceIdentifier(final_components);
        }

        private static String[] JoinArrays(String[] first , String[] second)
        {
            if (first is null) {
                return second ?? Array.Empty<String>();
            } else if (second is null) {
                return first; // We have already checked for null above.
            } else {
                int l1 = first.Length , l2 = second.Length;
                String[] final_array = new String[l1 + l2];
                Array.ConstrainedCopy(first , 0 , final_array , 0 , l1);
                Array.ConstrainedCopy(second , 0 , final_array , l1 , l2);
                return final_array;
            }
        }

        /// <summary>
        /// Gets the absolute path for this <see cref="PartialResourceIdentifier"/>.
        /// </summary>
        public String FullName
        {
            get {
                StringBuilder builder = new(path_components.Length * 5);
                foreach (String s in path_components) {
                    builder.Append(s);
                    builder.Append(PATH_SEPARATOR);
                }
                if (builder.Length > 0) {
                    builder.Remove(builder.Length - 1, 1);
                }
                return builder.ToString();
            }
        }

        /// <summary>
        /// Gets the name of the last path component for this <see cref="PartialResourceIdentifier"/>, or the empty string if this <see cref="PartialResourceIdentifier"/> is empty.
        /// </summary>
        public String Name
            => (path_components.Length == 0) ? System.String.Empty : path_components[path_components.Length - 1];

        /// <summary>
        /// Gets the extension of the last path component for this <see cref="PartialResourceIdentifier"/>, or the empty string if the last component does not contain an extension.
        /// </summary>
        public String Extension
        {
            get {
                String s = Name;
                int idx = s.IndexOf('.');
                if (idx == -1) {
                    return System.String.Empty;
                } else {
                    return s.Substring(idx+1);
                }
            }
        }
    
        /// <summary>
        /// Gets all the path components comprising this <see cref="PartialResourceIdentifier"/> instance.
        /// </summary>
        /// <returns>An enumerable of strings providing the path components of this <see cref="PartialResourceIdentifier"/> instance.</returns>
        public IEnumerable<String> GetComponents() => path_components;

        /// <summary>
        /// Constructs a new <see cref="PartialResourceIdentifier"/> from the components of this partial resource identifier and more specified by <paramref name="additional_components"/>.
        /// </summary>
        /// <param name="additional_components">The additional path components to append to the constucted partial resource identifier.</param>
        /// <returns>The constructed <see cref="PartialResourceIdentifier"/>.</returns>
        public PartialResourceIdentifier With(params String[] additional_components)
        {
            ArgumentNullException.ThrowIfNull(additional_components);
            return BuildIdentifier(this , additional_components);
        }

        /// <summary>
        /// Creates a clone of this <see cref="PartialResourceIdentifier"/> class instance.
        /// </summary>
        /// <returns>The cloned instance.</returns>
        /// <remarks>
        /// Although cloning of the <see cref="Empty"/> object is allowed, using such an object is treated like it was <see cref="Empty"/>.
        /// </remarks>
        public PartialResourceIdentifier Clone() => new(path_components, hash_code);

        object ICloneable.Clone() => Clone();

        /// <summary>
        /// Gets a value whether the current and the specified <see cref="PartialResourceIdentifier"/> are considered equal.
        /// </summary>
        /// <param name="other">The other partial resource identifier to match against.</param>
        /// <returns>A value whether both instances have the same path components and in the same order as well.</returns>
        public bool Equals([MaybeNull] PartialResourceIdentifier other)
        {
            if (other is null || path_components.Length != other.path_components.Length) {
                return false;
            } else {
                for (int I = 0; I < path_components.Length; I++) {
                    if (!other.path_components[I].Equals(path_components[I])) { return false; }
                }
                return true;
            }
        }

        /// <summary>
        /// Gets a value whether the current <see cref="PartialResourceIdentifier"/> and the specified object are equal.
        /// </summary>
        /// <param name="obj">The object to compare this instance against.</param>
        /// <returns>A value whether <paramref name="obj"/> is an object of type <see cref="PartialResourceIdentifier"/> and it has the same path components and in the same order as this instance.</returns>
        public override bool Equals(object obj)
        {
            // We could also compare the hash codes of the objects, but we are not into a collection class.
            // So, check the path components explicitly instead.
            if (obj is PartialResourceIdentifier other) {
                if (path_components.Length != other.path_components.Length) {
                    return false;
                } else {
                    for (int I = 0; I < path_components.Length; I++) {
                        if (!other.path_components[I].Equals(path_components[I])) { return false; }
                    }
                    return true;
                }
            } else {
                return false;
            }
        }

        /// <summary>
        /// Returns a string describing the current partial resource identifier.
        /// </summary>
        /// <returns>A string describing the current partial resource identifier.</returns>
        public override string ToString() => $"Partial Resource Identifier: \"{FullName}\"";

        /// <summary>
        /// Gets a hash code for this <see cref="PartialResourceIdentifier"/>, that is suitable to be used in collection classes and hash tables.
        /// </summary>
        /// <returns>The pre-computed hash code for this <see cref="PartialResourceIdentifier"/>.</returns>
        public override int GetHashCode() => hash_code;
    }
}
