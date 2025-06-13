using System;
using Microsoft.IO;
using System.Drawing;
using Microsoft.Win32.SafeHandles;

namespace MP.Imaging
{
    /// <summary>
    /// Decodes raw bitmap images from a .NET stream directly. <br />
    /// The reader is fully managed.
    /// </summary>
    public unsafe sealed class RawBitmapReader : IImage
    {
        private const System.Byte RLE_EndOfLine = 0 , RLE_EndOfBitmap = 1 , RLE_Delta = 2;
        
        private System.IO.Stream rds;
        private BITMAPINFOHEADER info;
        private SafeLibcMemoryHandle memory;
        private System.Int64 size, baseidx , beforehdridx;

        private RawBitmapReader()
        {
            size = 0;
            baseidx = 0;
            info = default;
            memory = null;
            rds = null;
        }
        
        /// <summary>
        /// Creates a new instance of the <see cref="RawBitmapReader"/> class by reading the specified stream that contains the Windows bitmap to read.
        /// </summary>
        /// <param name="strm">The stream to read the bitmap from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="strm"/> was <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="strm"/> was not both readable and seekable, or it was not a valid Windows bitmap.</exception>
        /// <exception cref="NotSupportedException">Cannot find an appropriate decoder for the given image.</exception>
        public RawBitmapReader(System.IO.Stream strm) : this()
        {
            rds = strm;
            if (rds is null) { throw new ArgumentNullException(nameof(strm)); }
            if (rds.CanSeek == false) { throw new ArgumentException("Stream was unseekable." , nameof(strm)); }
            if (rds.CanRead == false) { throw new ArgumentException("Stream was unreadable.", nameof(strm)); }
            BITMAPFILEHEADER file = rds.ReadStructure<BITMAPFILEHEADER>();
            if (file.Type != BITMAPFILEHEADER.BMTYPE) {
                throw new ArgumentException("The given stream is not a bitmap stream," , nameof(strm));
            }
            size = file.Size;
            beforehdridx = rds.Position;
            // Skip BITMAPV4/V5HEADER structures.
            // We do only need the BITMAPINFOHEADER to be read for a minimal decode.
            // We might later need to read BITMAPV4/V5HEADER, see ReadColors method for more info.
            info = rds.ReadStructure<BITMAPINFOHEADER>();
            // Seek back by the known size of the raw BITMAPINFOHEADER
            // Then, seek forward again past the final header bytes.
            baseidx = rds.Seek(info.Size - sizeof(BITMAPINFOHEADER), System.IO.SeekOrigin.Current);
            ReadColors();
            if (memory is null) { throw new NotSupportedException("The current bitmap stream is not supported by this reader."); }
            rds = null;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="RawBitmapReader"/> class by reading the specified file.
        /// </summary>
        /// <param name="filepath">The path to the file to be read by <see cref="RawBitmapReader"/>.</param>
        /// <returns>The created bitmap reader, initialized from the specified file path.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="filepath"/> was <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">The given file data do not represent a valid Windows bitmap.</exception>
        /// <exception cref="NotSupportedException">Cannot find an appropriate decoder for the given image.</exception>
        public static RawBitmapReader FromFile(System.String filepath)
        {
            FileStream fsm = null;
            try {
                fsm = new(filepath , FileMode.Open , FileAccess.Read , FileShare.Read);
                return new(fsm);
            } finally {
                fsm?.Dispose();
                fsm = null;
            }
        }

        /// <summary>
        /// Unlike the constructor alternative , this method is used to skip the bitmap file header,
        /// making it suitable to be used, for example, for raw RES resources.
        /// </summary>
        /// <param name="strm">The stream to read from.</param>
        /// <returns>The read bitmap data.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="strm"/> was <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="strm"/> was not both readable and seekable.</exception>
        /// <exception cref="NotSupportedException">Cannot find an appropriate decoder for the given image.</exception>
        public static RawBitmapReader RawDecode(System.IO.Stream strm)
        {
            RawBitmapReader dec = new();
            dec.rds = strm;
            if (dec.rds is null) { throw new ArgumentNullException(nameof(strm)); }
            if (dec.rds.CanSeek == false) { throw new ArgumentException("Stream was unseekable.", nameof(strm)); }
            if (dec.rds.CanRead == false) { throw new ArgumentException("Stream was unreadable.", nameof(strm)); }
            dec.beforehdridx = dec.rds.Position;
            dec.info = dec.rds.ReadStructure<BITMAPINFOHEADER>();
            dec.baseidx = dec.rds.Seek(dec.info.Size - sizeof(BITMAPINFOHEADER), System.IO.SeekOrigin.Current);
            dec.ReadColors();
            if (dec.memory is null) { throw new NotSupportedException("The current bitmap stream is not supported by this reader."); }
            dec.rds = null;
            return dec;
        }

        /// <summary>
        /// Unlike the constructor alternative , this method is used to skip the bitmap file header,
        /// making it suitable to be used, for example, for raw icon bitmap resources.
        /// </summary>
        /// <param name="strm">The stream to read from.</param>
        /// <returns>The read bitmap data.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="strm"/> was <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="strm"/> was not both readable and seekable.</exception>
        /// <exception cref="NotSupportedException">Cannot find an appropriate decoder for the given image.</exception>
        public static RawBitmapReader RawBitmapIconDecode(System.IO.Stream strm)
        {
            RawBitmapReader dec = new();
            dec.rds = strm;
            if (dec.rds is null) { throw new ArgumentNullException(nameof(strm)); }
            if (dec.rds.CanSeek == false) { throw new ArgumentException("Stream was unseekable.", nameof(strm)); }
            if (dec.rds.CanRead == false) { throw new ArgumentException("Stream was unreadable.", nameof(strm)); }
            dec.beforehdridx = dec.rds.Position;
            dec.info = dec.rds.ReadStructure<BITMAPINFOHEADER>();
            dec.baseidx = dec.rds.Seek(dec.info.Size - sizeof(BITMAPINFOHEADER), System.IO.SeekOrigin.Current);
            // Modify the height so that we do only get the factual image bytes , effectively skipping the 1-bit XOR indexes at the end (because we do want to load these as images only).
            dec.info.Height /= 2;
            dec.ReadColors();
            if (dec.memory is null) { throw new NotSupportedException("The current bitmap stream is not supported by this reader."); }
            dec.rds = null;
            return dec;
        }

        private void ReadColors()
        {
            // This method tests and routes appropriately which bitmap decoder should be used,
            // and prepares everything that the decoder might need.
            // Every decoder defined is into a new method which does all the heavy lifting.
            // This is just the common caller for the constructor and the two static methods.
            rds.Seek(baseidx , System.IO.SeekOrigin.Begin);
            switch (info.Compression)
            {
                case ImageType.BI_RGB:
                    switch (info.BitCount) {
                        case <= 8:
                            Read8BPPLessRGB(); 
                            return;
                        case 16: // When not specified along with BI_BITFIELDS , 16-bit decodes are always RGB555.
                            Read16BPP_RGB555();
                            return;
                        case 24:
                            Read24BPP();
                            return;
                        case 32:
                            Read32BPP();
                            return;
                    }
                    break;
                case ImageType.BI_BITFIELDS:
                    BITFIELDS current;
                    System.UInt32 alphabitmask = 0x000000FF;
                    if (info.Size >= sizeof(BITMAPV4HEADER))
                    {
                        // In such case, however, we do need the BITMAPV4HEADER to be read out.
                        // The helper field beforehdridx will effectively help us out to solve this problem and read the factual structure.
                        rds.Seek(beforehdridx, System.IO.SeekOrigin.Begin);
                        BITMAPV4HEADER bv4 = rds.ReadStructure<BITMAPV4HEADER>();
                        current = bv4.Masks; // Get masks
                        alphabitmask = bv4.AlphaMask; // Get the alpha mask
                        // OK. We have now whatever we need, we can once again skip the header bytes.
                        rds.Seek(baseidx , System.IO.SeekOrigin.Begin);
                    } else {
                        // The header is BITMAPINFOHEADER or older, which it was used to append these fields after the structure end.
                        // Plus, the alpha bitmask seems not to be defined, neither for 32-bit BITMAPINFOHEADER images.
                        current = rds.ReadStructure<BITFIELDS>();
                    }
                    switch (info.BitCount)
                    {
                        case 16:
                            // When we deal with images with bitfields and have 16-bit count, we have to find the RGB type to decode.
                            // By MSFT's definition, only RGB555 and RGB565 are implemented for 16-bit.
                            if (BITFIELDS.BitFieldsEqual(current, BITFIELDS.RGB555)) {
                                Read16BPP_RGB555();
                            } else if (BITFIELDS.BitFieldsEqual(current, BITFIELDS.RGB565)) {
                                Read16BPP_RGB565();
                            }
                            return;
                        // NOTE: We need to revisit cases 24 and 32 as it seems that bitmaps must be always saved into BGRA format
                        case 24:
                            // For 24-bit images we must explicitly pass the bitmasks around.
                            // However that is quite easy enough to do.
                            // Note that I pass for the alpha bitmask a value of 0 because 24-bit images do not define this!
                            Read24BPPFromBitFields(new(current, 0));
                            return;
                        case 32:
                            // For 32-bit images we must explicitly pass the bitmasks around.
                            // However that is quite easy enough to do:
                            Read32BPPFromBitFields(new(current , alphabitmask));
                            return;
                        default:
                            throw new NotImplementedException($"Bit Field decode out of {info.BitCount} bits is not implemented.");
                    }
                case ImageType.BI_RLE8:
                    ReadRLE8();
                    return;
                case ImageType.BI_RLE4:
                    ReadRLE4();
                    return;
                default:
                    throw new NotSupportedException($"Unsupported image type {info.Compression}");
            }
        }

        #region Decoders

        private void Read8BPPLessRGB()
        {
            // BI_RGB (<= 8 bits , color tables are required)
            System.Int32 I = 0;
            RGBQUAD[] colortable = new RGBQUAD[info.ColorTablesCount];
            // Read all color tables from the bitmap.
            while (I < colortable.Length)
            {
                colortable[I] = rds.ReadStructure<RGBQUAD>();
                I++;
            }
            System.Byte[] bitmapbits = rds.ReadBytes(info.DataSize);
            System.UInt32 bitalignment = (UnsafeMethods.PadAlignment(4, (info.Width * info.BitCount) / 8) * 8).ToUInt32();
            System.Int32 X, Y = 0 , ht = info.AbsoluteHeight;
            memory = new(info.Width * ht * 4);
            System.UInt32 idx = 0; // Declare it as a UInt32 because we have to store bit indexes , so automatically an index like this requires index * 8 numerical values!!
            System.Byte* temp;
            while (Y < ht)
            {
                X = 0;
                while (X < info.Width) {
                    RGBQUAD quad = colortable[bitmapbits.ReadBitLevelNumber((idx >> 3).ToInt32(), info.BitCount)];
                    temp = memory.MemoryPointer + (Y * info.Width + X) * 4;
                    // Because the data are saved as bit indexes to the color tables,
                    // the below just hands over the known RGB color retrieved from the color tables of the bitmap.
                    temp[0] = 255;
                    temp[1] = quad.R;
                    temp[2] = quad.G;
                    temp[3] = quad.B;
                    idx += info.BitCount;
                    X++;
                }
                // In each line end, add the alignment if any.
                idx += bitalignment;
                Y++;
            }
            colortable = null;
            bitmapbits = null;
        }

        private void Read16BPP_RGB565()
        {
            const System.UInt16 RedMask = 0xF800 , GreenMask = 0x7E0 , BlueMask = 0x1F;
            // BI_RGB (Image is 16-bit or more)
            // Skip the color table , we do not need it.
            rds.Seek(info.ColorTablesCount * sizeof(RGBQUAD) , System.IO.SeekOrigin.Current);
            System.Byte[] bitmapbytes = rds.ReadBytes(info.DataSize);
            System.Int32 bytealignment = UnsafeMethods.PadAlignment(4, (info.Width * info.BitCount) / 8);
            System.UInt16 pixel;
            System.Int32 idx = 0, X, Y = 0 , ht = info.AbsoluteHeight;
            memory = new(info.Width * ht * 4);
            System.Byte* temp;
            while (Y < ht) 
            {
                X = 0;
                while (X < info.Width) 
                {
                    pixel = bitmapbytes.ToUInt16(idx);
                    temp = memory.MemoryPointer + (Y * info.Width + X) * 4;
                    temp[0] = 255;
                    temp[1] = (((pixel & RedMask) >> 11) << 3).ToByte();
                    temp[2] = (((pixel & GreenMask) >> 5) << 2).ToByte();
                    temp[3] = ((pixel & BlueMask) << 3).ToByte();
                    X++;
                    idx += sizeof(System.UInt16);
                }
                idx += bytealignment;
                Y++;
            }
            bitmapbytes = null;
        }

        private void Read16BPP_RGB555()
        {
            const System.UInt16 RedMask = 0x7C00, GreenMask = 0x3E0, BlueMask = 0x1F;
            // BI_RGB or BI_BITFIELDS (Image is 16-bit)
            // Skip the color table , we do not need it.
            rds.Seek(info.ColorTablesCount * sizeof(RGBQUAD), System.IO.SeekOrigin.Current);
            System.Byte[] bitmapbytes = rds.ReadBytes(info.DataSize);
            System.Int32 bytealignment = UnsafeMethods.PadAlignment(4, (info.Width * info.BitCount) / 8);
            System.UInt16 pixel;
            System.Int32 X, Y = 0 , idx = 0, ht = info.AbsoluteHeight;
            memory = new(info.Width * ht * 4);
            System.Byte* temp;
            while (Y < ht)
            {
                X = 0;
                while (X < info.Width)
                {
                    pixel = bitmapbytes.ToUInt16(idx);
                    temp = memory.MemoryPointer + (Y * info.Width + X) * 4;
                    temp[0] = 255;
                    temp[1] = (((pixel & RedMask) >> 10) << 3).ToByte();
                    temp[2] = (((pixel & GreenMask) >> 5) << 3).ToByte();
                    temp[3] = ((pixel & BlueMask) << 3).ToByte();
                    X++;
                    idx += sizeof(System.UInt16);
                }
                idx += bytealignment;
                Y++;
            }
            bitmapbytes = null;
        }

        private void Read24BPP()
        {
            // BI_RGB (Image is 24-bit with 8 bits reference for each color).
            // Skip the color table , we do not need it.
            rds.Seek(info.ColorTablesCount * sizeof(RGBQUAD), System.IO.SeekOrigin.Current);
            // Data are packed as BGR.
            System.Byte[] bitmapbytes = rds.ReadBytes(info.DataSize);
            // MSFT says for all bitmaps that their scan lines must be DWORD-aligned.
            // The DWORD corresponds exactly to .NET's System.UInt32 type , thus it's size is 4 bytes.
            // To get a single scan length, we get the length in pixels multiplied by the number of bits required to represent each pixel
            // and dividing the result by 8 gives us the byte count.
            System.Int32 bytealignment = UnsafeMethods.PadAlignment(4, (info.Width * info.BitCount) / 8);
            System.Int32 X, Y = 0 , idx= 0 , ht = info.AbsoluteHeight;
            // Allocate resources, the final pixel data will be saved ala ARGB.
            memory = new(info.Width * ht * 4);
            System.Byte* temp;
            while (Y < ht)
            {
                X = 0; // X always plays between 0..info.Width-1 for each scan
                while (X < info.Width)
                {
                    temp = memory.MemoryPointer + (Y * info.Width + X) * 4;
                    temp[0] = 255; // Alpha color component, not used for this type so make this pixel always visible.
                    temp[1] = bitmapbytes[idx + 2]; // Red
                    temp[2] = bitmapbytes[idx + 1]; // Green
                    temp[3] = bitmapbytes[idx]; // Blue
                    X++;
                    idx += 3;
                }
                // A byte alignment might be needed in each scan so add this to the array index.
                idx += bytealignment;
                Y++;
            }
            // deallocate resources that are useless anymore
            bitmapbytes = null;
        }

        private void Read24BPPFromBitFields(BitFieldDecoder dec)
        {
            // BI_BITFIELDS (Image is 24-bit with 8 bits reference for each color, no alpha channel).
            // This is a very rare use case where the colors of a 24-bit bitmap is set via bit masks.
            // These bit masks define the data organization and intensities for the bitmap.
            // Skip the color table , we do not need it.
            rds.Seek(info.ColorTablesCount * sizeof(RGBQUAD), System.IO.SeekOrigin.Current);
            System.Byte[] bitmapbytes = rds.ReadBytes(info.DataSize);
            // MSFT says for all bitmaps that their scan lines must be DWORD-aligned.
            // The DWORD corresponds exactly to .NET's System.UInt32 type , thus it's size is 4 bytes.
            // To get a single scan length, we get the length in pixels multiplied by the number of bits required to represent each pixel
            // and dividing the result by 8 gives us the byte count.
            System.Int32 bytealignment = UnsafeMethods.PadAlignment(4, (info.Width * info.BitCount) / 8);
            System.Int32 X, Y = 0, idx = 0, ht = info.AbsoluteHeight;
            // Allocate resources, the final pixel data will be saved ala ARGB.
            memory = new(info.Width * ht * 4);
            while (Y < ht)
            {
                X = 0; // X always plays between 0..info.Width-1 for each scan
                while (X < info.Width)
                {
                    dec.ToARGB(memory.MemoryPointer + ((Y * info.Width + X) * 4), bitmapbytes, idx);
                    X++;
                    idx += 3;
                }
                // A byte alignment might be needed in each scan so add this to the array index.
                idx += bytealignment;
                Y++;
            }
            // deallocate resources that are useless anymore
            bitmapbytes = null;
        }

        private void Read32BPP()
        {
            // BI_RGB (Image is 32-bit with 8 bits reference for each color).
            // Skip the color table , we do not need it.
            rds.Seek(info.ColorTablesCount * sizeof(RGBQUAD), System.IO.SeekOrigin.Current);
            // Data are packed as BGRA.
            // Byte alignment in 32-bit images is not required since these will always be aligned correctly.
            System.Byte[] bitmapbytes = rds.ReadBytes(info.DataSize);
            System.Int32 X, Y = 0, idx = 0, ht = info.AbsoluteHeight;
            memory = new(info.Width * ht * 4);
            System.Byte* temp;
            while (Y < ht)
            {
                X = 0;
                while (X < info.Width)
                {
                    temp = memory.MemoryPointer + (Y * info.Width + X) * 4;
                    temp[0] = bitmapbytes[idx + 3];
                    temp[1] = bitmapbytes[idx + 2];
                    temp[2] = bitmapbytes[idx + 1];
                    temp[3] = bitmapbytes[idx];
                    idx += 4;
                    X++;
                }
                Y++;
            }
            bitmapbytes = null;
        }

        private void Read32BPPFromBitFields(BitFieldDecoder dec)
        {
            // BI_BITFIELDS (Image is 32-bit with 8 bits reference for each color).
            // This is a very rare use case where the colors of a 32-bit bitmap is set via bit masks.
            // These bit masks define the data organization and intensities for the bitmap.
            // Skip the color table , we do not need it.
            rds.Seek(info.ColorTablesCount * sizeof(RGBQUAD), System.IO.SeekOrigin.Current);
            // Byte alignment in 32-bit images is not required since these will always be aligned correctly.
            System.Byte[] bitmapbytes = rds.ReadBytes(info.DataSize);
            System.Int32 X, Y = 0, idx = 0, ht = info.AbsoluteHeight;
            memory = new(info.Width * ht * 4);
            while (Y < ht)
            {
                X = 0; // X always plays between 0..info.Width-1 for each scan
                while (X < info.Width)
                {
                    dec.ToARGB(memory.MemoryPointer + ((Y * info.Width + X) * 4), bitmapbytes, idx);
                    idx += 4;
                    X++;
                }
                Y++;
            }
            bitmapbytes = null;
        }

        private void ReadRLE8()
        {
            // BI_RLE8 (== 8 bits , color tables are required)
            System.Int32 i = 0;
            RGBQUAD[] colortable = new RGBQUAD[info.ColorTablesCount];
            // Read all color tables from the bitmap.
            while (i < colortable.Length)
            {
                colortable[i] = rds.ReadStructure<RGBQUAD>();
                i++;
            }
            memory = new(info.Height * info.Width * 4);
            memory.ZeroMemory(); // Zero the entire memory contents.
            System.Byte* pptr = memory.MemoryPointer;
            // Portions of the code used are adapted from https://github.com/Extender/BMPDecoder/blob/master/bmp.cpp
            // CPP code was adapted to C# by mdcdi1315 at 2025.
            System.Int32 Y = info.Height > 0 ? info.Height - 1 : 0, X = 0;
            System.Int32 height = info.Height;
            System.Int32 width = info.Width;
            if (height <= 0) { height *= -1; }
            System.Byte nextbyte, secondbyte;
            System.Boolean run = true;
            while (run)
            {
                nextbyte = rds.ReadLiteralByte();
                if (nextbyte > 0)
                {
                    // Encoded mode
                    // First byte: number of pixels
                    // Second byte: indexed color
                    RGBQUAD color = colortable[rds.ReadLiteralByte()];
                    for (System.Byte pos = 0; pos < nextbyte; pos++) {
                        System.Int32 bi = Y * width + X++;
                        pptr[bi] = 255;
                        pptr[bi + 1] = color.R;
                        pptr[bi + 2] = color.G;
                        pptr[bi + 3] = color.B;
                    }
                }
                else
                {
                    secondbyte = rds.ReadLiteralByte();
                    if (secondbyte > 0x2) {
                        // Absolute mode
                        RGBQUAD quad;
                        for (System.Byte pos = 0; pos < secondbyte; pos++) {
                            System.Int32 bi = Y * width + X++;
                            quad = colortable[rds.ReadLiteralByte()];
                            pptr[bi] = 255;
                            pptr[bi + 1] = quad.R;
                            pptr[bi + 2] = quad.G;
                            pptr[bi + 3] = quad.B;
                        }
                        rds.Seek(((secondbyte + 1) / 2) % 2, System.IO.SeekOrigin.Current); // Run must be word-aligned.
                    } else {
                        switch (secondbyte)
                        {
                            case RLE_EndOfLine:
                                // End of line.
                                if (height > 0) { Y--; } else { Y++; }
                                X = 0;
                                break;
                            case RLE_EndOfBitmap:
                                // The bitmap ended , exit.
                                run = false;
                                break;
                            case RLE_Delta:
                                // Reposition X and Y appropriately.
                                X += rds.ReadLiteralByte();
                                Y += rds.ReadLiteralByte();
                                break;
                        }
                    }
                }
            }
            colortable = null;
        }

        private void ReadRLE4()
        {
            // BI_RLE4 (<= 4 bits , color tables are required)
            System.Int32 i = 0;
            RGBQUAD[] colortable = new RGBQUAD[info.ColorTablesCount];
            // Read all color tables from the bitmap.
            while (i < colortable.Length)
            {
                colortable[i] = rds.ReadStructure<RGBQUAD>();
                i++;
            }
            memory = new(info.Height * info.Width * 4);
            memory.ZeroMemory(); // Zero the entire memory contents.
            System.Byte* pptr = memory.MemoryPointer;
            // Portions of the code used are adapted from https://github.com/Extender/BMPDecoder/blob/master/bmp.cpp
            // CPP code was adapted to C# by mdcdi1315 at 2025.
            System.Int32 height = info.Height , width = info.Width;
            System.Int32 Y = height > 0 ? height - 1 : 0, X = 0;
            if (height <= 0) { height *= -1; }
            System.Byte[] command;
            System.Boolean run = true;
            while (run)
            {
                command = rds.ReadBytes(2);
                if (command[0] > 0) {
                    // Encoded mode
                    // First byte: number of pixels
                    // Second byte: indexed colors (2)!
                    RGBQUAD q1 = colortable[command[1].ReadBitLevelByte(0, 4)];
                    RGBQUAD q2 = colortable[command[1].ReadBitLevelByte(4, 4)];
                    for (System.Byte pos = 0; pos < command[0]; pos++)
                    {
                        System.Int32 bi = (Y * width + X++) * 4;
                        if ((pos & 1) > 0) {
                            pptr[bi] = 255;
                            pptr[bi + 1] = q2.R;
                            pptr[bi + 2] = q2.G;
                            pptr[bi + 3] = q2.B;
                        } else {
                            pptr[bi] = 255;
                            pptr[bi + 1] = q1.R;
                            pptr[bi + 2] = q1.G;
                            pptr[bi + 3] = q1.B;
                        }
                    }
                } else {
                    if (command[1] > 0x2)
                    {
                        // Absolute mode
                        System.Byte pixel = 0;
                        bool second;
                        RGBQUAD tq;
                        System.Byte[] indices = rds.ReadBytes(command[1]);
                        System.Byte I = 0;
                        for (System.Byte pos = 0; pos < command[1]; pos++)
                        {
                            if (!(second = (pos & 1) > 0)) { pixel = indices[I++]; }
                            System.Int32 bi = (Y * width + X++) * 4;
                            tq = colortable[pixel.ReadBitLevelByte((second ? 4 : 0).ToByte() , 4)];
                            pptr[bi] = 255;
                            pptr[bi + 1] = tq.R;
                            pptr[bi + 2] = tq.G;
                            pptr[bi + 3] = tq.B;
                        }
                        indices = null;
                        rds.Seek((((uint)command[1] + 1) / 2) & 1, System.IO.SeekOrigin.Current); // Run must be word-aligned.
                    } else {
                        switch (command[1])
                        {
                            case RLE_EndOfLine:
                                // End of line.
                                if (height > 0) { Y--; } else { Y++; }
                                X = 0;
                                break;
                            case RLE_EndOfBitmap:
                                // The bitmap ended , exit.
                                run = false;
                                break;
                            case RLE_Delta:
                                // Reposition X and Y appropriately.
                                X += rds.ReadLiteralByte();
                                Y += rds.ReadLiteralByte();
                                break;
                        }
                    }
                }
            }
            colortable = null;
        }
        
        #endregion

        public System.Single HorizontalResolution => info.HorizontalResolution;

        public System.Single VerticalResolution => info.VerticalResolution;

        public System.Int32 BitsPerPixel => info.BitCount;

        public System.UInt32 ColorTables => info.ColorTablesCount;

        public System.Boolean IsFlippedVertically => info.Height > 0;

        public System.Byte* NativePointer => memory.MemoryPointer;

        // Always it is ARGB because all the defined decoders always
        // decode into ARGB so that it can be easy for consumers
        // to manipulate the data in an atomic way.
        // However, by IImage definition the PixelFormat is not standard between images.
        public ImagePixelFormat PixelFormat => ImagePixelFormat.ARGB;

        public Size Size => new(info.Width , info.AbsoluteHeight);

        public System.UInt32 OriginalType => (System.UInt32)info.Compression;

        public void Dispose() 
        {
            memory?.Dispose();
            memory = null;
            info = default;
            size = 0;
            baseidx = 0;
        }
    }
}
