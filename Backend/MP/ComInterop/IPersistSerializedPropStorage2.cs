

using System.Runtime.InteropServices;

namespace MP.ComInterop
{
    [ComImport]
    [Guid(CommonInteropClsIds.IID_IPersistSerializedPropStorage2)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe interface IPersistSerializedPropStorage2 : IPersistSerializedPropStorage
    {
        [PreserveSig]
        public new HRESULT SetFlags(PERSIST_SPROPSTORE_FLAGS flags);

        [PreserveSig]
        public new HRESULT SetPropertyStorage(System.Byte* pdata, System.UInt32 sdata);

        [PreserveSig]
        public new HRESULT GetPropertyStorage(System.Byte** pdata, System.UInt32* sdata);

        [PreserveSig]
        public HRESULT GetPropertyStorageSize(System.UInt32* pcb);

        [PreserveSig] // Fails if cb is smaller than the total size of the serialized data.
        public HRESULT GetPropertyStorageBuffer(System.Byte** psps, System.UInt32 cb, System.UInt32* pcbWritten);
    }
}