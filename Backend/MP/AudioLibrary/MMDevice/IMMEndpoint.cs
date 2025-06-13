
using MP.ComInterop;
using System.Runtime.InteropServices;

namespace MP.AudioLibrary.MMDevice
{
    /// <summary>
    /// Provides additional methods beyond IMMDevice for Endpoint device objects
    /// </summary>
    [ComImport]
    [Guid(MMDeviceInterfaceIds.IID_IMMEndpoint)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe interface IMMEndpoint
    {
        /// <summary>
        /// Gets the dataflow of the Endpoint device
        /// </summary>
        /// <param name="pDataFlow">[out] Address of an <see cref="EDataFlow"/> that will receive the current dataflow direction</param>
        /// <returns><see cref="CommonHResults.S_OK"/> if successfull.</returns>
        [PreserveSig]
        public HRESULT GetDataFlow(EDataFlow* pDataFlow);
    }
}