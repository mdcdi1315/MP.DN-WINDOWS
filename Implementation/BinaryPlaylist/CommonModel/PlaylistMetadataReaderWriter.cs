
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace MP.BinaryPlaylist
{
    [StructLayout(LayoutKind.Explicit , Pack = 1 , Size = 12)]
    public struct PlaylistCriticalMetadata
    {
        [FieldOffset(0)]
        public System.UInt32 CurrentTrackIndex;

        [FieldOffset(4)]
        public System.Int64 CurrentTrackStoppedTimeInTicks;

        public readonly void Deconstruct(out System.UInt32 ci , out System.Int64 tstime)
        {
            ci = CurrentTrackIndex;
            tstime = CurrentTrackStoppedTimeInTicks;
        }
    }

    public sealed class PlaylistMetadataBlobWriter : PlaylistBlobWriter
    {
        private List<PlaylistMetadataItem> items;
        private PlaylistCriticalMetadata mdtacritical;

        public PlaylistMetadataBlobWriter() 
        {
            items = new(10);
        }

        public void WriteCriticalMetadata(PlaylistCriticalMetadata metadata)
            => mdtacritical = metadata;

        public void AddMetadataItem(PlaylistMetadataItem item)
        {
            if (item is null) {
                throw new System.ArgumentNullException(nameof(item));
            }
            items.Add(item);
        }

        public void AddMetadataItems(IEnumerable<PlaylistMetadataItem> items) 
        {
            if (items is null) { throw new System.ArgumentNullException(nameof(items)); }
            if (items is PlaylistMetadataItemCollection cll)
            {
                this.items.EnsureCapacity(this.items.Count + cll.Count);
                this.items.AddRange(cll);
                return;
            }
            foreach (var i in items) 
            {
                AddMetadataItem(i);
            }
        }

        private void WriteObject(System.Object obj , MetadataItemTypeCode code , ref System.Int64 length)
        {
            switch (code)
            {
                case MetadataItemTypeCode.Undefined:
                case MetadataItemTypeCode.Null:
                    return;
                case MetadataItemTypeCode.Boolean:
                    Write((System.Byte)((System.Boolean)obj ? 1 : 0));
                    length++;
                    break;
                case MetadataItemTypeCode.String:
                    System.String s = obj as System.String;
                    System.Int32 strlen = s.Length * sizeof(System.Char);
                    Write(strlen);
                    length += sizeof(System.Int32) + strlen;
                    Write(s, false);
                    break;
                case MetadataItemTypeCode.Byte:
                    Write((System.Byte)obj);
                    length++;
                    break;
                case MetadataItemTypeCode.SByte:
                    Write((System.SByte)obj);
                    length++;
                    break;
                case MetadataItemTypeCode.Int16:
                    Write((System.Int16)obj);
                    length += sizeof(System.Int16);
                    break;
                case MetadataItemTypeCode.UInt16:
                    Write((System.UInt16)obj);
                    length += sizeof(System.UInt16);
                    break;
                case MetadataItemTypeCode.Int32:
                    Write((System.Int32)obj);
                    length += sizeof(System.Int32);
                    break;
                case MetadataItemTypeCode.UInt32:
                    Write((System.UInt32)obj);
                    length += sizeof(System.UInt32);
                    break;
                case MetadataItemTypeCode.Int64:
                    Write((System.Int64)obj);
                    length += sizeof(System.Int64);
                    break;
                case MetadataItemTypeCode.UInt64:
                    Write((System.UInt64)obj);
                    length += sizeof(System.UInt64);
                    break;
                case MetadataItemTypeCode.Single:
                    Write(((System.Single)obj).ToInt32Bits());
                    length += sizeof(System.Int32);
                    break;
                case MetadataItemTypeCode.Double:
                    Write(((System.Double)obj).ToInt64Bits());
                    length += sizeof(System.Int64);
                    break;
                case MetadataItemTypeCode.Decimal:
                    System.Byte[] b = ((System.Decimal)obj).GetBytes();
                    Write(b, 0, b.Length);
                    length += b.LongLength;
                    break;
            }
        }

        public unsafe void FinalizeWriter()
        {
            System.Int64 len = sizeof(PlaylistCriticalMetadata);
            System.Int32 strlen;
            WriteStructure(mdtacritical);
            foreach (var i in items)
            {
                Write((System.Byte)i.TypeCode);
                len++;
                strlen = i.Name.Length * sizeof(System.Char);
                Write(strlen);
                len += sizeof(System.Int32) + strlen;
                Write(i.Name, false);
                WriteObject(i.Value, i.TypeCode, ref len);
            }
            Header = new() {
                Count = items.Count.ToUInt32(),
                Identifier1 = BlobFlags.Normal,
                Identifier2 = BlobTypes.METADATABLOB,
                Length = len,
                Version = 2
            };
            IsCompleted = true;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && items is not null) 
            {
                items.Clear();
                items = null;
            }
            base.Dispose(disposing);
        }
    }

    public sealed class PlaylistMetadataBlobReader : PlaylistBlobReader
    {
        private System.Int64 baseofs;
        private List<PlaylistMetadataItem> items;
        private PlaylistCriticalMetadata mdtacritical;

        public PlaylistMetadataBlobReader()
        {
            items = null;
            baseofs = -1;
        }

        private void StartReadOp()
        {
            if (baseofs == -1)
            {
                mdtacritical = ReadStructure<PlaylistCriticalMetadata>();
                baseofs = Position;
                if (Header.Version >= 2)
                {
                    // Be pre-allocative a bit and get space for all the metadata items.
                    items = new(Header.Count.ToInt32());
                }
            }
        }

        public PlaylistCriticalMetadata CriticalMetadata
        {
            get {
                StartReadOp();
                return mdtacritical;
            }
        }

        private unsafe System.String ReadString()
        {
            System.Int32 br = ReadNumber<System.Int32>();
            System.Byte[] data = new System.Byte[br];
            System.Int32 rb = Read(data, 0, data.Length);
            fixed (System.Byte* psrc = data)
            {
                return new((System.Char*)psrc, 0, br / sizeof(System.Char));
            }
        }

        private System.Object LoadObject(MetadataItemTypeCode tc)
        {
            switch (tc)
            {
                case MetadataItemTypeCode.Undefined:
                case MetadataItemTypeCode.Null:
                    return null;
                case MetadataItemTypeCode.Boolean:
                    return ReadByte() == 1;
                case MetadataItemTypeCode.Byte:
                    return ReadByte();
                case MetadataItemTypeCode.SByte:
                    return ReadByte().ToSByte();
                case MetadataItemTypeCode.Int16:
                    return ReadNumber<System.Int16>();
                case MetadataItemTypeCode.Int32:
                    return ReadNumber<System.Int32>();
                case MetadataItemTypeCode.Int64:
                    return ReadNumber<System.Int64>();
                case MetadataItemTypeCode.UInt16:
                    return ReadNumber<System.UInt16>();
                case MetadataItemTypeCode.UInt32:
                    return ReadNumber<System.UInt32>();
                case MetadataItemTypeCode.UInt64:
                    return ReadNumber<System.UInt64>();
                case MetadataItemTypeCode.Single:
                    return ReadNumber<System.Int32>().FromInt32Bits();
                case MetadataItemTypeCode.Double:
                    return ReadNumber<System.Int64>().FromInt64Bits();
                case MetadataItemTypeCode.Decimal:
                    return ReadNumber<System.Decimal>();
                case MetadataItemTypeCode.String:
                    return ReadString();
                default:
                    return null;
            }
        }

        public void ReadMetadata()
        {
            StartReadOp();
            if (Header.Version == 1) { return; }
            if (baseofs >= Header.Length) { return; }
            Seek(baseofs, System.IO.SeekOrigin.Begin);
            items.Clear();
            System.Int32 rc = 0;
            while (rc < Header.Count) 
            {
                MetadataItemTypeCode tc = (MetadataItemTypeCode)ReadByte();
                System.String name = ReadString();
                items.Add(new(name, LoadObject(tc)));
                rc++;
            }
        }

        public IEnumerable<PlaylistMetadataItem> Metadata => items;

        public PlaylistMetadataItem GetItemAt(System.Int32 idx) => items[idx];
    }
}