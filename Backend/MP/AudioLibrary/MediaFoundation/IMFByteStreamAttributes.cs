
using System;
using MP.WindowsInterop;
using System.Runtime.Versioning;

namespace MP.AudioLibrary.MediaFoundation
{
    public static class IMFByteStreamAttributes
    {
        [SupportedOSPlatform(WindowsVersions.NTDDI_WIN7)] // Windows 7
        public static Guid ORIGIN_NAME => new(0xfc358288, 0x3cb6, 0x460c, 0xa4, 0x24, 0xb6, 0x68, 0x12, 0x60, 0x37, 0x5a);

        [SupportedOSPlatform(WindowsVersions.NTDDI_WIN7)] // Windows 7
        public static Guid CONTENT_TYPE => new(0xfc358289, 0x3cb6, 0x460c, 0xa4, 0x24, 0xb6, 0x68, 0x12, 0x60, 0x37, 0x5a);
    }
}