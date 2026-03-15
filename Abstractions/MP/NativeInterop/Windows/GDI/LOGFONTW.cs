
using System;
using MP.Annotations.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Diagnostics.CodeAnalysis;

namespace MP.NativeInterop.Windows.GDI
{
    /// <summary>
    /// Specifies a loaded font in Windows GDI.
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct LOGFONTW
    {
        /// <summary>
        /// Max size of the <see cref="lfFaceName"/> member.
        /// </summary>
        public const System.Int32 LF_FACESIZE = 32;

        /// <summary></summary>
        [FieldOffset(0)]
        public System.Int32 lfHeight;
        /// <summary></summary>
        [FieldOffset(sizeof(System.Int32))]
        public System.Int32 lfWidth;
        /// <summary></summary>
        [FieldOffset(sizeof(System.Int32) * 2)]
        public System.Int32 lfEscapement;
        /// <summary></summary>
        [FieldOffset(sizeof(System.Int32) * 3)]
        public System.Int32 lfOrientation;
        /// <summary></summary>
        [FieldOffset(sizeof(System.Int32) * 4)]
        public System.Int32 lfWeight;
        /// <summary></summary>
        [FieldOffset(sizeof(System.Int32) * 5)]
        public System.Byte lfItalic;
        /// <summary></summary>
        [FieldOffset((sizeof(System.Int32) * 5) + 1)]
        public System.Byte lfUnderline;
        /// <summary></summary>
        [FieldOffset((sizeof(System.Int32) * 5) + 2)]
        public System.Byte lfStrikeOut;
        /// <summary></summary>
        [FieldOffset((sizeof(System.Int32) * 5) + 3)]
        public System.Byte lfCharSet;
        /// <summary></summary>
        [FieldOffset((sizeof(System.Int32) * 5) + 4)]
        public System.Byte lfOutPrecision;
        /// <summary></summary>
        [FieldOffset((sizeof(System.Int32) * 5) + 5)]
        public System.Byte lfClipPrecision;
        /// <summary></summary>
        [FieldOffset((sizeof(System.Int32) * 5) + 6)]
        public System.Byte lfQuality;
        /// <summary></summary>
        [FieldOffset((sizeof(System.Int32) * 5) + 7)]
        public System.Byte lfPitchAndFamily;
        /// <summary></summary>
        [FieldOffset((sizeof(System.Int32) * 5) + 8)]
        public fixed System.Char lfFaceName[LF_FACESIZE];

        /// <summary>Sets data on the <see cref="lfFaceName"/> field.</summary>
        /// <param name="faceName">The face name string to assign.</param>
        [Throws(typeof(ArgumentNullException))]
        public void SetFaceName(System.String faceName)
        {
            ArgumentNullException.ThrowIfNull(faceName);
            Unsafe.CopyBlockUnaligned(
                ref Unsafe.As<System.Char, System.Byte>(ref lfFaceName[0]), 
                ref Unsafe.As<System.Char, System.Byte>(ref Unsafe.AsRef(in faceName.GetPinnableReference())), 
                ((faceName.Length > LF_FACESIZE) ? LF_FACESIZE : faceName.Length).ToUInt32() * sizeof(System.Char)
            );
        }

        /// <summary>Retrieves data from the <see cref="lfFaceName"/> field.</summary>
        /// <returns>The currently assigned face name.</returns>
        [return: NotNull]
        public System.String GetFaceName()
        {
            int count = 0;
            fixed (System.Char* pp = lfFaceName)
            {
                System.Char* temp = pp;
                while (count < LF_FACESIZE && *temp != '\0') { temp++; count++; }
                return new(pp, 0, count);
            }
        }
    }
}