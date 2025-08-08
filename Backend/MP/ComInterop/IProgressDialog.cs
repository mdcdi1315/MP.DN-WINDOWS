
using MP.WindowsInterop;
using System.Runtime.Versioning;
using System.Runtime.InteropServices;

namespace MP.ComInterop
{
    [ComImport]
    [Guid(CommonInteropClsIds.IID_IProgressDialog)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Annotations.DefaultCOMInterfaceObjectGuid(CommonInteropClsIds.CLSID_ProgressDialog)]
    public unsafe interface IProgressDialog
    {
        [PreserveSig]
        public HRESULT StartProgressDialog(System.IntPtr hwnd , void* punkrsvd , ProgressDialogFlags flags , void* prsvd);

        [PreserveSig]
        public HRESULT StopProgressDialog();

        [PreserveSig]
        public HRESULT SetTitle(System.Char* pwzTitle);

        [PreserveSig]
        [UnsupportedOSPlatform("windows6.1")]
        public HRESULT SetAnimation(System.IntPtr hinstance, System.UInt32 resid);

        [PreserveSig]
        public BOOL HasUserCancelled();

        [PreserveSig]
        public HRESULT SetProgress(System.UInt32 progress , System.UInt32 total);

        [PreserveSig]
        public HRESULT SetProgress64(System.UInt64 progress, System.UInt64 total);

        [PreserveSig]
        public HRESULT SetLine(System.UInt32 line , System.Char* pwzString , BOOL fcompactpath , void* prsvd);

        [PreserveSig]
        public HRESULT SetCancelMsg(System.Char* pwzCancelMsg , void* prsvd);

        [PreserveSig]
        public HRESULT Timer(ProgressDialogTimerAction action , void* prsvd);
    }
}