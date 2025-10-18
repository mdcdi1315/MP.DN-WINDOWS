
using System.Runtime.InteropServices;

namespace MP.Graphics.Windowing.Graphics
{
    /// <summary>
    /// Defines a window graphics object. <br />
    /// Window graphic objects are handles retaining a reference to a primitive graphics shape (such as a rectangle).
    /// </summary>
    [StructLayout(LayoutKind.Explicit , Pack = 4 , Size = 4)]
    public readonly struct GraphicsObjectID : Utilities.INullable
    {
        [FieldOffset(0)]
        private readonly uint id;

        /// <summary>Creates a new graphics object.</summary>
        /// <param name="id">The ID of the graphics object to be retained by the newly created instance.</param>
        public GraphicsObjectID(uint id) => this.id = id;

        /// <summary>Gets the actual handle value held by this instance.</summary>
        public readonly uint ID => id;

        /// <summary>
        /// Gets a value whether this is an invalid graphics object.
        /// </summary>
        public readonly bool IsNull => id == 0;

        /// <summary>
        /// Gets a hash code for this <see cref="GraphicsObjectID"/> structure instance.
        /// </summary>
        /// <returns>The hash code of this structure instance.</returns>
        public readonly override int GetHashCode() => id.ToInt32();
    }
}