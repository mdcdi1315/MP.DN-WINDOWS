
using MP.Annotations;
using MP.WindowsInterop;
using System.Runtime.InteropServices;

namespace MP.ComInterop
{
    [ComImport]
    [Guid(CommonInteropClsIds.IID_IFileSaveDialog)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe interface IFileSaveDialog : IFileDialog
    {
        [PreserveSig]
        public new HRESULT Show(System.IntPtr hwnd);

        [PreserveSig]
        public new HRESULT SetFileTypes(System.UInt32 count, COMDLG_FILTERSPEC* filters);

        [PreserveSig]
        public new HRESULT SetFileTypeIndex(System.UInt32 index);

        [PreserveSig]
        public new HRESULT GetFileTypeIndex(System.UInt32* ppout);

        [PreserveSig]
        public new HRESULT Advise(System.IntPtr fdis, System.UInt32* Cookie);

        [PreserveSig]
        public new HRESULT Unadvise(System.UInt32 cookie);

        [PreserveSig]
        public new HRESULT SetOptions(FILEOPENDIALOGOPTIONS options);

        [PreserveSig]
        public new HRESULT GetOptions(FILEOPENDIALOGOPTIONS* ppoptions);

        [PreserveSig]
        public new HRESULT SetDefaultFolder([IsPointerToCOMInterfaceType(typeof(IShellItem))] void* pishellitem);

        [PreserveSig]
        public new HRESULT SetFolder([IsPointerToCOMInterfaceType(typeof(IShellItem))] void* pishellitem);

        [PreserveSig]
        public new HRESULT GetFolder([IsPointerToCOMInterfaceType(typeof(IShellItem))] void** pishellitem);

        [PreserveSig]
        public new HRESULT GetCurrentSelection([IsPointerToCOMInterfaceType(typeof(IShellItem))] void** pishellitem);

        [PreserveSig]
        public new HRESULT SetFileName(System.Char* filename);

        [PreserveSig]
        public new HRESULT GetFileName(System.Char** filename);

        [PreserveSig]
        public new HRESULT SetTitle(System.Char* Title);

        [PreserveSig]
        public new HRESULT SetOkButtonLabel(System.Char* NewLabel);

        [PreserveSig]
        public new HRESULT SetFileNameLabel(System.Char* NewFileLabel);

        [PreserveSig]
        public new HRESULT GetResult(/* IShellItem** */ System.IntPtr* pishellitem);

        [PreserveSig]
        public new HRESULT AddPlace([IsPointerToCOMInterfaceType(typeof(IShellItem))] void* pishellitem, System.Int32 fdap);

        [PreserveSig]
        public new HRESULT SetDefaultExtension(System.Char* DefaultExtension);

        [PreserveSig]
        public new HRESULT Close(HRESULT hr);

        [PreserveSig]
        public new HRESULT SetClientGuid(GUID* guid);

        [PreserveSig]
        public new HRESULT ClearClientData();

        [PreserveSig]
        public new HRESULT SetFilter(System.IntPtr filterptr);

        [PreserveSig]
        public HRESULT SetSaveAsItem([IsPointerToCOMInterfaceType(typeof(IShellItem))] void* ishellitemptr);
    }
}
