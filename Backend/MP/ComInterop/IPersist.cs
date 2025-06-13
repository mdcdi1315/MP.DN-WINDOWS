using MP.WindowsInterop;
using System;
using System.Runtime.InteropServices;

namespace MP.ComInterop
{
    [ComImport]
    [Guid(CommonInteropClsIds.IID_IPersist)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IPersist
    {
        [PreserveSig]
        public unsafe HRESULT GetClassID(GUID* clsid);
    }
}
