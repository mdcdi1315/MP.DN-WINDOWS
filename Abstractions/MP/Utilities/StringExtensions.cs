
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace MP.Utilities
{
    /// <summary>
    /// Defines extensions around the <see cref="System.String"/> class.
    /// </summary>
    public static class StringExtensions
    {
        private const System.Int32 LARGEST_NEG_TOSTRING_INT16 = 6;
        private const System.Int32 LARGEST_NEG_TOSTRING_INT32 = 11;
        private const System.Int32 LARGEST_NEG_TOSTRING_INT64 = 20;

        private const System.Int32 LARGEST_POS_TOSTRING_INT16 = 5;
        private const System.Int32 LARGEST_POS_TOSTRING_INT32 = 10;
        private const System.Int32 LARGEST_POS_TOSTRING_INT64 = 19;

        [Flags]
        private enum NumberTypeFlags : System.Byte
        {
            None = 0,
            Int16 = 1,
            Int32 = 2,
            Int64 = 4,
            Unsigned = 64,
            Negative = 128
        }

        [System.Diagnostics.StackTraceHidden]
        private sealed class ToNumberParsingContext
        {
            private System.Int32 sidx;
            private System.Char[] strret;
            private NumberTypeFlags flags;

            public ToNumberParsingContext(System.String str , System.Boolean unsigned = false)
            {
                strret = str.ToCharArray();
                sidx = 0;
                flags = unsigned ? NumberTypeFlags.Unsigned : NumberTypeFlags.None;
                CreateContext();
            }

            // Retrieve the data with a non-optimized way.
            private void CreateContext()
            {
                if (strret.Length == 0) {
                    throw new FormatException("This string is too small to represent any valid number.");
                }
                flags |= (strret[sidx] == '-') ? NumberTypeFlags.Negative : NumberTypeFlags.None;
                if (HasFlagFast(NumberTypeFlags.Negative)) { 
                    if (HasFlagFast(NumberTypeFlags.Unsigned)) {
                        throw new FormatException("Negative numbers cannot be parsed in unsigned contexts.");
                    }
                    if (strret.Length == 1) {
                        throw new FormatException("This string is too small to represent any valid number.");
                    }
                    sidx++; 
                }
                for (System.Int32 I = sidx; I < strret.Length; I++)
                {
                    if (IsDigit(strret[I]) == false) {
                        throw new FormatException("This string does not represent a valid numerical sequence.");
                    }
                }
                if (HasFlagFast(NumberTypeFlags.Negative)) {
                    flags |= strret.Length switch {
                        <= LARGEST_NEG_TOSTRING_INT16 => NumberTypeFlags.Int16,
                        <= LARGEST_NEG_TOSTRING_INT32 => NumberTypeFlags.Int32,
                        <= LARGEST_NEG_TOSTRING_INT64 => NumberTypeFlags.Int64,
                        _ => throw new FormatException("The number given is too long."),
                    };
                } else {
                    flags |= strret.Length switch {
                        <= LARGEST_POS_TOSTRING_INT16 => NumberTypeFlags.Int16,
                        <= LARGEST_POS_TOSTRING_INT32 => NumberTypeFlags.Int32,
                        <= LARGEST_POS_TOSTRING_INT64 => NumberTypeFlags.Int64,
                        _ => throw new FormatException("The number given is too long."),
                    };
                }
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private System.Boolean HasFlagFast(NumberTypeFlags flgsexpected) => (flags & flgsexpected) == flgsexpected;

            [System.Diagnostics.StackTraceHidden]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void ThrowOnOverflow(System.Boolean sign)
            {
                System.Boolean NEG = HasFlagFast(NumberTypeFlags.Negative);
                // sign parameter means the number sign:
                // If false, it is negative.
                // If true, it is zero or positive.
                if ((sign && NEG) || (sign == false && NEG == false)) {
                    throw new OverflowException("The arithmetic parse operation was overflown.");
                }
            }

            public System.Char[] Data => strret;

            public System.Int32 StartIndex => sidx;

            public System.Boolean IsNegative => HasFlagFast(NumberTypeFlags.Negative);

            public System.Boolean IsUnsigned => HasFlagFast(NumberTypeFlags.Unsigned);

            public System.Boolean IsInt16 => HasFlagFast(NumberTypeFlags.Int16);

            public System.Boolean IsInt32 => HasFlagFast(NumberTypeFlags.Int32) | HasFlagFast(NumberTypeFlags.Int16);

            public System.Boolean IsInt64 => HasFlagFast(NumberTypeFlags.Int64) | HasFlagFast(NumberTypeFlags.Int32) | HasFlagFast(NumberTypeFlags.Int16);
        }

        /// <summary>
        /// Returns a value whether the given string is <see langword="null"/> or consists only of white-space characters.
        /// </summary>
        /// <param name="str">The string to test.</param>
        /// <returns>A value whether the provided string is <see langword="null"/> or consists only of white-space characters.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.Boolean IsNullOrWhiteSpace(System.String str)
        {
            if (str is null) { return true; }
            if (str.Length == 0) { return true; }
            // Originally presume that the string is consisting only of white-space characters.
            System.Boolean ret = true;
            // Use managed pointers for faster access to the string data
            ref System.Char c = ref Unsafe.AsRef(str.GetPinnableReference());
            ref System.Char ct = ref Unsafe.Add(ref c , str.Length);
            do
            {
                if (IsWhiteSpace(c) == false)
                {
                    // This is not a white-space character, break the loop and return false.
                    ret = false;
                    break;
                }
                c = ref Unsafe.Add(ref c, 1);
            } while (Unsafe.IsAddressLessThan(ref c, ref ct));
            return ret;
        }

        /// <summary>
        /// Returns a value whether the specified character is a digit.
        /// </summary>
        /// <param name="c">The character to test.</param>
        /// <returns>A value whether the given character is into the range of numerical values (from 0 to 9).</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.Boolean IsDigit(this System.Char c) => (c - 48U) < 10U;

        /// <summary>
        /// Returns a value whether the specified character is a valid hexadecimal character with uppercase letters.
        /// </summary>
        /// <param name="c">The character to test.</param>
        /// <returns>A value whether the given character is a valid hexadecimal character.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        // Older and non valid formula was: IsDigit(c) || (c - 65U) < 71U;
        public static System.Boolean IsHexadecimalUpper(this System.Char c) => IsDigit(c) || (c > 64U & c < 71U);

        /// <summary>
        /// Returns a value whether the specified character is a valid hexadecimal character with lowercase letters.
        /// </summary>
        /// <param name="c">The character to test.</param>
        /// <returns>A value whether the given character is a valid hexadecimal character.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        // Older and non valid formula was: IsDigit(c) || (c - 97U) < 103U;
        public static System.Boolean IsHexadecimalLower(this System.Char c) => IsDigit(c) || (c > 98U & c < 104U);

        /// <summary>
        /// Returns a value whether the specified character is a valid hexadecimal character.
        /// </summary>
        /// <param name="c">The character to test.</param>
        /// <returns>A value whether the given character is a valid hexadecimal character.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        // Older and non valid formula was:  IsDigit(c) || (c - 65U) < 71U || (c - 97U) < 103U;
        public static System.Boolean IsHexadecimal(this System.Char c) => IsDigit(c) || (c > 64U & c < 98U) || (c > 98U & c < 104U);

        /// <summary>
        /// Returns a value whether the specified character is a white-space character.
        /// </summary>
        /// <param name="c">The character to test.</param>
        /// <returns>A value whether the given character is a valid white-space character.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.Boolean IsWhiteSpace(this System.Char c) => c == 32 || (c - 9U) < 14U;

        /// <summary>
        /// Returns a value whether the specified character is one of the 26 characters of the English alphabet.
        /// </summary>
        /// <param name="c">The character to test.</param>
        /// <returns>A value whether the given character is one of the 26 English alphabet characters.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        // Older and non valid formula was: ((c - 65U) < 91U) || ((c - 97U) < 122U);
        public static System.Boolean IsEnglishCharacter(this System.Char c) => (c > 64U & c < 92U) || (c > 96U & c < 123U);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static System.Int16 ToInt16OPT(ToNumberParsingContext pc)
        {
            System.Char[] dt = pc.Data;
            System.Int32 idx = pc.StartIndex;
            System.Int16 result = 0, shift = 1;
            for (System.Int32 I = dt.Length - 1; I >= idx; I--)
            {
                result += ((dt[I] - 48) * shift).ToInt16();
                shift *= 10;
            }
            if (pc.IsNegative) { result *= -1; }
            pc.ThrowOnOverflow(result >= 0);
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static System.UInt16 ToUInt16OPT(ToNumberParsingContext pc)
        {
            System.Char[] dt = pc.Data;
            System.Int32 idx = pc.StartIndex;
            System.UInt16 result = 0, shift = 1;
            for (System.Int32 I = dt.Length - 1; I >= idx; I--)
            {
                result += ((dt[I] - 48) * shift).ToUInt16();
                shift *= 10;
            }
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static System.Int32 ToInt32OPT(ToNumberParsingContext pc)
        {
            System.Char[] dt = pc.Data;
            System.Int32 result = 0, shift = 1, idx = pc.StartIndex;
            for (System.Int32 I = dt.Length - 1; I >= idx; I--)
            {
                result += (dt[I] - 48) * shift;
                shift *= 10;
            }
            dt = null;
            if (pc.IsNegative) { result = -result; }
            pc.ThrowOnOverflow(result >= 0);
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static System.UInt32 ToUInt32OPT(ToNumberParsingContext pc)
        {
            System.Char[] dt = pc.Data;
            System.Int32 idx = pc.StartIndex;
            System.UInt32 result = 0, shift = 1;
            for (System.Int32 I = dt.Length - 1; I >= idx; I--)
            {
                result += (dt[I] - 48U) * shift;
                shift *= 10;
            }
            dt = null;
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static System.Int64 ToInt64OPT(ToNumberParsingContext pc)
        {
            System.Char[] dt = pc.Data;
            System.Int32 idx = pc.StartIndex;
            System.Int64 result = 0, shift = 1;
            for (System.Int32 I = dt.Length - 1; I >= idx; I--)
            {
                result += (dt[I] - 48L) * shift;
                shift *= 10L;
            }
            if (pc.IsNegative) { result = -result; }
            pc.ThrowOnOverflow(result >= 0);
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static System.UInt64 ToUInt64OPT(ToNumberParsingContext pc)
        {
            System.Char[] dt = pc.Data;
            System.Int32 idx = pc.StartIndex;
            System.UInt64 result = 0, shift = 1;
            for (System.Int32 I = dt.Length - 1; I >= idx; I--)
            {
                result += (dt[I] - 48UL) * shift;
                shift *= 10UL;
            }
            return result;
        }

        /// <summary>
        /// Retrieves an <see cref="System.Int16"/> from this string.
        /// </summary>
        /// <param name="number">The string to retrieve as an <see cref="System.Int16"/>.</param>
        /// <returns>The parsed <see cref="System.Int16"/>.</returns>
        /// <exception cref="FormatException">The string is malformed.</exception>
        public static System.Int16 ToInt16(this System.String number)
        {
            ToNumberParsingContext pc = new(number);
            if (pc.IsInt16 == false) {
                throw new FormatException("This value is too large so that it can be parsed into a Int16.");
            }
            return ToInt16OPT(pc);
        }

        /// <summary>
        /// Retrieves an <see cref="System.UInt16"/> from this string.
        /// </summary>
        /// <param name="number">The string to retrieve as an <see cref="System.UInt16"/>.</param>
        /// <returns>The parsed <see cref="System.Int16"/>.</returns>
        /// <exception cref="FormatException">The string is malformed.</exception>
        public static System.UInt16 ToUInt16(this System.String number)
        {
            ToNumberParsingContext pc = new(number , true);
            if (pc.IsInt16 == false)
            {
                throw new FormatException("This value is too large so that it can be parsed into an UInt16.");
            }
            return ToUInt16OPT(pc);
        }

        /// <summary>
        /// Retrieves an <see cref="System.Int32"/> from this string.
        /// </summary>
        /// <param name="number">The string to retrieve as an <see cref="System.Int32"/>.</param>
        /// <returns>The parsed <see cref="System.Int32"/>.</returns>
        /// <exception cref="FormatException">The string is malformed.</exception>
        public static System.Int32 ToInt32(this System.String number)
        {
            ToNumberParsingContext pc = new(number);
            if (pc.IsInt32 == false) {
                throw new FormatException("This value is too large so that it can be parsed into a Int32.");
            }
            return ToInt32OPT(pc);
        }

        /// <summary>
        /// Retrieves an <see cref="System.UInt32"/> from this string.
        /// </summary>
        /// <param name="number">The string to retrieve as an <see cref="System.UInt32"/>.</param>
        /// <returns>The parsed <see cref="System.UInt32"/>.</returns>
        /// <exception cref="FormatException">The string is malformed.</exception>
        public static System.UInt32 ToUInt32(this System.String number)
        {
            ToNumberParsingContext pc = new(number , true);
            if (pc.IsInt32 == false) {
                throw new FormatException("This value is too large so that it can be parsed into an UInt32.");
            }
            return ToUInt32OPT(pc);
        }

        /// <summary>
        /// Retrieves an <see cref="System.Int64"/> from this string.
        /// </summary>
        /// <param name="number">The string to retrieve as an <see cref="System.Int64"/>.</param>
        /// <returns>The parsed <see cref="System.Int64"/>.</returns>
        /// <exception cref="FormatException">The string is malformed.</exception>
        public static System.Int64 ToInt64(this System.String number)
        {
            ToNumberParsingContext pc = new(number);
            if (pc.IsInt64 == false) {
                throw new FormatException("This value is too large so that it can be parsed into a Int64.");
            }
            return ToInt64OPT(pc);
        }

        /// <summary>
        /// Retrieves an <see cref="System.Int64"/> from this string.
        /// </summary>
        /// <param name="number">The string to retrieve as an <see cref="System.Int64"/>.</param>
        /// <returns>The parsed <see cref="System.Int64"/>.</returns>
        /// <exception cref="FormatException">The string is malformed.</exception>
        public static System.UInt64 ToUInt64(this System.String number)
        {
            ToNumberParsingContext pc = new(number);
            if (pc.IsInt64 == false) {
                throw new FormatException("This value is too large so that it can be parsed into an UInt64.");
            }
            return ToUInt64OPT(pc);
        }

        /// <summary>
        /// Finds the number of times that a specified character is found in the string.
        /// </summary>
        /// <param name="subject">The string to scan.</param>
        /// <param name="character">The character to find it's occurence.</param>
        /// <returns>The number of occurences of <paramref name="character"/>. Can also be 0 , indicating that the character was not found.</returns>
        public static System.Int32 FindCharOccurence(this System.String subject, System.Char character)
        {
            System.Int32 result = 0;
            for (System.Int32 I = 0; I < subject.Length; I++) { if (subject[I] == character) { result++; } }
            return result;
        }

        /// <summary>
        /// Finds all character occurences for the specified characters in the current string.
        /// </summary>
        /// <param name="subject">The string to find the character occurences for.</param>
        /// <param name="characters">The characters to find their occurence in <paramref name="subject"/>.</param>
        /// <returns>An array containing the character occurences from <paramref name="characters"/> and <paramref name="subject"/>.</returns>
        public static CharOccurence[] FindCharOccurence(this System.String subject , params System.Char[] characters)
        {
            List<CharOccurence> col = new(10);
            System.Char cc;
            for (System.Int32 I = 0; I < subject.Length; I++)
            {
                cc = subject[I]; // Load the character only once
                foreach (var c in characters)
                {
                    if (cc == c) {
                        col.Add(new(c, I));
                        break; // We can break the inner loop since no other character will ever match at this string position.
                    }
                }
            }
            return col.ToArray();
        }

        /// <summary>
        /// Tests whether the current file path string contains a file extension provided from the <paramref name="allowedext"/> string.
        /// </summary>
        /// <param name="path">The path to test against.</param>
        /// <param name="allowedext">A list, delimited by ';' characters , to test file extensions.</param>
        /// <returns>A value whether at least one element from <paramref name="allowedext"/> list has matched the extension situated in <paramref name="path"/>.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="path"/> and/or <paramref name="allowedext"/> were <see langword="null"/>.</exception>
        /// <exception cref="System.ArgumentException"><paramref name="path"/> does not have at least one dot , that indicates the file extension.</exception>
        public static System.Boolean ContainsFileExtension(this System.String path, System.String allowedext)
        {
            if (path is null) { throw new System.ArgumentNullException(nameof(path)); }
            if (allowedext is null) { throw new System.ArgumentNullException(nameof(allowedext)); }
            System.Int32 idx = path.LastIndexOf('.');
            if (idx == -1)
            {
                throw new System.ArgumentException("The path is expected to have a dot that indicates the file extension.");
            }
            System.Boolean cond;
            foreach (System.String d in allowedext.Split(';'))
            {
                if (d.Length == 0) { continue; }
                if (d[0] == '*') {
                    cond = d.Substring(1) == path.Substring(idx + 1);
                } else {
                    cond = d == path.Substring(idx + 1);
                }
                if (cond) { return true; }
            }
            return false;
        }

    }
}