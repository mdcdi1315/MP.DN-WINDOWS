
using System;
using System.Runtime.InteropServices;

namespace MP.ComInterop
{
    // IPropertyStore interface definition.
    // To use it with .NET, see the PropertyStore class.
    [ComImport]
    [Guid(CommonInteropClsIds.IID_IPropertyStore)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe interface IPropertyStore
    {
        [PreserveSig]
        public HRESULT GetCount(System.UInt32* ppropcount);
        
        [PreserveSig]
        public HRESULT GetAt(System.UInt32 property, PROPERTYKEY* poutkey);
        
        [PreserveSig]
        public HRESULT GetValue(PROPERTYKEY* pkey, PROPVARIANT* value);

        [PreserveSig]
        public HRESULT SetValue(PROPERTYKEY* pkey, PROPVARIANT* value);

        [PreserveSig]
        public HRESULT Commit();
    }
}
