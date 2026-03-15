
using System.Runtime.InteropServices;

namespace MP.NativeInterop.Windows.GDI
{
    /// <summary>
    /// This is NOT an actual structure within Windows GDI, it is defined just to assist bitmap decoders to decode an 
    /// bitmap image with type set to <see cref="BitmapImageType.BI_BITFIELDS"/> correctly. <br />
    /// This is typically used together with the <see cref="StreamMethods.ReadStructure{T}(IO.IDataStreamAccess)"/> method.
    /// </summary>
    [StructLayout(LayoutKind.Explicit , Size = 12 , Pack = 4)]
    public struct BITFIELDS
    {
        /// <summary>Red bit mask.</summary>
        [FieldOffset(0)]
        public System.UInt32 RedMask;

        /// <summary>Green bit mask.</summary>
        [FieldOffset(4)]
        public System.UInt32 GreenMask;

        /// <summary>Blue bit mask.</summary>
        [FieldOffset(8)]
        public System.UInt32 BlueMask;

        // Usage of the below properties is to identify the color format to be decoded.

        /// <summary>Gets the bit fields for the RGB 555 color format.</summary>
        public static BITFIELDS RGB555 // RGB555 color masks.
                => new() { RedMask = 0x7C00 , GreenMask = 0x3E0 , BlueMask = 0x7F };

        /// <summary>Gets the bit fields for the RGB 565 color format.</summary>
        public static BITFIELDS RGB565 // RGB565 color masks.
                => new() { RedMask = 0xF800, GreenMask = 0x7E0, BlueMask = 0x1F };

        /// <summary>Determines equality of two <see cref="BITFIELDS"/> structures.</summary>
        /// <param name="one">The first <see cref="BITFIELDS"/> structure to compare.</param>
        /// <param name="two">The second <see cref="BITFIELDS"/> structure to compare.</param>
        /// <returns>A value whether the passed <see cref="BITFIELDS"/> structures are equal.</returns>
        public static System.Boolean BitFieldsEqual(BITFIELDS one, BITFIELDS two)
            => one.RedMask == two.RedMask && one.GreenMask == two.GreenMask && one.BlueMask == two.BlueMask;
    }
}