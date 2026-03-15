using System;
using MP.Utilities;
using MP.Collections;
using System.Runtime.CompilerServices;

namespace MP.CGISettings
{
    /// <summary>
    /// Defines a writer implementation that can write CGI settings into the 
    /// CGI Interchargeable Binary Format.
    /// </summary>
    public sealed class CGISettingsWriter : ICGISettingsWriter<SettingEntry> , IStreamOwnerBase
    {
        private sealed class ConstructedSettingEntry
        {
            public System.Byte[] Data;
            public CGISettingType Type;
            public System.Boolean IsArrayType;
            public System.UInt16 NumberOfElements;

            public ConstructedSettingEntry(System.Byte[] data, CGISettingType type)
            {
                Data = data;
                Type = type;
                IsArrayType = false;
                NumberOfElements = 0;
            }
        }

        internal const System.Int32 Version = 1 , EntriesVersionCurrent = 2;
        internal const System.String PadString = "PAD";
        private ArrayBasedList<ICGISettingExtension> exts;
        private ArrayBasedList<SettingEntry> settings;
        private System.Text.Encoding stringencoding;
        private System.Boolean leaveopen , dirty;
        private IO.DataStream stream;
        private System.String appname;
        private CGIHEADER header;

        private CGISettingsWriter()
        {
            settings = new();
            leaveopen = false;
            dirty = false;
            stream = null;
            appname = System.String.Empty;
            header = new();
            exts = new(10);
            header.Version = Version;
            header.Mask = CGISettingType.String;
            header.EntriesVersion = EntriesVersionCurrent;
            stringencoding = System.Text.Encoding.Unicode;
            header.StringEncoding = stringencoding.CodePage;
        }

        /// <summary>
        /// Creates a new CGI Settings writer instance that will write to the specified stream.
        /// </summary>
        /// <param name="stream">The stream to write the constructed CGI data into.</param>
        /// <exception cref="ArgumentNullException"><paramref name="stream"/> was <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="stream"/> was unwriteable.</exception>
        public CGISettingsWriter(IO.DataStream stream) : this(stream , true) { }

        /// <summary>
        /// Creates a new CGI Settings writer instance that will write to the specified stream. <br />
        /// Additionally a text encoding can be passed which will indicate the format to save all the strings under.
        /// </summary>
        /// <param name="stream">The stream to write the constructed CGI data into.</param>
        /// <param name="encoding">The format under all strings should be saved as.</param>
        /// <exception cref="ArgumentNullException"><paramref name="stream"/> and/or <paramref name="encoding"/> were <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="stream"/> was unwriteable.</exception>
        public CGISettingsWriter(IO.DataStream stream, System.Text.Encoding encoding) : this(stream, true)
        {
            ArgumentNullException.ThrowIfNull(encoding);
            stringencoding = encoding;
        }

        /// <summary>
        /// Creates a new CGI Settings writer instance that will write to the specified stream. <br />
        /// Additionally a text encoding can be passed which will indicate the format to save all the strings under, 
        /// and a boolean value controls whether the provided stream lifetime is managed by the newly constructed instance or not.
        /// </summary>
        /// <param name="stream">The stream to write the constructed CGI data into.</param>
        /// <param name="encoding">The format under all strings should be saved as.</param>
        /// <param name="leaveopen">A value whether the stream should be left open after disposing.</param>
        /// <exception cref="ArgumentNullException"><paramref name="stream"/> and/or <paramref name="encoding"/> were <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="stream"/> was unwriteable.</exception>
        public CGISettingsWriter(IO.DataStream stream, System.Text.Encoding encoding, System.Boolean leaveopen) : this(stream , leaveopen)
        {
            ArgumentNullException.ThrowIfNull(encoding);
            stringencoding = encoding;
        }

        /// <summary>
        /// Creates a new CGI Settings writer instance that will write to the specified stream. <br />
        /// Additionally a boolean value controls whether the provided stream lifetime is managed by the newly constructed instance or not.
        /// </summary>
        /// <param name="stream">The stream to write the constructed CGI data into.</param>
        /// <param name="leaveopen">A value whether the stream should be left open after disposing.</param>
        /// <exception cref="ArgumentNullException"><paramref name="stream"/> was <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="stream"/> was unwriteable.</exception>
        public CGISettingsWriter(IO.DataStream stream, System.Boolean leaveopen) : this()
        {
            ArgumentNullException.ThrowIfNull(stream);
            if (stream.CanWrite == false) {
                throw new ArgumentException("Stream must be writeable.", nameof(stream));
            }
            this.leaveopen = leaveopen;
            this.stream = stream;
        }

        /// <inheritdoc />
        public System.String ApplicationName
        {
            get => appname;
            set {
                if (dirty) { throw new InvalidOperationException("Cannot change the application name after the first setting has been written."); }
                ArgumentNullException.ThrowIfNull(value);
                appname = value;
                header.AppNameLength = appname.Length;
            }
        }

        /// <summary>
        /// Gets or sets a value whether this instance manages the stream provided in the constructors or not.
        /// </summary>
        public System.Boolean IsStreamOwner
        {
            get => leaveopen == false;
            set => leaveopen = (value == false);
        }

        /// <inheritdoc />
        public System.Text.Encoding StringsEncoding
        {
            get => stringencoding;
            set {
                if (dirty) { throw new InvalidOperationException("Cannot change the strings encoding after the first setting has been written."); }
                ArgumentNullException.ThrowIfNull(value);
                stringencoding = value;
                header.StringEncoding = stringencoding.CodePage;
            }
        }

        private void WriteHeader() 
        {
            header.StringEncoding = stringencoding.CodePage;
            System.Byte[] namestr = System.Text.Encoding.UTF8.GetBytes(appname);
            header.AppNameLength = namestr.Length;
            header.Padding = UnsafeMethods.PadAlignment(4, Unsafe.SizeOf<CGIHEADER>() + header.AppNameLength);
            header.Padding = header.Padding > 512 ? 4 : header.Padding;
            stream.WriteStructure(header);
            stream.WritePadString(PadString, header.Padding);
            stream.Write(namestr, 0, namestr.Length);
        }

        /// <inheritdoc />
        public void Add(SettingEntry value)
        {
            ObjectDisposedException.ThrowIf(stream is null, this);
            ArgumentNullException.ThrowIfNull(value);
            var type = GetCorrespondingTypeForObject(value.Value);
            if (header.Mask < type) { header.Mask = type; }
            value.Type = type;
            settings.Add(value);
        }

        private CGISettingType GetCorrespondingTypeForObject(System.Object obj)
        {
            // First search if any of the extensions implement logic for this object
            Type t = obj.GetType();
            foreach (ICGISettingExtension ext in exts) {
                if (ext.WrappingType == t) { return ext.RegisteredType; }
            }
            // No extension found, search the primitive types
            return obj switch {
                System.Byte[] => CGISettingType.ByteArray,
                System.String => CGISettingType.String,
                System.Byte => CGISettingType.Byte,
                System.SByte => CGISettingType.SByte,
                System.Int16 => CGISettingType.Int16,
                System.Int32 => CGISettingType.Int32,
                System.Int64 => CGISettingType.Int64,
                System.UInt16 => CGISettingType.UInt16,
                System.UInt32 => CGISettingType.UInt32,
                System.UInt64 => CGISettingType.UInt64,
                System.Single => CGISettingType.Single,
                System.Double => CGISettingType.Double,
                System.Boolean => CGISettingType.Boolean,
                /*
                Microsoft.IO.FileInfo => CGISettingType.FileInfo,
                Microsoft.IO.DirectoryInfo => CGISettingType.DirectoryInfo,
                System.Drawing.Color => CGISettingType.Color,
                */
                // Finally throw an exception if the object's type cannot be encoded.
                _ => throw new ArgumentException($"Cannot encode the setting with type {obj.GetType().FullName}"),
            };
        }

        private ConstructedSettingEntry GetArrayData(SettingEntry se)
        {
            CGISettingType type = se.Type;
            System.Int32 elements = se.ValueList.Count, len;
            IO.MemoryStream ms = new();
            try {
                ConstructedSettingEntry constructed;
                for (System.Int32 I = 0; I < elements; I++)
                {
                    // Construct a temporary and forged SettingEntry to use the GetData method.
                    // Otherwise we will only retrieve the constructed data and nothing else.
                    constructed = GetData(new() {
                        Name = "___EDMMMMM",
                        Value = se.ValueList[I],
                        IsArrayOfType = false,
                        Type = type
                    });
                    if ((len = constructed.Data.Length) > Array.MaxLength - 4) {
                        throw new InvalidOperationException("Attempted to add an object which surpasses the defined CGI setting size limits.");
                    }
                    ms.WriteInt32(len);
                    ms.WriteBytes(constructed.Data);
                }
                return new(ms.ToArray(), type) { IsArrayType = true, NumberOfElements = elements.ToUInt16() };
            } finally {
                ms.Dispose();
            }
        }

        private ConstructedSettingEntry GetData(SettingEntry obj)
        {
            if (obj is null) { throw new ArgumentException("All settings values must not be null."); }
            if (obj.IsArrayOfType)
            {
                // Array type other than a byte array , must be handled seperately.
                return GetArrayData(obj);
            }
            // If it was determined that it will be encoded by an extension, do it then through that object
            foreach (ICGISettingExtension ext in exts) {
                if (ext.RegisteredType == obj.Type)
                {
                    IO.MemoryStream ms = new();
                    StreamProvidedCGIExtensionDataSource ds = new(ms, stringencoding, 0);
                    try {
                        ext.SaveObject(ds, obj.Value);
                        return new(ms.ToArray(), obj.Type);
                    } finally {
                        ds?.Dispose();
                        ds = null;
                        ms?.Dispose();
                        ms = null;
                    }
                }
            }
            // Otherwise fall back to the internal types.
            switch (obj.Value)
            {
                case System.Byte[] data:
                    return new(data, CGISettingType.ByteArray);
                case System.String data:
                    return new(stringencoding.GetBytes(data), CGISettingType.String);
                case System.Byte b:
                    return new(new System.Byte[] { b }, CGISettingType.Byte);
                case System.SByte sb:
                    return new(new System.Byte[] { sb.ToByte() }, CGISettingType.SByte);
                case System.Int16 s:
                    return new(s.GetBytes(), CGISettingType.Int16);
                case System.Int32 i:
                    return new(i.GetBytes(), CGISettingType.Int32);
                case System.Int64 l:
                    return new(l.GetBytes(), CGISettingType.Int64);
                case System.UInt16 us:
                    return new(us.GetBytes(), CGISettingType.UInt16);
                case System.UInt32 ui:
                    return new(ui.GetBytes(), CGISettingType.UInt32);
                case System.UInt64 ul:
                    return new(ul.GetBytes(), CGISettingType.UInt64);
                case System.Single f:
                    return new(f.GetBytes(), CGISettingType.Single);
                case System.Double fd:
                    return new(fd.GetBytes(), CGISettingType.Double);
                case System.Boolean bl:
                    return new(new System.Byte[] { (bl ? 1 : 0).ToByte() }, CGISettingType.Boolean);
                default:
                    throw new ArgumentException($"Cannot encode the setting with type {obj.GetType().FullName}");
            }
        }

        /// <summary>
        /// Generates the data and writes those into the provided stream.
        /// </summary>
        /// <exception cref="ObjectDisposedException">The stream was closed when <see cref="Generate"/> was called.</exception>
        public void Generate()
        {
            ObjectDisposedException.ThrowIf(stream is null, this);
            WriteHeader();
            System.Byte[] name, value;
            foreach (var setting in settings)
            {
                name = stringencoding.GetBytes(setting.Name);
                var dat = GetData(setting);
                value = dat.Data;
                CGISETTINGV2 settinghdr = new();
                settinghdr.CoreHeader.NameLength = name.Length;
                settinghdr.CoreHeader.Length = value.LongLength;
                // This change is not breaking since all the older written versions of the setting should normally read the expected pad bytes.
                settinghdr.CoreHeader.Padding = UnsafeMethods.PadAlignment(8 , value.Length + name.Length).ToUInt16();
                // However, PadAlignment can return extravagant values that WritePadString cannot handle.
                // So , explicitly set to an aligned value.
                if (settinghdr.CoreHeader.Padding > 512) { settinghdr.CoreHeader.Padding = 12; }
                settinghdr.CoreHeader.SettingType = dat.Type;
                if (dat.IsArrayType)
                {
                    settinghdr.Flags |= SettingFlags.Array;
                    settinghdr.NumberOfElements = dat.NumberOfElements;
                }
                stream.WriteStructure(settinghdr);
                stream.WritePadString(PadString, settinghdr.CoreHeader.Padding);
                stream.Write(name, 0, name.Length);
                stream.Write(value, 0, value.Length);
                name = value = null;
            }
            dirty = true;
        }
    
        /// <summary>
        /// Disposes this <see cref="CGISettingsWriter"/> instance.
        /// </summary>
        public void Dispose()
        {
            if (dirty == false) { Generate(); }
            if (stream is not null)
            {
                if (leaveopen == false) { stream.Dispose(); }
                stringencoding = null;
                appname = null;
                settings?.Clear();
                settings = null;
                stream = null;
            }
        }

        /// <inheritdoc />
        public void RegisterExtension(ICGISettingExtension extension)
        {
            ArgumentNullException.ThrowIfNull(extension);
            switch (extension.RegisteredType)
            {
                case CGISettingType.String:
                case CGISettingType.Boolean:
                case CGISettingType.Byte:
                case CGISettingType.SByte:
                case CGISettingType.Int16:
                case CGISettingType.UInt16:
                case CGISettingType.Int32:
                case CGISettingType.UInt32:
                case CGISettingType.Int64:
                case CGISettingType.UInt64:
                case CGISettingType.Single:
                case CGISettingType.Double:
                    throw new ArgumentException("Cannot override the primitive and protected types.");
            }
            for (System.Int32 I = 0; I < exts.Count; I++)
            {
                if (exts[I].RegisteredType == extension.RegisteredType)
                {
                    // Override found - replace the preexisting extension with this one
                    exts[I] = extension;
                    return;
                }
            }
            // Otherwise, just add it 
            exts.Add(extension);
        }
    }
}
