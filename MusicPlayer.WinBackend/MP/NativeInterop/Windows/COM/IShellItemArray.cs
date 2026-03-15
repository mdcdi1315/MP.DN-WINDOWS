using MP.Annotations;

namespace MP.NativeInterop.Windows.COM
{
    [COMInterfaceGenerator(CommonInteropClsIds.IID_IShellItemArray)]
    public unsafe partial interface IShellItemArray
    {
        public HRESULT BindToHandler(void* handlercint, GUID* hguid, GUID* refiid, void** output);

        public HRESULT GetPropertyStore(GETPROPERTYSTOREFLAGS flags, GUID* riid, [IsPointerToCOMInterfaceType(typeof(IPropertyStore))] void** output);

        public HRESULT GetPropertyDescriptionList(PROPERTYKEY* key, GUID* riid, void** output);

        public HRESULT GetAttributes(SIATTRIBFLAGS attribflags, SFGAO mask, SFGAO* output);

        public HRESULT GetCount(uint* itemsptr);

        public HRESULT GetItemAt(uint index, [IsPointerToCOMInterfaceType(typeof(IShellItem))] void** ishellitemptr);

        public HRESULT EnumItems(void** ppenumShellItems);
    }
}
