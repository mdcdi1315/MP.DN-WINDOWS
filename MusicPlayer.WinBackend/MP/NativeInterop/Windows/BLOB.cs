
using System;
using System.Runtime.InteropServices;

namespace MP.NativeInterop.Windows
{
    // I could make it to work by defining FieldOffset = 8 at Data field , but make it sequential 
    // so that it is easier to change the alignment using a single field.
    /// <summary>
    /// Representation of a large binary object container. <br />
    /// Note that in pointer size changes the Pack field must be adjusted properly.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct BLOB
    {
        /// <summary>
        /// Length of binary object.
        /// </summary>
        public UInt32 Length;
        /// <summary>
        /// Pointer to buffer storing data.
        /// </summary>
        public System.Byte* Data;
    }
}