// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.IO
{
    /// <summary>
    /// Provides centralized methods for creating exceptions for Microsoft.IO.FileSystem.
    /// </summary>
    internal static class Error
    {
        internal static Exception GetEndOfFile()
        {
            return new EndOfStreamException(SR.IO_EOF_ReadBeyondEOF);
        }

        // An alternative to Win32Marshal with friendlier messages for drives
        internal static Exception GetExceptionForLastWin32DriveError(string driveName)
        {
            int errorCode = Runtime.InteropServices.Marshal.GetLastWin32Error();
            return GetExceptionForWin32DriveError(errorCode, driveName);
        }

        internal static Exception GetExceptionForWin32DriveError(int errorCode, string driveName)
        {
            switch (errorCode)
            {
                case Interop.Errors.ERROR_PATH_NOT_FOUND:
                case Interop.Errors.ERROR_INVALID_DRIVE:
                    return new Microsoft.IO.DriveNotFoundException(String.Format(SR.IO_DriveNotFound_Drive, driveName));

                default:
                    return Win32Marshal.GetExceptionForWin32Error(errorCode, driveName);
            }
        }

    }
}
