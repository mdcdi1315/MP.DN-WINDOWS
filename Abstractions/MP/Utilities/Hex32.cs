
using System;
using System.Numerics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Diagnostics.CodeAnalysis;

namespace MP.Utilities
{
    /// <summary>
    /// Defines a new numeric type that is instead specified by hexadecimal numbers.
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Pack = 4, Size = 4)]
    public readonly struct Hex32 : INumber<Hex32>, IDerivedStruct<Hex32, Int32>
    {
        /// <summary>
        /// One of the prefixes that the <see cref="Hex32"/> parser recognizes.
        /// </summary>
        public const System.String PREFIX1 = "0x";

        /// <summary>
        /// One of the prefixes that the <see cref="Hex32"/> parser recognizes.
        /// </summary>
        public const System.String PREFIX2 = "0X";

        /// <summary>
        /// One of the prefixes that the <see cref="Hex32"/> parser recognizes.
        /// </summary>
        public const System.String PREFIX3 = "0HEX";

        [FieldOffset(0)]
        private readonly int number;

        /// <inheritdoc />
        public static Hex32 One => new(1);

        /// <inheritdoc />
        public static int Radix => 16;

        /// <inheritdoc />
        public static Hex32 Zero => new(0);

        /// <inheritdoc />
        public static Hex32 AdditiveIdentity => new(1);

        /// <inheritdoc />
        public static Hex32 MultiplicativeIdentity => new(1);

        /// <summary>
        /// Initializes a new <see cref="Hex32"/> number from a raw 32-bit signed integer.
        /// </summary>
        /// <param name="raw">The raw signed integer.</param>
        public Hex32(int raw) => number = raw;

        private static int GetCharacterValue(System.Char c)
        {
            if (c.IsDigit()) {
                return c - 48;
            } else if (c > 64U & c < 71U) {
                return 10 + (c - 65);
            } else if (c > 96U & c < 103U) {
                return 10 + (c - 97);
            } else {
                throw new FormatException($"The character was not a hexadecimal character: {c}");
            }
        }

        private static System.Int32 IsValidHexPrefixInternal(System.String str)
        {
            foreach (var prefix in new System.String[] { PREFIX1, PREFIX2, PREFIX3 })
            {
                if (str.StartsWith(prefix))
                {
                    return prefix.Length;
                }
            }
            return -1;
        }

        /// <summary>
        /// Tests whether the specified string is one of the valid prefixes for the <see cref="Hex32"/> number parser.
        /// </summary>
        /// <param name="str">The string to test.</param>
        /// <returns>A value whether <paramref name="str"/> starts with one of the valid prefixes.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="str"/> was <see langword="null"/>.</exception>
        public static System.Boolean IsValidHexPrefix(System.String str)
        {
            ArgumentNullException.ThrowIfNull(str);
            return IsValidHexPrefixInternal(str) > -1;
        }

        /// <summary>
        /// Constructs a new <see cref="Hex32"/> structure from a string that is a hexadecimal number. <br />
        /// Also understands the predefined prefixes and +- signs.
        /// </summary>
        /// <param name="str">The string that contains a hexadecimal number.</param>
        /// <returns>The created hexadecimal number.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="str"/> was <see langword="null"/>.</exception>
        /// <exception cref="FormatException"><paramref name="str"/> was a malformed hexadecimal string.</exception>
        public static Hex32 FromString(System.String str)
        {
            ArgumentNullException.ThrowIfNull(str);
            if (str.Length == 0) { return new(); }
            int rp = IsValidHexPrefixInternal(str);
            if (rp == -1) { rp = 0; }
            if (str.Length == rp) { return new(); }
            int p = str.Length - rp, value = 0;
            if (str[rp] == '-' || str[rp] == '+') { rp++; p--; }
            for (int I = rp; I < str.Length; I++)
            {
                value += (int)(Math.Pow(16, --p) * GetCharacterValue(str[I]));
            }
            if (str[rp] == '-') { value = -value; }
            return new(value);
        }

        private System.String ToHexStringLowerCase()
        {
            System.Text.StringBuilder sb = new(50);
            if (number < 0) { sb.Append('-'); }
            sb.Append("0x");
            sb.Append(number.ToString("x2"));
            return sb.ToString();
        }

        private System.String ToHexStringUpperCase()
        {
            System.Text.StringBuilder sb = new(50);
            if (number < 0) { sb.Append('-'); }
            sb.Append("0x");
            sb.Append(number.ToString("X2"));
            return sb.ToString();
        }

        /// <summary>
        /// Returns the actual number representing the current <see cref="Hex32"/> structure.
        /// </summary>
        /// <returns>The number equal to the hex representation returned in <see cref="ToString()"/>.</returns>
        public System.Int32 ToInt32() => number;

        /// <summary>Gets the number repesentation as a string.</summary>
        /// <returns>The number's representation as a string.</returns>
        public readonly override string ToString() => ToHexStringLowerCase();

        /// <inheritdoc />
        public int CompareTo(object obj) => obj is Hex32 h32 ? CompareTo(h32) : -1;

        /// <inheritdoc />
        public int CompareTo(Hex32 other) => other.number.CompareTo(number);

        /// <inheritdoc />
        public static Hex32 Abs(Hex32 value) => new(Int32.Abs(value.number));

        /// <inheritdoc />
        public static bool IsCanonical(Hex32 value) => true;

        /// <inheritdoc />
        public static bool IsComplexNumber(Hex32 value) => false;

        /// <inheritdoc />
        public static bool IsEvenInteger(Hex32 value) => Int32.IsEvenInteger(value.number);

        /// <inheritdoc />
        public static bool IsFinite(Hex32 value) => false;

        /// <inheritdoc />
        public static bool IsImaginaryNumber(Hex32 value) => false;

        /// <inheritdoc />
        public static bool IsInfinity(Hex32 value) => false;

        /// <inheritdoc />
        public static bool IsInteger(Hex32 value) => true;
        
        /// <inheritdoc />
        public static bool IsNaN(Hex32 value) => false;

        /// <inheritdoc />
        public static bool IsNegative(Hex32 value) => value.number < 0;

        /// <inheritdoc />
        public static bool IsNegativeInfinity(Hex32 value) => false;

        /// <inheritdoc />
        public static bool IsNormal(Hex32 value) => false;

        /// <inheritdoc />
        public static bool IsOddInteger(Hex32 value) => Int32.IsOddInteger(value.number);

        /// <inheritdoc />
        public static bool IsPositive(Hex32 value) => Int32.IsPositive(value.number);

        /// <inheritdoc />
        public static bool IsPositiveInfinity(Hex32 value) => false;

        /// <inheritdoc />
        public static bool IsRealNumber(Hex32 value) => true;

        /// <inheritdoc />
        public static bool IsSubnormal(Hex32 value) => false;

        /// <inheritdoc />
        public static bool IsZero(Hex32 value) => value.number == 0;

        /// <inheritdoc />
        public static Hex32 MaxMagnitude(Hex32 x, Hex32 y) => new(Int32.MaxMagnitude(x.number, y.number));

        /// <inheritdoc />
        public static Hex32 MaxMagnitudeNumber(Hex32 x, Hex32 y) => new(Int32.MaxMagnitude(x.number, y.number));

        /// <inheritdoc />
        public static Hex32 MinMagnitude(Hex32 x, Hex32 y) => new(Int32.MinMagnitude(x.number, y.number));

        /// <inheritdoc />
        public static Hex32 MinMagnitudeNumber(Hex32 x, Hex32 y) => new(Int32.MinMagnitude(x.number, y.number));

        /// <inheritdoc />
        public static Hex32 Parse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider provider) => FromString(s.ToString());

        /// <inheritdoc />
        public static Hex32 Parse(string s, NumberStyles style, IFormatProvider provider) => FromString(s);

        /// <inheritdoc />
        public static bool TryConvertFromChecked<TOther>(TOther value, [MaybeNullWhen(false)] out Hex32 result) where TOther : INumberBase<TOther>
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public static bool TryConvertFromSaturating<TOther>(TOther value, [MaybeNullWhen(false)] out Hex32 result) where TOther : INumberBase<TOther>
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public static bool TryConvertFromTruncating<TOther>(TOther value, [MaybeNullWhen(false)] out Hex32 result) where TOther : INumberBase<TOther>
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public static bool TryConvertToChecked<TOther>(Hex32 value, [MaybeNullWhen(false)] out TOther result) where TOther : INumberBase<TOther>
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public static bool TryConvertToSaturating<TOther>(Hex32 value, [MaybeNullWhen(false)] out TOther result) where TOther : INumberBase<TOther>
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public static bool TryConvertToTruncating<TOther>(Hex32 value, [MaybeNullWhen(false)] out TOther result) where TOther : INumberBase<TOther>
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public static bool TryParse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider provider, [MaybeNullWhen(false)] out Hex32 result)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public static bool TryParse([NotNullWhen(true)] string s, NumberStyles style, IFormatProvider provider, [MaybeNullWhen(false)] out Hex32 result)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public bool Equals(Hex32 other) => other.number == number;

        /// <inheritdoc />
        public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider provider) => number.TryFormat(destination, out charsWritten, format, provider);

        /// <inheritdoc />
        public string ToString(string format, IFormatProvider formatProvider) => number.ToString(format, formatProvider);

        /// <inheritdoc />
        public static Hex32 Parse(ReadOnlySpan<char> s, IFormatProvider provider) => FromString(s.ToString());

        /// <inheritdoc />
        public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider provider, [MaybeNullWhen(false)] out Hex32 result)
        {
            try {
                result = FromString(s.ToString());
                return true;
            } catch (FormatException) {
                result = default;
                return false;
            }
        }

        /// <inheritdoc />
        public static Hex32 Parse(string s, IFormatProvider provider) => FromString(s);

        /// <inheritdoc />
        public static bool TryParse([NotNullWhen(true)] string s, IFormatProvider provider, [MaybeNullWhen(false)] out Hex32 result)
        {
            try {
                result = FromString(s.ToString());
                return true;
            } catch (FormatException) {
                result = default;
                return false;
            }
        }

        /// <inheritdoc />
        public static bool operator >(Hex32 left, Hex32 right) => left.number > right.number;

        /// <inheritdoc />
        public static bool operator >=(Hex32 left, Hex32 right) => left.number >= right.number;

        /// <inheritdoc />
        public static bool operator <(Hex32 left, Hex32 right) => left.number < right.number;

        /// <inheritdoc />
        public static bool operator <=(Hex32 left, Hex32 right) => left.number <= right.number;

        /// <inheritdoc />
        public static Hex32 operator %(Hex32 left, Hex32 right) => new(left.number % right.number);

        /// <inheritdoc />
        public static Hex32 operator +(Hex32 left, Hex32 right) => new(left.number + right.number);

        /// <inheritdoc />
        public static Hex32 operator --(Hex32 value) => new(value.number - 1);

        /// <inheritdoc />
        public static Hex32 operator /(Hex32 left, Hex32 right) => new(left.number / right.number);

        /// <inheritdoc />
        public static bool operator ==(Hex32 left, Hex32 right) => left.number == right.number;

        /// <inheritdoc />
        public static bool operator !=(Hex32 left, Hex32 right) => left.number != right.number;

        /// <inheritdoc />
        public static Hex32 operator ++(Hex32 value) => new(value.number + 1);

        /// <inheritdoc />
        public static Hex32 operator *(Hex32 left, Hex32 right) => new(left.number * right.number);

        /// <inheritdoc />
        public static Hex32 operator -(Hex32 left, Hex32 right) => new(left.number - right.number);

        /// <inheritdoc />
        public static Hex32 operator -(Hex32 value) => new(-value.number);

        /// <inheritdoc />
        public static Hex32 operator +(Hex32 value) => new(+value.number);

        /// <inheritdoc />
        public static implicit operator int([DisallowNull] Hex32 t) => t.number;

        /// <inheritdoc />
        public readonly override bool Equals(object obj) => obj is Hex32 h && Equals(h);

        /// <inheritdoc />
        public readonly override int GetHashCode() => number;
    }
}