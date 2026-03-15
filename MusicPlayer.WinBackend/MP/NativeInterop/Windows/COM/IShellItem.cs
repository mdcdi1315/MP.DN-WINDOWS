
using MP.Annotations;

namespace MP.NativeInterop.Windows.COM
{
    /// <summary>
    /// Defines the native <see cref="IShellItem"/> interface.
    /// </summary>
    [COMInterfaceGenerator(CommonInteropClsIds.IID_IShellItem)]
    public unsafe partial interface IShellItem
    {
        public HRESULT BindToHandler(System.IntPtr handler, void* hguid, void* refiid, void** output);

        public HRESULT GetParent([IsPointerToCOMInterfaceType(typeof(IShellItem))] void** parentshellitem);

        public HRESULT GetDisplayName(SIGDN name, System.Char** dispname);

        public HRESULT GetAttributes(SFGAO mask, SFGAO* definedflags);

        public HRESULT Compare([IsPointerToCOMInterfaceType(typeof(IShellItem))] void* psi, SICHINTF hint, System.Int32* piOrder);
    }
}
