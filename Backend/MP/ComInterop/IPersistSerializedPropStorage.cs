

using System.Runtime.InteropServices;

namespace MP.ComInterop
{
    [ComImport]
    [Guid(CommonInteropClsIds.IID_IPersistSerializedPropStorage)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe interface IPersistSerializedPropStorage
    {
        [PreserveSig]
        public HRESULT SetFlags(PERSIST_SPROPSTORE_FLAGS flags);

        [PreserveSig]
        public HRESULT SetPropertyStorage(System.Byte* pdata , System.UInt32 sdata);

        [PreserveSig]
        public HRESULT GetPropertyStorage(System.Byte** pdata , System.UInt32* sdata);
    }
}