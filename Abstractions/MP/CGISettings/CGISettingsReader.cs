using System;
using MP.Utilities;
using MP.Collections;
using System.Collections;
using System.Collections.Generic;

namespace MP.CGISettings
{
    /// <summary>
    /// Defines the reader that has the ability to read CGI settings from the 
    /// CGI Interchargeable Binary Format.
    /// </summary>
    public sealed partial class CGISettingsReader : ICGISettingsReader<SettingEntry> , IStreamOwnerBase
    {
        private ArrayBasedList<ICGISettingExtension> exts;
        private System.Text.Encoding stringenc;
        private IO.DataStream stream;
        private System.Boolean strmown;
        private System.String appname;
        private System.Int64 offset;
        private CGIHEADER header;

        /// <summary>
        /// Constructs a new instance of the <see cref="CGISettingsReader"/> class from the specified stream 
        /// that has data that represent the CGI Interchargeable Binary Format.
        /// </summary>
        /// <param name="stream">The stream to read data from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="stream"/> was <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="stream"/> was not readable and/or seekable.</exception>
        public CGISettingsReader(IO.DataStream stream)
        {
            ArgumentNullException.ThrowIfNull(stream);
            if (stream.CanRead == false || stream.CanSeek == false)  {
                throw new ArgumentException("The stream must both be readable and seekable." , nameof(stream));
            }
            this.stream = stream;
            this.strmown = false;
            ReadHeader();
        }

        private void ReadHeader()
        {
            header = stream.ReadStructure<CGIHEADER>();
            if (header.IsValid == false) {
                throw new ExceptionSystem.CGIInvalidFormatException("This is not the CGI Settings format.");
            }
            if (header.Version > CGISettingsWriter.Version) { throw new ExceptionSystem.CGIInvalidFormatException($"The version {header.Version} is invalid."); }
            if (header.EntriesVersion > CGISettingsWriter.EntriesVersionCurrent) { throw new ExceptionSystem.CGIInvalidFormatException($"The reader cannot read the entries version {header.EntriesVersion}."); }
            stream.Seek(header.Padding, IO.SeekDisplacement.Current);
            appname = stream.ReadString(System.Text.Encoding.UTF8, header.AppNameLength);
            stringenc = System.Text.Encoding.GetEncoding(header.StringEncoding);
            offset = stream.Position;
            exts = new(10);
        }

        /// <summary>
        /// Gets or sets a value whether the stream provided in the constructor should be disposed upon calling <see cref="Dispose"/>.
        /// </summary>
        public System.Boolean IsStreamOwner
        {
            get => strmown;
            set => strmown = value;
        }

        /// <summary>
        /// Gets the CGI stream version. <br />
        /// Note: This does not return the entries version, it just returns the header version only.
        /// </summary>
        public System.Int32 Version => header.Version.ToInt32();

        /// <summary>
        /// Gets the name of the application that created this stream. May also be null or empty.
        /// </summary>
        public System.String ApplicationName => appname;

        /// <summary>
        /// Gets the encoding that is currently used for saving string data inside this stream.
        /// </summary>
        public System.Text.Encoding Encoding => stringenc;

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

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public Enumerator GetEnumerator() => new(this);

        IEnumerator<SettingEntry> IEnumerable<SettingEntry>.GetEnumerator() => GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// Disposes this <see cref="CGISettingsReader"/> instance.
        /// </summary>
        public void Dispose()
        {
            if (stream is not null)
            {
                if (strmown) { stream.Dispose(); }
                stringenc = null;
                appname = null;
                stream = null;
            }
        }
    }
}
