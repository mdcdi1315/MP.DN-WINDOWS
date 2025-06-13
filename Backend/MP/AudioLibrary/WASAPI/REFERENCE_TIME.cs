

using System.Runtime.InteropServices;

namespace MP.AudioLibrary.WASAPI
{
    /// <summary>
    /// Used in some WASAPI interfaces. <br />
    /// This is just to define explicitness and time utilities in interop code. <br />
    /// Because this is defined as a read-only structure , I expect this to not introduce any new bottlenecks.
    /// </summary>
    [StructLayout(LayoutKind.Explicit , Pack = 1 , Size = 8)]
    public readonly struct REFERENCE_TIME
    {
        /// <summary>
        /// Gets a constant that it defines how many <see cref="REFERENCE_TIME"/> units are required to represent a single second.
        /// </summary>
        public const System.Double REFERENCE_TIMES_PER_SEC = 10000000;

        /// <summary>
        /// Gets a constant that it defines how many <see cref="REFERENCE_TIME"/> units are required to represent a single millisecond.
        /// </summary>
        public const System.Double REFERENCE_TIMES_PER_MILLISEC = 10000;

        [FieldOffset(0)]
        public readonly System.Int64 Time;

        public REFERENCE_TIME(System.Int64 time)
        {
            Time = time;
        }

        public REFERENCE_TIME(System.TimeSpan time)
        {
            Time = (System.Int64)(time.TotalSeconds * REFERENCE_TIMES_PER_SEC);
        }

        public static REFERENCE_TIME FromMilliseconds(System.Double ms) => new((System.Int64)(ms * REFERENCE_TIMES_PER_MILLISEC));

        /// <summary>
        /// Gets the number of seconds that this <see cref="REFERENCE_TIME"/> structure represents.
        /// </summary>
        /// <returns>The number of seconds represented by the specified <see cref="REFERENCE_TIME"/>.</returns>
        public readonly System.Double ToSeconds() => Time / REFERENCE_TIMES_PER_SEC;

        /// <summary>
        /// Gets the number of milliseconds that this <see cref="REFERENCE_TIME"/> structure represents.
        /// </summary>
        /// <returns>The number of milliseconds represented by the specified <see cref="REFERENCE_TIME"/>.</returns>
        public readonly System.Double ToMilliseconds() => Time / REFERENCE_TIMES_PER_MILLISEC;

        /// <summary>
        /// Gets a time span that represents the time that this <see cref="REFERENCE_TIME"/> structure holds.
        /// </summary>
        /// <returns>A <see cref="System.TimeSpan"/> representing the time that this instance represents.</returns>
        public readonly System.TimeSpan ToTimeSpan() => System.TimeSpan.FromSeconds(Time / REFERENCE_TIMES_PER_SEC);

        public static implicit operator REFERENCE_TIME(System.Int64 time) => new(time);

        public static implicit operator System.Int64(REFERENCE_TIME time) => time.Time;
    }
}