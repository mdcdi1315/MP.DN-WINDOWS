
using MP.ComInterop;
using System.Runtime.InteropServices;

namespace MP.AudioLibrary.MediaFoundation
{
    [ComImport]
    [Guid(MediaFoundationInterfaceIds.IID_IMFCollection)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe interface IMFCollection
    {
        [PreserveSig]
        public HRESULT GetElementCount(System.UInt32* pcElements);

        [PreserveSig]
        public HRESULT GetElement(System.UInt32 dwElementIndex, void** ppUnkElement);

        [PreserveSig]
        public HRESULT AddElement(void* pUnkElement);

        [PreserveSig]
        public HRESULT RemoveElement(System.UInt32 dwElementIndex, void** ppUnkElement);

        [PreserveSig]
        public HRESULT InsertElementAt(System.UInt32 dwIndex, void* pUnknown);

        [PreserveSig]
        public HRESULT RemoveAllElements();
    }
}