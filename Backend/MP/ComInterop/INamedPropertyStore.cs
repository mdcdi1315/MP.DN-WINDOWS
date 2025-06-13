

using System.Runtime.InteropServices;

namespace MP.ComInterop
{
    // INamedPropertyStore COM interface declaration.
    // For typical application and usage, see the NamedPropertyStore , which is it's interop class.
    [ComImport]
    [Guid(CommonInteropClsIds.IID_INamedPropertyStore)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe interface INamedPropertyStore
    {
        [PreserveSig]
        public HRESULT GetNamedValue(System.Char* pszName, PROPVARIANT* ppropvar);

        [PreserveSig]
        public HRESULT SetNamedValue(System.Char* pszName, PROPVARIANT* propvar);

        [PreserveSig]
        public HRESULT GetNameCount(System.UInt32* pdwCount);

        // BEWARE: pbstrName is a BSTR* !!!!
        [PreserveSig]
        public HRESULT GetNameAt(System.UInt32 iProp, /* BSTR* */ System.Char** pbstrName);
    }
}