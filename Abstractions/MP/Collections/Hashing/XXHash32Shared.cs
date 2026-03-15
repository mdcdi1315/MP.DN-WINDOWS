
using System.Numerics;
using System.Runtime.CompilerServices;

namespace MP.Collections.Hashing
{
    internal static class XXHash32Shared
    {
        public const System.UInt32 XXH_PRIME32_1 = 0x9E3779B1U;  /*!< 0b10011110001101110111100110110001 */
        public const System.UInt32 XXH_PRIME32_2 = 0x85EBCA77U;  /*!< 0b10000101111010111100101001110111 */
        public const System.UInt32 XXH_PRIME32_3 = 0xC2B2AE3DU;  /*!< 0b11000010101100101010111000111101 */
        public const System.UInt32 XXH_PRIME32_4 = 0x27D4EB2FU;  /*!< 0b00100111110101001110101100101111 */
        public const System.UInt32 XXH_PRIME32_5 = 0x165667B1U;  /*!< 0b00010110010101100110011110110001 */

        public static void InitializeAccumulators(out System.UInt32[] accumulators, System.UInt32 seed)
        {
            accumulators = new System.UInt32[4];
            accumulators[0] = seed + XXH_PRIME32_1 + XXH_PRIME32_2;
            accumulators[1] = seed + XXH_PRIME32_2;
            accumulators[2] = seed; // + 0;
            accumulators[3] = seed - XXH_PRIME32_1;
        }

        public static System.UInt32 Round(System.UInt32 acc, System.UInt32 input)
        {
            acc += input * XXH_PRIME32_2;
            acc = BitOperations.RotateLeft(acc, 13);
            acc *= XXH_PRIME32_1;
            return acc;
        }

        public static System.UInt32 Avalanche(System.UInt32 hash)
        {
            hash ^= hash >> 15;
            hash *= XXH_PRIME32_2;
            hash ^= hash >> 13;
            hash *= XXH_PRIME32_3;
            hash ^= hash >> 16;
            return hash;
        }

        public static System.UInt32 Merge(System.UInt32[] accumulators) =>
            BitOperations.RotateLeft(accumulators[0], 1) +
            BitOperations.RotateLeft(accumulators[1], 7) +
            BitOperations.RotateLeft(accumulators[2], 12) +
            BitOperations.RotateLeft(accumulators[3], 18);

        public static ref System.Byte Consume(System.UInt32[] accumulators, ref System.Byte input, int length)
        {
            if (length >= 16)
            {
                ref System.Byte limit = ref Unsafe.Subtract(ref Unsafe.Add(ref input, length), 15);
                do {
                    accumulators[0] = Round(accumulators[0], Unsafe.As<System.Byte, System.UInt32>(ref input));
                    input = ref Unsafe.Add(ref input, sizeof(System.UInt32));
                    accumulators[1] = Round(accumulators[1], Unsafe.As<System.Byte, System.UInt32>(ref input));
                    input = ref Unsafe.Add(ref input, sizeof(System.UInt32));
                    accumulators[2] = Round(accumulators[2], Unsafe.As<System.Byte, System.UInt32>(ref input));
                    input = ref Unsafe.Add(ref input, sizeof(System.UInt32));
                    accumulators[3] = Round(accumulators[3], Unsafe.As<System.Byte, System.UInt32>(ref input));
                    input = ref Unsafe.Add(ref input, sizeof(System.UInt32));
                } while (Unsafe.IsAddressLessThan(ref input, ref limit));
            }

            return ref input;
        }

        public static System.UInt32 ConsumeLessThan8(System.UInt32 hash, ref System.Byte input, int length)
        {
            length &= 15;
            while (length >= 4) {
                hash += Unsafe.As<System.Byte, System.UInt32>(ref input) * XXH_PRIME32_3;
                input = ref Unsafe.Add(ref input, sizeof(System.UInt32));
                hash = BitOperations.RotateLeft(hash, 17) * XXH_PRIME32_4;
                length -= 4;
            }
            while (length > 0) {
                hash += input * XXH_PRIME32_5;
                input = ref Unsafe.Add(ref input, 1);
                hash = BitOperations.RotateLeft(hash, 11) * XXH_PRIME32_1;
                length--;
            }
            return hash;
        }

        public static System.UInt32 Finalize(System.UInt32 hash, ref System.Byte input, int length) => Avalanche(ConsumeLessThan8(hash, ref input, length));
    }
}