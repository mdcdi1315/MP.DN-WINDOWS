
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace Microsoft.Diagnostics
{
    // This class uses high-resolution performance counter if the installed
    // hardware supports it. Otherwise, the class will fall back to DateTime
    // and uses ticks as a measurement.

    [System.Diagnostics.DebuggerDisplay("{GetDebuggerDisplay(),nq}")]
    public sealed class Stopwatch
    {
        private const System.Int64 TicksPerMillisecond = 10000;
        private const System.Int64 TicksPerSecond = TicksPerMillisecond * 1000;

        private System.Int64 _elapsed , _startTimeStamp;
        private System.Double tickfrequency;
        private System.Boolean _isRunning;

        public const System.Boolean IsHighResolution = true;

        public static System.Int64 Frequency => Interop.Kernel32.QueryPerformanceFrequency();

        public Stopwatch()
        {
            tickfrequency = (System.Double)TicksPerSecond / Interop.Kernel32.QueryPerformanceFrequency();
            Reset();
        }

        public void Start()
        {
            // Calling start on a running Stopwatch is a no-op.
            if (!_isRunning)
            {
                _startTimeStamp = GetTimestamp();
                _isRunning = true;
            }
        }

        public static Stopwatch StartNew()
        {
            Stopwatch s = new();
            s.Start();
            return s;
        }

        public void Stop()
        {
            // Calling stop on a stopped Stopwatch is a no-op.
            if (_isRunning)
            {
                long endTimeStamp = GetTimestamp();
                long elapsedThisPeriod = endTimeStamp - _startTimeStamp;
                _elapsed += elapsedThisPeriod;
                _isRunning = false;

                if (_elapsed < 0)
                {
                    // When measuring small time periods the Stopwatch.Elapsed*
                    // properties can return negative values.  This is due to
                    // bugs in the basic input/output system (BIOS) or the hardware
                    // abstraction layer (HAL) on machines with variable-speed CPUs
                    // (e.g. Intel SpeedStep).

                    _elapsed = 0;
                }
            }
        }

        public void Reset()
        {
            _elapsed = 0;
            _isRunning = false;
            _startTimeStamp = 0;
        }

        // Convenience method for replacing {sw.Reset(); sw.Start();} with a single sw.Restart()
        public void Restart()
        {
            _elapsed = 0;
            _startTimeStamp = GetTimestamp();
            _isRunning = true;
        }

        public void RestartFromPoint(System.TimeSpan specific)
        {
            _elapsed = 0;
            _startTimeStamp = GetTimestamp() + unchecked((System.Int64)(specific.Ticks * tickfrequency));
            _isRunning = true;
        }

        /// <summary>
        /// Returns the <see cref="Elapsed"/> time as a string.
        /// </summary>
        /// <returns>
        /// Elapsed time string in the same format used by <see cref="TimeSpan.ToString()"/>.
        /// </returns>
        public override string ToString() => Elapsed.ToString();

        public bool IsRunning => _isRunning;

        public TimeSpan Elapsed => new(GetElapsedDateTimeTicks());

        public long ElapsedMilliseconds => GetElapsedDateTimeTicks() / TicksPerMillisecond;

        public System.Int64 ElapsedDateTimeTicks => unchecked((long)(GetRawElapsedTicks() * tickfrequency));

        public long ElapsedTicks => GetRawElapsedTicks();

        public static long GetTimestamp()
        {
            System.Diagnostics.Debug.Assert(IsHighResolution);
            return Interop.Kernel32.QueryPerformanceCounter();
        }

        /// <summary>Gets the elapsed time since the <paramref name="startingTimestamp"/> value retrieved using <see cref="GetTimestamp"/>.</summary>
        /// <param name="startingTimestamp">The timestamp marking the beginning of the time period.</param>
        /// <returns>A <see cref="TimeSpan"/> for the elapsed time between the starting timestamp and the time of this call.</returns>
        public static TimeSpan GetElapsedTime(long startingTimestamp) =>
            GetElapsedTime(startingTimestamp, GetTimestamp());

        /// <summary>Gets the elapsed time between two timestamps retrieved using <see cref="GetTimestamp"/>.</summary>
        /// <param name="startingTimestamp">The timestamp marking the beginning of the time period.</param>
        /// <param name="endingTimestamp">The timestamp marking the end of the time period.</param>
        /// <returns>A <see cref="TimeSpan"/> for the elapsed time between the starting and ending timestamps.</returns>
        public static TimeSpan GetElapsedTime(long startingTimestamp, long endingTimestamp) =>
            new TimeSpan((long)((endingTimestamp - startingTimestamp) * ((System.Double)TicksPerSecond / Interop.Kernel32.QueryPerformanceFrequency())));

        // Get the elapsed ticks.
        private long GetRawElapsedTicks()
        {
            long timeElapsed = _elapsed;

            if (_isRunning)
            {
                // If the Stopwatch is running, add elapsed time since
                // the Stopwatch is started last time.
                long currentTimeStamp = GetTimestamp();
                long elapsedUntilNow = currentTimeStamp - _startTimeStamp;
                timeElapsed += elapsedUntilNow;
            }
            return timeElapsed;
        }

        // Get the elapsed ticks.
        private long GetElapsedDateTimeTicks()
        {
            System.Diagnostics.Debug.Assert(IsHighResolution);
            // convert high resolution perf counter to DateTime ticks
            return unchecked((long)(GetRawElapsedTicks() * tickfrequency));
        }

        private string GetDebuggerDisplay() => $"Microsoft.Diagnostics.Stopwatch: [ {Elapsed} (IsRunning = {_isRunning}) ]";
    }
}