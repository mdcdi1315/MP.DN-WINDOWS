
using System;
using System.Runtime.CompilerServices;

namespace MP.Collections.Hashing
{
    /// <summary>
    /// Specifies the XXHash 32-bit hasher. <br />
    /// This class cannot be inherited.
    /// </summary>
    public sealed class XXHash32 : I32BitHasher
    {
        private readonly System.UInt32 seed;
        private readonly System.UInt32[] accumulators;

        /// <summary>
        /// Initializes a new instance of the <see cref="XXHash32"/> hasher.
        /// </summary>
        /// <param name="seed">The initial seed of the hasher.</param>
        public XXHash32(System.UInt32 seed)
        {
            this.seed = seed;
            XXHash32Shared.InitializeAccumulators(out accumulators, seed);
        }

        /// <inheritdoc />
        public System.UInt32 Hash(ref System.Byte input, System.Int32 length)
        {
            if (Unsafe.IsNullRef(ref input)) {
                throw new ArgumentNullException(nameof(input));
            } else if (length < 0) {
                throw new ArgumentOutOfRangeException(nameof(length), "Length cannot be a negative value.");
            } else {
                System.UInt32 ret;
                ref System.Byte cpy = ref input;

                if (length >= 16) {
                    cpy = ref XXHash32Shared.Consume(accumulators, ref cpy, length);

                    ret = XXHash32Shared.Merge(accumulators);
                } else {
                    ret = seed + XXHash32Shared.XXH_PRIME32_5;
                }

                ret += length.ToUInt32();

                return XXHash32Shared.Finalize(ret, ref cpy, length);
            }
        }
    }
}