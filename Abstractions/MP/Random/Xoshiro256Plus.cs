
using MP.Annotations;
using System.Runtime.CompilerServices;

namespace MP.Random
{
    /*  
        Written in 2018 by David Blackman and Sebastiano Vigna (vigna@acm.org)

        To the extent possible under law, the author has dedicated all copyright
        and related and neighboring rights to this software to the public domain
        worldwide.

        Permission to use, copy, modify, and/or distribute this software for any
        purpose with or without fee is hereby granted.

        THE SOFTWARE IS PROVIDED "AS IS" AND THE AUTHOR DISCLAIMS ALL WARRANTIES
        WITH REGARD TO THIS SOFTWARE INCLUDING ALL IMPLIED WARRANTIES OF
        MERCHANTABILITY AND FITNESS. IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR
        ANY SPECIAL, DIRECT, INDIRECT, OR CONSEQUENTIAL DAMAGES OR ANY DAMAGES
        WHATSOEVER RESULTING FROM LOSS OF USE, DATA OR PROFITS, WHETHER IN AN
        ACTION OF CONTRACT, NEGLIGENCE OR OTHER TORTIOUS ACTION, ARISING OUT OF OR
        IN CONNECTION WITH THE USE OR PERFORMANCE OF THIS SOFTWARE.

        mdcdi1315: Ported to C# for the Music Player app in 2025. 
    */

    /// <summary>
    /// Another default implementation of the <see cref="IRandomSource"/> interface. <br />
    /// The generator used is the Xoshiro 256+ RNG. <br />
    /// You can find more information about it in <see href="http://prng.di.unimi.it"/>.
    /// </summary>
    public sealed class Xoshiro256Plus : IRandomSource
    {
        private System.UInt64[] state;
        private System.Int64 seed;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static System.UInt64 RotateLeft(System.UInt64 x, System.Int32 k) => (x << k) | (x >> (64 - k));

        /// <summary>
        /// Creates a new instance of the <see cref="Xoshiro256Plus"/> class, with a seed intialized from the current date and time.
        /// </summary>
        [RequiresNativeLayer]
        public Xoshiro256Plus()
        {
            state = new System.UInt64[4];
            Init(SystemInfo.Now.Ticks);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="Xoshiro256Plus"/> class, with the specified seed, initializing the random number generator.
        /// </summary>
        /// <param name="seed">The seed to use so that to initilalize the Xoshiro random number generator.</param>
        public Xoshiro256Plus(System.Int64 seed)
        {
            state = new System.UInt64[4];
            Init(seed);
        }

        /// <inheritdoc />
        public System.Int64 Seed => seed;

        /// <inheritdoc />
        public void Init(System.Int64 seed)
        {
            new SplitMix64(seed.ToUInt64()).FillArray(state);
            this.seed = seed;
        }

        /// <inheritdoc />
        public System.UInt64 Next()
        {
            System.UInt64 result = state[0] + state[3];

            System.UInt64 t = state[1] << 17;

            state[2] ^= state[0];
            state[3] ^= state[1];
            state[1] ^= state[2];
            state[0] ^= state[3];

            state[2] ^= t;

            state[3] = RotateLeft(state[3], 45);

            return result;
        }
    }
}