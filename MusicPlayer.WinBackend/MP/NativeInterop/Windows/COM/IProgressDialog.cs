
using MP.Annotations;
using System.Runtime.Versioning;

namespace MP.NativeInterop.Windows.COM
{
    [COMInterfaceGenerator(CommonInteropClsIds.IID_IProgressDialog)]
    [DefaultCOMInterfaceObjectGuid(CommonInteropClsIds.CLSID_ProgressDialog)]
    public unsafe partial interface IProgressDialog
    {
        public HRESULT StartProgressDialog(System.IntPtr hwnd , void* punkrsvd , ProgressDialogFlags flags , void* prsvd);

        public HRESULT StopProgressDialog();

        public HRESULT SetTitle(System.Char* pwzTitle);

        [UnsupportedOSPlatform(WindowsVersions.NTDDI_WIN7)]
        public HRESULT SetAnimation(System.IntPtr hinstance, System.UInt32 resid);

        public BOOL HasUserCancelled();

        public HRESULT SetProgress(System.UInt32 progress , System.UInt32 total);

        public HRESULT SetProgress64(System.UInt64 progress, System.UInt64 total);

        public HRESULT SetLine(System.UInt32 line , System.Char* pwzString , BOOL fcompactpath , void* prsvd);

        public HRESULT SetCancelMsg(System.Char* pwzCancelMsg , void* prsvd);

        public HRESULT Timer(ProgressDialogTimerAction action , void* prsvd);
    }
}