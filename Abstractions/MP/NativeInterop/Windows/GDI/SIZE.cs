using System.Runtime.InteropServices;

namespace MP.NativeInterop.Windows.GDI
{
    /// <summary>
    /// Specifies a 2D size in Windows GDI.
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Pack = sizeof(int), Size = sizeof(int) * 2)]
    public struct SIZE
    {
        /// <summary>The size's width.</summary>
        [FieldOffset(0)]
        public int cx;
        /// <summary>The size's height.</summary>
        [FieldOffset(sizeof(int))]
        public int cy;
    }
}