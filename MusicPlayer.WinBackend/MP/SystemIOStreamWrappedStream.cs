
using System;
using MP.Annotations;
using MP.NativeInterop;
using MP.NativeInterop.Windows;
using MP.NativeInterop.Windows.COM;

namespace MP
{
    public unsafe sealed class SystemIOStreamWrappedStream : IStream
    {
        private readonly FILETIME creation_time;
        private readonly System.IO.Stream stream;
        private FILETIME access_time, modification_time;

        public SystemIOStreamWrappedStream(System.IO.Stream s)
        {
            ArgumentNullException.ThrowIfNull(s);
            stream = s;
            modification_time = access_time = creation_time = FILETIME.FromDateTime(SystemInfo.UtcNow);
        }

        public HRESULT Clone([IsPointerToCOMInterfaceType(typeof(IStream))] void** ppstm)
        {
            *ppstm = ComInterop.CreateCCW<IStream>(this);
            return CommonHResults.S_OK;
        }

        public HRESULT Commit(STGC grfCommitFlags) => CommonHResults.S_OK;

        public HRESULT CopyTo([IsPointerToCOMInterfaceType(typeof(IStream))] void* pstm, ulong cb, ulong* pcbRead, ulong* pcbWritten)
        {
            throw new System.NotImplementedException();
        }

        public HRESULT LockRegion(ulong libOffset, ulong cb, LOCKTYPE dwLockType) => CommonHResults.E_NOTIMPL;

        public HRESULT UnlockRegion(ulong libOffset, ulong cb, LOCKTYPE dwLockType) => CommonHResults.E_NOTIMPL;

        public HRESULT Read(void* buffer, uint n_bytes, uint* p_read)
        {
            if (buffer is null) {
                return CommonHResults.STG_E_INVALIDPOINTER;
            } else if (n_bytes > System.Int32.MaxValue) {
                return CommonHResults.E_INVALIDARG;
            } else {
                int r = stream.Read(new System.Span<System.Byte>(buffer, n_bytes.ToInt32()));
                if (p_read is not null) { *p_read = r.ToUInt32(); }
                access_time = FILETIME.FromDateTime(SystemInfo.UtcNow);
                return CommonHResults.S_OK;
            }
        }

        public HRESULT Revert() => CommonHResults.S_OK;

        public HRESULT Seek(long dlibMove, STREAM_SEEK dwOrigin, ulong* plibNewPosition)
        {
            if (!stream.CanSeek) { return CommonHResults.E_FAIL; }
            long value;
            try {
                switch (dwOrigin)
                {
                    case STREAM_SEEK.STREAM_SEEK_SET:
                        value = stream.Seek(dlibMove, System.IO.SeekOrigin.Begin);
                        break;
                    case STREAM_SEEK.STREAM_SEEK_CUR:
                        value = stream.Seek(dlibMove, System.IO.SeekOrigin.Current);
                        break;
                    case STREAM_SEEK.STREAM_SEEK_END:
                        value = stream.Seek(dlibMove, System.IO.SeekOrigin.End);
                        break;
                    default:
                        return CommonHResults.STG_E_INVALIDFUNCTION;
                }
                if (plibNewPosition is not null) {
                    *plibNewPosition = value.ToUInt64();
                }
            } catch (ArgumentException) {
                return CommonHResults.STG_E_INVALIDFUNCTION;
            }
            return CommonHResults.S_OK;
        }

        public HRESULT SetSize(ulong libNewSize)
        {
            if (libNewSize > System.Int64.MaxValue) {
                return CommonHResults.E_INVALIDARG;
            } else {
                try {
                    stream.SetLength(libNewSize.ToInt64());
                    return CommonHResults.S_OK;
                } catch (System.IO.IOException) {
                    return CommonHResults.STG_E_WRITEFAULT;
                } catch (NotSupportedException) {
                    return CommonHResults.E_FAIL;
                }
            }
        }

        public HRESULT Stat(STATSTG* pstatstg, STATFLAG grfStatFlag)
        {
            if (pstatstg is null) {
                return CommonHResults.STG_E_INVALIDPOINTER;
            } else {
                if (!grfStatFlag.HasFlag(STATFLAG.STATFLAG_NONAME)) {
                    try {
                        var sh = WindowsCOMLibrary.COMMemoryManager.CreateUTF16LEStringHandle("EDT_WAPPED_MP_DN8_BACKEND");
                        pstatstg->pwcsName = sh;
                        sh.SetHandleAsInvalid();
                        sh = null;
                    } catch (InsufficientMemoryException) {
                        return CommonHResults.STG_E_INSUFFICIENTMEMORY;
                    }
                }
                pstatstg->type = STGTY.STGTY_STREAM;
                try {
                    pstatstg->cbSize = stream.Length.ToUInt64();
                } catch (NotSupportedException) {
                    return CommonHResults.E_FAIL;
                }
                pstatstg->grfMode = 0;
                pstatstg->grfStateBits = 0;
                pstatstg->grfLocksSupported = 0;
                pstatstg->atime = access_time;
                pstatstg->ctime = creation_time;
                pstatstg->mtime = modification_time;

                return CommonHResults.S_OK;
            }
        }

        public HRESULT Write(void* buffer, uint n_bytes, uint* p_written)
        {
            if (n_bytes > System.Int32.MaxValue) {
                return CommonHResults.E_INVALIDARG;
            } else {
                stream.Write(new System.Span<System.Byte>(buffer, n_bytes.ToInt32()));
                modification_time = FILETIME.FromDateTime(SystemInfo.UtcNow);
                return CommonHResults.S_OK;
            }
        }
    }
}