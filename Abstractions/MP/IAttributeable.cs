
using MP.Annotations.CodeAnalysis;
using System.Diagnostics.CodeAnalysis;

namespace MP
{
    /// <summary>
    /// Classes implementing this interface are able to have attributes, either these to be got or are to be set. <br />
    /// Attributes on such classes describe additional mutable state that the object must have.
    /// </summary>
    public interface IAttributeable
    {
        /// <summary>
        /// Attempts to get an atttribute defined in the current object. <br />
        /// The contents of the <paramref name="attribute"/> parameter must not be null or the empty string. <br />
        /// If the attribute does not exist, 
        /// <see langword="false"/> is returned by the method and
        /// <see langword="null"/> is returned by the <paramref name="value"/> parameter.
        /// </summary>
        /// <param name="attribute">The name of the attribute to get.</param>
        /// <param name="value">The current value of the attribute to get.</param>
        /// <returns>A value whether the specified attribute is defined.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="attribute"/> was null or empty.</exception>
        [Throws(typeof(System.ArgumentNullException))]
        public System.Boolean TryGetAttribute(System.String attribute, [MaybeNullWhen(true)] out System.Object value);

        /// <summary>
        /// Sets or updates the attribute's value with the specified name. <br />
        /// The name must not be null or the empty string.
        /// </summary>
        /// <param name="name">The name of the attribute to set the <paramref name="value"/> to.</param>
        /// <param name="value">The value of the attribute with name <paramref name="name"/>.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="name"/> was null or empty.</exception>
        [Throws(typeof(System.ArgumentNullException))]
        public void SetAttribute(System.String name, [AllowNull] System.Object value);
    }

    /// <summary>
    /// Common and handy extension methods for the <see cref="IAttributeable"/> interface.
    /// </summary>
    public static class IAttributeableExtensions
    {
        /// <summary>
        /// Sets a boolean attribute on the specified class that implements the <see cref="IAttributeable"/> interface.
        /// </summary>
        /// <param name="attributeable">The object that implements the <see cref="IAttributeable"/> logic.</param>
        /// <param name="name">The name of the attribute to set.</param>
        /// <param name="value">The new value of the attribute.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="name"/> was null or empty.</exception>
        public static void SetBooleanAttribute(this IAttributeable attributeable, System.String name, System.Boolean value)
        {
            if (System.String.IsNullOrEmpty(name)) { throw new System.ArgumentNullException(nameof(name) , "Attribute name must not be empty."); }
            attributeable.SetAttribute(name, value);
        }

        /// <summary>
        /// Sets a string attribute on the specified class that implements the <see cref="IAttributeable"/> interface.
        /// </summary>
        /// <param name="attributeable">The object that implements the <see cref="IAttributeable"/> logic.</param>
        /// <param name="name">The name of the attribute to set.</param>
        /// <param name="value">The new value of the attribute.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="name"/> was null or empty.</exception>
        public static void SetStringAttribute(this IAttributeable attributeable, System.String name, System.String value)
        {
            if (System.String.IsNullOrEmpty(name)) { throw new System.ArgumentNullException(nameof(name), "Attribute name must not be empty."); }
            attributeable.SetAttribute(name, value);
        }

        /// <summary>
        /// Gets an attribute with the specified name. <br />
        /// The contents of the <paramref name="name"/> parameter must not be null or the empty string.
        /// </summary>
        /// <param name="attributeable">The object that implements the <see cref="IAttributeable"/> logic.</param>
        /// <param name="name">The name of the attribute to retrieve.</param>
        /// <returns>The attribute's value.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="name"/> was null or empty.</exception>
        /// <exception cref="ExceptionSystem.AttributeNotFoundException"><paramref name="name"/> was not found.</exception>
        [Throws(typeof(System.ArgumentNullException), typeof(ExceptionSystem.AttributeNotFoundException))]
        public static System.Object GetAttribute(this IAttributeable attributeable, System.String name)
        {
            if (System.String.IsNullOrEmpty(name)) { 
                throw new System.ArgumentNullException(nameof(name), "Attribute name must not be empty."); 
            }
            System.Object value;
            if (attributeable.TryGetAttribute(name, out value)) {
                return value;
            } else {
                throw new ExceptionSystem.AttributeNotFoundException(name);
            }
        }

        /// <summary>
        /// Gets a boolean attribute from the specified class that implements the <see cref="IAttributeable"/> interface.
        /// </summary>
        /// <param name="attributeable">The object that implements the <see cref="IAttributeable"/> logic.</param>
        /// <param name="name">The name of the attribute to get.</param>
        /// <returns>The current value of the attribute.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="name"/> was null or empty.</exception>
        /// <exception cref="System.InvalidCastException">The attribute value is not a <see cref="System.Boolean"/>.</exception>
        /// <exception cref="MP.ExceptionSystem.AttributeNotFoundException"><paramref name="name"/> was not found.</exception>
        public static System.Boolean GetBooleanAttribute(this IAttributeable attributeable, System.String name)
        {
            if (System.String.IsNullOrEmpty(name)) { throw new System.ArgumentNullException(nameof(name), "Attribute name must not be empty."); }
            return (System.Boolean)attributeable.GetAttribute(name);
        }

        /// <summary>
        /// Gets a string attribute from the specified class that implements the <see cref="IAttributeable"/> interface.
        /// </summary>
        /// <param name="attributeable">The object that implements the <see cref="IAttributeable"/> logic.</param>
        /// <param name="name">The name of the attribute to get.</param>
        /// <returns>The current value of the attribute.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="name"/> was null or empty.</exception>
        /// <exception cref="System.InvalidCastException">The attribute value is not a <see cref="System.String"/>.</exception>
        /// <exception cref="MP.ExceptionSystem.AttributeNotFoundException"><paramref name="name"/> was not found.</exception>
        public static System.String GetStringAttribute(this IAttributeable attributeable, System.String name)
        {
            if (System.String.IsNullOrEmpty(name)) { throw new System.ArgumentNullException(nameof(name), "Attribute name must not be empty."); }
            return attributeable.GetAttribute(name) as System.String;
        }

        /// <summary>
        /// Gets an attribute of type <typeparamref name="T"/> from the specified class that implements the <see cref="IAttributeable"/> interface.
        /// </summary>
        /// <param name="attributeable">The object that implements the <see cref="IAttributeable"/> logic.</param>
        /// <param name="name">The name of the attribute to get.</param>
        /// <returns>The current value of the attribute.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="name"/> was null or empty.</exception>
        /// <exception cref="System.InvalidCastException">The attribute value is not a <typeparamref name="T"/>.</exception>
        /// <exception cref="MP.ExceptionSystem.AttributeNotFoundException"><paramref name="name"/> was not found.</exception>
        public static T GetCustomAttribute<T>(this IAttributeable attributeable , System.String name)
        {
            if (System.String.IsNullOrEmpty(name)) { throw new System.ArgumentNullException(nameof(name), "Attribute name must not be empty."); }
            return (T)attributeable.GetAttribute(name);
        }

        /// <summary>
        /// Sets a custom attribute of type <typeparamref name="T"/> on the specified class that implements the <see cref="IAttributeable"/> interface.
        /// </summary>
        /// <param name="attributeable">The object that implements the <see cref="IAttributeable"/> logic.</param>
        /// <param name="name">The name of the attribute to set.</param>
        /// <param name="value">The new value of the attribute.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="name"/> was null or empty.</exception>
        public static void SetCustomAttribute<T>(this IAttributeable attributeable, System.String name, T value)
        {
            if (System.String.IsNullOrEmpty(name)) { throw new System.ArgumentNullException(nameof(name), "Attribute name must not be empty."); }
            attributeable.SetAttribute(name, value);
        }

        /// <summary>
        /// Attempts to get an attribute of type <typeparamref name="T"/> from the specified class that implements the <see cref="IAttributeable"/> interface. <br />
        /// If the attribute exists but is not the type <typeparamref name="T"/>, it returns <see langword="false"/>.
        /// </summary>
        /// <param name="attributeable">The object that implements the <see cref="IAttributeable"/> logic.</param>
        /// <param name="name">The name of the attribute to get.</param>
        /// <param name="value">The current value of the attribute, if found and the value is <typeparamref name="T"/>.</param>
        /// <returns>The current value of the attribute.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="name"/> was null or empty.</exception>
        public static System.Boolean TryGetCustomAttribute<T>(this IAttributeable attributeable , System.String name , out T value)
        {
            if (System.String.IsNullOrEmpty(name)) { throw new System.ArgumentNullException(nameof(name), "Attribute name must not be empty."); }
            value = default;
            try {
                System.Object v;
                System.Boolean b = attributeable.TryGetAttribute(name, out v);
                if (b) { value = (T)v; }
                return b;
            } catch (System.InvalidCastException) {
                return false;
            }
        }
    }

}