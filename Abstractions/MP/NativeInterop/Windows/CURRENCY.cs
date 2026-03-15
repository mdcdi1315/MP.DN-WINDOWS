
using System;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Diagnostics.CodeAnalysis;

namespace MP.NativeInterop.Windows
{ 
    /// <summary>
    /// A simplified version of the CY structure for Ole Automation, plus some operations for creating and getting currencies from a PROPVARIANT. <br />
    /// A <see cref="CURRENCY"/> structure is used to save economical values. <br />
    /// Unlike other floating-point implementations, the structure allows to save only up to 4 decimal digits, and any numeric operations
    /// happening on them with more than 4 digits rounds those to the closest fraction part with 4 decimal digits. <br />
    /// It provides specialized operators for working with currencies, plus performance operators for working also with doubles altogether. <br />
    /// Among these, it also supports everything related to comparing and manipulating the <see cref="CURRENCY"/> value and has a specialized method for retrieveing an absolute <see cref="CURRENCY"/> value.
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Pack = 4, Size = 8)]
    public readonly struct CURRENCY : 
        ICloneable,
        IFormattable,
        IParsable<CURRENCY>,
        IEquatable<CURRENCY>,
        IComparable<CURRENCY>,
        IMinMaxValue<CURRENCY>,
        IEquatable<System.Int32>,
        IEquatable<System.Int64>,
        IEquatable<System.Single>,
        ISpanParsable<CURRENCY>,
        IEquatable<System.Double>,
        ITruncatable<System.Int64>,
        IIncrementOperators<CURRENCY>,
        IDecrementOperators<CURRENCY>,
        IUnaryPlusOperators<CURRENCY, CURRENCY>,
        IUnaryNegationOperators<CURRENCY, CURRENCY>,
        IAdditionOperators<CURRENCY , CURRENCY , CURRENCY>,
        IAdditionOperators<CURRENCY, System.Int32, CURRENCY>,
        IAdditionOperators<CURRENCY, System.Int64, CURRENCY>,
        ISubtractionOperators<CURRENCY, System.Int32, CURRENCY>,
        ISubtractionOperators<CURRENCY, System.Int64, CURRENCY>,
        ISubtractionOperators<CURRENCY, CURRENCY, CURRENCY>,
        IMultiplyOperators<CURRENCY, CURRENCY, CURRENCY>,
        IMultiplyOperators<CURRENCY, System.Int32, CURRENCY>,
        IMultiplyOperators<CURRENCY, System.Int64, CURRENCY>,
        IDivisionOperators<CURRENCY, CURRENCY, CURRENCY>,
        IDivisionOperators<CURRENCY, System.Int32, CURRENCY>,
        IDivisionOperators<CURRENCY, System.Int64, CURRENCY>,
        IModulusOperators<CURRENCY, CURRENCY, CURRENCY>,
        IModulusOperators<CURRENCY, System.Int32, CURRENCY>,
        IModulusOperators<CURRENCY, System.Int64, CURRENCY>,
        IComparisonOperators<CURRENCY, CURRENCY, System.Boolean>,
        IComparisonOperators<CURRENCY, System.Single, System.Boolean>,
        IComparisonOperators<CURRENCY, System.Double, System.Boolean>
    {
        [FieldOffset(0)]
        private readonly System.Int64 Raw;

        private CURRENCY(System.Int64 raw_data) => Raw = raw_data;

        /// <inheritdoc />
        public static CURRENCY MinValue => new(-9223372036854775625); // To continue supporting the 4 fractional digits.

        /// <inheritdoc />
        public static CURRENCY MaxValue => new(9223372036854775625); // To continue supporting the 4 fractional digits.

        /// <summary>
        /// Creates a new <see cref="CURRENCY"/> value from a <see cref="System.Double"/> value.
        /// </summary>
        /// <param name="dbl">The <see cref="System.Double"/> value to create the <see cref="CURRENCY"/> from.</param>
        /// <returns>The constructed <see cref="CURRENCY"/> for <paramref name="dbl"/>.</returns>
        public static CURRENCY FromDouble(System.Double dbl) => new((long)(dbl * 10000d));

        /// <summary>
        /// Creates a new <see cref="CURRENCY"/> value from a <see cref="System.Single"/> value.
        /// </summary>
        /// <param name="flt">The <see cref="System.Single"/> value to create the <see cref="CURRENCY"/> from.</param>
        /// <returns>The constructed <see cref="CURRENCY"/> for <paramref name="flt"/>.</returns>
        public static CURRENCY FromSingle(System.Single flt) => new((long)(flt * 10000f));

        /// <summary>Computes the absolute value of <paramref name="current"/>.</summary>
        /// <param name="current">The value to compute it's absolute value.</param>
        /// <returns>The <see cref="CURRENCY"/> representing the absolute value of <paramref name="current"/>.</returns>
        public static CURRENCY Abs(CURRENCY current) => new(Math.Abs(current.Raw));

        /// <inheritdoc />
        public static CURRENCY Parse(string s, IFormatProvider provider) => new((long)(System.Double.Parse(s, System.Globalization.NumberStyles.AllowCurrencySymbol | System.Globalization.NumberStyles.Currency, provider) * 10000d));

        /// <inheritdoc />
        public static CURRENCY Parse(ReadOnlySpan<char> s, IFormatProvider provider) => new((long)(System.Double.Parse(s, System.Globalization.NumberStyles.AllowCurrencySymbol | System.Globalization.NumberStyles.Currency, provider) * 10000d));

        /// <inheritdoc />
        public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider provider, [MaybeNullWhen(false)] out CURRENCY result)
        {
            bool p = System.Double.TryParse(s, System.Globalization.NumberStyles.AllowCurrencySymbol | System.Globalization.NumberStyles.Currency, provider, out double d);
            if (p) {
                result = new((long)(d * 10000d));
                return true;
            } else {
                result = default;
                return false;
            }
        }

        /// <inheritdoc />
        public static bool TryParse([NotNullWhen(true)] string s, IFormatProvider provider, [MaybeNullWhen(false)] out CURRENCY result)
        {
            bool p = System.Double.TryParse(s, System.Globalization.NumberStyles.AllowCurrencySymbol | System.Globalization.NumberStyles.Currency, provider, out double d);
            if (p) {
                result = new((long)(d * 10000d));
                return true;
            } else {
                result = default;
                return false;
            }
        }

        /// <summary>Implicitly converts a <see cref="System.Double"/> value to a <see cref="CURRENCY"/> value.</summary>
        /// <param name="dbl">The <see cref="System.Double"/> to be converted into a <see cref="CURRENCY"/> value.</param>
        public static implicit operator CURRENCY(System.Double dbl) => FromDouble(dbl);

        /// <summary>Implicitly converts a <see cref="System.Single"/> value to a <see cref="CURRENCY"/> value.</summary>
        /// <param name="flt">The <see cref="System.Single"/> to be converted into a <see cref="CURRENCY"/> value.</param>
        public static implicit operator CURRENCY(System.Single flt) => FromSingle(flt);

        /// <summary>Explicitly converts a <see cref="CURRENCY"/> to a <see cref="System.Double"/> value.</summary>
        /// <param name="c">The <see cref="CURRENCY"/> to be converted into a <see cref="System.Double"/> value.</param>
        public static explicit operator System.Double(CURRENCY c) => c.Raw / 10000d;

        /// <summary>Explicitly converts a <see cref="CURRENCY"/> to a <see cref="System.Single"/> value.</summary>
        /// <param name="c">The <see cref="CURRENCY"/> to be converted into a <see cref="System.Single"/> value.</param>
        public static explicit operator System.Single(CURRENCY c) => c.Raw / 10000f;

        /// <inheritdoc />
        public static System.Boolean operator ==(CURRENCY left, CURRENCY right) => left.Raw == right.Raw;

        /// <inheritdoc />
        public static System.Boolean operator ==(CURRENCY left, System.Single right) => left.Raw == (long)(right * 10000f);

        /// <inheritdoc />
        public static System.Boolean operator ==(CURRENCY left, System.Double right) => left.Raw == (long)(right * 10000d);

        /// <inheritdoc />
        public static System.Boolean operator !=(CURRENCY left, CURRENCY right) => left.Raw != right.Raw;

        /// <inheritdoc />
        public static System.Boolean operator !=(CURRENCY left, System.Single right) => left.Raw != (long)(right * 10000f);

        /// <inheritdoc />
        public static System.Boolean operator !=(CURRENCY left, System.Double right) => left.Raw != (long)(right * 10000d);

        /// <inheritdoc />
        public static System.Boolean operator <(CURRENCY left, CURRENCY right) => left.Raw < right.Raw;

        /// <inheritdoc />
        public static System.Boolean operator <(CURRENCY left, System.Double right) => left.Raw < (long)(right * 10000d);

        /// <inheritdoc />
        public static System.Boolean operator <(CURRENCY left, System.Single right) => left.Raw < (long)(right * 10000f);

        /// <inheritdoc />
        public static System.Boolean operator >(CURRENCY left, CURRENCY right) => left.Raw > right.Raw;

        /// <inheritdoc />
        public static System.Boolean operator >(CURRENCY left, System.Single right) => left.Raw > (long)(right * 10000f);

        /// <inheritdoc />
        public static System.Boolean operator >(CURRENCY left, System.Double right) => left.Raw > (long)(right * 10000d);

        /// <inheritdoc />
        public static System.Boolean operator >=(CURRENCY left, CURRENCY right) => left.Raw >= right.Raw;

        /// <inheritdoc />
        public static System.Boolean operator >=(CURRENCY left, System.Single right) => left.Raw >= (long)(right * 10000f);

        /// <inheritdoc />
        public static System.Boolean operator >=(CURRENCY left, System.Double right) => left.Raw >= (long)(right * 10000d);

        /// <inheritdoc />
        public static System.Boolean operator <=(CURRENCY left, CURRENCY right) => left.Raw <= right.Raw;

        /// <inheritdoc />
        public static System.Boolean operator <=(CURRENCY left, System.Single right) => left.Raw <= (long)(right * 10000f);

        /// <inheritdoc />
        public static System.Boolean operator <=(CURRENCY left, System.Double right) => left.Raw <= (long)(right * 10000d);

        /// <inheritdoc />
        public static CURRENCY operator -(CURRENCY value) => new(-value.Raw);

        /// <inheritdoc />
        public static CURRENCY operator --(CURRENCY value) => new(value.Raw - 10000L);

        /// <inheritdoc />
        public static CURRENCY operator +(CURRENCY value) => new(+value.Raw);

        /// <inheritdoc />
        public static CURRENCY operator ++(CURRENCY value) => new(value.Raw + 10000L);

        /// <inheritdoc />
        public static CURRENCY operator -(CURRENCY left, CURRENCY right) => new(left.Raw - right.Raw);

        /// <inheritdoc />
        public static CURRENCY operator -(CURRENCY left, System.Int32 right) => new(left.Raw - (right * 10000));

        /// <inheritdoc />
        public static CURRENCY operator -(CURRENCY left, System.Int64 right) => new(left.Raw - (right * 10000L));

        /// <inheritdoc />
        public static CURRENCY operator +(CURRENCY left, CURRENCY right) => new(left.Raw + right.Raw);

        /// <inheritdoc />
        public static CURRENCY operator +(CURRENCY left, System.Int32 right) => new(left.Raw + (right * 10000));

        /// <inheritdoc />
        public static CURRENCY operator +(CURRENCY left, System.Int64 right) => new(left.Raw + (right * 10000L));

        /// <inheritdoc />
        public static CURRENCY operator *(CURRENCY left, CURRENCY right) => new(left.Raw * right.Raw);

        /// <inheritdoc />
        public static CURRENCY operator *(CURRENCY left, System.Int32 right) => new(left.Raw * right * 10000);

        /// <inheritdoc />
        public static CURRENCY operator *(CURRENCY left, System.Int64 right) => new(left.Raw * right * 10000L);

        /// <inheritdoc />
        public static CURRENCY operator /(CURRENCY left, CURRENCY right) => new(left.Raw / right.Raw);

        /// <inheritdoc />
        public static CURRENCY operator /(CURRENCY left, System.Int32 right) => new(left.Raw / (right * 10000));

        /// <inheritdoc />
        public static CURRENCY operator /(CURRENCY left, System.Int64 right) => new(left.Raw / (right * 10000L));

        /// <inheritdoc />
        public static CURRENCY operator %(CURRENCY left, CURRENCY right) => new(left.Raw % right.Raw);

        /// <inheritdoc />
        public static CURRENCY operator %(CURRENCY left, System.Int32 right) => new(left.Raw % (right * 10000));

        /// <inheritdoc />
        public static CURRENCY operator %(CURRENCY left, System.Int64 right) => new(left.Raw % (right * 10000L));

        /// <inheritdoc cref="ICloneable.Clone"/>
        public readonly CURRENCY Clone() => new(Raw);

        /// <inheritdoc />
        public readonly System.Int64 Truncate() => Raw / 10000;

        /// <inheritdoc />
        public readonly System.Int32 CompareTo(CURRENCY other) => Raw.CompareTo(other.Raw);

        /// <inheritdoc />
        public readonly System.Boolean Equals(CURRENCY other) => other.Raw == Raw;

        /// <summary>
        /// Indicates whether the current object is equal to another object of type <see cref="System.Int32"/>.
        /// </summary>
        /// <param name="other">An object of type <see cref="System.Int32"/> to compare with this object.</param>
        /// <returns><see langword="true"/> if the current object is equal to the <paramref name="other"/> parameter; otherwise, <see langword="false"/>.</returns>
        public readonly System.Boolean Equals(System.Int32 other) => Raw == (other * 10000);

        /// <summary>
        /// Indicates whether the current object is equal to another object of type <see cref="System.Int64"/>.
        /// </summary>
        /// <param name="other">An object of type <see cref="System.Int64"/> to compare with this object.</param>
        /// <returns><see langword="true"/> if the current object is equal to the <paramref name="other"/> parameter; otherwise, <see langword="false"/>.</returns>
        public readonly System.Boolean Equals(System.Int64 other) => Raw == (other * 10000L);

        /// <summary>
        /// Indicates whether the current object is equal to another object of type <see cref="System.Single"/>.
        /// </summary>
        /// <param name="other">An object of type <see cref="System.Single"/> to compare with this object.</param>
        /// <returns><see langword="true"/> if the current object is equal to the <paramref name="other"/> parameter; otherwise, <see langword="false"/>.</returns>
        public readonly System.Boolean Equals(System.Single other) => Raw == (long)(other * 10000f);

        /// <summary>
        /// Indicates whether the current object is equal to another object of type <see cref="System.Double"/>.
        /// </summary>
        /// <param name="other">An object of type <see cref="System.Double"/> to compare with this object.</param>
        /// <returns><see langword="true"/> if the current object is equal to the <paramref name="other"/> parameter; otherwise, <see langword="false"/>.</returns>
        public readonly System.Boolean Equals(System.Double other) => Raw == (long)(other * 10000d);

        /// <inheritdoc />
        public readonly override System.Boolean Equals([NotNullWhen(true)] object obj) => obj switch {
            System.Single flt => Equals(flt),
            System.Int32 i32 => Equals(i32),
            System.Int64 i64 => Equals(i64),
            System.Double dbl => Equals(dbl),
            CURRENCY other => Equals(other),
            _ => false,
        };

        /// <inheritdoc />
        public readonly override System.Int32 GetHashCode() => Raw.GetHashCode();

        /// <summary>Returns a string representing the held value of the current <see cref="CURRENCY"/> instance.</summary>
        /// <returns>The value that the current <see cref="CURRENCY"/> holds.</returns>
        public readonly override System.String ToString() => (Raw / 10000d).ToString();

        /// <inheritdoc />
        public readonly System.String ToString(System.String format, IFormatProvider formatProvider) => (Raw / 10000d).ToString(format, formatProvider);

        readonly object ICloneable.Clone() => Clone();
    }
}
