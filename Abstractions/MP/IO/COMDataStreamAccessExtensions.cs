
using System;
using MP.Utilities;
using MP.IO.Buffers;
using MP.Annotations;
using MP.NativeInterop;
using MP.NativeInterop.Windows;
using MP.NativeInterop.Windows.COM;

namespace MP.IO
{
    /// <summary>
    /// Provides several extension methods for the <see cref="IDataStreamAccess"/> interface and Windows COM.
    /// </summary>
    public static class COMDataStreamAccessExtensions
    {
        private sealed class DataStreamAccess2SequentialStream : ISequentialStream
        {
            private readonly IDataStreamAccess access;

            public DataStreamAccess2SequentialStream(IDataStreamAccess access) => this.access = access;

            public unsafe HRESULT Read(void* buffer, uint n_bytes, uint* p_read)
            {
                if (n_bytes > System.Int32.MaxValue) {
                    return CommonHResults.E_INVALIDARG;
                } else {
                    try {
                        int read = access.Read(new Span<byte>(buffer, n_bytes.ToInt32()));
                        if (read == -1) {
                            if (p_read is not null) { *p_read = 0U; }
                            return CommonHResults.S_FALSE;
                        } else {
                            uint u_read = read.ToUInt32();
                            if (p_read is not null) { *p_read = u_read; }
                            if (u_read < n_bytes) {
                                return CommonHResults.S_FALSE;
                            } else {
                                return CommonHResults.S_OK;
                            }
                        }
                    } catch (UnauthorizedAccessException) {
                        if (p_read is not null) { *p_read = 0U; }
                        return CommonHResults.STG_E_ACCESSDENIED;
                    } catch (IOException) {
                        if (p_read is not null) { *p_read = 0U; }
                        return CommonHResults.STG_E_READFAULT;
                    }
                }
            }

            public unsafe HRESULT Write(void* buffer, uint n_bytes, uint* p_written)
            {
                if (buffer is null) {
                    return CommonHResults.STG_E_INVALIDPOINTER;
                } else if (n_bytes > System.Int32.MaxValue) {
                    return CommonHResults.E_INVALIDARG;
                } else {
                    try {
                        access.Write(new ReadOnlySpan<byte>(buffer, n_bytes.ToInt32()));
                        if (p_written is not null) { *p_written = n_bytes; }
                        return CommonHResults.S_OK;
                    } catch (UnauthorizedAccessException) {
                        if (p_written is not null) { *p_written = 0U; }
                        return CommonHResults.STG_E_ACCESSDENIED;
                    } catch (IOException) {
                        if (p_written is not null) { *p_written = 0U; }
                        return CommonHResults.STG_E_WRITEFAULT;
                    }
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "COM helpers")]
        private sealed class DataStream2Stream : IStream
        {
            private const int STGM_READ = 0x00000000;
            private const int STGM_WRITE = 0x00000001;
            private const int STGM_READWRITE = 0x00000002;

            private readonly DataStream sd;
            private readonly FILETIME crt_time;
            private FILETIME acc_time, mod_time;

            public DataStream2Stream(DataStream d)
            {
                sd = d;
                crt_time = mod_time = acc_time = FILETIME.FromDateTime(SystemInfo.Now);
            }

            public unsafe HRESULT CopyTo([IsPointerToCOMInterfaceType(typeof(IStream))] void* pstm, ulong cb, ulong* pcbRead, ulong* pcbWritten)
            {
                if (pstm is null) {
                    return CommonHResults.STG_E_INVALIDPOINTER;
                } else if (ComInterop.TryCreateRCW(pstm, out IStream target)) {

                    uint w, r;
                    HRESULT hr;
                    int rdt, buf_size;
                    ulong rd = 0UL, wr = 0UL;

                    using (var cxt = System.Buffers.ArrayPool<System.Byte>.Shared.RentByContext(2048))
                    {
                        Span<System.Byte> buf = cxt.Buffer;
                        buf_size = cxt.ActualLength;

                        try {
                            while (rd < cb)
                            {
                                rdt = sd.Read(buf.Slice(0, MathHelpers.ComputeBufferSize(rd, cb, buf_size)));
                                if (rdt > 0) {
                                    fixed (System.Byte* p = buf) { hr = target.Write(p, r = rdt.ToUInt32(), &w); }
                                    if (hr.FAILED) { return hr; }
                                    wr += w;
                                    rd += r;
                                } else {
                                    break;
                                }
                            }
                        } catch (IOException) {
                            return CommonHResults.STG_E_READFAULT;
                        } catch (NotSupportedException) {
                            return CommonHResults.STG_E_INVALIDFUNCTION;
                        }
                    }

                    if (pcbRead is not null) { *pcbRead = rd; }
                    if (pcbWritten is not null) { *pcbWritten = wr; }
                    return CommonHResults.S_OK;
                } else {
                    return CommonHResults.E_UNEXPECTED;
                }
            }

            public unsafe HRESULT Read(void* buffer, uint n_bytes, uint* p_read)
            {
                if (n_bytes > System.Int32.MaxValue) {
                    return CommonHResults.E_INVALIDARG;
                } else {
                    try {
                        int read = sd.Read(new Span<byte>(buffer, n_bytes.ToInt32()));
                        if (read == -1) {
                            if (p_read is not null) { *p_read = 0U; }
                            return CommonHResults.S_FALSE;
                        } else {
                            uint u_read = read.ToUInt32();
                            acc_time = FILETIME.FromDateTime(SystemInfo.Now);
                            if (p_read is not null) { *p_read = u_read; }
                            if (u_read < n_bytes) {
                                return CommonHResults.S_FALSE;
                            } else {
                                return CommonHResults.S_OK;
                            }
                        }
                    } catch (NotSupportedException) {
                        if (p_read is not null) { *p_read = 0U; }
                        return CommonHResults.STG_E_ACCESSDENIED;
                    } catch (IOException) {
                        if (p_read is not null) { *p_read = 0U; }
                        return CommonHResults.STG_E_READFAULT;
                    }
                }
            }

            public unsafe HRESULT Write(void* buffer, uint n_bytes, uint* p_written)
            {
                if (buffer is null) {
                    return CommonHResults.STG_E_INVALIDPOINTER;
                } else if (n_bytes > System.Int32.MaxValue) {
                    return CommonHResults.E_INVALIDARG;
                } else {
                    try {
                        sd.Write(new ReadOnlySpan<byte>(buffer, n_bytes.ToInt32()));
                        mod_time = FILETIME.FromDateTime(SystemInfo.Now);
                        if (p_written is not null) { *p_written = n_bytes; }
                        return CommonHResults.S_OK;
                    } catch (UnauthorizedAccessException) {
                        if (p_written is not null) { *p_written = 0U; }
                        return CommonHResults.STG_E_ACCESSDENIED;
                    } catch (IOException) {
                        if (p_written is not null) { *p_written = 0U; }
                        return CommonHResults.STG_E_WRITEFAULT;
                    }
                }
            }

            public unsafe HRESULT Seek(long dlibMove, STREAM_SEEK dwOrigin, ulong* plibNewPosition)
            {
                if (plibNewPosition is not null) { *plibNewPosition = 0UL; }
                try {
                    ulong u = sd.Seek(dlibMove, (SeekDisplacement)dwOrigin).ToUInt64();
                    if (plibNewPosition is not null) { *plibNewPosition = u; }
                    return CommonHResults.S_OK;
                } catch (UnauthorizedAccessException) {
                    return CommonHResults.STG_E_ACCESSDENIED;
                } catch (IOException) {
                    return CommonHResults.STG_E_SEEKERROR;
                } catch (ArgumentException) {
                    return CommonHResults.E_INVALIDARG;
                }
            }

            public HRESULT SetSize(ulong libNewSize)
            {
                if (libNewSize > System.Int64.MaxValue) {
                    return CommonHResults.E_INVALIDARG;
                } else if (sd is AbstractFileStream afs) {
                    try {
                        afs.SetLength(libNewSize.ToInt64());
                        return CommonHResults.S_OK;
                    } catch (NotSupportedException) {
                        return CommonHResults.STG_E_ACCESSDENIED;
                    }
                } else {
                    return CommonHResults.S_OK;
                }
            }

            public unsafe HRESULT Stat(STATSTG* pstatstg, STATFLAG grfStatFlag)
            {
                if (pstatstg is null) {
                    return CommonHResults.STG_E_INVALIDPOINTER;
                } else {
                    if (!grfStatFlag.HasFlag(STATFLAG.STATFLAG_NONAME)) {
                        try {
                            var sh = SystemInfo.LayerUsed.GetMemoryManager(COMMemoryManager.NAME).CreatePlatformDependentUTF16StringHandle("MP_DN8_STREAM_DOTNET");
                            pstatstg->pwcsName = sh;
                            sh.SetHandleAsInvalid();
                            sh = null;
                        } catch (InsufficientMemoryException) {
                            return CommonHResults.STG_E_INSUFFICIENTMEMORY;
                        }
                    }
                    
                    try {
                        pstatstg->cbSize = sd.Length.ToUInt64();
                    } catch (NotSupportedException) {
                        return CommonHResults.E_FAIL;
                    }

                    uint m = 0U;
                    if (sd.Mode.HasFlag(DataStreamMode.Read)) { m |= STGM_READ; }
                    if (sd.Mode.HasFlag(DataStreamMode.Write)) { m |= STGM_WRITE; }
                    if (sd.Mode.HasFlag(DataStreamMode.Seek)) { m |= STGM_READWRITE; }
                    pstatstg->grfMode = m;
                    pstatstg->grfStateBits = 0U;
                    pstatstg->grfLocksSupported = 0U;
                    pstatstg->type = STGTY.STGTY_STREAM;
                    pstatstg->ctime = crt_time;
                    pstatstg->atime = acc_time;
                    pstatstg->mtime = mod_time;

                    return CommonHResults.S_OK;
                }
            }

            public HRESULT Commit(STGC grfCommitFlags)
            {
                if (sd is AbstractFileStream afs) { afs.Flush(); }
                return CommonHResults.S_OK;
            }

            public HRESULT Revert() => CommonHResults.STG_E_INVALIDFUNCTION;

            public HRESULT LockRegion(ulong libOffset, ulong cb, LOCKTYPE dwLockType) => CommonHResults.STG_E_INVALIDFUNCTION;

            public HRESULT UnlockRegion(ulong libOffset, ulong cb, LOCKTYPE dwLockType) => CommonHResults.STG_E_INVALIDFUNCTION;

            public unsafe HRESULT Clone([IsPointerToCOMInterfaceType(typeof(IStream))] void** ppstm) => CommonHResults.STG_E_INVALIDFUNCTION;
        }

        /// <summary>
        /// Returns a new instance of the <see cref="ISequentialStream"/> interface that can be used to wrap the current <see cref="IDataStreamAccess"/> object in COM.
        /// </summary>
        /// <remarks>
        /// It should be noted down that the returned object does not own the stream; as such, when this is used 
        /// in COM scenarios, when the object will be released, it won't release the current <see cref="IDataStreamAccess"/> object.
        /// </remarks>
        /// <param name="access">The data stream access object to wrap.</param>
        /// <returns>A new instance of the <see cref="ISequentialStream"/> interface that wraps the current data stream access object.</returns>
        public static ISequentialStream ToCOMSequentialStream(this IDataStreamAccess access)
        {
            ArgumentNullException.ThrowIfNull(access);
            return new DataStreamAccess2SequentialStream(access);
        }

        /// <summary>
        /// Returns a new instance of the <see cref="IStream"/> interface that can be used to wrap the current <see cref="DataStream"/> object in COM.
        /// </summary>
        /// <remarks>
        /// It should be noted down that the returned object does not own the stream; as such, when this is used 
        /// in COM scenarios, when the object will be released, it won't release the current <see cref="DataStream"/> object.
        /// </remarks>
        /// <param name="dsm">The data stream object to wrap.</param>
        /// <returns>A new instance of the <see cref="DataStream"/> object that wraps the current data stream access object.</returns>
        public static IStream ToCOMStream(this DataStream dsm)
        {
            ArgumentNullException.ThrowIfNull(dsm);
            return new DataStream2Stream(dsm);
        }
    }
}