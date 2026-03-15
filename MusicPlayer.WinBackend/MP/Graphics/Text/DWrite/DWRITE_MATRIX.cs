using System.Runtime.InteropServices;

namespace MP.Graphics.Text.DWrite
{
    /// <summary>
    /// The <see cref="DWRITE_MATRIX"/> structure specifies the graphics transform to be applied to rendered glyphs.
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public struct DWRITE_MATRIX
    {
        /// <summary>
        /// A value indicating the horizontal scaling / cosine of rotation.
        /// </summary>
        [FieldOffset(0)]
        public System.Single m11;
        /// <summary>
        /// A value indicating the vertical shear / sine of rotation.
        /// </summary>
        [FieldOffset(sizeof(System.Single))]
        public System.Single m12;
        /// <summary>
        /// A value indicating the horizontal shear / negative sine of rotation.
        /// </summary>
        [FieldOffset(sizeof(System.Single) * 2)]
        public System.Single m21;
        /// <summary>
        /// A value indicating the vertical scaling / cosine of rotation.
        /// </summary>
        [FieldOffset(sizeof(System.Single) * 3)]
        public System.Single m22;
        /// <summary>
        /// A value indicating the horizontal shift (always orthogonal regardless of rotation).
        /// </summary>
        [FieldOffset(sizeof(System.Single) * 4)]
        public System.Single dx;
        /// <summary>
        /// A value indicating the vertical shift (always orthogonal regardless of rotation.)
        /// </summary>
        [FieldOffset(sizeof(System.Single) * 5)]
        public System.Single dy;
    }
}