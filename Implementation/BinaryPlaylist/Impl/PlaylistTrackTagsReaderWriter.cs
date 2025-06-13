using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace MP.BinaryPlaylist.MusicPlayer
{
    [StructLayout(LayoutKind.Explicit , Size = 13, Pack = 1)]
    public unsafe struct PLAYLISTDATATAG
    {
        [FieldOffset(0)]
        public System.UInt32 FileStringIndex;

        [FieldOffset(4)]
        public System.UInt32 PropertiesLength;

        [FieldOffset(8)]
        public System.Boolean CoverImagePresent;

        [FieldOffset(9)]
        public System.UInt32 CoverImageDataBlobOffset;
    }

    /// <summary>
    /// Represents the base structure for V2 Tag Properties.
    /// </summary>
    [StructLayout(LayoutKind.Explicit , Size = 12 , Pack = 1)]
    public unsafe struct PLAYLISTTAGPROPERTY
    {
        public const System.UInt16 PKValue = 19280;
        public const System.UInt16 PVValue = 22096;

        [FieldOffset(0)]
        public System.UInt16 KeyIdentifier; // The value of this identifier must always be PK.

        [FieldOffset(2)]
        public System.UInt32 PropertyKeyUInt32; // The value contains the property key encoded as a UInt32. The property key must always be in ASCII format.

        [FieldOffset(6)]
        public System.UInt16 ValueIdentifier; // The value of this identifier must always be PV.

        // The byte length of a single character in the current encoding.
        [FieldOffset(8)]
        public System.UInt16 SinglePropertyCharSize;

        // Contains the length in characters of the property value.
        // The final number of bytes is computed together with the SinglePropertyCharSize field.
        [FieldOffset(10)]
        public System.UInt16 PropertyCharLength;

        public System.String PropertyKey
        {
            get {
                System.Byte[] dt = PropertyKeyUInt32.GetBytes();
                System.Int32 sidx = 0;
                while (sidx < dt.Length && dt[sidx] == 0) { sidx++; }
                System.String ret = System.String.Empty;
                for (System.Int32 I = sidx; I < dt.Length; I++)
                {
                    ret += dt[I].ToChar();
                }
                dt = null;
                return ret;
            }
            set {
                if (System.String.IsNullOrEmpty(value)) { PropertyKeyUInt32 = 0; return; }
                System.Int32 index = 0;
                System.Byte[] dt = new System.Byte[sizeof(System.UInt32)];
                switch (value.Length) {
                    case 4:
                        break;
                    case 3:
                        dt[0] = 0;
                        index++;
                        break;
                    case 2:
                        dt[1] = 0;
                        index++;
                        goto case 3;
                    case 1:
                        dt[2] = 0;
                        index++;
                        goto case 2;
                    default:
                        throw new System.ArgumentOutOfRangeException(nameof(value),
                        "Property key value length must be no more than 4 characters.");
                }
                System.Char temp;
                for (System.Int32 I = index , J = 0; I < dt.Length && J < value.Length; I++ , J++)
                {
                    temp = value[J];
                    if (temp > 255) { throw new System.ArgumentOutOfRangeException(nameof(value) ,"Only ASCII chars can be encoded."); }
                    dt[I] = temp.ToByte();
                }
                PropertyKeyUInt32 = dt.ToUInt32(0);
            }
        }
    
        public static PLAYLISTTAGPROPERTY CreateDefault()
        {
            PLAYLISTTAGPROPERTY prop = new();
            prop.KeyIdentifier = PKValue;
            prop.ValueIdentifier = PVValue;
            return prop;
        }
    }

    public unsafe sealed class PlaylistTrackTagsWriter : PlaylistBlobWriter
    {
        private System.Int32 tagidx;
        private PlaylistDataWriter writer;
        private IPlaylist plt;

        public PlaylistTrackTagsWriter(PlaylistDataWriter dw , IPlaylist playlist) 
        {
            if (dw is null) { throw new System.ArgumentNullException(nameof(dw)); }
            if (playlist is null) { throw new System.ArgumentNullException(nameof(playlist)); }
            writer = dw;
            plt = playlist;
            tagidx = 0; 
        }

        public System.Int32 AddDataTag_V1(System.UInt32 stringindex , IPlaylistFile file)
        { 
            SavedDataTag sdt = plt.GetTagFromFile(file);
            if (sdt is not null && sdt.DataExist) {
                PLAYLISTDATATAG hdr = new();
                if (hdr.CoverImagePresent = sdt.Image is not null) { hdr.CoverImageDataBlobOffset = writer.WriteByteArray(sdt.Image , 0 , sdt.Image.Length); }
                hdr.PropertiesLength = 0;
                foreach (var kvp in sdt.NativeProperties)
                {
                    if (System.String.IsNullOrEmpty(kvp.Value)) { continue; }
                    hdr.PropertiesLength += (kvp.Name.Length + (kvp.Value.Length * 2)).ToUInt32();
                    hdr.PropertiesLength += 2; // Each tag property will be suffixed with a NUL value and the = sign required to split the strings to two.
                }
                hdr.PropertiesLength++; // A final NUL value.
                WriteStructure(hdr);
                foreach (var kvp in sdt.NativeProperties)
                {
                    if (System.String.IsNullOrEmpty(kvp.Value)) { continue; }
                    for (System.Int32 I = 0; I < kvp.Name.Length; I++)
                    {
                        Write(kvp.Name[I].ToByte());
                    }
                    Write('='.ToByte());
                    for (System.Int32 I = 0; I < kvp.Value.Length; I++)
                    {
                        Write(kvp.Value[I]);
                    }
                    Write(0.ToByte());
                }
                Write(0.ToByte());
            } else {
                return -1;
            }
            return tagidx++;
        }
    
        public System.Int32 AddDataTag_V2(System.UInt32 stringindex , IPlaylistFile file)
        {
            SavedDataTag sdt = plt.GetTagFromFile(file);
            if (sdt is not null && sdt.DataExist) {
                PLAYLISTDATATAG hdr = new();
                if (hdr.CoverImagePresent = sdt.Image is not null) { hdr.CoverImageDataBlobOffset = writer.WriteByteArray(sdt.Image, 0, sdt.Image.Length); }
                hdr.PropertiesLength = 0;
                hdr.FileStringIndex = stringindex;
                System.UInt32 structlen = sizeof(PLAYLISTTAGPROPERTY).ToUInt32();
                foreach (var kvp in sdt.NativeProperties)
                {
                    // Exclude the properties with a length larger than 65535 so as to not break the format.
                    if (System.String.IsNullOrEmpty(kvp.Value) || kvp.Value.Length > 65535) { continue; }
                    // Properties are now standardized further by their new header structure , PLAYLISTTAGPROPERTY.
                    hdr.PropertiesLength += structlen;
                    // Properties value lengths is normally a UTF-16LE string currently.
                    hdr.PropertiesLength += (kvp.Value.Length * sizeof(System.Char)).ToUInt32();
                }
                hdr.PropertiesLength++; // A final NUL value , just keep it in V2 to verify data lengths.
                WriteStructure(hdr);
                foreach (var kvp in sdt.NativeProperties)
                {
                    if (System.String.IsNullOrEmpty(kvp.Value) || kvp.Value.Length > 65535) { continue; }
                    // The things are now simplified: 
                    PLAYLISTTAGPROPERTY tagprop = PLAYLISTTAGPROPERTY.CreateDefault(); // Creates a default tag.
                    tagprop.PropertyKey = kvp.Name;
                    tagprop.SinglePropertyCharSize = sizeof(System.Char);
                    tagprop.PropertyCharLength = kvp.Value.Length.ToUInt16();
                    // Write the structure first.
                    WriteStructure(tagprop);
                    for (System.Int32 I = 0; I < kvp.Value.Length; I++) // Write the property value at the end of structure.
                    {
                        Write(kvp.Value[I]);
                    }
                }
                Write(0.ToByte()); // Write the final NUL value.
            } else {
                return -1;
            }
            return tagidx++;
        }

        public System.Int32 AddDataTag(System.UInt32 stringindex, IPlaylistFile file) => AddDataTag_V2(stringindex, file);

        public void FinalizeWriter()
        {
            // By default from now on V2 blobs are written to the playlist.
            Header = new() { Count = tagidx.ToUInt32() , Identifier1 = BlobFlags.ReadOnly , Identifier2 = BlobTypes.TAGBLOB , Version = 2 };
            IsCompleted = true;
            // Dereference all the used redundant stuff
            writer = null;
            plt = null;
        }
    }

    public sealed record class PlaylistTrackTag
    {
        public System.UInt32 Index;
        public PLAYLISTDATATAG TagNative;
        public SavedDataTagProperty[] TagProperties;
    }

    public sealed class PlaylistTrackTagsReader : PlaylistBlobReader
    {
        private System.Byte[] shared;
        private List<PlaylistTrackTag> tagsfast;

        public unsafe PlaylistTrackTagsReader() : base() { shared = new System.Byte[sizeof(PLAYLISTDATATAG)]; tagsfast = new(10); }

        private SavedDataTagProperty[] ReadTagProperties_V1(System.UInt32 size)
        {
            List<SavedDataTagProperty> pairs = new(8);
            System.Byte[] tagprops = new System.Byte[size];
            if (Read(tagprops, 0, tagprops.Length) != tagprops.Length) { throw new System.IO.IOException($"Could not read {tagprops.Length} bytes from the stream."); }
            System.Int32 cb = tagprops.Length - 1;
            if (tagprops[cb] != 0) { throw new PropertyFormatInvalidException("The property store end marker could not be found."); }
            cb--;
            if (tagprops[cb] != 0) { throw new PropertyFormatInvalidException(); }
            cb--;
            System.String name = System.String.Empty, value = System.String.Empty;
            Stack<System.Char> tempstack = new(400);
            while (cb > 0)
            {
                System.Byte fd = 0;
                while (tagprops[cb] != '=')
                {
                    if (fd == 1) {
                        tempstack.Push(tagprops.ToChar(cb));
                        fd = 0;
                    } else { fd++; }
                    cb--;
                }
                foreach (var c in tempstack) { value += c; }
                tempstack.Clear();
                cb--;
                while (cb >= 0 && tagprops[cb] != 0)
                {
                    tempstack.Push(tagprops[cb].ToChar());
                    cb--;
                }
                foreach (var c in tempstack) { name += c; }
                tempstack.Clear();
                if (cb >= 0 && tagprops[cb] != 0) { throw new PropertyFormatInvalidException(); }
                cb--;
                pairs.Add(new(name, value));
                name = System.String.Empty;
                value = System.String.Empty;
            }
            tempstack.Clear();
            tempstack = null;
            tagprops = null;
            try {
                return pairs.ToArray();
            } finally {
                pairs?.Clear();
                pairs = null;
            }
        }

        private unsafe SavedDataTagProperty[] ReadTagProperties_V2(System.UInt32 size)
        {
            List<SavedDataTagProperty> pairs = new(8);
            System.Byte[] tagprops = new System.Byte[size];
            if (Read(tagprops, 0, tagprops.Length) != tagprops.Length) { throw new System.IO.IOException($"Could not read {tagprops.Length} bytes from the stream."); }
            // V1 format read these in the reverse way , but because of our new structure we can read it normally.
            // We are expecting a NUL terminating byte at the end of all properties. Assert that. 
            if (tagprops[tagprops.Length - 1] != 0) { throw new PropertyFormatInvalidException("The property store end marker could not be found."); }
            System.Int32 idx = 0; // The index inside the tagprops array.
            System.Text.StringBuilder sb = new(60);
            while (idx < tagprops.Length - 1) // length -1 because we checked for the terminating NUL byte at the end. 
            {
                PLAYLISTTAGPROPERTY tagprop = tagprops.ReadStructure<PLAYLISTTAGPROPERTY>(idx);
                idx += sizeof(PLAYLISTTAGPROPERTY); // Mark the struct bytes as read.
                if (tagprop.KeyIdentifier != PLAYLISTTAGPROPERTY.PKValue ||
                     tagprop.ValueIdentifier != PLAYLISTTAGPROPERTY.PVValue) {
                    throw new PropertyFormatInvalidException("The magic values on the property header do not match!");
                }
                // Get property value length.
                System.Int32 bytelen = tagprop.PropertyCharLength * tagprop.SinglePropertyCharSize;
                // Next step is to read the value , and save the results.
                switch (tagprop.SinglePropertyCharSize)
                {
                    case 1:
                        // The value was encoded in ASCII , so simply read and save the data directly.
                        System.Int32 I = 0;
                        while (I < bytelen)
                        {
                            sb.Append(tagprops[I + idx].ToChar()); // Each byte is converted to an equal UTF-16LE character.
                            I++;
                        }
                        break;
                    case 2:
                        // The value was encoded in UTF-16LE , so read 2 bytes each time now.
                        I = 0;
                        while (I < bytelen)
                        {
                            sb.Append(tagprops.ToChar(I + idx)); // Read 2 bytes from the array to form a UTF-16LE character. Notice the differences.
                            I += 2;
                        }
                        break;
                }
                // Add the data to the property list.
                pairs.Add(new(tagprop.PropertyKey, sb.ToString()));
                // Clean the string builder.
                sb.Clear();
                // Finally , mark all the read value bytes as processed.
                idx += bytelen;
                // Continue with the next structure.
            }
            // Destroy the used string builder.
            sb = null;
            try {
                return pairs.ToArray(); // And , return our decoded properties..
            } finally {
                pairs?.Clear();
                pairs = null;
            }
        }

        private unsafe PlaylistTrackTag GetTagPrivate_V1(System.UInt32 id)
        {
            if (id >= Header.Count) { throw new System.ArgumentOutOfRangeException(nameof(id)); }
            Position = 0;
            PLAYLISTDATATAG tagtemp;
            for (System.UInt32 I = 0; I < Header.Count && I < id; I++)
            {
                if (Read(shared, 0, shared.Length) != shared.Length) { throw new System.IO.IOException($"Could not read {shared.Length} bytes from the stream."); }
                tagtemp = shared.ReadStructure<PLAYLISTDATATAG>(0);
                Seek(tagtemp.PropertiesLength , System.IO.SeekOrigin.Current);
            }
            if (Read(shared, 0, shared.Length) != shared.Length) { throw new System.IO.IOException($"Could not read {shared.Length} bytes from the stream."); }
            tagtemp = shared.ReadStructure<PLAYLISTDATATAG>(0);
            return new() { Index = id, TagNative = tagtemp, TagProperties = ReadTagProperties_V1(tagtemp.PropertiesLength) };
        }

        private unsafe PlaylistTrackTag GetTagPrivate_V2(System.UInt32 id)
        {
            if (id >= Header.Count) { throw new System.ArgumentOutOfRangeException(nameof(id)); }
            Position = 0;
            PLAYLISTDATATAG tagtemp;
            for (System.UInt32 I = 0; I < Header.Count && I < id; I++)
            {
                if (Read(shared, 0, shared.Length) != shared.Length) { throw new System.IO.IOException($"Could not read {shared.Length} bytes from the stream."); }
                tagtemp = shared.ReadStructure<PLAYLISTDATATAG>(0);
                Seek(tagtemp.PropertiesLength, System.IO.SeekOrigin.Current);
            }
            if (Read(shared, 0, shared.Length) != shared.Length) { throw new System.IO.IOException($"Could not read {shared.Length} bytes from the stream."); }
            tagtemp = shared.ReadStructure<PLAYLISTDATATAG>(0);
            return new() { Index = id, TagNative = tagtemp, TagProperties = ReadTagProperties_V2(tagtemp.PropertiesLength) };
        }

        private PlaylistTrackTag GetTagAt_V1(System.UInt32 index)
        {
            for (System.Int32 I = 0; I < tagsfast.Count; I++)
            {
                if (tagsfast[I].Index == index) { return tagsfast[I]; }
            }
            if (tagsfast.Count > 10) { tagsfast.Clear(); }
            PlaylistTrackTag ret = GetTagPrivate_V1(index);
            tagsfast.Add(ret);
            return ret;
        }

        private PlaylistTrackTag GetTagAt_V2(System.UInt32 index)
        {
            for (System.Int32 I = 0; I < tagsfast.Count; I++)
            {
                if (tagsfast[I].Index == index) { return tagsfast[I]; }
            }
            if (tagsfast.Count > 10) { tagsfast.Clear(); }
            PlaylistTrackTag ret = GetTagPrivate_V2(index);
            tagsfast.Add(ret);
            return ret;
        }

        public PlaylistTrackTag GetTagAt(System.UInt32 index)
        {
            switch (Header.Version)
            {
                case 1:
                    return GetTagAt_V1(index);
                case 2:
                    return GetTagAt_V2(index);
                default:
                    throw new System.InvalidOperationException("Internal header error occured.");
            }
        }

        public IEnumerable<PlaylistTrackTag> GetAll()
        {
            switch (Header.Version)
            {
                case 1:
                    return GetAll_V1();
                case 2:
                    return GetAll_V2();
                default:
                    throw new System.InvalidOperationException("Internal header error occured.");
            }
        }

        private IEnumerable<PlaylistTrackTag> GetAll_V1()
        {
            PLAYLISTDATATAG temp;
            Position = 0;
            for (System.UInt32 I = 0; I < Header.Count; I++)
            {
                if (Read(shared, 0, shared.Length) != shared.Length) { throw new System.IO.IOException($"Could not read {shared.Length} bytes from the stream."); }
                temp = shared.ReadStructure<PLAYLISTDATATAG>(0);
                yield return new() { Index = I, TagNative = temp, TagProperties = ReadTagProperties_V1(temp.PropertiesLength) };
            }
        }

        private IEnumerable<PlaylistTrackTag> GetAll_V2()
        {
            PLAYLISTDATATAG temp;
            Position = 0;
            for (System.UInt32 I = 0; I < Header.Count; I++)
            {
                if (Read(shared, 0, shared.Length) != shared.Length) { throw new System.IO.IOException($"Could not read {shared.Length} bytes from the stream."); }
                temp = shared.ReadStructure<PLAYLISTDATATAG>(0);
                yield return new() { Index = I, TagNative = temp, TagProperties = ReadTagProperties_V2(temp.PropertiesLength) };
            }
        }

        protected override void Dispose(bool disposing)
        {
            shared = null;
            tagsfast?.Clear();
            tagsfast = null;
            base.Dispose(disposing);
        }
    }
}
