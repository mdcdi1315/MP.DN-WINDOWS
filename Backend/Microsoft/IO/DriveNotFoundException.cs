// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;

namespace Microsoft.IO
{
    //Thrown when trying to access a drive that is not available.
    [Serializable]
    [System.Runtime.CompilerServices.TypeForwardedFrom("mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
    public class DriveNotFoundException : IOException
    {

        private const int COR_E_DIRECTORYNOTFOUND = unchecked((int)0x80070003);
        private const int COR_E_ENDOFSTREAM = unchecked((int)0x80070026);  // OS defined
        private const int COR_E_FILELOAD = unchecked((int)0x80131621);
        private const int COR_E_FILENOTFOUND = unchecked((int)0x80070002);
        private const int COR_E_IO = unchecked((int)0x80131620);
        private const int COR_E_PATHTOOLONG = unchecked((int)0x800700CE);

        public DriveNotFoundException()
            : base(SR.IO_DriveNotFound)
        {
            HResult = COR_E_DIRECTORYNOTFOUND;
        }

        public DriveNotFoundException(string message)
            : base(message)
        {
            HResult = COR_E_DIRECTORYNOTFOUND;
        }

        public DriveNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
            HResult = COR_E_DIRECTORYNOTFOUND;
        }
    }
}
