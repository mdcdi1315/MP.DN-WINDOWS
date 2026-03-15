
/* 
This class has been based off on the DirectXColors.h Windows header:

//-------------------------------------------------------------------------------------
// DirectXColors.h -- C++ Color Math library
//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// http://go.microsoft.com/fwlink/?LinkID=615560
//-------------------------------------------------------------------------------------

*/

namespace MP.Graphics
{
    /// <summary>
    /// Defines system and well-known colors, expressed instead with <see cref="FloatColor"/> instances. <br />
    /// These provide a better accuracy on the actual physical colors, and these should be used instead
    /// when interoperating with a component that accepts a <see cref="FloatColor"/> instance.
    /// </summary>
    public static class FloatColors
    {
        /// <summary>Gets a system-defined color that has an ARGB value of #FFF0F8FF.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor AliceBlue => new(0.941176534f, 0.972549081f, 1.000000000f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FFFAEBD7.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor AntiqueWhite => new(0.980392218f, 0.921568692f, 0.843137324f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FF00FFFF.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor Aqua => new(0.000000000f, 1.000000000f, 1.000000000f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FF7FFFD4.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor Aquamarine => new(0.498039246f, 1.000000000f, 0.831372619f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FFF0FFFF.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor Azure => new(0.941176534f, 1.000000000f, 1.000000000f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FFF5F5DC.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor Beige => new(0.960784376f, 0.960784376f, 0.862745166f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FFFFE4C4.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor Bisque => new(1.000000000f, 0.894117713f, 0.768627524f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FF000000.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor Black => new(0.000000000f, 0.000000000f, 0.000000000f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FFFFEBCD.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor BlanchedAlmond => new(1.000000000f, 0.921568692f, 0.803921640f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FF0000FF.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor Blue => new(0.000000000f, 0.000000000f, 1.000000000f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FF8A2BE2.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor BlueViolet => new(0.541176498f, 0.168627456f, 0.886274576f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FFA52A2A.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor Brown => new(0.647058845f, 0.164705887f, 0.164705887f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FFA52A2A.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor BurlyWood => new(0.870588303f, 0.721568644f, 0.529411793f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FF5F9EA0.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor CadetBlue => new(0.372549027f, 0.619607866f, 0.627451003f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FF7FFF00.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor Chartreuse => new(0.498039246f, 1.000000000f, 0.000000000f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FFD2691E.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor Chocolate => new(0.823529482f, 0.411764741f, 0.117647067f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FFFF7F50.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor Coral => new(1.000000000f, 0.498039246f, 0.313725501f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FF6495ED.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor CornflowerBlue => new(0.392156899f, 0.584313750f, 0.929411829f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FFFFF8DC.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor Cornsilk => new(1.000000000f, 0.972549081f, 0.862745166f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FFDC143C.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor Crimson => new(0.862745166f, 0.078431375f, 0.235294133f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FF00FFFF.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor Cyan => new(0.000000000f, 1.000000000f, 1.000000000f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FF00008B.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor DarkBlue => new(0.000000000f, 0.000000000f, 0.545098066f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FF008B8B.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor DarkCyan => new(0.000000000f, 0.545098066f, 0.545098066f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FFB8860B.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor DarkGoldenrod => new(0.721568644f, 0.525490224f, 0.043137256f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FFA9A9A9.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor DarkGray => new(0.662745118f, 0.662745118f, 0.662745118f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FF006400.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor DarkGreen => new(0.000000000f, 0.392156899f, 0.000000000f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FFBDB76B.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor DarkKhaki => new(0.741176486f, 0.717647076f, 0.419607878f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FF8B008B.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor DarkMagenta => new(0.545098066f, 0.000000000f, 0.545098066f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FF556B2F.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor DarkOliveGreen => new(0.333333343f, 0.419607878f, 0.184313729f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FFFF8C00.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor DarkOrange => new(1.000000000f, 0.549019635f, 0.000000000f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FF9932CC.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor DarkOrchid => new(0.600000024f, 0.196078449f, 0.800000072f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FF8B0000.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor DarkRed => new(0.545098066f, 0.000000000f, 0.000000000f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FFE9967A.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor DarkSalmon => new(0.913725555f, 0.588235319f, 0.478431404f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FF8FBC8B.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor DarkSeaGreen => new(0.560784340f, 0.737254918f, 0.545098066f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FF483D8B.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor DarkSlateBlue => new(0.282352954f, 0.239215702f, 0.545098066f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FF2F4F4F.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor DarkSlateGray => new(0.184313729f, 0.309803933f, 0.309803933f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FF00CED1.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor DarkTurquoise => new(0.000000000f, 0.807843208f, 0.819607913f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FF9400D3.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor DarkViolet => new(0.580392182f, 0.000000000f, 0.827451050f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FFFF1493.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor DeepPink => new(1.000000000f, 0.078431375f, 0.576470613f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FF00BFFF.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor DeepSkyBlue => new(0.000000000f, 0.749019623f, 1.000000000f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FF696969.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor DimGray => new(0.411764741f, 0.411764741f, 0.411764741f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FF1E90FF.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor DodgerBlue => new(0.117647067f, 0.564705908f, 1.000000000f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FFB22222.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor Firebrick => new(0.698039234f, 0.133333340f, 0.133333340f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FFFFFAF0.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor FloralWhite => new(1.000000000f, 0.980392218f, 0.941176534f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FF228B22.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor ForestGreen => new(0.133333340f, 0.545098066f, 0.133333340f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FFFF00FF.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor Fuchsia => new(1.000000000f, 0.000000000f, 1.000000000f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FFDCDCDC.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor Gainsboro => new(0.862745166f, 0.862745166f, 0.862745166f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FFF8F8FF.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor GhostWhite => new(0.972549081f, 0.972549081f, 1.000000000f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FFFFD700.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor Gold => new(1.000000000f, 0.843137324f, 0.000000000f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FFDAA520.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor Goldenrod => new(0.854902029f, 0.647058845f, 0.125490203f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FF808080.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor Gray => new(0.501960814f, 0.501960814f, 0.501960814f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FF008000.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor Green => new(0.000000000f, 0.501960814f, 0.000000000f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FFADFF2F.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor GreenYellow => new(0.678431392f, 1.000000000f, 0.184313729f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FFF0FFF0.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor Honeydew => new(0.941176534f, 1.000000000f, 0.941176534f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FFFF69B4.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor HotPink => new(1.000000000f, 0.411764741f, 0.705882370f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FFCD5C5C.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor IndianRed => new(0.803921640f, 0.360784322f, 0.360784322f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FF4B0082.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor Indigo => new(0.294117659f, 0.000000000f, 0.509803951f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FFFFFFF0.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor Ivory => new(1.000000000f, 1.000000000f, 0.941176534f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FFF0E68C.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor Khaki => new(0.941176534f, 0.901960850f, 0.549019635f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FFE6E6FA.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor Lavender => new(0.901960850f, 0.901960850f, 0.980392218f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FFFFF0F5.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor LavenderBlush => new(1.000000000f, 0.941176534f, 0.960784376f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FF7CFC00.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor LawnGreen => new(0.486274540f, 0.988235354f, 0.000000000f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FFFFFACD.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor LemonChiffon => new(1.000000000f, 0.980392218f, 0.803921640f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FFADD8E6.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor LightBlue => new(0.678431392f, 0.847058892f, 0.901960850f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FFF08080.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor LightCoral => new(0.941176534f, 0.501960814f, 0.501960814f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FFE0FFFF.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor LightCyan => new(0.878431439f, 1.000000000f, 1.000000000f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FFFAFAD2.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor LightGoldenrodYellow => new(0.980392218f, 0.980392218f, 0.823529482f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FF90EE90.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor LightGreen => new(0.564705908f, 0.933333397f, 0.564705908f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FFD3D3D3.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor LightGray => new(0.827451050f, 0.827451050f, 0.827451050f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FFFFB6C1.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor LightPink => new(1.000000000f, 0.713725507f, 0.756862819f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FFFFA07A.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor LightSalmon => new(1.000000000f, 0.627451003f, 0.478431404f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FF20B2AA.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor LightSeaGreen => new(0.125490203f, 0.698039234f, 0.666666687f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FF87CEFA.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor LightSkyBlue => new(0.529411793f, 0.807843208f, 0.980392218f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FF778899.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor LightSlateGray => new(0.466666698f, 0.533333361f, 0.600000024f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FFB0C4DE.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor LightSteelBlue => new(0.690196097f, 0.768627524f, 0.870588303f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FFFFFFE0.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor LightYellow => new(1.000000000f, 1.000000000f, 0.878431439f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FF00FF00.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor Lime => new(0.000000000f, 1.000000000f, 0.000000000f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FF32CD32.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor LimeGreen => new(0.196078449f, 0.803921640f, 0.196078449f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FFFAF0E6.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor Linen => new(0.980392218f, 0.941176534f, 0.901960850f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FFFF00FF.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor Magenta => new(1.000000000f, 0.000000000f, 1.000000000f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FF800000.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor Maroon => new(0.501960814f, 0.000000000f, 0.000000000f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FF66CDAA.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor MediumAquamarine => new(0.400000036f, 0.803921640f, 0.666666687f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FF0000CD.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor MediumBlue => new(0.000000000f, 0.000000000f, 0.803921640f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FFBA55D3.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor MediumOrchid => new(0.729411781f, 0.333333343f, 0.827451050f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FF9370DB.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor MediumPurple => new(0.576470613f, 0.439215720f, 0.858823597f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FF3CB371.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor MediumSeaGreen => new(0.235294133f, 0.701960802f, 0.443137288f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FF7B68EE.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor MediumSlateBlue => new(0.482352972f, 0.407843173f, 0.933333397f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FF00FA9A.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor MediumSpringGreen => new(0.000000000f, 0.980392218f, 0.603921592f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FF48D1CC.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor MediumTurquoise => new(0.282352954f, 0.819607913f, 0.800000072f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FFC71585.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor MediumVioletRed => new(0.780392230f, 0.082352944f, 0.521568656f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FFC71585.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor MidnightBlue => new(0.098039225f, 0.098039225f, 0.439215720f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FFF5FFFA.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor MintCream => new(0.960784376f, 1.000000000f, 0.980392218f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FFFFE4E1.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor MistyRose => new(1.000000000f, 0.894117713f, 0.882353008f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FFFFE4B5.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor Moccasin => new(1.000000000f, 0.894117713f, 0.709803939f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FFFFDEAD.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor NavajoWhite => new(1.000000000f, 0.870588303f, 0.678431392f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FF000080.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor Navy => new(0.000000000f, 0.000000000f, 0.501960814f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FFFDF5E6.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor OldLace => new(0.992156923f, 0.960784376f, 0.901960850f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FF808000.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor Olive => new(0.501960814f, 0.501960814f, 0.000000000f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FF6B8E23.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor OliveDrab => new(0.419607878f, 0.556862772f, 0.137254909f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FFFFFF00.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor Orange => new(1.000000000f, 0.647058845f, 0.000000000f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FFFF4500.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor OrangeRed => new(1.000000000f, 0.270588249f, 0.000000000f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FFDA70D6.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor Orchid => new(0.854902029f, 0.439215720f, 0.839215755f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FFEEE8AA.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor PaleGoldenrod => new(0.933333397f, 0.909803987f, 0.666666687f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FF98FB98.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor PaleGreen => new(0.596078455f, 0.984313786f, 0.596078455f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FFAFEEEE.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor PaleTurquoise => new(0.686274529f, 0.933333397f, 0.933333397f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FFDB7093.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor PaleVioletRed => new(0.858823597f, 0.439215720f, 0.576470613f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FFFFEFD5.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor PapayaWhip => new(1.000000000f, 0.937254965f, 0.835294187f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FFFFDAB9.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor PeachPuff => new(1.000000000f, 0.854902029f, 0.725490212f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FFCD853F.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor Peru => new(0.803921640f, 0.521568656f, 0.247058839f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FFFFC0CB.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor Pink => new(1.000000000f, 0.752941251f, 0.796078503f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FFDDA0DD.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor Plum => new(0.866666734f, 0.627451003f, 0.866666734f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FFB0E0E6.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor PowderBlue => new(0.690196097f, 0.878431439f, 0.901960850f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FF800080.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor Purple => new(0.501960814f, 0.000000000f, 0.501960814f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FFFF0000.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor Red => new(1.000000000f, 0.000000000f, 0.000000000f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FFBC8F8F.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor RosyBrown => new(0.737254918f, 0.560784340f, 0.560784340f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FF4169E1.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor RoyalBlue => new(0.254901975f, 0.411764741f, 0.882353008f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FF8B4513.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor SaddleBrown => new(0.545098066f, 0.270588249f, 0.074509807f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FFFA8072.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor Salmon => new(0.980392218f, 0.501960814f, 0.447058856f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FFF4A460.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor SandyBrown => new(0.956862807f, 0.643137276f, 0.376470625f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FF2E8B57.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor SeaGreen => new(0.180392161f, 0.545098066f, 0.341176480f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FFFFF5EE.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor SeaShell => new(1.000000000f, 0.960784376f, 0.933333397f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FFA0522D.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor Sienna => new(0.627451003f, 0.321568638f, 0.176470593f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FFC0C0C0.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor Silver => new(0.752941251f, 0.752941251f, 0.752941251f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FF87CEEB.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor SkyBlue => new(0.529411793f, 0.807843208f, 0.921568692f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FF6A5ACD.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor SlateBlue => new(0.415686309f, 0.352941185f, 0.803921640f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FF708090.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor SlateGray => new(0.439215720f, 0.501960814f, 0.564705908f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FFFFFAFA.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor Snow => new(1.000000000f, 0.980392218f, 0.980392218f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FF00FF7F.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor SpringGreen => new(0.000000000f, 1.000000000f, 0.498039246f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FF4682B4.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor SteelBlue => new(0.274509817f, 0.509803951f, 0.705882370f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FFD2B48C.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor Tan => new(0.823529482f, 0.705882370f, 0.549019635f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FF008080.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor Teal => new(0.000000000f, 0.501960814f, 0.501960814f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FFD8BFD8.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor Thistle => new(0.847058892f, 0.749019623f, 0.847058892f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FFFF6347.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor Tomato => new(1.000000000f, 0.388235331f, 0.278431386f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #00FFFFFF.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor Transparent => new(0.000000000f, 0.000000000f, 0.000000000f, 0.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FF40E0D0.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor Turquoise => new(0.250980407f, 0.878431439f, 0.815686345f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FFEE82EE.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor Violet => new(0.933333397f, 0.509803951f, 0.933333397f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FFF5DEB3.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor Wheat => new(0.960784376f, 0.870588303f, 0.701960802f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FFFFFFFF.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor White => new(1.000000000f, 1.000000000f, 1.000000000f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FFF5F5F5.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor WhiteSmoke => new(0.960784376f, 0.960784376f, 0.960784376f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FFFFFF00.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor Yellow => new(1.000000000f, 1.000000000f, 0.000000000f, 1.000000000f);

        /// <summary>Gets a system-defined color that has an ARGB value of #FF9ACD32.</summary>
        /// <returns>A <see cref="FloatColor"/> representing a system-defined color.</returns>
        public static FloatColor YellowGreen => new(0.603921592f, 0.803921640f, 0.196078449f, 1.000000000f);
    }
}