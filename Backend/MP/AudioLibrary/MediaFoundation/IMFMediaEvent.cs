
using MP.ComInterop;
using MP.WindowsInterop;
using System.Runtime.InteropServices;

namespace MP.AudioLibrary.MediaFoundation
{
    [ComImport]
    [Guid(MediaFoundationInterfaceIds.IID_IMFMediaEvent)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe interface IMFMediaEvent
    {
        /// <summary>
        ///     Retrieves the event type.
        /// </summary>
        /// <param name="pmet">
        ///     Out parameter that will receive the event type.
        ///     Common MF event types are listed in the MediaEventType
        ///     enumerated type, but other types are possible.
        /// </param>
        [PreserveSig]
        public HRESULT GetType(MediaEventType* pmet);

        /// <summary>
        ///     Retrieves the event's extended type.
        ///     This is for use with events of the MEExtendedType type
        /// </summary>
        /// <param name="pguidExtendedType">
        ///     Out parameter that will receive the extended type
        /// </param>
        [PreserveSig]
        public HRESULT GetExtendedType(GUID* pguidExtendedType);

        /// <summary>
        ///     Retrieves the status code for this event.
        ///     For example, an event such as MESessionTopologySet that signals
        ///     the completion of an asynchronous operation can return a failure
        ///     code if the operation failed.
        /// </summary>
        /// <param name="phrStatus">
        ///     Out param that will receive the HRESULT status code
        /// </param>
        [PreserveSig]
        public HRESULT GetStatus(HRESULT* phrStatus);

        /// <summary>
        ///     Retrieves extra data for the event
        /// </summary>
        /// <param>
        ///     Out param that will receive the extra data.
        ///     The meaning of this value is event-specific.
        ///     Some events have no extra data.
        /// </param>
        [PreserveSig]
        public HRESULT GetValue(PROPVARIANT* pvValue);
    }
}
