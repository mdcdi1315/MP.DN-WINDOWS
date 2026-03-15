
using System;
using MP.Collections;

namespace MP.TagReading.ID3
{
    internal static class ID3V2Utils
    {
        // WARN: The null_terminated parameter on the below methods just means that by format definition the string is NULL-terminated, 
        // and not that the method should read the string as null-terminated.
        public static String ReadString_ISO_8859_3(IO.DataStream stream, bool null_terminated, out long length_used, long length = -1)
        {
            long len = (length > -1) ? length : 0;
            if (len < 1)
            {
                while (stream.Position < stream.Length)
                {
                    if (stream.ReadByte() > 0) {
                        len++;
                    } else {
                        // Unread the last read byte.
                        stream.Seek(-1, IO.SeekDisplacement.Current);
                        break;
                    }
                }
                // OK... We do now have the length of our string. We can just process it.
                // Unread the bytes that we used to detect the string's length...
                stream.Seek(-len, IO.SeekDisplacement.Current);
            }

            System.Text.StringBuilder sb = new((len / 4).ToInt32());
            for (long I = 0; I < len && stream.Position < stream.Length; I++)
            {
                sb.Append(stream.ReadByte().ToChar());
            }

            if (null_terminated) {
                stream.ReadByte(); // Process the NULL termination char, if this method is to be subsequently called again.
                length_used = len + 1;
            } else {
                length_used = len;
            }

            return sb.ToString();
        }

        public static String ReadString_UTF8(IO.DataStream stream, bool null_terminated, out long length_used, long length = -1)
        {
            long len = (length > -1) ? length : 0;
            if (len < 1)
            {
                while (stream.Position < stream.Length)
                {
                    if (stream.ReadByte() > 0) {
                        len++;
                    } else {
                        // Unread the last read byte.
                        stream.Seek(-1, IO.SeekDisplacement.Current);
                        break;
                    }
                }
                // OK... We do now have the length of our string. We can just process it.
                // Unread the bytes that we used to detect the string's length...
                stream.Seek(-len, IO.SeekDisplacement.Current);
            }

            // Read the string from the stream.
            String s = stream.ReadString(System.Text.Encoding.UTF8, len);

            if (null_terminated) {
                stream.ReadByte(); // Process the NULL termination char, if this method is to be subsequently called again.
                length_used = len + 1;
            } else {
                length_used = len;
            }
            return s;
        }

        public static String ReadString_UTF16BE(IO.DataStream stream, bool null_terminated, out long length_used, long length = -1)
        {
            long len = (length > -1) ? length : 0;
            if (len < 1)
            {
                ushort temp;
                while (stream.Position + (sizeof(ushort) - 1) < stream.Length)
                {
                    temp = stream.ReadUInt16();
                    if (temp == 0) {
                        // Unread the last read 2 bytes.
                        stream.Seek(-2, IO.SeekDisplacement.Current);
                        break;
                    } else {
                        len += 2;
                    }
                }

                // OK... We do now have the length of our string. We can just process it.
                // Unread the bytes that we used to detect the string's length...
                stream.Seek(-len, IO.SeekDisplacement.Current);
            }

            String su = stream.ReadString(System.Text.Encoding.BigEndianUnicode, len);
            
            if (null_terminated) {
                // Process the NULL termination char, if this method is to be subsequently called again.
                stream.ReadByte();
                stream.ReadByte();
                length_used = len + 2;
            } else {
                length_used = len;
            }

            return su;
        }

        public static String ReadString_UTF16BOM(IO.DataStream stream, bool null_terminated, out long length_used, long length = -1)
        {
            long len = (length > -1) ? length : 0;
            System.Boolean big_endian = stream.ReadLiteralByte() == 254 & stream.ReadLiteralByte() == 255;
            if (len < 1)
            {
                ushort temp;
                while (stream.Position + (sizeof(ushort) - 1) < stream.Length)
                {
                    temp = stream.ReadUInt16();
                    if (temp == 0) {
                        // Unread the last read 2 bytes.
                        stream.Seek(-2, IO.SeekDisplacement.Current);
                        break;
                    } else {
                        len += 2;
                    }
                }

                // OK... We do now have the length of our string. We can just process it.
                // Unread the bytes that we used to detect the string's length...
                stream.Seek(-len, IO.SeekDisplacement.Current);
            }

            String su = stream.ReadString(new System.Text.UnicodeEncoding(big_endian, false), len);

            if (null_terminated) {
                // Process the NULL termination char, if this method is to be subsequently called again.
                stream.ReadByte();
                stream.ReadByte();
                length_used = len + 4;
            } else {
                length_used = len;
            }

            return su;
        }

        public static String ReadString_EncodingDependent(IO.DataStream stream, bool null_terminated, out long length_used, out ID3V2TextEncoding encoding_used, long length = -1)
        {
            encoding_used = (ID3V2TextEncoding)stream.ReadLiteralByte();
            switch (encoding_used)
            {
                case ID3V2TextEncoding.ISO_8859_1:
                    return ReadString_ISO_8859_3(stream, null_terminated, out length_used, length);
                case ID3V2TextEncoding.UTF8:
                    return ReadString_UTF8(stream, null_terminated, out length_used, length);
                case ID3V2TextEncoding.UTF16BOM:
                    return ReadString_UTF16BOM(stream, null_terminated, out length_used, length);
                case ID3V2TextEncoding.UTF16BE:
                    return ReadString_UTF16BE(stream, null_terminated, out length_used, length);
                default:
                    throw new NotImplementedException($"Encoding of type {encoding_used} is not implemented or the reader is not aware of this encoding!");
            }
        }

        public static String ReadString_EncodingDependentAndKnown(IO.DataStream stream, bool null_terminated, out long length_used, ID3V2TextEncoding encoding_used, long length = -1)
        {
            switch (encoding_used)
            {
                case ID3V2TextEncoding.ISO_8859_1:
                    return ReadString_ISO_8859_3(stream, null_terminated, out length_used, length);
                case ID3V2TextEncoding.UTF8:
                    return ReadString_UTF8(stream, null_terminated, out length_used, length);
                case ID3V2TextEncoding.UTF16BOM:
                    return ReadString_UTF16BOM(stream, null_terminated, out length_used, length);
                case ID3V2TextEncoding.UTF16BE:
                    return ReadString_UTF16BE(stream, null_terminated, out length_used, length);
                default:
                    throw new NotImplementedException($"Encoding of type {encoding_used} is not implemented or the reader is not aware of this encoding!");
            }
        }

        public static UserDefinedTextFrameData ReadUserDefinedTextFrame(IO.DataStream stream)
        {
            ID3V2TextEncoding encoding;
            String desc = ReadString_EncodingDependent(stream, true, out long string_1_len, out encoding);
            String value = ReadString_EncodingDependentAndKnown(stream, false, out _, encoding, stream.Length - string_1_len - 1);
            return new UserDefinedTextFrameData(desc, value);
        }

        public static UserDefinedURLFrameData ReadUserDefinedURLLinkFrame(IO.DataStream stream)
        {
            ID3V2TextEncoding encoding;
            String desc = ReadString_EncodingDependent(stream, true, out long string_1_len, out encoding);
            String value = ReadString_EncodingDependentAndKnown(stream, false, out _, encoding, stream.Length - string_1_len - 1);
            return new UserDefinedURLFrameData(desc, value);
        }

        public static UnsynchronisedLyricsFrameData ReadUnsynchronisedLyricsFrame(IO.DataStream stream)
        {
            ID3V2TextEncoding encoding = (ID3V2TextEncoding)stream.ReadLiteralByte();
            // Read three bytes from the stream and store them as the language
            String lang = stream.ReadString(System.Text.Encoding.ASCII, 3);
            String content_descriptor = ReadString_EncodingDependentAndKnown(stream, true, out long cd_length, encoding);
            String lyrics = ReadString_EncodingDependentAndKnown(stream, false, out _, encoding, stream.Length - cd_length - 4);
            return new UnsynchronisedLyricsFrameData(lang, lyrics, content_descriptor);
        }

        public static SynchronisedLyricsFrameData ReadSynchronisedLyricsFrame(IO.DataStream stream)
        {
            ID3V2TextEncoding encoding = (ID3V2TextEncoding)stream.ReadLiteralByte();
            // Read three bytes from the stream and store them as the language
            String lang = stream.ReadString(System.Text.Encoding.ASCII, 3);
            ID3V2TimeStampFormat format = (ID3V2TimeStampFormat)stream.ReadLiteralByte();
            SynchronisedLyricsFrameData.ContentType ct = (SynchronisedLyricsFrameData.ContentType)stream.ReadLiteralByte();
            String content_descriptor = ReadString_EncodingDependentAndKnown(stream, true, out long cd_length, encoding);
            ArrayBasedList<SynchronisedLyricsFrameData.Syllable> syllables = new(50);
            
            while (stream.Position < stream.Length)
            {
                // TODO: Implement this later on...
            }

            SynchronisedLyricsFrameData.Syllable[] data = new SynchronisedLyricsFrameData.Syllable[syllables.Count];
            syllables.CopyTo(data, 0);
            return new SynchronisedLyricsFrameData(
                lang,
                content_descriptor,
                ct,
                format, 
                data
            );
        }

        public static CommentFrameData ReadCommentFrame(IO.DataStream stream)
        {
            ID3V2TextEncoding encoding = (ID3V2TextEncoding)stream.ReadLiteralByte();
            // Read three bytes from the stream and store them as the language
            String lang = stream.ReadString(System.Text.Encoding.ASCII, 3);
            String content_description = ReadString_EncodingDependentAndKnown(stream, true, out long cd_length, encoding);
            String value = ReadString_EncodingDependentAndKnown(stream, false, out _, encoding, stream.Length - cd_length - 4);
            return new CommentFrameData(
                value,
                lang,
                content_description
            );
        }

        public static AttachedPictureFrameData ReadAttachedPicture(IO.DataStream stream)
        {
            ID3V2TextEncoding encoding = (ID3V2TextEncoding)stream.ReadLiteralByte();
            String mime_type = ReadString_ISO_8859_3(stream, true, out _);
            AttachedPictureFrameData.PictureType pt = (AttachedPictureFrameData.PictureType)stream.ReadLiteralByte();
            String description = ReadString_EncodingDependentAndKnown(stream, true, out _, encoding);
            IO.DataStream data_rest = new IO.MemoryStream();
            stream.DirectCopyToStream(data_rest);
            data_rest.Position = 0;
            return new AttachedPictureFrameData(
                pt,
                mime_type,
                description,
                data_rest
            );
        }

        public static TermsOfUseFrameData ReadTermsOfUseFrame(IO.DataStream stream)
        {
            ID3V2TextEncoding encoding = (ID3V2TextEncoding)stream.ReadLiteralByte();
            // Read three bytes from the stream and store them as the language
            String lang = stream.ReadString(System.Text.Encoding.ASCII, 3);
            String text = ReadString_EncodingDependentAndKnown(stream, false, out _, encoding, stream.Length - 4);
            return new TermsOfUseFrameData(lang, text);
        }

        public static OwnershipFrameData ReadOwnershipFrame(IO.DataStream stream)
        {
            ID3V2TextEncoding encoding = (ID3V2TextEncoding)stream.ReadLiteralByte();
            String price = ReadString_ISO_8859_3(stream, true, out long price_len);
            String date = stream.ReadString(System.Text.Encoding.ASCII, 8);
            String seller = ReadString_EncodingDependentAndKnown(stream , false, out _, encoding, stream.Length - price_len - 8);
            return new OwnershipFrameData(
                price,
                date,
                seller
            );
        }

        public static SynchronisedLyricsFrameData ReadSyncronizedLyrics(IO.DataStream stream)
        {
            ID3V2TextEncoding encoding = (ID3V2TextEncoding)stream.ReadLiteralByte();
            string lang = stream.ReadString(System.Text.Encoding.ASCII, 3);
            ID3V2TimeStampFormat format = (ID3V2TimeStampFormat)stream.ReadLiteralByte();
            SynchronisedLyricsFrameData.ContentType content_type = (SynchronisedLyricsFrameData.ContentType)stream.ReadLiteralByte();
            string cd = ReadString_EncodingDependentAndKnown(stream, true, out _, encoding);
            ArrayBasedList<SynchronisedLyricsFrameData.Syllable> list = new();

            string temp = ReadString_EncodingDependentAndKnown(stream, true, out _, encoding);

            if (stream.Position >= stream.Length) {
                // Sync was undefined, return directly
                list.Add(new(temp, 0));
            } else {
                int ts_temp = stream.ReadInt32BE();
                list.Add(new(temp, ts_temp));
                do {
                    temp = ReadString_EncodingDependentAndKnown(stream, true, out _, encoding);
                    ts_temp = stream.ReadInt32BE();
                    list.Add(new(temp, ts_temp));
                } while (stream.Position < stream.Length);
            }

            SynchronisedLyricsFrameData.Syllable[] d = new SynchronisedLyricsFrameData.Syllable[list.Count];
            list.CopyTo(d, 0);
            return new(lang, cd, content_type, format, d);
        }

        public static System.Object ReadTextFrame(IO.DataStream stream)
        {
            ArrayBasedList<System.String> values = new(10);

            ID3V2TextEncoding encoding = (ID3V2TextEncoding)stream.ReadLiteralByte();

            while (stream.Position < stream.Length)
            {
                values.Add(
                    ReadString_EncodingDependentAndKnown(stream, true, out _, encoding)
                );
            }

            if (values.Count == 1) {
                return values[0];
            } else {
                System.String[] f_values = new System.String[values.Count];
                values.CopyTo(f_values, 0);
                return f_values;
            }
        }

        public static long GetNullByteSize(ID3V2TextEncoding encoding) => encoding switch { 
            ID3V2TextEncoding.ISO_8859_1 => 1, 
            ID3V2TextEncoding.UTF8 => 1, 
            ID3V2TextEncoding.UTF16BOM => 2, 
            ID3V2TextEncoding.UTF16BE => 2,
            _ => 0
        };
    }
}