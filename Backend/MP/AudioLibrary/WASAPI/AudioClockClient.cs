using System;
using MP.ComInterop;
using Microsoft.Diagnostics;

namespace MP.AudioLibrary.WASAPI
{
    /// <summary>
    /// Audio Clock Client
    /// </summary>
    public sealed unsafe class AudioClockClient : IDisposable
    {
        private IAudioClock audioClockClientInterface;

        internal AudioClockClient(IAudioClock audioclockclient)
        {
            ArgumentNullException.ThrowIfNull(audioclockclient);
            audioClockClientInterface = audioclockclient;
        }

        /// <summary>
        /// Characteristics
        /// </summary>
        public AUDIOCLOCK_CHARACTERISTIC Characteristics
        {
            get {
                AUDIOCLOCK_CHARACTERISTIC chs;
                var hr = audioClockClientInterface.GetCharacteristics(&chs);
                hr.ThrowOnFailure();
                return chs;
            }
        }

        /// <summary>
        /// Frequency
        /// </summary>
        public ulong Frequency
        {
            get {
                System.UInt64 freq;
                audioClockClientInterface.GetFrequency(&freq).ThrowOnFailure();
                return freq;
            }
        }

        /// <summary>
        /// Get Position
        /// </summary>
        public bool GetPosition(out ulong position, out ulong qpcPosition)
        {
            System.UInt64 ps, qps;
            var hr = audioClockClientInterface.GetPosition(&ps , &qps);
            position = ps;
            qpcPosition = qps;
            if (hr == -1) {
                return false;
            }
            hr.ThrowOnFailure();
            return true;
        }

        /// <summary>
        /// Gets the actual device position. This feature requires the <see cref="IAudioClock2"/> interface to have been defined on the object.
        /// </summary>
        /// <param name="frameposition">The frame position of the device</param>
        /// <param name="qpcposition">The QueryPerformanceCounter position of the device</param>
        /// <returns>A value whether the call is implemented or not. If not implemented, the parameters are filled with zero.</returns>
        public System.Boolean GetDevicePosition(out System.UInt64 frameposition , out System.UInt64 qpcposition)
        {
            if (audioClockClientInterface is IAudioClock2 ac2)
            {
                System.UInt64 fp, qpc;
                var hr = ac2.GetDevicePosition(&fp , &qpc);
                hr.ThrowOnFailure();
                frameposition = fp;
                qpcposition = qpc;
                return true;
            }
            frameposition = 0;
            qpcposition = 0;
            return false;
        }

        /// <summary>
        /// Adjusted Position
        /// </summary>
        public ulong AdjustedPosition
        {
            get
            {
                ulong pos, qpos;
                int cnt = 0;
                while (!GetPosition(out pos, out qpos))
                {
                    if (++cnt == 5)
                    {
                        // we've tried too many times, so now we have to just run with what we have...
                        break;
                    }
                }

                if (Stopwatch.IsHighResolution)
                {
                    // cool, we can adjust our position appropriately

                    // get the current qpc count (in ticks)
                    var qposNow = (ulong)((Stopwatch.GetTimestamp() * 10000000M) / Stopwatch.Frequency);

                    // find out how many ticks have passed since the device reported the position
                    var qposDiff = qposNow - qpos;

                    // find out how many device position units (usually bytes) would have played in that time span
                    var posDiff = (qposDiff * Frequency) / TimeSpan.TicksPerSecond;

                    // add it to the position
                    pos += posDiff;
                }
                return pos;
            }
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            if (audioClockClientInterface is not null)
            {
                ComMarshalling.ReleaseInteropObject(audioClockClientInterface);
                audioClockClientInterface = null;
                GC.SuppressFinalize(this);
            }
        }
    }
}
