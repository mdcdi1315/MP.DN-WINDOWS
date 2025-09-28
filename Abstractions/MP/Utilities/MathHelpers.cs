
using System;
using System.Runtime.CompilerServices;

namespace MP.Utilities
{
    /// <summary>
    /// Mathematical utilities found useful at times.
    /// </summary>
    public static class MathHelpers
    {
        /// <summary>
        /// Maps a value in the range [0..1] to the range [<paramref name="start"/>..<paramref name="end"/>].
        /// </summary>
        /// <param name="value">The value to map.</param>
        /// <param name="start">The lower bound of the range to map the value to.</param>
        /// <param name="end">The upper bound of the range to map the value to.</param>
        /// <returns>The mapped value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Lerp(double value , double start , double end) => (start + value) * (end - start);

        /// <summary>
        /// Maps a value in the range [0..1] to the range [<paramref name="start"/>..<paramref name="end"/>].
        /// </summary>
        /// <param name="value">The value to map.</param>
        /// <param name="start">The lower bound of the range to map the value to.</param>
        /// <param name="end">The upper bound of the range to map the value to.</param>
        /// <returns>The mapped value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Lerp(float value, float start, float end) => (start + value) * (end - start);

        /// <summary>Clamps the specified value into the specified range.</summary>
        /// <param name="value">The value to be clamped.</param>
        /// <param name="min">The lower bound that is the minimum value that can be returned by this method.</param>
        /// <param name="max">The upper bound that is the maximum value that can be returned by this method.</param>
        /// <returns>A value in the exact range [<paramref name="min"/>..<paramref name="max"/>].</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Clamp(double value, double min, double max) => value > max ? max : Math.Max(value, min);

        /// <summary>Clamps the specified value into the specified range.</summary>
        /// <param name="value">The value to be clamped.</param>
        /// <param name="min">The lower bound that is the minimum value that can be returned by this method.</param>
        /// <param name="max">The upper bound that is the maximum value that can be returned by this method.</param>
        /// <returns>A value in the exact range [<paramref name="min"/>..<paramref name="max"/>].</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Clamp(float value, float min, float max) => value > max ? max : Math.Max(value, min);

        /// <summary>
        /// Maps a value from the range [<paramref name="inputlowerbound"/>..<paramref name="inputupperbound"/>] to the range [<paramref name="outputlowerbound"/>..<paramref name="outputupperbound"/>].
        /// The value is clamped, if necessary.
        /// </summary>
        /// <param name="input">The input value to map.</param>
        /// <param name="inputlowerbound">The lower bound of the input value range.</param>
        /// <param name="inputupperbound">The upper bound of the input value range.</param>
        /// <param name="outputlowerbound">The lower bound of the output value range.</param>
        /// <param name="outputupperbound">The upper bound of the output value range.</param>
        /// <returns>The mapped value, clamped to [<paramref name="outputlowerbound"/>..<paramref name="outputupperbound"/>].</returns>
        /*
             * What does it do:
             * What it could be written as a math expression (assuming that 'input' is in range):
             * ToNormalRange(input , inputlowerbound , inputupperbound) * (outputupperbound - outputlowerbound)) + outputlowerbound
             * That is , I am expressing input in the range (0..1).
             * Then, I can transform such a range to any possible number range thus multiplying with (outputupperbound - outputlowerbound) would give us a range of [0..(outputupperbound - outputlowerbound)].
             * Finally add outputlowerbound to the result to bring it into [outputlowerbound..outputupperbound] range. 
        */
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double ClampedMapToRange(double input, double inputlowerbound, double inputupperbound, double outputlowerbound, double outputupperbound) 
            => (ToNormalRange(Clamp(input, inputlowerbound, inputupperbound), inputlowerbound, inputupperbound) * (outputupperbound - outputlowerbound)) + outputlowerbound;

        /// <summary>
        /// Maps a value from the range [<paramref name="inputlowerbound"/>..<paramref name="inputupperbound"/>] to the range [<paramref name="outputlowerbound"/>..<paramref name="outputupperbound"/>].
        /// The value is clamped, if necessary.
        /// </summary>
        /// <param name="input">The input value to map.</param>
        /// <param name="inputlowerbound">The lower bound of the input value range.</param>
        /// <param name="inputupperbound">The upper bound of the input value range.</param>
        /// <param name="outputlowerbound">The lower bound of the output value range.</param>
        /// <param name="outputupperbound">The upper bound of the output value range.</param>
        /// <returns>The mapped value, clamped to [<paramref name="outputlowerbound"/>..<paramref name="outputupperbound"/>].</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ClampedMapToRange(float input, float inputlowerbound, float inputupperbound, float outputlowerbound, float outputupperbound)
            => (ToNormalRange(Clamp(input , inputlowerbound , inputupperbound), inputlowerbound, inputupperbound) * (outputupperbound - outputlowerbound)) + outputlowerbound;

        /// <summary>
        /// Maps a number of any range to the normal range. <br />
        /// That is, a conversion happens from (<paramref name="min"/>..<paramref name="max"/>) to (0..1) range respectively.
        /// </summary>
        /// <param name="v">The number in the (<paramref name="min"/>..<paramref name="max"/>) range to be converted to the (0..1) range.</param>
        /// <param name="min">The lower bound of the values that the number in <paramref name="v"/> can be into,</param>
        /// <param name="max">The upper bound of the values that the number in <paramref name="v"/> can be into,</param>
        /// <returns>A mapped number in the range (0..1).</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double ToNormalRange(double v, double min, double max) => Math.Abs(v - min) / Math.Abs(min - max);
        
        /// <summary>
        /// Maps a number of any range to the normal range. <br />
        /// That is, a conversion happens from (<paramref name="min"/>..<paramref name="max"/>) to (0..1) range respectively.
        /// </summary>
        /// <param name="v">The number in the (<paramref name="min"/>..<paramref name="max"/>) range to be converted to the (0..1) range.</param>
        /// <param name="min">The lower bound of the values that the number in <paramref name="v"/> can be into,</param>
        /// <param name="max">The upper bound of the values that the number in <paramref name="v"/> can be into,</param>
        /// <returns>A mapped number in the range (0..1).</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ToNormalRange(float v, float min, float max) => Math.Abs(v - min) / Math.Abs(min - max);
    }
}
