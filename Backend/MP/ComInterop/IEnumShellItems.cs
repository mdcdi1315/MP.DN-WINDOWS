

using System.Runtime.InteropServices;

namespace MP.ComInterop
{
    [ComImport]
    [Guid(CommonInteropClsIds.IID_IEnumShellItems)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe interface IEnumShellItems
    {
        [PreserveSig]
        public HRESULT Next(System.UInt32 celt , void** rgelt , System.UInt32 pceltfetched);

        [PreserveSig]
        public HRESULT Skip(System.UInt32 celt);

        [PreserveSig]
        public HRESULT Reset();

        [PreserveSig]
        public HRESULT Clone(void** ppenum);
    }
}