
using System;

namespace MP.TagReading.ID3
{
    /// <summary>
    /// Defines a reader for reading a single frame from an ID3V2 tag. <br />
    /// This class cannot be inherited.
    /// </summary>
    public sealed class ID3V2CompleteFrame
    {
        /// <summary>
        /// The raw frame header.
        /// </summary>
        public ID3V2FRAMEHEADER Header;
        /// <summary>
        /// The raw value of the data. Useful for non-textual data.
        /// </summary>
        public System.Byte[] RawValue;
        /// <summary>
        /// The type of this ID3V2 frame.
        /// </summary>
        public ID3V2FrameType Type;
        /// <summary>
        /// Forwards to <see cref="ID3V2FRAMEHEADER.FrameID"/> when non-user defined data , otherwise this is properly initialized
        /// </summary>
        public System.String Key;

        /// <summary>
        /// Retrieves text data if the frame represents a single string.
        /// </summary>
        public System.String TextData
        {
            get
            {
                System.Text.StringBuilder sb = null;
                System.Int32 idx = 0;
                switch (Type)
                {
                    case ID3V2FrameType.UserDefinedTextFrame:
                    // User-defined text frames would have already decoded and the Key field will contain the key of the frame.
                    // When such decoding will happen, a behind-the-scenes task will run that will save the string only as such how the 
                    // the Text fields have been encoded as.
                    case ID3V2FrameType.UnsyncronizedLyrics:
                    case ID3V2FrameType.Comments:
                    case ID3V2FrameType.Text:
                        sb = new(RawValue.Length / 2);
                        ID3V2TextEncoding enc = (ID3V2TextEncoding)RawValue[idx++];
                        switch (enc)
                        {
                            case ID3V2TextEncoding.ISO_8859_1:
                                // Just contains ASCII characters
                                for (; idx < RawValue.Length; idx++) { sb.Append(RawValue[idx].ToChar()); }
                                break;
                            case ID3V2TextEncoding.UTF16BE:
                                // UTF-16 Big-Endian , no byte order marks.
                                sb.Append(new System.Text.UnicodeEncoding(true, false).GetString(RawValue, idx, RawValue.Length - idx));
                                break;
                            case ID3V2TextEncoding.UTF8:
                                // UTF-8 encoding , no byte order marks.
                                sb.Append(new System.Text.UTF8Encoding(false).GetString(RawValue, idx, RawValue.Length - idx));
                                break;
                            case ID3V2TextEncoding.UTF16BOM:
                                // UTF-16 with byte order mark , detect it as follows:
                                System.Boolean bigendian = RawValue[idx] == 254 && RawValue[idx + 1] == 255;
                                idx += 2;
                                sb.Append(new System.Text.UnicodeEncoding(bigendian, false).GetString(RawValue, idx, RawValue.Length - idx));
                                break;
                        }
                        break;
                    case ID3V2FrameType.URL:
                    case ID3V2FrameType.UserDefinedURLLinkFrame: // See comment at UserDefinedTextFrame case.
                        sb = new(RawValue.Length / 2);
                        // URL's are just encoded with ISO-8859-1
                        for (; idx < RawValue.Length; idx++) { sb.Append(RawValue[idx].ToChar()); }
                        break;
                    default:
                        throw new InvalidOperationException("Cannot retrieve binary data as text data.");
                }
                return sb.ToString();
            }
        }

        private static System.String DirectStringBuild(System.Byte[] data, System.Int32 startindex, ID3V2TextEncoding enc, out System.Int32 rb)
        {
            System.Int32 index = startindex;
            rb = 0;
            System.Boolean bigendian = false;
            switch (enc)
            {
                case ID3V2TextEncoding.UTF8:
                case ID3V2TextEncoding.ISO_8859_1:
                    for (; index < data.Length; index++)
                    {
                        if (data[index] == 0) { break; }
                        rb++;
                    }
                    break;
                case ID3V2TextEncoding.UTF16BOM:
                    // Read BOM value.
                    bigendian = data[index++] == 254 & data[index++] == 255;
                    goto case ID3V2TextEncoding.UTF16BE;
                case ID3V2TextEncoding.UTF16BE:
                    for (; index < data.Length; index += 2)
                    {
                        if (data[index] == 0 && data[index + 1] == 0) { break; }
                        rb += 2;
                    }
                    break;
            }
            System.Int32 rc = rb;
            switch (enc)
            {
                case ID3V2TextEncoding.UTF8:
                case ID3V2TextEncoding.ISO_8859_1:
                    rb++;
                    return new System.Text.UTF8Encoding(false).GetString(data, startindex, rc);
                case ID3V2TextEncoding.UTF16BOM:
                    rb += 2;
                    return new System.Text.UnicodeEncoding(bigendian, false).GetString(data, startindex + 2, rc);
                case ID3V2TextEncoding.UTF16BE:
                    rb += 2;
                    return new System.Text.UnicodeEncoding(true, false).GetString(data, startindex, rc);
                default:
                    throw new InvalidProgramException("Invalid code path");
            }
        }

        private static System.Int32 StringLength(System.Byte[] data, System.Int32 startindex, ID3V2TextEncoding enc)
        {
            System.Int32 index = startindex;
            System.Int32 rb = 0;
            switch (enc)
            {
                case ID3V2TextEncoding.UTF8:
                case ID3V2TextEncoding.ISO_8859_1:
                    for (; index < data.Length; index++)
                    {
                        if (data[index] == 0) { break; }
                        rb++;
                    }
                    rb++;
                    break;
                case ID3V2TextEncoding.UTF16BOM:
                    // Skip BOM value.
                    index += 2;
                    goto case ID3V2TextEncoding.UTF16BE;
                case ID3V2TextEncoding.UTF16BE:
                    for (; index < data.Length; index += 2)
                    {
                        if (data[index] == 0 && data[index + 1] == 0) { break; }
                        rb += 2;
                    }
                    rb += 2;
                    break;
            }
            return rb;
        }

        /// <summary>
        /// Reads an ID3V2 frame. This operation requires the ID3V2 header to have been provided.
        /// </summary>
        /// <param name="stream">The stream to read the frame from</param>
        /// <param name="hdr">The ID3V2 header to denote how the data should be read.</param>
        /// <exception cref="ArgumentNullException"><paramref name="stream"/> was <see langword="null"/>.</exception>
        public ID3V2CompleteFrame(System.IO.Stream stream, ID3V2HEADER hdr)
        {
            if (stream is null) { throw new ArgumentNullException(nameof(stream)); }
            Header = stream.ReadStructure<ID3V2FRAMEHEADER>();
            if (Header.IsInvalid) { return; }
            System.Int32 hdrlen;
            if (hdr.VersionMajor < 4)
            {
                hdrlen = Header.Length.ToInt32Old();
            }
            else
            {
                hdrlen = Header.Length.ToInt32();
            }
            // If explicitly set by an writer , this frame must be skipped
            if (Header.Status == ID3V2FrameStatusFlags.DiscardFrame)
            {
                stream.Seek(hdrlen, System.IO.SeekOrigin.Current);
                return;
            }
            if (Header.Format.HasFlag(ID3V2FrameFormatFlags.DataLengthPresent))
            {
                stream.Position += 4; // Allocates 4 additional bytes at the start of the frame data
                hdrlen -= 4;
            }
            if (Header.Format.HasFlag(ID3V2FrameFormatFlags.ContainsGroupInformation))
            {
                stream.Position++; // Allocates 1 additional byte at the start of the frame
                hdrlen--;
            }
            Key = Header.FrameID;
            RawValue = stream.ReadBytes(hdrlen);
            Type = Key switch
            {
                "TIT1" => ID3V2FrameType.Text,
                "TIT2" => ID3V2FrameType.Text,
                "TIT3" => ID3V2FrameType.Text,
                "TALB" => ID3V2FrameType.Text,
                "TOAL" => ID3V2FrameType.Text,
                "TRCK" => ID3V2FrameType.Text,
                "TPOS" => ID3V2FrameType.Text,
                "TSST" => ID3V2FrameType.Text,
                "TSRC" => ID3V2FrameType.Text,
                "TPE1" => ID3V2FrameType.Text,
                "TPE2" => ID3V2FrameType.Text,
                "TPE3" => ID3V2FrameType.Text,
                "TPE4" => ID3V2FrameType.Text,
                "TOPE" => ID3V2FrameType.Text,
                "TEXT" => ID3V2FrameType.Text,
                "TOLY" => ID3V2FrameType.Text,
                "TCOM" => ID3V2FrameType.Text,
                "TMCL" => ID3V2FrameType.Text,
                "TIPL" => ID3V2FrameType.Text,
                "TENC" => ID3V2FrameType.Text,
                "TCOP" => ID3V2FrameType.Text,
                "TPRO" => ID3V2FrameType.Text,
                "TPUB" => ID3V2FrameType.Text,
                "TOWN" => ID3V2FrameType.Text,
                "TRSN" => ID3V2FrameType.Text,
                "TRSO" => ID3V2FrameType.Text,
                "TCON" => ID3V2FrameType.Text,
                "TYER" => ID3V2FrameType.Text,
                "TDAT" => ID3V2FrameType.Text,
                "TXXX" => ID3V2FrameType.UserDefinedTextFrame,
                "WCOM" => ID3V2FrameType.URL,
                "WCOP" => ID3V2FrameType.URL,
                "WOAF" => ID3V2FrameType.URL,
                "WOAR" => ID3V2FrameType.URL,
                "WOAS" => ID3V2FrameType.URL,
                "WORS" => ID3V2FrameType.URL,
                "WPAY" => ID3V2FrameType.URL,
                "WPUB" => ID3V2FrameType.URL,
                "WXXX" => ID3V2FrameType.UserDefinedURLLinkFrame,
                "USLT" => ID3V2FrameType.UnsyncronizedLyrics,
                "SYLT" => ID3V2FrameType.SyncronizedLyrics,
                "APIC" => ID3V2FrameType.Picture,
                "COMM" => ID3V2FrameType.Comments,
                _ => ID3V2FrameType.None,
            };
            AdjustFrameData();
        }

        private void AdjustFrameData()
        {
            System.Int32 idx = 0;
            ID3V2TextEncoding enc = (ID3V2TextEncoding)RawValue[idx++];
            switch (Type)
            {
                case ID3V2FrameType.UserDefinedTextFrame:
                case ID3V2FrameType.UserDefinedURLLinkFrame:
                    Key = DirectStringBuild(RawValue, idx, enc, out var idxlast);
                    // Re-build RawValue array
                    System.Byte[] valrem = new System.Byte[RawValue.Length - idxlast];
                    RawValue.Copy(idx + idxlast, valrem, 1, (valrem.Length - 1).ToUInt32());
                    valrem[0] = (System.Byte)enc;
                    RawValue = valrem;
                    break;
                case ID3V2FrameType.UnsyncronizedLyrics:
                    // In Key member we must store the culture of the lyrics. 
                    // Additionally, the RawValue array must be re-bulit in order to keep the lyrics only.
                    // We have already read the encoding , read 3 bytes that consist of the language.
                    Key = $"{RawValue[idx++].ToChar()}{RawValue[idx++].ToChar()}{RawValue[idx++].ToChar()}";
                    // Now , skip the description string (whatever the contents are)
                    idxlast = StringLength(RawValue, idx, enc) - 1;
                    // And, re-build the array.
                    valrem = new System.Byte[RawValue.Length - (idxlast + idx)];
                    RawValue.Copy(idx + idxlast + 1, valrem, 1, (valrem.Length - 1).ToUInt32());
                    valrem[0] = (System.Byte)enc;
                    RawValue = valrem;
                    break;
                case ID3V2FrameType.Picture:
                    // Pictures are a different story and are laid out as follows:
                    // -> Text encoding byte - we read that already
                    // -> MIME type string , this is the imaging format written ala UTF-8 and should be saved on the Key field. It terminates with a \0.
                    // -> Picture type - A single byte
                    // -> Textual description of the image contents. Depends on the text encoding byte. Must be terminated by \0. (Or \0\0 in UTF-16)
                    // -> The raw image data. We will re-build the array only based by these data.
                    Key = DirectStringBuild(RawValue, idx, enc, out idxlast);
                    idx += idxlast;
                    // Skip the next byte which is the picture type, and skip the description string too.
                    System.Int32 strindex = StringLength(RawValue, idx + 1, enc);
                    // Now the image data can be read directly. Re-build the array.
                    valrem = new System.Byte[RawValue.Length - (strindex + idx)];
                    RawValue.Copy(idx + strindex + 1, valrem, 0, (valrem.Length - 1).ToUInt32());
                    RawValue = valrem; // Done!
                    break;
                case ID3V2FrameType.Comments:
                    // 3-byte Language Identifier
                    Key = $"{RawValue[idx++].ToChar()}{RawValue[idx++].ToChar()}{RawValue[idx++].ToChar()}";
                    idx++; // Because the above string is also terminated with NULL.
                    valrem = new System.Byte[RawValue.Length - (idx - 1)];
                    RawValue.Copy(idx, valrem, 1, (valrem.Length - 1).ToUInt32());
                    valrem[0] = (System.Byte)enc;
                    RawValue = valrem; // Done!
                    break;
            }
        }
    }
}