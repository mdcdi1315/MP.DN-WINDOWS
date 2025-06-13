using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace MP.BinaryPlaylist.MusicPlayer
{
    [StructLayout(LayoutKind.Explicit , Size = 14)]
    internal struct PREFERENCE
    {
        [FieldOffset(0)]
        public System.Int32 NameLength;

        [FieldOffset(4)]
        public System.UInt32 ValueSizeInBytes;

        [FieldOffset(8)]
        public System.Int32 DescriptionLength;

        [FieldOffset(12)]
        public PreferenceValueType Type;

        [FieldOffset(13)]
        public PreferenceBehaviorFlags Flags;
    }

    public sealed class PlaylistPreferencesWriter : PlaylistBlobWriter
    {
        private System.Int32 count;

        public PlaylistPreferencesWriter() { count = 0; }

        public void WritePreferences(PlaylistPreferences prefs)
        {
            count = prefs.Count;
            System.Byte[] valarray = null;
            for (System.Int32 I = 0; I < count; I++)
            {
                valarray = prefs[I].Value switch {
                    System.String s => System.Text.Encoding.Unicode.GetBytes(s),
                    System.Int64 num => num.GetBytes(),
                    System.Boolean bl => new System.Byte[] { 0, (bl ? 1 : 0).ToByte() },
                    _ => throw new System.ArgumentException($"Value type not supported: {prefs[I].Value.GetType().FullName}.")
                };
                System.Byte[] namearray = System.Text.Encoding.UTF8.GetBytes(prefs[I].Name);
                System.Byte[] descarray = System.Text.Encoding.Unicode.GetBytes(prefs[I].Description ?? System.String.Empty);
                PREFERENCE pf = new();
                pf.NameLength = namearray.Length;
                pf.DescriptionLength = descarray.Length;
                pf.ValueSizeInBytes = valarray.LongLength.ToUInt32();
                pf.Type = prefs[I].TypeOfValue;
                pf.Flags = prefs[I].Flags;
                WriteStructure(pf);
                Write(namearray, 0, namearray.Length);
                Write(valarray , 0 , valarray.Length);
                Write(descarray , 0 , descarray.Length);
            }
        }

        /*
        public void WritePreferencesOld(PlaylistPreferences prefs)
        {
            count = prefs.Count;
            for (System.Int32 I = 0; I < count; I++) 
            {
                System.Int32 length = prefs[I].Name.Length;
                length += prefs[I].Value.Length;
                length += prefs[I].Description.Length * 2;
                length += 1; // Type of preference
                Write(length);
                Write((prefs[I].Name.Length).ToUInt16());
                Write(prefs[I].Name , true);
                Write((prefs[I].Value.Length).ToUInt16());
                Write(prefs[I].Value , true);
                Write(prefs[I].Description.Length * 2);
                Write(prefs[I].Description , false);
                Write((System.Byte)prefs[I].TypeOfValue);
            }
        }*/

        public void FinalizeWriter() 
        {
            Header = new() { Count = count.ToUInt32(), Identifier1 = BlobFlags.Normal, Identifier2 = BlobTypes.PREFBLOB, Version = 2 };
            IsCompleted = true;
        }
    }

    public sealed class PlaylistPreferencesReader : PlaylistBlobReader
    {
        private System.Text.StringBuilder builder;
        private Dictionary<System.UInt32, Preference> prefsfast;

        public PlaylistPreferencesReader() : base() { prefsfast = new(10); builder = new(500); }

        private Preference GetPreference_Old(System.UInt32 index)
        {
            if (index >= Header.Count) { throw new System.ArgumentOutOfRangeException(nameof(index)); }
            System.Int32 length;
            Position = 0;
            for (System.UInt32 I = 0; I < Header.Count && I < index; I++)
            {
                length = ReadNumber<System.Int32>();
                Seek(length + sizeof(System.UInt16) * 2 + sizeof(System.Int32), System.IO.SeekOrigin.Current);
            }
            length = ReadNumber<System.Int32>();
            // The preference name is written in ASCII.
            System.UInt16 stringlen = ReadNumber<System.UInt16>();
            System.Byte[] temp = new System.Byte[stringlen];
            if (Read(temp, 0, temp.Length) != temp.Length) { throw new System.IO.IOException($"Could not read {temp.Length} bytes from the stream."); }
            for (System.Int32 I = 0; I < temp.Length; I++) { builder.Append(temp[I].ToChar()); }
            System.String name = builder.ToString();
            builder.Clear();
            // The preference value is written in ASCII.
            stringlen = ReadNumber<System.UInt16>();
            temp = new System.Byte[stringlen];
            if (Read(temp, 0, temp.Length) != temp.Length) { throw new System.IO.IOException($"Could not read {temp.Length} bytes from the stream."); }
            for (System.Int32 I = 0; I < temp.Length; I++) { builder.Append(temp[I].ToChar()); }
            System.String value = builder.ToString();
            builder.Clear();
            // The preference description is written in UTF16-LE.
            length = ReadNumber<System.Int32>();
            temp = new System.Byte[length];
            if (Read(temp, 0, temp.Length) != temp.Length) { throw new System.IO.IOException($"Could not read {temp.Length} bytes from the stream."); }
            for (System.Int32 I = 0; I < temp.Length; I += 2) { builder.Append(temp.ToChar(I)); }
            System.String desc = builder.ToString();
            builder.Clear();
            // Finally , read the type of this preference and return
            return new(name , value , desc , (PreferenceValueType)ReadByte());
        }

        private Preference GetPreference_New(System.UInt32 index)
        {
            if (index >= Header.Count) { throw new System.ArgumentOutOfRangeException(nameof(index)); }
            Position = 0;
            PREFERENCE prf;
            for (System.UInt32 I = 0; I < Header.Count && I < index; I++)
            {
                prf = ReadStructure<PREFERENCE>();
                Seek(prf.NameLength + prf.ValueSizeInBytes + prf.DescriptionLength, System.IO.SeekOrigin.Current);
            }
            prf = ReadStructure<PREFERENCE>();
            System.String name = System.Text.Encoding.UTF8.GetString(ReadBytes(prf.NameLength));
            System.Object value = null;
            System.Byte[] data = ReadBytes(prf.ValueSizeInBytes);
            switch (prf.Type)
            {
                case PreferenceValueType.String:
                    value = System.Text.Encoding.Unicode.GetString(data);
                    break;
                case PreferenceValueType.Boolean:
                    value = data[1] != 0;
                    break;
                case PreferenceValueType.Number:
                    value = data.ToInt64(0);
                    break;
            }
            data = ReadBytes(prf.DescriptionLength);
            System.String desc = System.Text.Encoding.Unicode.GetString(data);
            return new(name, value , desc , prf.Type ,prf.Flags); 
        }

        public Preference GetPreferenceAt(System.UInt32 index)
        {
            Preference ret;
            if (prefsfast.TryGetValue(index, out ret)) { return ret; }
            if (prefsfast.Count > 9) { prefsfast.Clear(); }
            switch (Header.Version)
            {
                case 1:
                    ret = GetPreference_Old(index);
                    break;
                case 2:
                    ret = GetPreference_New(index);
                    break;
            }
            prefsfast.Add(index, ret);
            return ret;
        }

        public PlaylistPreferences GetAll()
        {
            switch (Header.Version)
            {
                case 1:
                    return GetAll_Old();
                case 2:
                    return GetAll_New();
                default:
                    return null;
            }
        }
        
        private PlaylistPreferences GetAll_New()
        {
            PlaylistPreferences build = new();
            if (Header.Count <= 0) { return build; }
            Position = 0;
            PREFERENCE prf;
            for (System.UInt32 M = 0; M < Header.Count; M++)
            {
                prf = ReadStructure<PREFERENCE>();
                System.String name = System.Text.Encoding.UTF8.GetString(ReadBytes(prf.NameLength));
                System.Object value = null;
                System.Byte[] data = ReadBytes(prf.ValueSizeInBytes);
                switch (prf.Type)
                {
                    case PreferenceValueType.String:
                        value = System.Text.Encoding.Unicode.GetString(data);
                        break;
                    case PreferenceValueType.Boolean:
                        value = data[1] != 0;
                        break;
                    case PreferenceValueType.Number:
                        value = data.ToInt64(0);
                        break;
                }
                data = ReadBytes(prf.DescriptionLength);
                System.String desc = System.Text.Encoding.Unicode.GetString(data);
                build.Add(new(name, value, desc, prf.Type , prf.Flags));
            }
            return build;
        }

        private PlaylistPreferences GetAll_Old()
        {
            PlaylistPreferences build = new();
            System.Int32 length;
            if (Header.Count <= 0) { return build; }
            Position = 0;
            for (System.UInt32 M = 0; M < Header.Count; M++)
            {
                length = ReadNumber<System.Int32>();
                // The preference name is written in ASCII.
                System.UInt16 stringlen = ReadNumber<System.UInt16>();
                System.Byte[] temp = new System.Byte[stringlen];
                if (Read(temp, 0, temp.Length) != temp.Length) { throw new System.IO.IOException($"Could not read {temp.Length} bytes from the stream."); }
                for (System.Int32 I = 0; I < temp.Length; I++) { builder.Append(temp[I].ToChar()); }
                System.String name = builder.ToString();
                builder.Clear();
                // The preference value is written in ASCII.
                stringlen = ReadNumber<System.UInt16>();
                temp = new System.Byte[stringlen];
                if (Read(temp, 0, temp.Length) != temp.Length) { throw new System.IO.IOException($"Could not read {temp.Length} bytes from the stream."); }
                for (System.Int32 I = 0; I < temp.Length; I++) { builder.Append(temp[I].ToChar()); }
                System.String value = builder.ToString();
                builder.Clear();
                // The preference description is written in UTF16-LE.
                length = ReadNumber<System.Int32>();
                temp = new System.Byte[length];
                if (Read(temp, 0, temp.Length) != temp.Length) { throw new System.IO.IOException($"Could not read {temp.Length} bytes from the stream."); }
                for (System.Int32 I = 0; I < temp.Length; I += 2) { builder.Append(temp.ToChar(I)); }
                System.String desc = builder.ToString();
                builder.Clear();
                // Finally , read the type of this preference and continue to the next preference.
                build.Add(new(name, value, desc, (PreferenceValueType)ReadByte()));
            }
            return build;
        }

        protected override void Dispose(bool disposing)
        {
            builder?.Clear();
            builder = null;
            prefsfast?.Clear();
            prefsfast = null;
            base.Dispose(disposing);
        }
    }
}
