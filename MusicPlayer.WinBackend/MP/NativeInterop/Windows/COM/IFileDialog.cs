
using MP.Annotations;

namespace MP.NativeInterop.Windows.COM
{
    [COMInterfaceGenerator(CommonInteropClsIds.IID_IFileDialog)]
    public unsafe partial interface IFileDialog
    {
        public HRESULT Show(System.IntPtr hwnd);

        public HRESULT SetFileTypes(System.UInt32 count, COMDLG_FILTERSPEC* filters);

        public HRESULT SetFileTypeIndex(System.UInt32 index);

        public HRESULT GetFileTypeIndex(System.UInt32* ppout);

        public HRESULT Advise(System.IntPtr fdis , System.UInt32* Cookie);

        public HRESULT Unadvise(System.UInt32 cookie);

        public HRESULT SetOptions(FILEOPENDIALOGOPTIONS options);

        public HRESULT GetOptions(FILEOPENDIALOGOPTIONS* ppoptions);

        public HRESULT SetDefaultFolder([IsPointerToCOMInterfaceType(typeof(IShellItem))] void* pishellitem);

        public HRESULT SetFolder([IsPointerToCOMInterfaceType(typeof(IShellItem))] void* pishellitem);

        public HRESULT GetFolder([IsPointerToCOMInterfaceType(typeof(IShellItem))] void** pishellitem);

        public HRESULT GetCurrentSelection([IsPointerToCOMInterfaceType(typeof(IShellItem))] void** pishellitem);

        public HRESULT SetFileName(System.Char* filename);

        public HRESULT GetFileName(System.Char** filename);

        public HRESULT SetTitle(System.Char* Title);

        public HRESULT SetOkButtonLabel(System.Char* NewLabel);

        public HRESULT SetFileNameLabel(System.Char* NewFileLabel);

        public HRESULT GetResult([IsPointerToCOMInterfaceType(typeof(IShellItem))] System.IntPtr* pishellitem);

        public HRESULT AddPlace([IsPointerToCOMInterfaceType(typeof(IShellItem))] void* pishellitem , System.Int32 fdap);

        public HRESULT SetDefaultExtension(System.Char* DefaultExtension);

        public HRESULT Close(HRESULT hr);
        
        public HRESULT SetClientGuid(GUID* guid);

        public HRESULT ClearClientData();

        public HRESULT SetFilter(System.IntPtr filterptr);
    }
}
