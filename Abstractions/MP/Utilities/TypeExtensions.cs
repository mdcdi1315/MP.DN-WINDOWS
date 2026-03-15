
using System;
using System.Reflection;
using MP.Annotations.CodeAnalysis;

namespace MP.Utilities
{
    /// <summary>
    /// Defines extension methods around the <see cref="Type"/> instance and the Reflection API's.
    /// </summary>
    public static class TypeExtensions
    {
        // A helper to provide the actual implementation of DerivesFrom and IsTypeOrDerivesFrom methods.
        // Useful also for many other useful methods that this class does additionally provide.
        private static System.Boolean DerivesFromInternal(Type current, Type other)
        {
            Type temp = current, ct;
            // Search the inheritance tree whether the current class extends the class provided in 'other' parameter.
            while ((ct = temp.BaseType) is not null)
            {
                if (ct == other) { return true; }
                temp = ct;
            }
            // Inheritance tree finished , no valid result was found so return false.
            return false;
        }

        [System.Diagnostics.StackTraceHidden]
        private static void DerivesFromCommonValidation(Type current, Type other)
        {
            if (current.IsClass == false) {
                throw new InvalidOperationException("The current type object is not a class object.");
            } else if (other is null) { 
                throw new ArgumentNullException(nameof(other)); 
            } else if (other.IsClass == false) {
                throw new ArgumentException("Derivability can be checked only between classes.", nameof(other));
            }
        }

        /// <summary>
        /// Returns a value whether the class represented by the current <see cref="Type"/> object derives from the type specfied in the <paramref name="other" /> parameter.
        /// </summary>
        /// <param name="current">The type object representing the class that you want to test against.</param>
        /// <param name="other">The type object representing the base class to test against the <paramref name="current"/> parameter.</param>
        /// <returns>A value whether the class represented by the current <see cref="Type"/> instance derives from the specified <see cref="Type"/> instance.</returns>
        /// <exception cref="InvalidOperationException">The current <see cref="Type"/> object does not represent a class.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="other"/> was <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="other"/> was not a <see cref="Type"/> object describing a class type.</exception>
        [Throws(
            typeof(ArgumentNullException),
            typeof(ArgumentException),
            typeof(InvalidOperationException)
        )]
        public static System.Boolean DerivesFrom(this Type current, Type other)
        {
            DerivesFromCommonValidation(current, other);
            return DerivesFromInternal(current, other);
        }

        /// <summary>
        /// Returns a value whether the class represented by the current <see cref="Type"/> object derives from, or is the type specfied in the <paramref name="other" /> parameter.
        /// </summary>
        /// <param name="current">The type object representing the class that you want to test against.</param>
        /// <param name="other">The type object representing the class to test against the <paramref name="current"/> parameter.</param>
        /// <returns>A value whether the class represented by the current <see cref="Type"/> instance derives from, or is the specified <see cref="Type"/> instance.</returns>
        /// <exception cref="InvalidOperationException">The current <see cref="Type"/> object does not represent a class.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="other"/> was <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="other"/> was not a <see cref="Type"/> object describing a class type.</exception>
        [Throws(
            typeof(ArgumentNullException),
            typeof(ArgumentException),
            typeof(InvalidOperationException)
        )]
        public static System.Boolean IsTypeOrDerivesFrom(this Type current, Type other)
        {
            DerivesFromCommonValidation(current, other);
            return current == other || DerivesFromInternal(current, other);
        }

        /// <summary>
        /// Returns a value whether the current type object implements the interface specified in the <paramref name="other" /> parameter.
        /// </summary>
        /// <param name="current">The type object representing the type that you want to test against.</param>
        /// <param name="other">The type object representing the interface to test against the <paramref name="current"/> parameter.</param>
        /// <returns>A value whether the type represented by the current <see cref="Type"/> instance implements the specified interface.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="other"/> was <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="other"/> was not a <see cref="Type"/> object describing an interface type.</exception>
        [Throws(
            typeof(ArgumentNullException),
            typeof(ArgumentException)
        )]
        public static System.Boolean ImplementsInterface(this Type current, Type other)
        {
            ArgumentNullException.ThrowIfNull(other, nameof(other));
            if (other.IsInterface == false) {
                throw new ArgumentException("The current Type object does not represent an interface.");
            } else {
                Type[] types = current.GetInterfaces();
                if (other.IsGenericTypeDefinition) {
                    foreach (var i in types)
                    {
                        if ((i.IsGenericType && i.GetGenericTypeDefinition() == other) || i == other) { return true; }
                    }
                } else {
                    foreach (var i in types) { if (i == other) { return true; } }
                }
                return false;
            }
        }

        /// <summary>
        /// Returns a value whether the current type object implements the interface specified in the type parameter.
        /// </summary>
        /// <param name="current">The type object representing the type that you want to test against.</param>
        /// <returns>A value whether the type represented by the current <see cref="Type"/> instance implements the specified interface.</returns>
        /// <exception cref="ArgumentException"><typeparamref name="T"/> was not a <see cref="Type"/> object describing an interface type.</exception>
        [Throws(typeof(ArgumentException))]
        public static System.Boolean ImplementsInterface<T>(this Type current) 
            where T : notnull
        {
            // T must be a complete interface type, so there is no need to apply the generic type definition case as done above.
            Type other = typeof(T);
            if (other.IsInterface == false) {
                throw new ArgumentException("The current Type object does not represent an interface.");
            } else {
                foreach (var i in current.GetInterfaces())
                {
                    if (i == other) { return true; }
                }
                return false;
            }
        }

        /// <summary>
        /// Returns a value whether the current <see cref="Type"/> matches the <paramref name="other"/> type. <br />
        /// Any generics are stripped out from the test.
        /// </summary>
        /// <param name="type">The current <see cref="Type"/> object.</param>
        /// <param name="other">The other <see cref="Type"/> object to see whether there is a match.</param>
        /// <returns>A value whether the type matches as decribed in the summary.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="other"/> is <see langword="null"/>.</exception>
        [Throws(typeof(ArgumentNullException))]
        public static System.Boolean IsTypeNonGenericMatch(this Type type, Type other)
        {
            if (other is null) {
                throw new ArgumentNullException(nameof(other));
            } else if (type.IsGenericType) {
                return type.GetGenericTypeDefinition() == other;
            } else {
                return type == other;
            }
        }

        /// <summary>
        /// Returns a value whether the current member has been decorated with the specified attribute.
        /// </summary>
        /// <param name="current">The member to test against.</param>
        /// <param name="attributetype">The attribute type to look up on the <paramref name="current"/> member.</param>
        /// <returns>A value whether the member represented by the current <see cref="ICustomAttributeProvider"/> instance has the specified attribute.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="attributetype"/> was <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="attributetype"/> does not represent a valid .NET attribute class.</exception>
        [Throws(
            typeof(ArgumentNullException),
            typeof(ArgumentException)
        )]
        public static System.Boolean HasAttribute(this ICustomAttributeProvider current, Type attributetype)
        {
            ArgumentNullException.ThrowIfNull(attributetype, nameof(attributetype));
            if (DerivesFromInternal(attributetype, typeof(Attribute)) == false) {
                throw new ArgumentException("A custom attribute must inherit from the System.Attribute class.");
            } else {
                return current.GetCustomAttributes(attributetype, false).LongLength > 0;
            }
        }

        /// <summary>
        /// Returns a value whether the current member has been decorated with the specified attribute
        /// specified in the <typeparamref name="T"/> type parameter.
        /// </summary>
        /// <param name="current">The member to test against.</param>
        /// <returns>A value whether the member represented by the current <see cref="ICustomAttributeProvider"/> instance has the specified attribute.</returns>
        public static System.Boolean HasAttribute<T>(this ICustomAttributeProvider current)
            where T : notnull, Attribute => current.GetCustomAttributes(typeof(T), false).LongLength > 0;

        /// <summary>
        /// Attempts to retrieve all the attributes defined on the current member and returns a value whether one or more attributes exist in <paramref name="attrinstances"/> parameter.
        /// </summary>
        /// <param name="current">The member information to query.</param>
        /// <param name="attributetype">The attribute type to look up on the <paramref name="current"/> member.</param>
        /// <param name="attrinstances">The attribute instances of type <paramref name="attributetype"/> if the operation was successfull.</param>
        /// <returns>A value whether one or more attribute instances of type <paramref name="attributetype"/> were written to <paramref name="attrinstances"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="attributetype"/> was <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="attributetype"/> does not represent a valid .NET attribute class.</exception>
        [Throws(
            typeof(ArgumentNullException),
            typeof(ArgumentException)
        )]
        public static System.Boolean TryGetAttributeInstances(this ICustomAttributeProvider current, Type attributetype, out System.Object[] attrinstances)
        {
            ArgumentNullException.ThrowIfNull(attributetype, nameof(attributetype));
            if (DerivesFromInternal(attributetype, typeof(Attribute)) == false) {
                throw new ArgumentException("A custom attribute must inherit from the System.Attribute class.");
            } else {
                attrinstances = current.GetCustomAttributes(attributetype, false);
                return attrinstances.LongLength > 0;
            }
        }

        /// <summary>
        /// Gets a value whether the current <see cref="Type"/> represents or derives from the <see cref="Exception"/> class.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> to test.</param>
        /// <returns><see langword="true"/> if this <see cref="Type"/> represents or derives from the <see cref="Exception"/> class; otherwise, <see langword="false"/>.</returns>
        public static System.Boolean IsException(this Type type) => type.IsClass && DerivesFromInternal(type, typeof(Exception));
    }
}