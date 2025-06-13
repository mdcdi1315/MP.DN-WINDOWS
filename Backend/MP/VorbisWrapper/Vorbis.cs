
using System;
using MP.AudioLibrary;
using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace MP.VorbisWrapper
{
    /// <summary>
    /// Directly reads Ogg Vorbis streams from plain .NET streams! <br />
    /// Note that the data returned are COMPLETELY RAW!!! <br />
    /// These do need resampling or some other kind of manipulation after processing it.
    /// </summary>
    public unsafe sealed class VorbisProvider : AudioStream
    {
        private Interop.VorbisFile.VORBIS_INFO info; // Base stream information , all decodes will be translated based on the sample rate saved here
        private AbstractPropertyStream stream;
        private SafeLibcMemoryHandle native; // The native memory that Vorbis is using to save it's decode data
        private System.Boolean seekable;
        private System.Int32 bsmcurrent;
        // Keep these functions as fields in the instance so that .NET cannot randomly crash due to accidentally freeing these buddies.
        private Interop.VorbisFile.ReadFunction rf;
        private Interop.VorbisFile.SeekFunction sf;
        private Interop.VorbisFile.TellFunction tf;

        private System.UInt64 ReadPrototype(void* buffer, System.UInt64 singleelementsize, System.UInt64 elementcount, void* pointerunused)
        {
            System.Byte[] temp = new System.Byte[elementcount * singleelementsize];
            System.Int32 rd = stream.Read(temp, 0, temp.Length);
            // We cannot translate 'buffer' as managed pointer because it is not a managed pointer.
            // So use of fixed keyword becomes mandatory.
            fixed (System.Byte* psrc = temp)
            {
                Unsafe.CopyBlockUnaligned(buffer, psrc, rd.ToUInt32());
            }
            return (System.UInt64)rd;
        }

        private System.Int32 SeekPrototype(void* pointerunused, System.Int64 offset, System.IO.SeekOrigin origin)
        {
            // If the stream is not seekable , no problem!
            // This is handled by the below if statement.
            if (stream.CanSeek == false) { return -1; }
            return stream.Seek(offset, origin).ToInt32();
        }

        private System.Int32 TellPrototype(void* pointerunused)
        {
            // Tells Vorbis which is the next byte position , otherwise returns the stream length.
            if (stream.Position >= stream.Length) { return stream.Length.ToInt32(); }
            return (stream.Position + 1).ToInt32();
        }

        /// <summary>
        /// Creates a new instance of the <see cref="VorbisProvider"/> class with the specified stream to read.
        /// </summary>
        /// <param name="stream">The Music Player Stream to read.</param>
        /// <exception cref="ArgumentNullException"><paramref name="stream"/> was null.</exception>
        /// <exception cref="ArgumentException"><paramref name="stream"/> was not readable.</exception>
        /// <exception cref="ExceptionSystem.VorbisWrapperException">An error has been occured on the native side.</exception>
        public VorbisProvider(AbstractPropertyStream stream)
        {
            if (stream is null) { throw new ArgumentNullException(nameof(stream)); }
            if (stream.CanRead == false) { throw new ArgumentException("Stream must be readable." , nameof(stream)); }
            this.stream = stream;
            rf = ReadPrototype;
            sf = SeekPrototype;
            tf = TellPrototype;
            Init();
        }

        private void Init()
        {
            // Initializes the VorbisFile reader.
            Interop.VorbisFile.Errors err = Interop.VorbisFile.ov_open_callbacks(new() {
                tell = Marshal.GetFunctionPointerForDelegate(tf),
                read = Marshal.GetFunctionPointerForDelegate(rf),
                close = System.IntPtr.Zero, // Equals to NULL
                seek = Marshal.GetFunctionPointerForDelegate(sf),
            }, out native);
            if (err < 0) { throw new MP.ExceptionSystem.VorbisWrapperException(err); }
            // Get stream and audio format information
            info = Interop.VorbisFile.ov_info(native, -1);
            seekable = Interop.VorbisFile.ov_seekable(native); // we will need it when setting times
            if (info.Rate == 0 && info.Channels == 0) {
                Dispose();
                throw new MP.ExceptionSystem.VorbisWrapperException("Cannot initialize Vorbis backend.");
            }
        }

        /// <summary>
        /// Represents the audio format under which the hooked-up resampler should output results
        /// </summary>
        public AudioFormat NomimalAudioFormat => AudioFormat.CreatePCM(info.Rate, 16, info.Channels);

        /// <summary>
        /// Represents the audio format from the last <see cref="Read(byte[], int, int)"/> operation. <br />
        /// Usually this is to be handled by an resampler.
        /// </summary>
        public override AudioFormat Format
        {
            get {
                // Query the Vorbis information for the currently read packet
                Interop.VorbisFile.VORBIS_INFO vif = Interop.VorbisFile.ov_info(native , bsmcurrent);
                // Return the packet's audio format
                return AudioFormat.CreatePCM(vif.Rate, 16, vif.Channels);
            }
        }

        /// <summary>
        /// Returns the number of logical bitstreams provided in the current file.
        /// </summary>
        public System.Int32 LogicalBitstreams => Interop.VorbisFile.ov_streams(native);

        public System.Int32 Read(System.Byte[] bytes, System.Int32 offset, System.Int32 length , out System.Int32 bitstream)
        {
            // Get native read data.
            // NAudio expects this to be 16-bit unsigned integers. (That's why false, 2 and true parameters)
        g_repeat:
            Interop.VorbisFile.Errors err = Interop.VorbisFile.ov_read(native, bytes, offset, length, false, 2, true, out bitstream);
            bsmcurrent = bitstream;
            // Use a switch statement to handle different cases fast.
            switch (err)
            {
                case Interop.VorbisFile.Errors.HOLE:
                    // HOLE is informational reading can normally continue, so call ov_read again.
                    goto g_repeat;
                case < 0:
                    // If this is an error , report it
                    throw new ExceptionSystem.VorbisWrapperException(err);
                default:
                    // Otherwise it is the number of PCM data bytes read and thus, return this as is
                    return (System.Int32)err;
            }
        }

        /// <summary>
        /// Reads PCM audio data from the Vorbis backend.
        /// </summary>
        /// <param name="bytes">The buffer to place the data to</param>
        /// <param name="offset">The offset in the buffer to start writing to</param>
        /// <param name="length">The number of bytes to write to the <paramref name="bytes"/> buffer.</param>
        /// <returns>The number of bytes placed into <paramref name="bytes"/>.</returns>
        public override System.Int32 Read(System.Byte[] bytes , System.Int32 offset , System.Int32 length)
            => Read(bytes, offset, length , out _);

        /// <summary>
        /// Gets the current time inside the data.
        /// </summary>
        public override TimeSpan CurrentTime
        {
            get => Interop.VorbisFile.ov_time_tell(native);
            set {
                Interop.VorbisFile.Errors err;
                if (seekable && (err = Interop.VorbisFile.ov_time_seek(native, value)) < 0) {
                    throw new InvalidOperationException($"The data could not be set at this time , please retry later. (Error {err})");
                }
            }
        }

        /// <summary>
        /// Gets the total time of the current stream.
        /// </summary>
        public override TimeSpan TotalTime => Interop.VorbisFile.ov_time_total(native, -1);

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                if (stream is null) { return; }
                // Allow native mem to be freed , even on VorbisFile catastrophic failures.
                // This could later cause AVE but I don't think that VorbisFile after the failure accesses anything else.
                try {
                    Interop.VorbisFile.Errors err;
                    if ((err = Interop.VorbisFile.ov_clear(native)) < 0)
                    {
                        throw new ExceptionSystem.VorbisWrapperException(err);
                    }
                } finally {
                    native?.Dispose();
                    native = null;
                    stream?.Dispose();
                    stream = null;
                }
            }
        }
    }
}