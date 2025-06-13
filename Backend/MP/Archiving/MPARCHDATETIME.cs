
using System.Runtime.InteropServices;

namespace MP.Archiving
{
    /// <summary>
    /// Defines a date time structure for use in Music Player archives. <br />
    /// The datetime is expressed as microseconds. <br />
    /// The date/time epoch for this structure is in <see cref="System.DateTime"/> binary as 5250472970188713601.
    /// </summary>
    [StructLayout(LayoutKind.Explicit , Pack = 1)]
    public readonly struct MPARCHDATETIME
    {
        private const System.Int64 SerializedEpochTimeDateTimeInterop = 5250472970188713601;

        [FieldOffset(0)]
        private readonly System.Double datetime;

        /// <summary>
        /// Represents the epoch of this structure which is approximately at 2025/03/27 17:59:36.13256 in UTC.
        /// </summary>
        public static MPARCHDATETIME Epoch => new();

        public static MPARCHDATETIME UtcNow => new(MP.SystemInfo.UtcNow);

        public static MPARCHDATETIME Now => new(SystemInfo.Now);

        public MPARCHDATETIME() { datetime = 0; }

        public MPARCHDATETIME(System.DateTime datetime)
        {
            this.datetime = datetime.Subtract(System.DateTime.FromBinary(SerializedEpochTimeDateTimeInterop)).TotalMicroseconds;
        }

        public System.DateTime DateTime => System.DateTime.FromBinary(5250472970188713601).Add(System.TimeSpan.FromMicroseconds(datetime));

        /// <summary>
        /// Returns a formatted string representing the current time. 
        /// </summary>
        /// <returns>A formatted string representing the current time.</returns>
        public override string ToString() => DateTime.ToString();

        public System.String ToString(System.String format) => DateTime.ToString(format);
    }
}