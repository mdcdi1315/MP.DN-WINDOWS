
using System;
using System.Runtime.InteropServices;

namespace MP.NativeInterop.Windows
{
    /// <summary>
    /// Defines a time interval in Windows. Expressed as the kernel's file time.
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Size = sizeof(System.UInt32) * 2, Pack = sizeof(System.UInt32))]
    public readonly struct FILETIME
    {
        /// <summary>
        /// The lower 32-bits of the file time.
        /// </summary>
        [FieldOffset(0)]
        public readonly System.UInt32 dwLowDateTime;
        /// <summary>
        /// The upper 32-bits of the file time.
        /// </summary>
        [FieldOffset(sizeof(System.UInt32))]
        public readonly System.UInt32 dwHighDateTime;

        /// <summary>
        /// Initializes a new instance of the <see cref="FILETIME"/> structure from the specified value.
        /// </summary>
        /// <param name="fileTime">The raw file time, in ticks.</param>
        public FILETIME(System.Int64 fileTime)
        {
            dwLowDateTime = fileTime.ToUInt32();
            dwHighDateTime = (fileTime >> 32).ToUInt32();
        }

        /// <summary>Converts this <see cref="FILETIME"/> to a tick value.</summary>
        /// <returns>The converted ticks.</returns>
        public readonly System.Int64 ToTicks() => (dwHighDateTime.ToInt64() << 32) + dwLowDateTime;
        /// <summary>
        /// Converts this <see cref="FILETIME"/> to a <see cref="DateTime"/> instance, expressed as UTC.
        /// </summary>
        /// <returns>A <see cref="DateTime"/> instance representing as UTC the current <see cref="FILETIME"/> instance.</returns>
        public readonly DateTime ToDateTimeUtc() => DateTime.FromFileTimeUtc(ToTicks());

        /// <summary>
        /// Converts a <see cref="DateTime"/> instance to a <see cref="FILETIME"/> instance.
        /// </summary>
        /// <param name="dt">The <see cref="DateTime"/> to convert.</param>
        /// <returns>A new <see cref="FILETIME"/> instance.</returns>
        public static FILETIME FromDateTime(DateTime dt) => new(dt.ToFileTimeUtc());

        /*
        public readonly SYSTEMTIME ToSystemTime()
        {
            BOOL br = Kernel32.FileTimeToSystemTime(this, out var st);
            if (br == BOOL.FALSE) { throw new MP.ExceptionSystem.NativeWindowsException(); }
            return st;
        }
        */

        /// <summary>Forwards the <see cref="DateTime.ToString()"/> method.</summary>
        public override readonly string ToString() => ToDateTimeUtc().ToString();

        // public static FILETIME Now => Kernel32.GetSystemTimeAsFileTime();

        // public static explicit operator LongFileTime(FILETIME fileTime) => new() { TicksSince1601 = fileTime.ToTicks() };
    }
}