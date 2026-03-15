

using System;

namespace MP.Random
{
    /*  
        Written in 2015 by Sebastiano Vigna (vigna@acm.org)

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
    /// This is a fixed-increment version of Java 8's SplittableRandom generator <br />
    /// See <see href="http://dx.doi.org/10.1145/2714064.2660195"/> and <br />
    /// <see href="http://docs.oracle.com/javase/8/docs/api/java/util/SplittableRandom.html"/> <br /> <br />
    /// 
    /// It is a very fast generator passing BigCrush, and it can be useful if
    /// for some reason you absolutely want 64 bits of state.
    /// </summary>
    // This is not implementing IRandomSource delibrately since it is not an actual PRNG,
    // it is just a state generator
    public sealed class SplitMix64
    {
        private System.UInt64 x;

        /// <summary>
        /// Creates a new instance of the <see cref="SplitMix64"/> generator.
        /// </summary>
        /// <param name="initstate">The initially desired seed value.</param>
        public SplitMix64(System.UInt64 initstate) => x = initstate;

        /// <summary>
        /// Produces a new random number.
        /// </summary>
        /// <returns>The newly created random number.</returns>
        public System.UInt64 Next()
        {
            System.UInt64 z = (x += 0x9e3779b97f4a7c15);
            z = (z ^ (z >> 30)) * 0xbf58476d1ce4e5b9;
            z = (z ^ (z >> 27)) * 0x94d049bb133111eb;
            return z ^ (z >> 31);
        }

        /// <summary>
        /// Fills an array of <see cref="System.UInt64"/>'s so that initialzation of states can be done more easily.
        /// </summary>
        /// <param name="ulongs">The array to fill with random <see cref="System.UInt64"/>s.</param>
        public void FillArray(System.UInt64[] ulongs)
        {
            ArgumentNullException.ThrowIfNull(ulongs);
            for (var I = 0; I < ulongs.Length; I++)
            {
                ulongs[I] = Next();
            }
        }
    }
}