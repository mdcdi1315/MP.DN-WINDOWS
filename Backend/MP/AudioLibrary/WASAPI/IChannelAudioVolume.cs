
using MP.ComInterop;
using MP.WindowsInterop;
using System.Runtime.InteropServices;

namespace MP.AudioLibrary.WASAPI
{
    // Ported 1-1 from Audioclient.idl of Windows SDK 10.0.19041.0.
    // See line 1560 of the file for more information.

    [ComImport]
    [Guid(WASAPIInterfaceIds.IID_IChannelAudioVolume)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe interface IChannelAudioVolume
    {
        //-------------------------------------------------------------------------
        // Description:
        //
        //  Get the channel count for the audio session associated with
        //  this client.
        //
        // Parameters:
        //
        //     pdwCount - [out] The current channel count.
        //
        // See Also:
        //
        //  IChannelAudioVolume::GetChannelCount
        //
        // Return values:
        //
        //     S_OK        Successful completion.
        //     OTHER       Other error.
        //
        //
        [PreserveSig]
        public HRESULT GetChannelCount(System.UInt32* pdwCount);

        //-------------------------------------------------------------------------
        // Description:
        //
        //  Set the volume for a particular channel on the audio session
        //  associated with this client.
        //
        // Parameters:
        //
        //     dwIndex - [in] The channel # to set
        //     fLevel -  [in] The volume level for that channel
        //     EventContext - [in] Context passed to notification routine, GUID_NULL if NULL.
        //
        // See Also:
        //
        //  IChannelAudioVolume::GetChannelVolume
        //
        // Return values:
        //
        //     S_OK        Successful completion.
        //     OTHER       Other error.
        //
        //
        [PreserveSig]
        public HRESULT SetChannelVolume(System.UInt32 dwIndex, System.Single fLevel, GUID* EventContext);

        //-------------------------------------------------------------------------
        // Description:
        //
        //  Get the volume for a particular channel.
        //
        // Parameters:
        //
        //     dwIndex - [in] The channel # to get
        //     pfLevel -  [out] The volume level for that channel
        //
        // See Also:
        //
        //  IChannelAudioVolume::GetChannelVolume
        //
        // Return values:
        //
        //     S_OK        Successful completion.
        //     OTHER       Other error.
        //
        //
        [PreserveSig]
        public HRESULT GetChannelVolume(System.UInt32 dwIndex, System.Single* pfLevel);

        //-------------------------------------------------------------------------
        // Description:
        //
        //  Set the volume for all audio channels.
        //
        // Parameters:
        //
        //     dwCount - [in] Number of entries in the pfVolumes array.  Must be the same as IChannelAudioVolume::GetChannelCount
        //     pfVolumes - [in] Array of volumes.
        //     EventContext - [in] Context passed to notification routine, GUID_NULL if NULL.
        //
        // See Also:
        //
        //  IChannelAudioVolume::GetAllVolumes
        //
        // Return values:
        //
        //     S_OK        Successful completion.
        //     OTHER       Other error.
        //
        //
        [PreserveSig]
        public HRESULT SetAllVolumes(System.UInt32 dwCount, /* [in, size_is(dwCount), annotation("_In_reads_(dwCount)")] */ System.Single* pfVolumes, /*LPCGUID ??*/ GUID* EventContext);

        //-------------------------------------------------------------------------
        // Description:
        //
        //  Get the volume for all audio channels.
        //
        // Parameters:
        //
        //     dwCount - [in] Number of entries in the pfVolumes array.  Must be the same as IChannelAudioVolume::GetChannelCount
        //     pfVolumes - [out] Array of volumes filled in with the current channel volumes.
        //
        // See Also:
        //
        //  IChannelAudioVolume::SetAllVolumes
        //
        // Return values:
        //
        //     S_OK        Successful completion.
        //     OTHER       Other error.
        //
        //
        [PreserveSig]
        public HRESULT GetAllVolumes(System.UInt32 dwCount, /*[out, size_is(dwCount), , annotation("_Out_writes_(dwCount)")]*/ System.Single* pfVolumes);
    }
}