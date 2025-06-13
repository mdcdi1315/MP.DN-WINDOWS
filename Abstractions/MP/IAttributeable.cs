

namespace MP
{
    /// <summary>
    /// Classes implementing this interface are able to have attributes, either these to be got or are to be set. <br />
    /// Attributes on such classes describe additional mutable state that the object must have.
    /// </summary>
    public interface IAttributeable
    {
        /// <summary>
        /// Gets an attribute with the specified name. <br />
        /// The name must not be null or the empty string.
        /// </summary>
        /// <param name="name">The name of the attribute to retrieve.</param>
        /// <returns>The attribute's value.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="name"/> was null or empty.</exception>
        /// <exception cref="MP.ExceptionSystem.AttributeNotFoundException"><paramref name="name"/> was not found.</exception>
        public System.Object GetAttribute(System.String name);

        /// <summary>
        /// Sets or updates the attribute's value with the specified name. <br />
        /// The name must not be null or the empty string.
        /// </summary>
        /// <param name="name">The name of the attribute to set the <paramref name="value"/> to.</param>
        /// <param name="value">The value of the attribute with name <paramref name="name"/>.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="name"/> was null or empty.</exception>
        public void SetAttribute(System.String name, System.Object value);
    }

    /// <summary>
    /// Common and handy extension methods for the <see cref="IAttributeable"/> interface.
    /// </summary>
    public static class IAttributeableExtensions
    {
        /// <summary>
        /// Sets a boolean attribute on the specified class that implements the <see cref="IAttributeable"/> interface.
        /// </summary>
        /// <param name="attributeable">The class that implements the <see cref="IAttributeable"/> logic.</param>
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
        /// <param name="attributeable">The class that implements the <see cref="IAttributeable"/> logic.</param>
        /// <param name="name">The name of the attribute to set.</param>
        /// <param name="value">The new value of the attribute.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="name"/> was null or empty.</exception>
        public static void SetStringAttribute(this IAttributeable attributeable, System.String name, System.String value)
        {
            if (System.String.IsNullOrEmpty(name)) { throw new System.ArgumentNullException(nameof(name), "Attribute name must not be empty."); }
            attributeable.SetAttribute(name, value);
        }

        /// <summary>
        /// Gets a boolean attribute from the specified class that implements the <see cref="IAttributeable"/> interface.
        /// </summary>
        /// <param name="attributeable">The class that implements the <see cref="IAttributeable"/> logic.</param>
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
        /// <param name="attributeable">The class that implements the <see cref="IAttributeable"/> logic.</param>
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
        /// <param name="attributeable">The class that implements the <see cref="IAttributeable"/> logic.</param>
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
        /// <param name="attributeable">The class that implements the <see cref="IAttributeable"/> logic.</param>
        /// <param name="name">The name of the attribute to set.</param>
        /// <param name="value">The new value of the attribute.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="name"/> was null or empty.</exception>
        public static void SetCustomAttribute<T>(this IAttributeable attributeable, System.String name, T value)
        {
            if (System.String.IsNullOrEmpty(name)) { throw new System.ArgumentNullException(nameof(name), "Attribute name must not be empty."); }
            attributeable.SetAttribute(name, value);
        }
    }

}