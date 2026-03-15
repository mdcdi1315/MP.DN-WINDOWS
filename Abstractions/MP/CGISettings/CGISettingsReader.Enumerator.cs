
using System;
using System.Collections.Generic;

namespace MP.CGISettings
{
    partial class CGISettingsReader
    {
        /// <summary>
        /// Enumerates all the settings found in a CGI Settings file.
        /// </summary>
        public sealed class Enumerator : IEnumerator<SettingEntry>
        {
            private SettingEntry entry;
            private CGISettingsReader rdr;

            internal Enumerator(CGISettingsReader reader)
            {
                rdr = reader;
                entry = null;
            }

            /// <summary>
            /// Moves to the next setting inside the stream. <br />
            /// Note that the setting must be read by calling the <see cref="Current"/> property 
            /// so that the enumerator can actually continue enumeration.
            /// </summary>
            /// <returns>A value whether moving to a next entry inside the stream was succeeded.</returns>
            public System.Boolean MoveNext()
            {
                System.Boolean mov = rdr is not null && (rdr.stream.Position < rdr.stream.Length);
                if (mov) { LoadEntry(); }
                return mov;
            }

            /// <summary>
            /// Resets the enumerator before the first-defined CGI setting entry.
            /// </summary>
            public void Reset() => rdr.stream.Seek(rdr.offset, IO.SeekDisplacement.Begin);

            /// <summary>
            /// Gets the entry type that is contained in the <see cref="Current"/> property.
            /// </summary>
            public CGISettingType EntryType => entry is null ? throw new InvalidOperationException("The enumeration has not begun yet.") : entry.Type;

            /// <summary>
            /// Gets a value whether the entry contained in the <see cref="Current"/> property represents an array of values.
            /// </summary>
            public System.Boolean EntryIsAnArray => entry is null ? throw new InvalidOperationException("The enumeration has not begun yet.") : entry.IsArrayOfType;

            private ICGISettingExtension GetExtension()
            {
                foreach (ICGISettingExtension e in rdr.exts) { if (e.RegisteredType == entry.Type) { return e; } }
                return null;
            }

            private System.Object GetSimpleValue(System.Byte[] temp)
            {
                // Search first if there are any extensions supporting this type
                ICGISettingExtension ext = GetExtension();
                if (ext is not null) {
                    // Extension found, use that instead
                    IO.MemoryStream ms = new(temp);
                    StreamProvidedCGIExtensionDataSource ds = new(ms, rdr.stringenc, temp.LongLength);
                    temp = null;
                    try {
                        return ext.LoadObject(ds);
                    } finally {
                        ds?.Dispose();
                        ds = null;
                        ms?.Dispose();
                        ms = null;
                    }
                } else {
                    switch (entry.Type)
                    {
                        case CGISettingType.String:
                            return rdr.stringenc.GetString(temp);
                        case CGISettingType.Boolean:
                            return temp[0] == 1;
                        case CGISettingType.Byte:
                            return temp[0];
                        case CGISettingType.SByte:
                            return (System.SByte)temp[0];
                        case CGISettingType.Int16:
                            return temp.ToInt16(0);
                        case CGISettingType.UInt16:
                            return temp.ToUInt16(0);
                        case CGISettingType.Int32:
                            return temp.ToInt32(0);
                        case CGISettingType.UInt32:
                            return temp.ToUInt32(0);
                        case CGISettingType.Int64:
                            return temp.ToInt64(0);
                        case CGISettingType.UInt64:
                            return temp.ToUInt64(0);
                        case CGISettingType.Single:
                            return temp.ToSingle(0);
                        case CGISettingType.Double:
                            return temp.ToDouble(0);
                        case CGISettingType.ByteArray:
                            return temp;
                        default:
                            throw new NotImplementedException($"Conversion function for type {entry.Type} is not registered as a CGI Settings extension or is invalid.");
                    }
                }
            }

            private void ReadEntryV1()
            {
                entry = new();
                var setting = rdr.stream.ReadStructure<CGISETTINGV1>();
                entry.Type = setting.CoreHeader.SettingType;
                if (entry.Type > rdr.header.Mask) { throw new System.Security.SecurityException("Reading this entry may lead to malicious code execution."); }
                rdr.stream.Seek(setting.CoreHeader.Padding, IO.SeekDisplacement.Current);
                entry.Name = rdr.stream.ReadString(rdr.stringenc, setting.CoreHeader.NameLength);
                entry.Value = GetSimpleValue(rdr.stream.ReadBytes(setting.CoreHeader.Length));
            }

            private void ReadEntryV2()
            {
                entry = new();
                var setting = rdr.stream.ReadStructure<CGISETTINGV2>();
                entry.Type = setting.CoreHeader.SettingType;
                if (entry.Type > rdr.header.Mask) { throw new System.Security.SecurityException("Reading this entry may lead to malicious code execution."); }
                rdr.stream.Seek(setting.CoreHeader.Padding, IO.SeekDisplacement.Current);
                entry.Name = rdr.stream.ReadString(rdr.stringenc, setting.CoreHeader.NameLength);
                if (entry.IsArrayOfType = setting.Flags.HasFlag(SettingFlags.Array))
                {
                    entry.ValueList.Clear();
                    System.UInt16 nelementsread = 0;
                    // The arrays are saved as byte arrays , with Int32's describing the length in bytes of a single element.
                    // Each array must be at most the .NET's maximum array length - 4.
                    while (nelementsread < setting.NumberOfElements)
                    {
                        System.Int32 ellength = rdr.stream.ReadInt32();
                        if (ellength > System.Array.MaxLength - 4)
                        {
                            throw new ExceptionSystem.CGIInvalidFormatException("The reader requests more than the array limit. The CGI format is invalid.");
                        }
                        entry.ValueList.Add(GetSimpleValue(rdr.stream.ReadBytes(ellength)));
                        nelementsread++;
                    }
                } else {
                    entry.Value = GetSimpleValue(rdr.stream.ReadBytes(setting.CoreHeader.Length));
                }
            }

            /// <summary>
            /// Gets the last read settings entry from the source.
            /// </summary>
            public SettingEntry Current => entry;

            private void LoadEntry()
            {
                System.Int16 ver = rdr.stream.ReadByte();
                rdr.stream.Seek(-1, IO.SeekDisplacement.Current); // get the position back , we have the structure version.
                switch (ver)
                {
                    case -1:
                        throw new IO.StreamReachedEndException("Stream ended.");
                    case 1:
                        ReadEntryV1();
                        break;
                    case 2:
                        ReadEntryV2();
                        break;
                    default:
                        throw new ExceptionSystem.CGIInvalidFormatException($"The reader is not aware of this entry version: {ver}");
                }
            }

            object System.Collections.IEnumerator.Current => Current;

            /// <summary>
            /// Effectively disposes this <see cref="Enumerator"/> instance.
            /// </summary>
            public void Dispose()
            {
                rdr = null;
                entry = null;
            }
        }
    }
}