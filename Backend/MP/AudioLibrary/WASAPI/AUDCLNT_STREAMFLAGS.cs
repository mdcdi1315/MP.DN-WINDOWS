

using System;
using MP.WindowsInterop;
using System.Runtime.Versioning;

namespace MP.AudioLibrary.WASAPI
{
    /// <summary>
    /// Audio client stream flags, copied from AudioSessionTypes.h of 10.0.19041.0 SDK, see line 73.
    /// </summary>
    [Flags]
    public enum AUDCLNT_STREAMFLAGS : System.UInt32
    {
        /// <summary>
        /// Audio policy control for this stream will be shared with other process sessions that use the same audio session GUID.
        /// </summary>
        // The prefix AUDCLNT_STREAMFLAGS_ is omitted for brevity.
        CROSSPROCESS = 0x00010000,
        /// <summary>
        /// Initializes a renderer endpoint for a loopback audio application. <br />
        /// In this mode, a capture stream will be opened on the specified renderer endpoint. <br />
        /// Shared mode and a renderer endpoint is required. <br />
        /// Otherwise the IAudioClient::Initialize call will fail.  <br />
        /// If the initialize is successful, a capture stream will be available from the IAudioClient object
        /// </summary>
        // The prefix AUDCLNT_STREAMFLAGS_ is omitted for brevity.
        LOOPBACK = 0x00020000,
        /// <summary>
        /// An exclusive mode client will supply an event handle that will be signaled when an IRP completes (or a waveRT buffer completes) telling it to fill the next buffer
        /// </summary>
        // The prefix AUDCLNT_STREAMFLAGS_ is omitted for brevity.
        EVENTCALLBACK = 0x00040000,
        /// <summary>Session state will not be persisted</summary>
        // The prefix AUDCLNT_STREAMFLAGS_ is omitted for brevity.
        NOPERSIST = 0x00080000,
        /// <summary>
        /// The sample rate of the stream is adjusted to a rate specified by an application.
        /// </summary>
        // The prefix AUDCLNT_STREAMFLAGS_ is omitted for brevity.
        RATEADJUST = 0x00100000,
        /// <summary>
        /// When used with <see cref="AUTOCONVERTPCM"/>, a sample rate converter with better quality than the default conversion but with a higher performance cost is used. <br />
        /// This should be used if the audio is ultimately intended to be heard by humans as opposed to other scenarios such as pumping silence or populating a meter.
        /// </summary>
        // The prefix AUDCLNT_STREAMFLAGS_ is omitted for brevity.
        [SupportedOSPlatform(WindowsVersions.NTDDI_WINTHRESHOLD)]
        SRC_DEFAULT_QUALITY = 0x08000000,
        /// <summary>
        /// A channel matrixer and a sample rate converter are inserted as necessary to convert between the uncompressed format supplied to IAudioClient::Initialize and the audio engine mix format.
        /// </summary>
        // The prefix AUDCLNT_STREAMFLAGS_ is omitted for brevity.
        [SupportedOSPlatform(WindowsVersions.NTDDI_WINTHRESHOLD)]
        AUTOCONVERTPCM = 0x80000000,
        /// <summary>Session expires when there are no streams and no owning session controls.</summary>
        // The prefix AUDCLNT_ is omitted for brevity.
        SESSIONFLAGS_EXPIREWHENUNOWNED = 0x10000000,
        /// <summary>Don't show volume control in the Volume Mixer.</summary>
        // The prefix AUDCLNT_ is omitted for brevity.
        SESSIONFLAGS_DISPLAY_HIDE = 0x20000000,
        /// <summary>Don't show volume control in the Volume Mixer after the session expires.</summary>
        // The prefix AUDCLNT_ is omitted for brevity.
        SESSIONFLAGS_DISPLAY_HIDEWHENEXPIRED = 0x40000000
    }
}