using MP.Annotations;
using MP.WindowsInterop;
using System;
using System.Runtime.InteropServices;

namespace MP.ComInterop
{
    [ComImport]
    [Guid(CommonInteropClsIds.IID_IFileDialog)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe interface IFileDialog
    {
        [PreserveSig]
        public HRESULT Show(System.IntPtr hwnd);

        [PreserveSig]
        public HRESULT SetFileTypes(System.UInt32 count, COMDLG_FILTERSPEC* filters);

        [PreserveSig]
        public HRESULT SetFileTypeIndex(System.UInt32 index);

        [PreserveSig]
        public HRESULT GetFileTypeIndex(System.UInt32* ppout);

        [PreserveSig]
        public HRESULT Advise(System.IntPtr fdis , System.UInt32* Cookie);

        [PreserveSig]
        public HRESULT Unadvise(System.UInt32 cookie);

        [PreserveSig]
        public HRESULT SetOptions(FILEOPENDIALOGOPTIONS options);

        [PreserveSig]
        public HRESULT GetOptions(FILEOPENDIALOGOPTIONS* ppoptions);

        [PreserveSig]
        public HRESULT SetDefaultFolder([IsPointerToCOMInterfaceType(typeof(IShellItem))] void* pishellitem);

        [PreserveSig]
        public HRESULT SetFolder([IsPointerToCOMInterfaceType(typeof(IShellItem))] void* pishellitem);

        [PreserveSig]
        public HRESULT GetFolder([IsPointerToCOMInterfaceType(typeof(IShellItem))] void** pishellitem);

        [PreserveSig]
        public HRESULT GetCurrentSelection([IsPointerToCOMInterfaceType(typeof(IShellItem))] void** pishellitem);

        [PreserveSig]
        public HRESULT SetFileName(System.Char* filename);

        [PreserveSig]
        public HRESULT GetFileName(System.Char** filename);

        [PreserveSig]
        public HRESULT SetTitle(System.Char* Title);

        [PreserveSig]
        public HRESULT SetOkButtonLabel(System.Char* NewLabel);

        [PreserveSig]
        public HRESULT SetFileNameLabel(System.Char* NewFileLabel);

        [PreserveSig]
        public HRESULT GetResult([IsPointerToCOMInterfaceType(typeof(IShellItem))] System.IntPtr* pishellitem);

        [PreserveSig]
        public HRESULT AddPlace([IsPointerToCOMInterfaceType(typeof(IShellItem))] void* pishellitem , System.Int32 fdap);

        [PreserveSig]
        public HRESULT SetDefaultExtension(System.Char* DefaultExtension);

        [PreserveSig]
        public HRESULT Close(HRESULT hr);
        
        [PreserveSig]
        public HRESULT SetClientGuid(GUID* guid);

        [PreserveSig]
        public HRESULT ClearClientData();

        [PreserveSig]
        public HRESULT SetFilter(System.IntPtr filterptr);
    }
}
