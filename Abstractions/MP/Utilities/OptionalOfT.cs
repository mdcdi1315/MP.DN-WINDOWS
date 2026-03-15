
using System;
using System.Numerics;
using MP.Annotations.CodeAnalysis;
using System.Diagnostics.CodeAnalysis;

namespace MP.Utilities
{
    /// <summary>
    /// A special structure for getting optional values from classes that use this. <br />
    /// Pretty much based on the <see href="https://docs.oracle.com/en/java/javase/25/docs/api/java.base/java/util/Optional.html"/> Java class.
    /// </summary>
    /// <typeparam name="T">The type of the object to be returned by the <see cref="Get"/> method.</typeparam>
    public readonly struct Optional<T> :
        IEqualityOperators<Optional<T>, Optional<T>, bool>,
        IEquatable<Optional<T>>,
        ISyncronized,
        ICloneable
        where T : class
    {
        private const System.String ToStringMethod_EMPTY = "<EMPTY>";

        private readonly T value;

        /// <summary>
        /// Initializes an empty <see cref="Optional{T}"/> instance.
        /// </summary>
        public Optional() => value = null;

        private Optional([AllowNull] T value) => this.value = value;

        /// <summary>
        /// Provides an <see cref="Optional{T}"/> instance that is always empty.
        /// </summary>
        public static Optional<T> Empty() => new();

        /// <summary>
        /// Creates a new <see cref="Optional{T}"/> instance that the value provided can be <see langword="null"/>.
        /// </summary>
        /// <param name="value">The value to create a new <see cref="Optional{T}"/> instance from.</param>
        /// <returns>The created <see cref="Optional{T}"/> instance.</returns>
        public static Optional<T> OfNullable([AllowNull] T value) => new(value);

        /// <summary>
        /// Creates a new <see cref="Optional{T}"/> instance from the specified value. The value cannot be <see langword="null"/>.
        /// </summary>
        /// <param name="value">The value to create a new <see cref="Optional{T}"/> instance from.</param>
        /// <returns>The created <see cref="Optional{T}"/> instance.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
        [Throws(typeof(ArgumentNullException))]
        public static Optional<T> Of(T value)
        {
            ArgumentNullException.ThrowIfNull(value);
            return new(value);
        }

        /// <summary>
        /// Maps the current <see cref="Optional{T}"/> value to an <see cref="Optional{U}"/> value, if the current <see cref="Optional{T}"/> contains a value. <br />
        /// Otherwise, the method returns an <see cref="Optional{U}"/> which is empty.
        /// </summary>
        /// <typeparam name="U">The mapped type to be returned.</typeparam>
        /// <param name="converter">The function that can map a <typeparamref name="T"/> type to a <typeparamref name="U"/> type.</param>
        /// <returns>The mapped <see cref="Optional{T}"/> value.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="converter"/> is <see langword="null"/>.</exception>
        [Throws(typeof(ArgumentNullException))]
        public Optional<U> Map<U>(Func<T , U> converter)
            where U : class
        {
            ArgumentNullException.ThrowIfNull(converter);
            return (value is null) ? Optional<U>.Empty() : new(converter(value));
        }

        /// <summary>
        /// Executes the given <see cref="Predicate{T}"/> if this <see cref="Optional{T}"/> has a value, 
        /// and if the predicate succeeds, it returns the current <see cref="Optional{T}"/> value. <br />
        /// In all other cases, the empty <see cref="Optional{T}"/> is instead returned.
        /// </summary>
        /// <param name="predicate">The predicate to test against the held value described by the current <see cref="Optional{T}"/> instance.</param>
        /// <returns>The value as described in the method's summary.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="predicate"/> is <see langword="null"/>.</exception>
        [Throws(typeof(ArgumentNullException))]
        public Optional<T> FilteredBy(Predicate<T> predicate)
        {
            ArgumentNullException.ThrowIfNull(predicate);
            if (value is null) {
                return this;
            } else if (predicate(value)) {
                return this;
            } else {
                return Empty();
            }
        }

        /// <summary>
        /// Executes the specified method reference if and only if the current <see cref="Optional{T}"/> instance has a value.
        /// </summary>
        /// <param name="on_present">The <see cref="Action{T}"/> to execute, if the current <see cref="Optional{T}"/> instance has a value.</param>
        /// <exception cref="ArgumentNullException"><paramref name="on_present"/> is <see langword="null"/>.</exception>
        [Throws(typeof(ArgumentNullException))]
        public void IfPresent(Action<T> on_present)
        {
            ArgumentNullException.ThrowIfNull(on_present);
            if (value is not null) {
                on_present(value);
            }
        }

        /// <summary>
        /// Gets a value whether this <see cref="Optional{T}"/> instance is initialized.
        /// </summary>
        public readonly bool HasValue => value is not null;

        /// <summary>
        /// Gets a value whether this <see cref="Optional{T}"/> instance is empty.
        /// </summary>
        public readonly bool IsEmpty => value is null;

        /// <summary>
        /// Gets the value contained in the current <see cref="Optional{T}"/> instance. <br />
        /// If the <see cref="Optional{T}"/> is empty, <see cref="InvalidOperationException"/> is thrown.
        /// </summary>
        /// <returns>The value held by the current <see cref="Optional{T}"/> instance.</returns>
        /// <exception cref="InvalidOperationException">No value was assigned to the current <see cref="Optional{T}"/> instance.</exception>
        [Throws(typeof(InvalidOperationException))]
        public readonly T Get() => (value is null) ? throw new InvalidOperationException("The current Optional instance does not have a value.") : value;

        /// <summary>
        /// Gets the value contained in the current <see cref="Optional{T}"/> instance, or if the 
        /// current instance is empty, it returns the value specified in the <paramref name="other"/> parameter.
        /// </summary>
        /// <param name="other">The value to return if the current <see cref="Optional{T}"/> instance is empty.</param>
        /// <returns>The value as described in the method's summary.</returns>
        public readonly T GetOrElse([AllowNull] T other) => (value is null) ? other : value;

        /// <summary>
        /// Creates a new <see cref="Optional{T}"/> instance that will have the value of the current <see cref="Optional{T}"/> instance.
        /// </summary>
        /// <returns>The cloned <see cref="Optional{T}"/> instance.</returns>
        public readonly Optional<T> Clone() => new(value);

        readonly object ICloneable.Clone() => Clone();

        /// <summary>
        /// User-defined operator for simplifying the Get method call.
        /// </summary>
        /// <param name="opt">The <see cref="Optional{T}"/> to unwrap.</param>
        public static explicit operator T(Optional<T> opt) => opt.Get();

        /// <summary>
        /// Gets the hash code of the value, if present, or it returns 0.
        /// </summary>
        /// <returns>The hash code of the value if present.</returns>
        public override readonly int GetHashCode() => (value is null) ? 0 : value.GetHashCode();

        /// <summary>
        /// Returns a string reprsentation of this <see cref="Optional{T}"/> structure.
        /// </summary>
        /// <returns>A non-empty, string reprsentation of the current <see cref="Optional{T}"/> structure.</returns>
        public override readonly string ToString() => $"Optional<{typeof(T).Name}> {{ {( value is null ? ToStringMethod_EMPTY : (value.ToString() ?? ToStringMethod_EMPTY))} }}";

        /// <summary>
        /// Gets a value whether two <see cref="Optional{T}"/> instances are equal. <br />
        /// If the underlying value supports the <see cref="IEquatable{T}"/> interface, that will be used to do the equality comparison instead.
        /// </summary>
        /// <param name="other">The other <see cref="Optional{T}"/> instance to compare this instance against.</param>
        /// <returns>A value whether both <see cref="Optional{T}"/> instances do retain the same values.</returns>
        public readonly bool Equals(Optional<T> other)
        {
            if (value is null) {
                return other.value is null;
            } else if (value is IEquatable<T> eq) {
                return eq.Equals(other.value);
            } else {
                return value.Equals(other.value);
            }
        }

        /// <inheritdoc />
        public readonly override bool Equals(object obj) => obj is Optional<T> optional && Equals(optional);

        /// <summary>
        /// Gets a value whether two <see cref="Optional{T}"/> instances are equal.
        /// </summary>
        /// <param name="left">The first <see cref="Optional{T}"/> to compare.</param>
        /// <param name="right">The second <see cref="Optional{T}"/> to compare.</param>
        /// <returns>A value whether the two optional instances are equal.</returns>
        public static bool operator ==(Optional<T> left, Optional<T> right) => left.Equals(right);

        /// <summary>
        /// Gets a value whether two <see cref="Optional{T}"/> instances are inequal.
        /// </summary>
        /// <param name="left">The first <see cref="Optional{T}"/> to compare.</param>
        /// <param name="right">The second <see cref="Optional{T}"/> to compare.</param>
        /// <returns>A value whether the two optional instances are inequal.</returns>
        public static bool operator !=(Optional<T> left, Optional<T> right) => !left.Equals(right);
    }
}