using MP.Annotations;

namespace MP.NativeInterop.Windows.COM
{
    [DefaultCOMInterfaceObjectGuid(CommonInteropClsIds.CLSID_FileOpenDialog)]
    [COMInterfaceGenerator(CommonInteropClsIds.IID_IFileOpenDialog, typeof(IFileDialog))]
    public unsafe partial interface IFileOpenDialog
    {
        public HRESULT GetResults([IsPointerToCOMInterfaceType(typeof(IShellItemArray))] void** ishellitemarrayptr);

        public HRESULT GetSelectedItems([IsPointerToCOMInterfaceType(typeof(IShellItemArray))] void** ishellitemarrayptr);
    }
}