

using System.Runtime.InteropServices;

namespace MP.NativeInterop.Windows.GDI
{
    /// <summary>
    /// Specifies a 2D LTRB rectangle on Windows GDI.
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Size = sizeof(System.Int32) * 4, Pack = sizeof(System.Int32))]
    public struct RECT
    {
        /// <summary></summary>
        [FieldOffset(0)]
        public System.Int32 left;
        /// <summary></summary>
        [FieldOffset(sizeof(System.Int32))]
        public System.Int32 top;
        /// <summary></summary>
        [FieldOffset(sizeof(System.Int32) * 2)]
        public System.Int32 right;
        /// <summary></summary>
        [FieldOffset(sizeof(System.Int32) * 3)]
        public System.Int32 bottom;
    }
}