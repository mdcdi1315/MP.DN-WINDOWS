
using MP.WindowsInterop;
using System.Runtime.Versioning;

namespace MP.AudioLibrary.MediaFoundation
{
    public enum MFT_MESSAGE_TYPE
    {
        //
        // commands - must be acted on
        //
        MFT_MESSAGE_COMMAND_FLUSH = 0x00000000,
        MFT_MESSAGE_COMMAND_DRAIN = 0x00000001,
        MFT_MESSAGE_SET_D3D_MANAGER = 0x00000002,
        [SupportedOSPlatform(WindowsVersions.NTDDI_WIN7)] // Windows 7
        MFT_MESSAGE_DROP_SAMPLES = 0x00000003,
        [SupportedOSPlatform(WindowsVersions.NTDDI_WIN8)] // Windows 8
        MFT_MESSAGE_COMMAND_TICK = 0x00000004,

        //
        // notifications - no action required; effect is transform-dependent
        //
        MFT_MESSAGE_NOTIFY_BEGIN_STREAMING = 0x10000000,
        MFT_MESSAGE_NOTIFY_END_STREAMING = 0x10000001,
        MFT_MESSAGE_NOTIFY_END_OF_STREAM = 0x10000002,

        //
        // send by pipeline before processing the first sample
        //
        MFT_MESSAGE_NOTIFY_START_OF_STREAM = 0x10000003,

        [SupportedOSPlatform(WindowsVersions._WIN32_WINNT_WINTHRESHOLD)] // Windows 10
        MFT_MESSAGE_NOTIFY_RELEASE_RESOURCES   = 0x10000004,
        [SupportedOSPlatform(WindowsVersions._WIN32_WINNT_WINTHRESHOLD)] // Windows 10
        MFT_MESSAGE_NOTIFY_REACQUIRE_RESOURCES = 0x10000005,
        [SupportedOSPlatform(WindowsVersions._WIN32_WINNT_WINTHRESHOLD)] // Windows 10
        MFT_MESSAGE_NOTIFY_EVENT               = 0x10000006,
        [SupportedOSPlatform(WindowsVersions._WIN32_WINNT_WINTHRESHOLD)] // Windows 10
        MFT_MESSAGE_COMMAND_SET_OUTPUT_STREAM_STATE = 0x10000007,
        [SupportedOSPlatform(WindowsVersions._WIN32_WINNT_WINTHRESHOLD)] // Windows 10
        MFT_MESSAGE_COMMAND_FLUSH_OUTPUT_STREAM = 0x10000008,

        //
        // commands (applicable to async MFTs only) - must be acted on
        //
        [SupportedOSPlatform(WindowsVersions.NTDDI_WIN7)] // Windows 7
        MFT_MESSAGE_COMMAND_MARKER = 0x20000000
    }
}