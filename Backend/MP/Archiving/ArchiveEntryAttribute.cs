
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace MP.Archiving
{
    public sealed class ArchiveEntryAttribute
    {
        private System.String name;
        private System.Object value;

        public ArchiveEntryAttribute() 
        {
            name = null;
            value = null;
        }

        public ArchiveEntryAttribute(System.String name , System.Object value)
        {
            this.name = name;
            this.value = value;
        }

        public System.String Name
        {
            get => name;
            set {
                // An archive attribute name is allowed to be null,
                if (value is null) { name = null; return; }
                // but it is not allowed it's character length to be more or less than 4 chars...
                if (value.Length != 4) { throw new ArgumentException("The string must be exactly 4 characters long."); }
                name = value;
            }
        }

        public System.Object Value
        {
            get => value;
            set => this.value = value;
        }

        internal System.Byte[] ToArray(ArchiveByteOrder ord)
        {
            System.Byte[] valuedata;
            MPARCHATTRIB header = new();
            switch (value)
            {
                case null:
                    valuedata = System.Array.Empty<System.Byte>();
                    header.Type = AttributeValueType.Null;
                    break;
                case System.String str:
                    valuedata = System.Text.Encoding.Unicode.GetBytes(str);
                    header.Type = AttributeValueType.String;
                    break;
                case MPARCHUINT unsignedarchint:
                    valuedata = new System.Byte[Unsafe.SizeOf<MPARCHUINT>()];
                    valuedata.WriteStructure(0 , unsignedarchint);
                    header.Type = AttributeValueType.ArchiveUnsignedInteger;
                    break;
                case MPARCHDATETIME archdt:
                    valuedata = new System.Byte[Unsafe.SizeOf<MPARCHDATETIME>()];
                    valuedata.WriteStructure(0, archdt);
                    header.Type = AttributeValueType.ArchiveDateTime;
                    break;
                case System.Boolean bl:
                    valuedata = new System.Byte[1] { (bl ? 1 : 0).ToByte() };
                    header.Type = AttributeValueType.Boolean;
                    break;
                case System.Byte b:
                    valuedata = new System.Byte[1] { b };
                    header.Type = AttributeValueType.Byte;
                    break;
                case System.SByte sb:
                    valuedata = new System.Byte[1] { sb.ToByte() };
                    header.Type = AttributeValueType.SignedByte;
                    break;
                case System.Int16 s:
                    valuedata = s.GetBytes();
                    header.Type = AttributeValueType.Short;
                    break;
                case System.UInt16 us:
                    valuedata = us.GetBytes();
                    header.Type = AttributeValueType.UnsignedShort;
                    break;
                case System.Int32 i:
                    valuedata = i.GetBytes();
                    header.Type = AttributeValueType.Integer;
                    break;
                case System.UInt32 iu:
                    valuedata = iu.GetBytes();
                    header.Type = AttributeValueType.UnsignedInteger;
                    break;
                case System.Int64 l:
                    valuedata = l.GetBytes();
                    header.Type = AttributeValueType.Long;
                    break;
                case System.UInt64 lu:
                    valuedata = lu.GetBytes();
                    header.Type = AttributeValueType.UnsignedLong;
                    break;
                default:
                    throw new System.ArgumentException($"Value {value.GetType().FullName} not supported.");
            }
            // Reverse the value if we want big-endian arithmetic encoding.
            switch (value)
            {
                case System.String:
                case MPARCHUINT:
                case MPARCHDATETIME:
                case System.Byte:
                case System.Boolean:
                    break;
                default:
                    if (ord == ArchiveByteOrder.BigEndian)
                    {
                        valuedata.Reverse();
                    }
                    break;
            }
            header.AttributeLength = MPARCHUINT.ToUnsignedSixByteInteger(valuedata.LongLength);
            header.AttributeId = name;
            System.Int32 hdrsize = Unsafe.SizeOf<MPARCHATTRIB>();
            System.Byte[] temp = new System.Byte[hdrsize + valuedata.LongLength];
            temp.WriteStructure(0, header);
            valuedata.Copy(0, temp, hdrsize, valuedata.LongLength.ToUInt32());
            valuedata = null;
            return temp;
        }
    }

    internal static class ArchiveEntryAttributeExtensions
    {
        public static ArchiveEntryAttribute GetByName(this IEnumerable<ArchiveEntryAttribute> attributes , System.String name)
        {
            if (System.String.IsNullOrEmpty(name)) { return null; }
            foreach (ArchiveEntryAttribute attribute in attributes) { 
                if (attribute.Name == name) { return attribute; }
            }
            return null;
        }

        public static System.Object GetValueByName(this IEnumerable<ArchiveEntryAttribute> attributes , System.String name)
        {
            if (System.String.IsNullOrEmpty(name)) { return null; }
            foreach (ArchiveEntryAttribute attribute in attributes)
            {
                if (attribute.Name == name) { return attribute.Value; }
            }
            return null;
        }

        public static T GetTValueByName<T>(this IEnumerable<ArchiveEntryAttribute> attributes, System.String name)
        {
            if (System.String.IsNullOrEmpty(name)) { return default; }
            foreach (ArchiveEntryAttribute attribute in attributes)
            {
                if (attribute.Name == name) { return (T)attribute.Value; }
            }
            return default;
        }

        public static void SetOrUpdate(this IList<ArchiveEntryAttribute> attributes , System.String name , System.Object value)
        {
            if (attributes is null) { return; }
            for (System.Int32 I = 0; I < attributes.Count; I++) 
            {
                if (attributes[I].Name == name) 
                {
                    attributes[I] = new(name, value);
                    return;
                }
            }
            attributes.Add(new ArchiveEntryAttribute(name, value));
        }
    }
}