
using System;
using System.Runtime.InteropServices;

namespace MP.ComInterop
{
    /// <summary>
    /// The <see cref="ISequentialStream"/> interface supports simplified sequential access to stream objects. <br />
    /// The <see cref="IStream"/> interface inherits its <see cref="IStream.Read(void*, uint, uint*)"/> and 
    /// <see cref="IStream.Write(void*, uint, uint*)"/> methods from <see cref="ISequentialStream"/>.
    /// </summary>
    [ComImport]
    [Guid(CommonInteropClsIds.IID_ISequentialStream)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe interface ISequentialStream
    {
        [PreserveSig]
        public HRESULT Read(void* pv, System.UInt32 cb, System.UInt32* pcbread);

        [PreserveSig]
        public HRESULT Write(void* pv , System.UInt32 cb, System.UInt32* pcbwritten);
    }
}
