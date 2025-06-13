
using System;
using MP.Annotations;
using System.Runtime.InteropServices;

namespace MP.ComInterop
{
    /// <summary>
    /// The IStream interface lets you read and write data to stream objects. 
    /// Stream objects contain the data in a structured storage object, where storages provide the structure. 
    /// Simple data can be written directly to a stream but, most frequently, streams are elements nested within a storage object. 
    /// They are similar to standard files. <br /> <br />
    /// 
    /// The IStream interface defines methods similar to the MS-DOS FAT file functions.
    /// For example, each stream object has its own access rights and a seek pointer.
    /// The main difference between a DOS file and a stream object is that in the latter 
    /// case, streams are opened using an IStream interface pointer rather than a file handle. <br /> <br />
    /// 
    /// The methods in this interface present your object's data as a contiguous sequence of bytes that you can read or write. 
    /// There are also methods for committing and reverting changes on streams that are open in transacted mode and methods 
    /// for restricting access to a range of bytes in the stream. <br /> <br />
    /// 
    /// Streams can remain open for long periods of time without consuming file-system resources.
    /// The IUnknown::Release method is similar to a close function on a file. 
    /// Once released, the stream object is no longer valid and cannot be used.
    /// </summary>
    [ComImport]
    [Guid(CommonInteropClsIds.IID_IStream)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe interface IStream : ISequentialStream
    {
        [PreserveSig]
        public new HRESULT Read(void* pv, System.UInt32 cb, System.UInt32* pcbread);

        [PreserveSig]
        public new HRESULT Write(void* pv, System.UInt32 cb, System.UInt32* pcbwritten);

        [PreserveSig]
        public HRESULT Seek(System.Int64 dlibmove , STREAM_SEEK origin , System.UInt64* plibnewposition);

        [PreserveSig]
        public HRESULT SetSize(System.UInt64 newsize);

        [PreserveSig]
        public HRESULT CopyTo([IsPointerToCOMInterfaceType(typeof(IStream))] void* output, System.UInt64 cb, System.UInt64* pbytesread, System.UInt64* pbyteswritten);

        [PreserveSig]
        public HRESULT Commit(STGC grfflags);

        [PreserveSig]
        public HRESULT Revert();

        [PreserveSig]
        public HRESULT LockRegion(System.UInt64 liboffset, System.UInt64 cb, LOCKTYPE locktype);

        [PreserveSig]
        public HRESULT UnlockRegion(System.UInt64 liboffset, System.UInt64 cb, LOCKTYPE locktype);

        [PreserveSig]
        public HRESULT Stat(STATSTG* pstatstg, STATFLAG grfStatFlag);

        [PreserveSig]
        public HRESULT Clone([IsPointerToCOMInterfaceType(typeof(IStream))] void** cloned);
    }
}
