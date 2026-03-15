
using MP.Annotations;

namespace MP.NativeInterop.Windows.COM
{
    [DefaultCOMInterfaceObjectGuid(CommonInteropClsIds.CLSID_FileSaveDialog)]
    [COMInterfaceGenerator(CommonInteropClsIds.IID_IFileSaveDialog, typeof(IFileDialog))]
    public unsafe partial interface IFileSaveDialog
    {
        public HRESULT SetSaveAsItem([IsPointerToCOMInterfaceType(typeof(IShellItem))] void* ishellitemptr);
    }
}
