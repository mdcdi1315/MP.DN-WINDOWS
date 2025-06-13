
using System;
using MP.ComInterop;
using System.Runtime.CompilerServices;

namespace MP
{
    /// <summary>
    /// This is the base music player stream that is used by the app to initialize the Media Foundation player. <br />
    /// To support such native code , the stream also partially implements the <see cref="IStream"/> interface. <br />
    /// Note that the class does not override , inherit or add any new functionality; for definitions and doc on how to use, <br />
    /// see the <see cref="AbstractPropertyStream"/> abstract class.
    /// </summary>
    public sealed unsafe class MusicPlayerStream : AbstractPropertyStream , IStream
    {
        private const System.String StreamCreationTimeStamp = "CreationTime";

        /// <summary>
        /// Creates a new instance of the <see cref="MusicPlayerStream"/> class by specifying the stream to wrap.
        /// </summary>
        /// <param name="wrappingstream">The stream to wrap.</param>
        public MusicPlayerStream(System.IO.Stream wrappingstream) : this(wrappingstream, false) { }

        public MusicPlayerStream(System.IO.Stream wrappingstream, System.Boolean synchronize) : base(wrappingstream, synchronize) {
            SetAttribute(StreamCreationTimeStamp, SystemInfo.Now);
        }

        /// <summary>
        /// Creates a new <see cref="MusicPlayerStream"/> object by using a memory stream.
        /// </summary>
        /// <param name="initcap">The initial memory stream capacity.</param>
        /// <returns>A music player stream created from a memory stream.</returns>
        public static MusicPlayerStream CreateMemoryStream(System.Int64 initcap)
        {
            MusicPlayerStream created = new(new Microsoft.IO.MemoryStream(initcap) , false);
            created.SetBooleanAttribute(IsStreamOwnerForced, true);
            return created;
        }

        HRESULT IStream.Seek(long dlibmove, STREAM_SEEK orig, System.UInt64* plibnewposition)
        {
            long val = Seek(dlibmove, (System.IO.SeekOrigin)orig);
            if (plibnewposition is not null) { *plibnewposition = val.ToUInt64(); }
            return CommonHResults.S_OK;
        }

        HRESULT IStream.SetSize(ulong newsize)
        {
            // Implementation limitation , due to .NET definitions.
            if (newsize > System.Int64.MaxValue)
            {
                return CommonHResults.E_INVALIDARG;
            }
            Wrapped.SetLength(newsize.ToInt64());
            return CommonHResults.S_OK;  
        }

        HRESULT IStream.CopyTo(void* output, ulong cb, ulong* pbytesread, ulong* pbyteswritten)
        {
            System.UInt64 br = 0, bw = 0;
            HRESULT hr;
            const int BUFFER_SIZE = 4096;
            IStream other = ComMarshalling.CreateInteropObject(output , -1) as IStream;
            try
            {
                System.Byte[] temp = new System.Byte[BUFFER_SIZE];
                System.UInt32 brt, bwt;
                // Copy the data using buffered techniques
                do
                {
                    // Read up to BUFFER_SIZE bytes
                    brt = Wrapped.Read(temp, 0, BUFFER_SIZE).ToUInt32();
                    if (brt == 0) { break; } // Exit in the event that the stream has reached it's end.
                    // brt has the actual bytes read.
                    br += brt;
                    fixed (System.Byte* src = temp)
                    {
                        // Write these read bytes to the resulting stream.
                        hr = other.Write(src, brt, &bwt);
                        // If failed , do not throw any exceptions but return the given code back to the caller , to know what went so wrong.
                        if (hr.FAILED) { return hr; }
                    }
                    bw += bwt;
                    // do this while the wrapped stream has still data
                } while (brt > 0);
                // Destroy internal states
                temp = null;
                // Write count variables if supported
                if (pbytesread is not null) { *pbytesread = br; }
                if (pbyteswritten is not null) { *pbyteswritten = bw; }
            } finally {
                ComMarshalling.ReleaseInteropObject(other);
            }
            return CommonHResults.S_OK; // Suggests successfull operation.
        }

        HRESULT IStream.Commit(STGC grfflags) => CommonHResults.S_OK;

        HRESULT IStream.Revert() => CommonHResults.S_OK;

        HRESULT IStream.LockRegion(ulong liboffset, ulong cb, LOCKTYPE locktype) => CommonHResults.S_OK;

        HRESULT IStream.UnlockRegion(ulong liboffset, ulong cb, LOCKTYPE locktype) => CommonHResults.S_OK;

        HRESULT IStream.Stat(STATSTG* pstatstg, STATFLAG grfStatFlag)
        {
            if (pstatstg is null)
            {
                // Return what is expected in this case by the clients: STG_E_INVALIDPOINTER.
                return CommonHResults.STG_E_INVALIDPOINTER;
            }

            const int STGM_READ = 0x00000000;
            const int STGM_WRITE = 0x00000001;
            const int STGM_READWRITE = 0x00000002;

            var tmp = grfStatFlag == STATFLAG.STATFLAG_DEFAULT ?  
                STATSTG.CreateStruct("EDT_STREAM_MP_DOTNET8") : 
                STATSTG.CreateStructWithoutName();

            tmp.Type = STGTY.STGTY_STREAM;
            tmp.StreamSize = Wrapped.Length.ToUInt64();
            tmp.CreationTime = (DateTime)GetAttribute(StreamCreationTimeStamp);
            tmp.LockTypesSupported = 0;
            tmp.StreamMode = 0;

            if (Wrapped.CanWrite && Wrapped.CanRead)
                tmp.StreamMode = STGM_READWRITE;
            else if (Wrapped.CanRead)
                tmp.StreamMode = STGM_READ;
            else if (Wrapped.CanWrite)
                tmp.StreamMode = STGM_WRITE;
            else
                throw new ObjectDisposedException("Stream");

            *pstatstg = tmp;
            return CommonHResults.S_OK;
        }

        // Because this is a Storage API , the STG_E_UNIMPLEMENTEDFUNCTION code should be returned instead.
        HRESULT IStream.Clone(void** cloned) => CommonHResults.STG_E_UNIMPLEMENTEDFUNCTION;

        private HRESULT ReadNativeCommon(void* pv, System.UInt32 cb, System.UInt32* pcbread)
        {
            if (pv is null) {
                return CommonHResults.STG_E_INVALIDPOINTER;
            }
            if (cb < 0) {
                return CommonHResults.E_INVALIDARG;
            }
            System.Byte[] data = new System.Byte[cb];
            // Read from managed stream
            int wr = Wrapped.Read(data, 0, data.Length);
            System.UInt32 pwr = wr.ToUInt32();
            // Write to native memory
            fixed (System.Byte* src = data) { Unsafe.CopyBlockUnaligned(pv, src, pwr); }
            data = null;
            // Write the counter variable, if that is a valid address.
            if (pcbread is not null) { *pcbread = pwr; }
            return CommonHResults.S_OK;
        }

        private HRESULT WriteNativeCommon(void* pv , System.UInt32 cb , System.UInt32* pcbwritten)
        {
            if (pv is null) {
                return CommonHResults.STG_E_INVALIDPOINTER;
            }
            if (cb < 0) {
                return CommonHResults.E_INVALIDARG;
            }
            System.Int64 prevpos = Wrapped.Position;
            System.Byte[] temp = new System.Byte[cb];
            // Read from unmanaged memory and put the data to managed memory.
            fixed (System.Byte* dst = temp) { Unsafe.CopyBlockUnaligned(dst, pv, cb); }
            // Write the converted managed memory to the managed stream
            Wrapped.Write(temp, 0, temp.Length);
            // Unallocate the array
            temp = null;
            // Write the counter variable
            if (pcbwritten is not null) { *pcbwritten = (Wrapped.Position - prevpos).ToUInt32(); }
            return CommonHResults.S_OK;
        }

        HRESULT IStream.Read(void* pv, uint cb, uint* pcbread) => ReadNativeCommon(pv, cb, pcbread);

        HRESULT IStream.Write(void* pv, uint cb, uint* pcbwritten) => WriteNativeCommon(pv, cb, pcbwritten);

        HRESULT ISequentialStream.Read(void* pv, uint cb, uint* pcbread) => ReadNativeCommon(pv , cb, pcbread);

        HRESULT ISequentialStream.Write(void* pv, uint cb, uint* pcbwritten) => WriteNativeCommon(pv , cb, pcbwritten);
    }
}
