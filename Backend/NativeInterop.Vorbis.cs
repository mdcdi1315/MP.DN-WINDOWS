
using MP;
using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;

// size_t correponds to System.UInt32 on x86 and to System.UInt64 on x64 or ARM64

partial class Interop
{

    public static unsafe class VorbisFile
    {
        // Prototypes for reading a stream with Vorbis
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate System.UInt64 ReadFunction(void* buffer, System.UInt64 singleelementsize, System.UInt64 elementcount, void* pointerunused);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate System.Int32 SeekFunction(void* pointerunused, System.Int64 offset, System.IO.SeekOrigin origin);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate System.Int32 CloseFunction(void* pointerunused);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate System.Int32 TellFunction(void* pointerunused);

        /// <summary>
        /// VorbisFile errors. These hard-coded values do remind me the errno Unix codes.
        /// </summary>
        public enum Errors : System.Int32
        {
            FALSE = -1,
            EOF = -2,
            HOLE = -3,
            EREAD = -128,
            EFAULT = -129,
            EIMPL = -130,
            EINVAL = -131,
            ENOTVORBIS = -132,
            EBADHEADER = -133,
            EVERSION = -134,
            ENOTAUDIO = -135,
            EBADPACKET = -136,
            EBADLINK = -137,
            ENOSEEK = -138
        }

        // Force packing = 1 where as possible

        // Defines the native callback functions that Vorbis will use to read from a stream.
        // Practical and it works with managed streams perfectly.
        [StructLayout(LayoutKind.Explicit , Size = 32 , Pack = 1)]
        public struct OV_CALLBACKS
        {
            [FieldOffset(0)]
            public System.IntPtr read; // Define it as IntPtr to just pass around the delegates as function pointers

            [FieldOffset(8)]
            public System.IntPtr seek;

            [FieldOffset(16)]
            public System.IntPtr close;

            [FieldOffset(24)]
            public System.IntPtr tell;
        }

        // Vorbis information for the current bitstream.
        [StructLayout(LayoutKind.Explicit , Size = 36 , Pack = 1)]
        public struct VORBIS_INFO
        {
            [FieldOffset(0)]
            public System.Int32 Version;

            [FieldOffset(4)]
            public System.Int32 Channels; // # of PCM channels

            [FieldOffset(8)]
            public System.Int32 Rate; // PCM sample rate

            [FieldOffset(12)]
            public System.Int32 BitRate_Upper;

            [FieldOffset(16)]
            public System.Int32 BitRate_Nomimal;

            [FieldOffset(20)]
            public System.Int32 BitRate_Lower;

            [FieldOffset(24)]
            public System.Int32 BitRate_Window;

            [FieldOffset(28)]
            public void* CodecSetupPtr; // DO NOT ACCESS THIS POINTER IT IS LIKELY TO CAUSE AN AVE
        }

        [DllImport(Libraries.VorbisFile , CallingConvention = CallingConvention.Cdecl , EntryPoint = "ov_open_callbacks" , ExactSpelling = true)]
        private static extern Errors ov_open_callbacks_Native(void* userdata, void* filestruct, System.Char* initialunused, System.Int32 readbytesunused, OV_CALLBACKS* callbacks);

        [DllImport(Libraries.VorbisFile , CallingConvention = CallingConvention.Cdecl , EntryPoint = "ov_clear", ExactSpelling = true)]
        private static extern Errors ov_clear_Native(void* filestruct);

        // ExactSpelling to avoid bottlenecks as most as possible.
        [DllImport(Libraries.VorbisFile , CallingConvention = CallingConvention.Cdecl , EntryPoint = "ov_read", ExactSpelling = true)]
        private static extern Errors ov_read_Native(void* filestruct , System.Byte* buffer , System.Int32 buffersize , System.Int32 endianess , System.Int32 word , System.Int32 signed , System.Int32* bitstream);

        [DllImport(Libraries.VorbisFile, CallingConvention = CallingConvention.Cdecl, EntryPoint = "ov_time_seek", ExactSpelling = true)]
        private static extern Errors ov_time_seek_Native(void* filestruct, System.Double time);

        [DllImport(Libraries.VorbisFile, CallingConvention = CallingConvention.Cdecl, EntryPoint = "ov_time_total", ExactSpelling = true)]
        private static extern System.Double ov_time_total_Native(void* filestruct , System.Int32 bitstream);

        [DllImport(Libraries.VorbisFile, CallingConvention = CallingConvention.Cdecl, EntryPoint = "ov_time_tell", ExactSpelling = true)]
        private static extern System.Double ov_time_tell_Native(void* filestruct);

        // Queries whether the current stream is seekable.
        // Declared as BOOL because FALSE means that the stream is not seekable.
        [DllImport(Libraries.VorbisFile, CallingConvention = CallingConvention.Cdecl, EntryPoint = "ov_seekable", ExactSpelling = true)]
        private static extern BOOL ov_seekable_Native(void* filestruct);

        [DllImport(Libraries.VorbisFile, CallingConvention = CallingConvention.Cdecl, EntryPoint = "ov_streams", ExactSpelling = true)]
        private static extern System.Int32 ov_streams_Native(void* filestruct);

        [DllImport(Libraries.VorbisFile, CallingConvention = CallingConvention.Cdecl, EntryPoint = "ov_info", ExactSpelling = true)]
        private static extern VORBIS_INFO* ov_info_Native(void* filestruct , System.Int32 bitstream);

        public static System.Boolean ov_seekable(SafeLibcMemoryHandle filestruct)
            => ov_seekable_Native(filestruct.MemoryPointer) != BOOL.FALSE;

        public static System.Int32 ov_streams(SafeLibcMemoryHandle filestruct)
            => ov_streams_Native(filestruct.MemoryPointer);

        public static VORBIS_INFO ov_info(SafeLibcMemoryHandle filestruct , System.Int32 bitstream)
        {
            // Allocated by the Vorbis library.
            VORBIS_INFO* v = ov_info_Native(filestruct.MemoryPointer, bitstream);
            if (v is null) { return default; }
            return *v;
        }

        public static System.TimeSpan ov_time_total(SafeLibcMemoryHandle filestruct, System.Int32 stream) => System.TimeSpan.FromSeconds(ov_time_total_Native(filestruct.MemoryPointer, stream));

        public static System.TimeSpan ov_time_tell(SafeLibcMemoryHandle filestruct) => System.TimeSpan.FromSeconds(ov_time_tell_Native(filestruct.MemoryPointer));

        public static Errors ov_time_seek(SafeLibcMemoryHandle filestruct, System.TimeSpan time) => ov_time_seek_Native(filestruct.MemoryPointer, time.TotalSeconds);

        public static Errors ov_read(SafeLibcMemoryHandle filestruct , System.Byte[] buffer , System.Int32 index, System.Int32 length , System.Boolean bigendian , System.Int32 word , System.Boolean signed , out System.Int32 bitstream)
        {
            bitstream = 0;
            fixed (System.Byte* dst = &buffer[index]) 
            {
                System.Int32 bsm;
                Errors err = ov_read_Native(filestruct.MemoryPointer, dst, length, bigendian ? 1 : 0, word, signed ? 1 : 0, &bsm);
                bitstream = bsm;
                return err;
            }
        }

        public static Errors ov_open_callbacks(OV_CALLBACKS callbacks, out SafeLibcMemoryHandle memhandle)
        {
            // Structure size in 64-bit machines is 840 bytes, however make it more to avoid bad pointer access issues
            // The vorbis creation MUST happen on native mem to definitely avoid GC issues.
            // Vorbis does make heavy use of this mem portion. Expand this value as required.
            memhandle = new(1000); 
            memhandle.ZeroMemory(); // Make memory full of zeroes
            // The first parameter must be arbitrarily set to a value so that vorbisfile consider it valid to continue with the read from it
            // (although that we will not even ever use it)
            return ov_open_callbacks_Native((void*)10, memhandle.MemoryPointer, null, 0, &callbacks);
        }

        public static Errors ov_clear(SafeLibcMemoryHandle memhandle) => ov_clear_Native(memhandle.MemoryPointer);
    }

}
