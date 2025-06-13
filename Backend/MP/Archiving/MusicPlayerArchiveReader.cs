
using System;
using MP.Utilities;
using System.Runtime.CompilerServices;

namespace MP.Archiving
{
    public sealed class MusicPlayerArchiveReader : IDisposable , IStreamOwnerBase
    {
        private System.Boolean streamowner;
        private System.IO.Stream underlying;
        private MPARCHHEADER hdr;

        public MusicPlayerArchiveReader(System.IO.Stream underlying)
        {
            if (underlying is null) { throw new ArgumentNullException(nameof(underlying)); }
            if (underlying.CanRead == false) { throw new ArgumentException("The given stream was unreadable." , nameof(underlying)); }
            this.underlying = underlying;
            streamowner = false;
            Init();
        }

        private void Init()
        { 
            hdr = underlying.ReadStructure<MPARCHHEADER>();
            if (hdr.IsValid == false) {
                throw new ExceptionSystem.InvalidMPArchiveFormatException("The format magic of this stream does not indicate the Music Player Archive.");
            }
        }

        private System.Object GetTranslatedAttributeValue(System.Byte[] bytes , AttributeValueType type)
        {
            switch (type)
            {
                case AttributeValueType.Null:
                case AttributeValueType.Boolean:
                case AttributeValueType.Byte:
                case AttributeValueType.ArchiveUnsignedInteger:
                case AttributeValueType.ArchiveDateTime:
                case AttributeValueType.String:
                    break;
                default:
                    if (hdr.ByteOrder == ArchiveByteOrder.BigEndian)
                    {
                        bytes.Reverse();
                    }
                    break;
            }
            switch (type)
            {
                case AttributeValueType.Null:
                    return null;
                case AttributeValueType.String:
                    return System.Text.Encoding.Unicode.GetString(bytes);
                case AttributeValueType.ArchiveUnsignedInteger:
                    return bytes.ReadStructure<MPARCHUINT>(0);
                case AttributeValueType.ArchiveDateTime:
                    return bytes.ReadStructure<MPARCHDATETIME>(0);
                case AttributeValueType.Boolean:
                    return bytes[0] == 1;
                case AttributeValueType.Byte:
                    return bytes[0];
                case AttributeValueType.SignedByte:
                    return (System.SByte)bytes[0];
                case AttributeValueType.Short:
                    return bytes.ToInt16(0);
                case AttributeValueType.UnsignedShort:
                    return bytes.ToUInt16(0);
                case AttributeValueType.Integer:
                    return bytes.ToInt32(0);
                case AttributeValueType.UnsignedInteger:
                    return bytes.ToUInt32(0);
                case AttributeValueType.Long:
                    return bytes.ToInt64(0);
                case AttributeValueType.UnsignedLong:
                    return bytes.ToUInt64(0);
                default:
                    throw new NotSupportedException($"Unknown type retrieved: {type}");
            }
        }

        /// <summary>
        /// Gets the next archived entry on the stream , or if reached the end, returns <see langword="null"/>.
        /// </summary>
        /// <returns>A new entry reader or <see langword="null"/> indicating that the stream has ended.</returns>
        /// <exception cref="NotSupportedException">The stream's contents are not supported or indicates corruption.</exception>
        /// <remarks>
        /// If at any time one such call throws an exception , do not consider that the next call will be safe. <br />
        /// Instead, stop reading and consider that the stream is corrupted. <br /> <br />
        /// Note that before moving on to a next entry , you should call <see cref="IDisposable.Dispose"/> on the previously returned <see cref="IArchiveEntryReader"/>. <br />
        /// The above allows the reader to correctly interpret the next entry.
        /// </remarks>
        public IArchiveEntryReader GetNextArchiveEntry()
        {
            System.Byte[] header = new System.Byte[Unsafe.SizeOf<MPARCHENTRY>()];
            if (underlying.Read(header, 0, header.Length) < header.Length) 
            {
                return null; // Not enough bytes to create an entry header so indicate that the archive has ended.
            }
            MPARCHENTRY native = header.ReadStructure<MPARCHENTRY>(0);
            ArchiveEntryAttribute[] attributes = new ArchiveEntryAttribute[native.NumberOfAttributes];
            MPARCHATTRIB na;
            for (System.Int32 I = 0; I < attributes.Length; I++) 
            {
                na = underlying.ReadStructure<MPARCHATTRIB>();
                // Re-using the header array.
                header = underlying.ReadBytes(na.AttributeLength.ToInt64());
                attributes[I] = new ArchiveEntryAttribute() { Name = na.AttributeId };
                if (na.Type > AttributeValueType.UnsignedLong) {
                    // we cannot read this attribute so save null instead and continue to the next one.
                    attributes[I].Value = null;
                } else {
                    attributes[I].Value = GetTranslatedAttributeValue(header, na.Type);
                }
            }
            switch (native.Compression)
            {
                case ArchiveEntryCompression.Store:
                    return new PlainArchiveEntryReader(new(native, attributes), new(underlying, native.FileSize.ToInt64()));
                case ArchiveEntryCompression.GZip:
                    return new GZipArchiveEntryReader(new(native, attributes), new(underlying, native.CompressedSize.ToInt64()));
                case ArchiveEntryCompression.BZip2:
                    return new BZip2ArchiveEntryReader(new(native, attributes), new(underlying, native.CompressedSize.ToInt64()));
                default:
                    throw new NotSupportedException($"Compression method is unknown: {native.Compression}");
            }
        }

        public System.Boolean IsStreamOwner
        {
            get => streamowner; 
            set => streamowner = value;
        }

        public void Dispose() 
        {
            if (underlying is not null)
            {
                if (streamowner) { underlying.Dispose(); }
                underlying = null;
            }
        }
    }
}