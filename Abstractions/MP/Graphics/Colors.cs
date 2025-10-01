

using System.Runtime.CompilerServices;

namespace MP.Graphics
{
    /// <summary>
    /// Defines system and well-known colors, as well as some color manipulation methods.
    /// </summary>
    public static class Colors
    {
        /// <summary>
        /// Converts a 32-bit integer to an <see cref="IColor"/> instance.
        /// </summary>
        /// <param name="argb">The ARGB value to convert.</param>
        /// <returns>The converted ARGB value.</returns>
        public static IColor FromArgb(this int argb) => Unsafe.As<int, ARGBColor>(ref argb);

        /// <summary>
        /// Converts a 32-bit unsigned integer to an <see cref="IColor"/> instance.
        /// </summary>
        /// <param name="argb">The ARGB value to convert.</param>
        /// <returns>The converted ARGB value.</returns>
        public static IColor FromArgb(this uint argb) => Unsafe.As<uint, ARGBColor>(ref argb);

        /// <summary>
        /// Converts a 32-bit integer to an <see cref="IColor"/> instance.
        /// </summary>
        /// <param name="rgba">The RGBA value to convert.</param>
        /// <returns>The converted RGBA value.</returns>
        public static IColor FromRgba(this int rgba) => Unsafe.As<int, RGBAColor>(ref rgba);

        /// <summary>
        /// Converts a 32-bit unsigned integer to an <see cref="IColor"/> instance.
        /// </summary>
        /// <param name="rgba">The RGBA value to convert.</param>
        /// <returns>The converted RGBA value.</returns>
        public static IColor FromRgba(this uint rgba) => Unsafe.As<uint, RGBAColor>(ref rgba);

        /// <summary>
        /// Converts a 32-bit integer to an <see cref="HSLColor"/> instance.
        /// </summary>
        /// <param name="hsl">The color to convert.</param>
        /// <returns>The translated HSL color from <paramref name="hsl"/>.</returns>
        public static HSLColor FromHsl(this int hsl) => Unsafe.As<int, HSLColor>(ref hsl);

        /// <summary>
        /// Converts any <see cref="IColor"/> instance to a <see cref="FloatColor"/> instance.
        /// </summary>
        /// <param name="color">The color to convert.</param>
        /// <returns>The converted color in floating point format.</returns>
        public static FloatColor ToFloat(this IColor color) => new(color);

        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FF66CDAA.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor MediumAquamarine => FromArgb(0xFF66CDAA);

        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FF0000CD.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor MediumBlue => FromArgb(0xFF0000CD);

        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FFBA55D3.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor MediumOrchid => FromArgb(0xFFBA55D3);

        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FF9370DB.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor MediumPurple => FromArgb(0xFF9370DB);

        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FF3CB371.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor MediumSeaGreen => FromArgb(0xFF3CB371);

        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FF7B68EE.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor MediumSlateBlue => FromArgb(0xFF7B68EE);

        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FF00FA9A.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor MediumSpringGreen => FromArgb(0xFF00FA9A);

        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FF48D1CC.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor MediumTurquoise => FromArgb(0xFF48D1CC);
        
        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FFC71585.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor MediumVioletRed => FromArgb(0xFFC71585);
        
        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FF191970.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor MidnightBlue => FromArgb(0xFF191970);

        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FFF5FFFA.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor MintCream => FromArgb(0xFFF5FFFA);

        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FFFFE4E1.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor MistyRose => FromArgb(0xFFFFE4E1);
        
        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FFFFE4B5.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor Moccasin => FromArgb(0xFFFFE4B5);
        
        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FFFFDEAD.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor NavajoWhite => FromArgb(0xFFFFDEAD);
        
        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FF000080.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor Navy => FromArgb(0xFF000080);

        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FFFDF5E6.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor OldLace => FromArgb(0xFFFDF5E6);

        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FF808000.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor Olive => FromArgb(0xFF808000);
        
        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FF800000.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor Maroon => FromArgb(0xFF800000);
        
        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FF6B8E23.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor OliveDrab => FromArgb(0xFF6B8E23);
        
        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FFFF00FF.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor Magenta => FromArgb(0xFFFF00FF);
        
        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FF32CD32.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor LimeGreen => FromArgb(0xFF32CD32);

        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FFFFF0F5.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor LavenderBlush => FromArgb(0xFFFFF0F5);
        
        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FF7CFC00.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor LawnGreen => FromArgb(0xFF7CFC00);
        
        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FFFFFACD.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor LemonChiffon => FromArgb(0xFFFFFACD);
        
        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FFADD8E6.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor LightBlue => FromArgb(0xFFADD8E6);
        
        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FFF08080.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor LightCoral => FromArgb(0xFFF08080);
        
        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FFE0FFFF.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor LightCyan => FromArgb(0xFFE0FFFF);
        
        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FFFAFAD2.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor LightGoldenrodYellow => FromArgb(0xFFFAFAD2);
        
        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FFD3D3D3.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor LightGray => FromArgb(0xFFD3D3D3);
       
        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FF90EE90.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor LightGreen => FromArgb(0xFF90EE90);
        
        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FFFFB6C1.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor LightPink => FromArgb(0xFFFFB6C1);
        
        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FFFFA07A.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor LightSalmon => FromArgb(0xFFFFA07A);
   
        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FF20B2AA.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor LightSeaGreen => FromArgb(0xFF20B2AA);
       
        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FF87CEFA.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor LightSkyBlue => FromArgb(0xFF87CEFA);
       
        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FF778899.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor LightSlateGray => FromArgb(0xFF778899);
        
        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FFB0C4DE.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor LightSteelBlue => FromArgb(0xFFB0C4DE);
       
        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FFFFFFE0.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor LightYellow => FromArgb(0xFFFFFFE0);

        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FF00FF00.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor Lime => FromArgb(0xFF00FF00);
        
        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FFFAF0E6.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor Linen => FromArgb(0xFFFAF0E6);
        
        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FFFFFF00.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor Yellow => FromArgb(0xFFFFFF00);
        
        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FFFFA500.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor Orange => FromArgb(0xFFFFA500);
       
        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FFDA70D6.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor Orchid => FromArgb(0xFFDA70D6);
        
        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FFC0C0C0.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor Silver => FromArgb(0xFFC0C0C0);
        
        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FF87CEEB.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor SkyBlue => FromArgb(0xFF87CEEB);
        
        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FF6A5ACD.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor SlateBlue => FromArgb(0xFF6A5ACD);
        
        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FF708090.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor SlateGray => FromArgb(0xFF708090);
        
        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FFFFFAFA.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor Snow => FromArgb(0xFFFFFAFA);
       
        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FF00FF7F.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor SpringGreen => FromArgb(0xFF00FF7F);
        
        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FF4682B4.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor SteelBlue => FromArgb(0xFF4682B4);
       
        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FFD2B48C.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor Tan => FromArgb(0xFFD2B48C);
        
        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FF008080.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor Teal => FromArgb(0xFF008080);
        
        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FFD8BFD8.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor Thistle => FromArgb(0xFFD8BFD8);

        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FFFF6347.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor Tomato => FromArgb(0xFFFF6347);

        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #00FFFFFF.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor Transparent => new ARGBColor(0, 255, 255, 255);
       
        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FF40E0D0.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor Turquoise => FromArgb(0xFF40E0D0);
       
        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FFEE82EE.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor Violet => FromArgb(0xFFEE82EE);
        
        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FFF5DEB3.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor Wheat => FromArgb(0xFFF5DEB3);
        
        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FFFFFFFF.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor White => FromArgb(0xFFFFFFFF);
        
        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FFF5F5F5.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor WhiteSmoke => FromArgb(0xFFF5F5F5);
       
        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FFA0522D.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor Sienna => FromArgb(0xFFA0522D);
    
        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FFFF4500.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor OrangeRed => FromArgb(0xFFFF4500);
        
        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FFFFF5EE.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor SeaShell => FromArgb(0xFFFFF5EE);

        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FFF4A460.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor SandyBrown => FromArgb(0xFFF4A460);
      
        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FFEEE8AA.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor PaleGoldenrod => FromArgb(0xFFEEE8AA);

        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FF98FB98.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor PaleGreen => FromArgb(0xFF98FB98);
  
        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FFAFEEEE.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor PaleTurquoise => FromArgb(0xFFAFEEEE);
        
        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FFDB7093.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor PaleVioletRed => FromArgb(0xFFDB7093);

        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FFFFEFD5.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor PapayaWhip => FromArgb(0xFFFFEFD5);

        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FFFFDAB9.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor PeachPuff => FromArgb(0xFFFFDAB9);
       
        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FFCD853F.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor Peru => FromArgb(0xFFCD853F);

        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FFFFC0CB.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor Pink => FromArgb(0xFFFFC0CB);

        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FFDDA0DD.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor Plum => FromArgb(0xFFDDA0DD);
       
        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FFB0E0E6.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor PowderBlue => FromArgb(0xFFB0E0E6);
        
        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FF800080.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor Purple => FromArgb(0xFF800080);
        
        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #663399.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor RebeccaPurple => FromArgb(0x663399);

        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FFFF0000.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor Red => FromArgb(0xFFFF0000);
        
        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FFBC8F8F.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor RosyBrown => FromArgb(0xFFBC8F8F);
        
        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FF4169E1.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor RoyalBlue => FromArgb(0xFF4169E1);

        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FF8B4513.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor SaddleBrown => FromArgb(0xFF8B4513);
       
        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FFFA8072.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor Salmon => FromArgb(0xFFFA8072);

        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FF2E8B57.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor SeaGreen => FromArgb(0xFF2E8B57);
        
        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FFF0E68C.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor Khaki => FromArgb(0xFFF0E68C);

        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FFE6E6FA.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor Lavender => FromArgb(0xFFE6E6FA);

        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FF00FFFF.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor Cyan => FromArgb(0xFF00FFFF);

        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FF8B008B.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor DarkMagenta => FromArgb(0xFF8B008B);

        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FFBDB76B.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor DarkKhaki => FromArgb(0xFFBDB76B);

        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FF006400.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor DarkGreen => FromArgb(0xFF006400);
        
        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FFA9A9A9.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor DarkGray => FromArgb(0xFFA9A9A9);

        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FFB8860B.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor DarkGoldenrod => FromArgb(0xFFB8860B);

        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FF008B8B.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor DarkCyan => FromArgb(0xFF008B8B);

        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FF00008B.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor DarkBlue => FromArgb(0xFF00008B);

        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FFFFFFF0.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor Ivory => FromArgb(0xFFFFFFF0);

        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FFDC143C.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor Crimson => FromArgb(0xFFDC143C);

        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FFFFF8DC.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor Cornsilk => FromArgb(0xFFFFF8DC);

        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FF6495ED.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor CornflowerBlue => FromArgb(0xFF6495ED);

        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FFFF7F50.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor Coral => FromArgb(0xFFFF7F50);

        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FFD2691E.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor Chocolate => FromArgb(0xFFD2691E);

        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FF556B2F.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor DarkOliveGreen => FromArgb(0xFF556B2F);
        
        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FF7FFF00.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor Chartreuse => FromArgb(0xFF7FFF00);
       
        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FFDEB887.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor BurlyWood => FromArgb(0xFFDEB887);

        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FFA52A2A.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor Brown => FromArgb(0xFFA52A2A);

        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FF8A2BE2.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor BlueViolet => FromArgb(0xFF8A2BE2);

        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FF0000FF.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor Blue => FromArgb(0xFF0000FF);

        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FFFFEBCD.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor BlanchedAlmond => FromArgb(0xFFFFEBCD);

        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FF000000.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor Black => FromArgb(0xFF000000);

        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FFFFE4C4.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor Bisque => FromArgb(0xFFFFE4C4);

        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FFF5F5DC.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor Beige => FromArgb(0xFFF5F5DC);

        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FFF0FFFF.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor Azure => FromArgb(0xFFF0FFFF);

        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FF7FFFD4.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor Aquamarine => FromArgb(0xFF7FFFD4);

        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FF00FFFF.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor Aqua => FromArgb(0xFF00FFFF);

        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FFFAEBD7
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor AntiqueWhite => FromArgb(0xFFFAEBD7);

        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FFF0F8FF.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor AliceBlue => FromArgb(0xFFF0F8FF);

        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FF5F9EA0.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor CadetBlue => FromArgb(0xFF5F9EA0);

        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FFFF8C00.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor DarkOrange => FromArgb(0xFFFF8C00);

        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FF9ACD32.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor YellowGreen => FromArgb(0xFF9ACD32);

        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FF8B0000.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor DarkRed => FromArgb(0xFF8B0000);

        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FF4B0082.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor Indigo => FromArgb(0xFF4B0082);
        
        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FFCD5C5C.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor IndianRed => FromArgb(0xFFCD5C5C);

        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FF9932CC.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor DarkOrchid => FromArgb(0xFF9932CC);

        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FFF0FFF0.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor Honeydew => FromArgb(0xFFF0FFF0);

        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FFADFF2F.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor GreenYellow => FromArgb(0xFFADFF2F);

        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FF008000.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor Green => FromArgb(0xFF008000);

        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FF808080.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor Gray => FromArgb(0xFF808080);

        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FFDAA520.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor Goldenrod => FromArgb(0xFFDAA520);

        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FFDAA520.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor Gold => FromArgb(0xFFDAA520);

        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FFF8F8FF.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor GhostWhite => FromArgb(0xFFF8F8FF);
        
        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FFDCDCDC.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor Gainsboro => FromArgb(0xFFDCDCDC);

        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FFFF00FF.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor Fuchsia => FromArgb(0xFFFF00FF);

        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FF228B22.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor ForestGreen => FromArgb(0xFF228B22);

        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FFFF69B4.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor HotPink => FromArgb(0xFFFF69B4);

        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FFB22222.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor Firebrick => FromArgb(0xFFB22222);

        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FFFFFAF0.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor FloralWhite => FromArgb(0xFFFFFAF0);

        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FF1E90FF.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor DodgerBlue => FromArgb(0xFF1E90FF);

        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FF696969.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor DimGray => FromArgb(0xFF696969);

        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FF00BFFF.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor DeepSkyBlue => FromArgb(0xFF00BFFF);

        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FFFF1493.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor DeepPink => FromArgb(0xFFFF1493);

        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FF9400D3.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor DarkViolet => FromArgb(0xFF9400D3);

        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FF00CED1.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor DarkTurquoise => FromArgb(0xFF00CED1);

        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FF2F4F4F.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor DarkSlateGray => FromArgb(0xFF2F4F4F);

        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FF483D8B.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor DarkSlateBlue => FromArgb(0xFF483D8B);

        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FF8FBC8B.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor DarkSeaGreen => FromArgb(0xFF8FBC8B);

        /// <summary>
        /// Gets a system-defined color that has an ARGB value of #FFE9967A.
        /// </summary>
        /// <returns>A <see cref="IColor"/> representing a system-defined color.</returns>
        public static IColor DarkSalmon => FromArgb(0xFFE9967A);

    }
}