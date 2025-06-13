using System;
using MP.Annotations;
using System.Runtime.InteropServices;

namespace MP.ComInterop
{
    /// <summary>
    /// Defines the native <see cref="IShellItem"/> interface.
    /// </summary>
    [ComImport]
    [Guid(CommonInteropClsIds.IID_IShellItem)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe interface IShellItem
    {
        [PreserveSig]
        public HRESULT BindToHandler(System.IntPtr handler, void* hguid, void* refiid, void** output);

        [PreserveSig]
        public HRESULT GetParent([IsPointerToCOMInterfaceType(typeof(IShellItem))] System.IntPtr* parentshellitem);

        [PreserveSig]
        public HRESULT GetDisplayName(SIGDN name, System.Char** dispname);

        [PreserveSig]
        public HRESULT GetAttributes(SFGAO mask, SFGAO* definedflags);

        [PreserveSig]
        public HRESULT Compare(/* IShellItem* */ void* psi, SICHINTF hint, System.Int32* piOrder);
    }
}
