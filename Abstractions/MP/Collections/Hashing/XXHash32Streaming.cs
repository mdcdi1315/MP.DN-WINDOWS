
using System.Runtime.CompilerServices;

namespace MP.Collections.Hashing
{
    /// <summary>
    /// Defines a hasher compliant with the <see cref="XXHash32"/> class, but it is able to accept multiple memory buffers instead.
    /// </summary>
    public sealed class XXHash32Streaming : I32BitStreamingHasher
    {
        private readonly System.Byte[] buffer;
        private readonly System.UInt32[] accumulators;

        private System.UInt32 large_len;
        private System.UInt32 total_len_32;
        private System.UInt32 buffered_size;

        /// <summary>
        /// Initializes a new instance of the <see cref="XXHash32Streaming"/> hasher.
        /// </summary>
        /// <param name="seed">The initial seed.</param>
        public XXHash32Streaming(System.UInt32 seed)
        {
            XXHash32Shared.InitializeAccumulators(out accumulators, seed);
            buffer = new System.Byte[16];
            large_len = buffered_size = total_len_32 = 0U;
        }

        /// <inheritdoc />
        public void Update(ref System.Byte input, int length)
        {
            uint ulen = length.ToUInt32();
            total_len_32 += ulen;
            large_len |= ((ulen >= 16U) ? 1U : 0U) | (total_len_32 >= 16U ? 1U : 0U);
            if (ulen < buffer.Length - buffered_size) {
                Unsafe.CopyBlockUnaligned(ref buffer[buffered_size], ref input, ulen);
                buffered_size += ulen;
            }

            ref System.Byte xinput = ref input, b_end = ref Unsafe.Add(ref input, length);

            if (buffered_size > 0U) {  /* non-empty buffer: complete first */
                Unsafe.CopyBlockUnaligned(ref buffer[buffered_size], ref xinput, buffer.Length.ToUInt32() - buffered_size);
                xinput = ref Unsafe.Add(ref xinput, buffer.Length.ToUInt32() - buffered_size);
                /* then process one round */
                XXHash32Shared.Consume(accumulators, ref buffer[0], buffer.Length);
                buffered_size = 0U;
            }

            nint byte_offset = Unsafe.ByteOffset(ref xinput, ref b_end);

            if (byte_offset >= buffer.Length) {
                xinput = XXHash32Shared.Consume(accumulators, ref xinput, byte_offset.ToInt32());
            }

            if (Unsafe.IsAddressLessThan(ref xinput, ref b_end)) 
            {
                Unsafe.CopyBlockUnaligned(
                    ref buffer[0], 
                    ref xinput, 
                    buffered_size = (uint)Unsafe.ByteOffset(ref xinput, ref b_end)
                );
            }
        }

        /// <inheritdoc />
        public System.UInt32 Digest()
        {
            System.UInt32 ret;

            if (large_len > 0U) {
                ret = XXHash32Shared.Merge(accumulators);
            } else {
                ret = accumulators[2] + XXHash32Shared.XXH_PRIME32_5;
            }

            return XXHash32Shared.Finalize(ret, ref buffer[0], buffered_size.ToInt32());
        }
    }
}