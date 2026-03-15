
using System.Runtime.InteropServices;

namespace MP.NativeInterop.OSX
{
    /// <summary>
    /// Defines the Core Foundation range structure in .NET .
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Size = 8, Pack = 4)]
    public struct CFRange
    {
        /// <summary>The index that the range starts from.</summary>
        [FieldOffset(0)]
        public System.Int32 Location;

        /// <summary>The range of the current length structure.</summary>
        [FieldOffset(4)]
        public System.Int32 Length;
    }
}