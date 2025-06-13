
using System.Runtime.CompilerServices;

namespace MP.Utilities
{
    /// <summary>
    /// Contains utility methods about getting and mapping percentages to other numeric units. <br />
    /// Useful for mapping numbers that have different ranges.
    /// </summary>
    public static class PercentageHelpers
    {
        /// <summary>
        /// Gets a percentage value based on the current value and the maximum value that can be reached.
        /// </summary>
        /// <param name="currentvalue">The current value to translate the percentage as.</param>
        /// <param name="maximumvalue">The maximum value that the <paramref name="currentvalue"/> can reach.</param>
        /// <returns>A percentage value, ranging from 0 to 100.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.Byte GetPercentage(System.Int64 currentvalue, System.Int64 maximumvalue)
            => (System.Byte)(currentvalue * (100f / maximumvalue));

        /// <summary>
        /// Gets a percentage value based on the current value and the maximum value that can be reached.
        /// </summary>
        /// <param name="currentvalue">The current value to translate the percentage as.</param>
        /// <param name="maximumvalue">The maximum value that the <paramref name="currentvalue"/> can reach.</param>
        /// <returns>A percentage value, ranging from 0 to 100.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.Byte GetPercentage(System.Single currentvalue, System.Single maximumvalue)
            => (System.Byte)(currentvalue * (100f / maximumvalue));

        /// <summary>
        /// Gets a percentage value based on the current value and the maximum value that can be reached.
        /// </summary>
        /// <param name="currentvalue">The current value to translate the percentage as.</param>
        /// <param name="maximumvalue">The maximum value that the <paramref name="currentvalue"/> can reach.</param>
        /// <returns>A percentage value, ranging from 0 to 100.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.Byte GetPercentage(System.Double currentvalue, System.Double maximumvalue)
            => (System.Byte)(currentvalue * (100d / maximumvalue));

        /// <summary>
        /// Gets a rough estimate of the current actual value if the current percentage value and 
        /// the maximum value of the inspecting number are given. <br />
        /// For best result accuracy, use the <see cref="MapNumberTo(long, long, long)"/> method.
        /// </summary>
        /// <param name="percentage">The percentage to compute the rough estimate.</param>
        /// <param name="maximumvalue">The maximum value that the actual inspecting number can reach.</param>
        /// <returns>A rough estimate of the current number given by the <paramref name="percentage"/> and <paramref name="maximumvalue"/> numbers.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.Int64 FromPercentage(System.Byte percentage, System.Int64 maximumvalue)
            => (System.Int64)(maximumvalue * (percentage / 100f));

        /// <summary>
        /// Gets a rough estimate of the current actual value if the current percentage value and 
        /// the maximum value of the inspecting number are given.
        /// </summary>
        /// <param name="percentage">The percentage to compute the rough estimate.</param>
        /// <param name="maximumvalue">The maximum value that the actual inspecting number can reach.</param>
        /// <returns>A rough estimate of the current number given by the <paramref name="percentage"/> and <paramref name="maximumvalue"/> numbers.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.Single FromPercentage(System.Byte percentage, System.Single maximumvalue)
            => maximumvalue * (percentage / 100f);

        /// <summary>
        /// Gets a rough estimate of the current actual value if the current percentage value and 
        /// the maximum value of the inspecting number are given.
        /// </summary>
        /// <param name="percentage">The percentage to compute the rough estimate.</param>
        /// <param name="maximumvalue">The maximum value that the actual inspecting number can reach.</param>
        /// <returns>A rough estimate of the current number given by the <paramref name="percentage"/> and <paramref name="maximumvalue"/> numbers.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.Double FromPercentage(System.Byte percentage, System.Double maximumvalue)
            => maximumvalue * (percentage / 100d);

        /// <summary>
        /// Performs a number mapping to a number that has a different numeric range than the source number. <br />
        /// The algorithm performs the mapping by getting the percentage from the <paramref name="currentvalue"/> and <paramref name="maximumvaluep0"/> parameters,
        /// and then that percentage is remapped to the range 0 to the value of the <paramref name="maximumvalueretp"/> parameter. <br />
        /// This method is more accurate than using <see cref="GetPercentage(long, long)"/> and then <see cref="FromPercentage(byte, long)"/> ,
        /// since it does not ignore the fractional part of the percentage.
        /// </summary>
        /// <param name="currentvalue">The number to be mapped to <paramref name="maximumvalueretp"/>.</param>
        /// <param name="maximumvaluep0">The maximum value that <paramref name="currentvalue"/> can reach.</param>
        /// <param name="maximumvalueretp">The maximum value in the range that the return parameter can reach.</param>
        /// <returns>The mapped number, ranging from 0 to <paramref name="maximumvalueretp"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.Int64 MapNumberTo(System.Int64 currentvalue , System.Int64 maximumvaluep0 , System.Int64 maximumvalueretp)
            => (System.Int64)(maximumvalueretp * ((currentvalue * (100f / maximumvaluep0)) / 100f));
    }
}