
using System.Runtime.Versioning;
using System.Runtime.InteropServices;

namespace MP.ComInterop
{
    [ComImport]
    [Guid(CommonInteropClsIds.IID_IAgileObject)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [SupportedOSPlatform(WindowsInterop.WindowsVersions.NTDDI_WIN8)]
    public interface IAgileObject { }
}