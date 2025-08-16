
using System;

namespace MP.AudioLibrary
{
    /// <summary>
    /// The <see cref="Decibel"/> structure defines another volume measurement in contrast to the most common attentuation scale from 0 to 1 in floating point.
    /// </summary>
    public struct Decibel
    {
        private System.Single db;

        /// <summary>
        /// Creates a <see cref="Decibel"/> instance which has a value of zero. (No volume)
        /// </summary>
        public Decibel() => db = 0;

        /// <summary>
        /// Creates a <see cref="Decibel"/> instance from the specified raw value.
        /// </summary>
        /// <param name="value">The raw value to be used.</param>
        public Decibel(System.Single value) => db = value;

        /// <summary>
        /// Creates a <see cref="Decibel"/> instance from an attentuation value.
        /// </summary>
        /// <param name="scale">The attentuation value to convert to decibels.</param>
        /// <returns>A newly created <see cref="Decibel"/> instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="scale"/> was less than zero and greater than 1.</exception>
        public static Decibel FromAttenuation(System.Single scale)
        {
            if (scale < 0 || scale > 1) { throw new ArgumentOutOfRangeException(nameof(scale), "Attenuation must be a value ranging from 0 to 1."); }
            return new() { db = 20f * MathF.Log(scale, 10f) };
        }

        /// <summary>
        /// Converts the value of this <see cref="Decibel"/> instance back to an attentuation value.
        /// </summary>
        /// <returns>The final attentuation value that is equvalent to the value contained into this instance.</returns>
        public readonly System.Single ToAttenuation() => MathF.Exp(db) / 20f;

        /// <summary>
        /// Gets the raw value of this <see cref="Decibel"/> structure.
        /// </summary>
        public readonly System.Single Value => db;

        /// <summary>
        /// Returns the value that this structure holds as a string.
        /// </summary>
        /// <returns>The value held by this structure.</returns>
        public readonly override System.String ToString() => $"{db:f2} DB";
    }
}