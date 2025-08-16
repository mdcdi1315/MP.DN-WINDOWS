

using System;
using System.Runtime.InteropServices;

namespace MP.Utilities
{
    /// <summary>
    /// Defines a new numeric type that is instead specified by hexadecimal numbers.
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Pack = 4, Size = 4)]
    public readonly struct Hex32
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
            sb.Append(number.ToString("X2"));
            return sb.ToString();
        }

        /// <summary>
        /// Returns the actual number representing the current <see cref="Hex32"/> structure.
        /// </summary>
        /// <returns>The number equal to the hex representation returned in <see cref="ToString"/>.</returns>
        public System.Int32 ToInt32() => number;

        /// <summary>
        /// Gets the number repesentation as a string.
        /// </summary>
        /// <returns>The number's representation as a string.</returns>
        public override string ToString() => ToHexStringLowerCase();
    }
}