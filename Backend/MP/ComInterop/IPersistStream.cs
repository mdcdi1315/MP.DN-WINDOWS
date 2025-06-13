
using System;
using MP.Annotations;
using MP.WindowsInterop;
using System.Runtime.InteropServices;

namespace MP.ComInterop
{
    [ComImport]
    [Guid(CommonInteropClsIds.IID_IPersistStream)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe interface IPersistStream : IPersist
    {
        [PreserveSig] // From IPersist
        public new unsafe HRESULT GetClassID(GUID* clsid);

        [PreserveSig]
        public HRESULT IsDirty();

        [PreserveSig]
        public HRESULT Load([IsPointerToCOMInterfaceType(typeof(IStream))] void* ppstm); // ppstm must be an IStream* pointer

        [PreserveSig]
        public HRESULT Save([IsPointerToCOMInterfaceType(typeof(IStream))] void* ppstm , BOOL fcleardirty); // ppstm must be an IStream* pointer

        [PreserveSig]
        public unsafe HRESULT GetSizeMax(System.UInt64* size);
    }
}
