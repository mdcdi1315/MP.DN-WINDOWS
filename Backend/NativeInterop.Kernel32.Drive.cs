using System;
using System.IO;
using System.Runtime.InteropServices;

partial class Interop
{
    public static partial class Kernel32
    {
        public const System.UInt32 FILE_SUPPORTS_ENCRYPTION = 0x00020000;

        [DllImport(Libraries.Kernel32, EntryPoint = "GetDriveTypeW", CharSet = CharSet.Unicode, SetLastError = true, BestFitMapping = false)]
        public static extern System.Int32 GetDriveType(System.String drive);

        // NOTE: The out parameters are PULARGE_INTEGERs and may require
        // some byte munging magic.
        [DllImport(Libraries.Kernel32, EntryPoint = "GetDiskFreeSpaceExW", CharSet = CharSet.Unicode, SetLastError = true, BestFitMapping = false)]
        public static extern BOOL GetDiskFreeSpaceEx(string drive, out long freeBytesForUser, out long totalBytes, out long freeBytes);

        /// <summary>
        /// WARNING: This method does not implicitly handle long paths. Use DeleteVolumeMountPoint.
        /// </summary>
        [DllImport(Libraries.Kernel32, EntryPoint = "DeleteVolumeMountPointW", SetLastError = true, CharSet = CharSet.Unicode, BestFitMapping = false)]
        public static extern BOOL DeleteVolumeMountPointPrivate(string mountPoint);

        public static bool DeleteVolumeMountPoint(string mountPoint)
        {
            mountPoint = PathInternal.EnsureExtendedPrefixIfNeeded(mountPoint);
            return DeleteVolumeMountPointPrivate(mountPoint) != BOOL.FALSE;
        }

        [DllImport(Libraries.Kernel32, EntryPoint = "GetVolumeInformationW", CharSet = CharSet.Unicode, SetLastError = true, BestFitMapping = false)]
        public static unsafe extern BOOL GetVolumeInformation(
            System.String drive,
            System.Char* volumeName,
            System.Int32 volumeNameBufLen,
            System.UInt32* volSerialNumber,
            System.Int32* maxFileNameLen,
            System.Int32* fileSystemFlags,
            System.Char* fileSystemName,
            System.Int32 fileSystemNameBufLen);

        [DllImport(Libraries.Kernel32, SetLastError = true)]
        public static extern System.Int32 GetLogicalDrives();

        [DllImport(Libraries.Kernel32, EntryPoint = "SetVolumeLabelW", CharSet = CharSet.Unicode, SetLastError = true, BestFitMapping = false)]
        public static extern BOOL SetVolumeLabel(System.String driveLetter, System.String volumeName);
    }
}
