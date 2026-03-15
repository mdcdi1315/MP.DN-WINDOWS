
using MP.Annotations;
using System.Runtime.CompilerServices;

namespace MP.Random
{
    /* 
      * Written in 2018 by David Blackman and Sebastiano Vigna (vigna@acm.org)
      *
      * To the extent possible under law, the author has dedicated all copyright
      * and related and neighboring rights to this software to the public domain
      * worldwide. This software is distributed without any warranty.
      *
      *	See <http://creativecommons.org/publicdomain/zero/1.0/>. 
      *	
      * mdcdi1315: Ported to C# for the Music Player app in 2025.
      */

    /// <summary>
    /// A default implementation of the <see cref="IRandomSource"/> interface. <br />
    /// The generator used is the Xoroshiro 256++ RNG. <br />
    /// You can find more information about it in <see href="http://prng.di.unimi.it"/>.
    /// </summary>
    public sealed class Xoroshiro256PlusPlus : IRandomSource
    {
        private System.UInt64[] state;
        private System.Int64 seed;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static System.UInt64 RotateLeft(System.UInt64 x, System.Int32 k) => (x << k) | (x >> (64 - k));

        /// <summary>
        /// Creates a new instance of the <see cref="Xoroshiro256PlusPlus"/> class, with a seed intialized from the current date and time.
        /// </summary>
        [RequiresNativeLayer]
        public Xoroshiro256PlusPlus()
        {
            state = new System.UInt64[4];
            Init(SystemInfo.Now.Ticks);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="Xoroshiro256PlusPlus"/> class, with the specified seed, initializing the random number generator.
        /// </summary>
        /// <param name="seed">The seed to use so that to initilalize the Xoroshiro random number generator.</param>
        public Xoroshiro256PlusPlus(System.Int64 seed)
        {
            state = new System.UInt64[4];
            Init(seed);
        }

        /// <inheritdoc />
        public void Init(System.Int64 seed)
        {
            new SplitMix64(seed.ToUInt64()).FillArray(state);
            this.seed = seed;
        }

        /// <inheritdoc />
        public System.UInt64 Next()
        {
            System.UInt64 result = RotateLeft(state[0] + state[3], 23) + state[0];
            System.UInt64 t = state[1] << 17;

            state[2] ^= state[0];
            state[3] ^= state[1];
            state[1] ^= state[2];
            state[0] ^= state[3];

            state[2] ^= t;

            state[3] = RotateLeft(state[3], 45);

            return result;
        }

        /// <inheritdoc />
        public System.Int64 Seed => seed;
    }
}