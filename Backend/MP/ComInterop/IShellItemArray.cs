
using MP.Annotations;
using MP.WindowsInterop;
using System.Runtime.InteropServices;

namespace MP.ComInterop
{
    [ComImport]
    [Guid(CommonInteropClsIds.IID_IShellItemArray)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe interface IShellItemArray
    {
        [PreserveSig]
        public HRESULT BindToHandler(void* handlercint, GUID* hguid, GUID* refiid, void** output);

        [PreserveSig]
        public HRESULT GetPropertyStore(GETPROPERTYSTOREFLAGS flags, GUID* riid, [IsPointerToCOMInterfaceType(typeof(IPropertyStore))] void** output);

        [PreserveSig]
        public HRESULT GetPropertyDescriptionList(PROPERTYKEY* key, GUID* riid, void** output);

        [PreserveSig]
        public HRESULT GetAttributes(SIATTRIBFLAGS attribflags, SFGAO mask, SFGAO* output);

        [PreserveSig]
        public HRESULT GetCount(System.UInt32* itemsptr);

        [PreserveSig]
        public HRESULT GetItemAt(System.UInt32 index, [IsPointerToCOMInterfaceType(typeof(IShellItem))] System.IntPtr* ishellitemptr);

        [PreserveSig]
        public HRESULT EnumItems(void** ppenumShellItems);
    }
}
