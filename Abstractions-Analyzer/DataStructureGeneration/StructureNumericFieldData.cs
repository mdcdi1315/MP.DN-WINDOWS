
using Microsoft.CodeAnalysis;

namespace MP.AbstractionsLib.Analyzer.DataStructureGeneration
{
    public class StructureNumericFieldData : StructureFieldData
    {
        public System.Boolean IsBigEndian { get; set; }

        public System.Boolean Use7BitEncoding { get; set; }

        public void GenerateReadingCode(System.Text.StringBuilder sb)
        {
            switch (FieldType.SpecialType)
            {
                case SpecialType.System_Char:
                    sb.AppendFormatLine("{0} = access.ReadUInt16{1}().ToChar();", Name, IsBigEndian ? "BE" : System.String.Empty);
                    break;
                case SpecialType.System_Byte:
                    sb.AppendFormatLine("{0} = access.ReadLiteralByte();", Name);
                    break;
                case SpecialType.System_SByte:
                    sb.AppendFormatLine("{0} = access.ReadLiteralByte().ToSByte();", Name);
                    break;
                case SpecialType.System_Int16:
                    sb.AppendFormatLine("{0} = access.ReadInt16{1}();", Name, IsBigEndian ? "BE" : System.String.Empty);
                    break;
                case SpecialType.System_UInt16:
                    sb.AppendFormatLine("{0} = access.ReadUInt16{1}();", Name, IsBigEndian ? "BE" : System.String.Empty);
                    break;
                case SpecialType.System_Int32:
                    if (Use7BitEncoding) {
                        sb.AppendFormatLine("{0} = access.Read7BitEncodedInt();", Name);
                    } else {
                        sb.AppendFormatLine("{0} = access.ReadInt32{1}();", Name, IsBigEndian ? "BE" : System.String.Empty);
                    }
                    break;
                case SpecialType.System_UInt32:
                    sb.AppendFormatLine("{0} = access.ReadUInt32{1}();", Name, IsBigEndian ? "BE" : System.String.Empty);
                    break;
                case SpecialType.System_Int64:
                    if (Use7BitEncoding) {
                        sb.AppendFormatLine("{0} = access.Read7BitEncodedLong();", Name);
                    } else {
                        sb.AppendFormatLine("{0} = access.ReadInt64{1}();", Name, IsBigEndian ? "BE" : System.String.Empty);
                    }
                    break;
                case SpecialType.System_UInt64:
                    sb.AppendFormatLine("{0} = access.ReadUInt64{1}();", Name, IsBigEndian ? "BE" : System.String.Empty);
                    break;
                case SpecialType.System_Decimal:
                    sb.AppendFormatLine("{0} = access.ReadDecimal{1}();", Name, IsBigEndian ? "BE" : System.String.Empty);
                    break;
                case SpecialType.System_Single:
                    sb.AppendFormatLine("{0} = access.ReadSingle{1}();",  Name, IsBigEndian ? "BE" : System.String.Empty);
                    break;
                case SpecialType.System_Double:
                    sb.AppendFormatLine("{0} = access.ReadDouble{1}();", Name, IsBigEndian ? "BE" : System.String.Empty);
                    break;
            }
        }

        public void GenerateWritingCode(System.Text.StringBuilder sb)
        {
            switch (FieldType.SpecialType)
            {
                case SpecialType.System_Char:
                    sb.AppendFormatLine("access.WriteUInt16{0}({1}.ToUInt16());", IsBigEndian ? "BE" : System.String.Empty, Name);
                    break;
                case SpecialType.System_SByte:
                    sb.AppendFormatLine("access.WriteSByte({0});", Name);
                    break;
                case SpecialType.System_Byte:
                    sb.AppendFormatLine("access.WriteByte({0});", Name);
                    break;
                case SpecialType.System_Int16:
                    sb.AppendFormatLine("access.WriteInt16{0}({1});", IsBigEndian ? "BE" : System.String.Empty, Name);
                    break;
                case SpecialType.System_UInt16:
                    sb.AppendFormatLine("access.WriteUInt16{0}({1});", IsBigEndian ? "BE" : System.String.Empty, Name);
                    break;
                case SpecialType.System_Int32:
                    if (Use7BitEncoding) {
                        sb.AppendFormatLine("access.Write7BitEncodedInt({0});", Name);
                    } else {
                        sb.AppendFormatLine("access.WriteInt32{0}({1});", IsBigEndian ? "BE" : System.String.Empty, Name);
                    }
                    break;
                case SpecialType.System_UInt32:
                    sb.AppendFormatLine("access.WriteUInt32{0}({1});", IsBigEndian ? "BE" : System.String.Empty, Name);
                    break;
                case SpecialType.System_Int64:
                    if (Use7BitEncoding) {
                        sb.AppendFormatLine("access.Write7BitEncodedLong({0});", Name);
                    } else {
                        sb.AppendFormatLine("access.WriteInt64{0}({1});", IsBigEndian ? "BE" : System.String.Empty, Name);
                    }
                    break;
                case SpecialType.System_UInt64:
                    sb.AppendFormatLine("access.WriteUInt64{0}({1});", IsBigEndian ? "BE" : System.String.Empty, Name);
                    break;
                case SpecialType.System_Decimal:
                    sb.AppendFormatLine("access.WriteDecimal{0}({1});", IsBigEndian ? "BE" : System.String.Empty, Name);
                    break;
                case SpecialType.System_Single:
                    sb.AppendFormatLine("access.WriteSingle{0}({1});",  IsBigEndian ? "BE" : System.String.Empty, Name);
                    break;
                case SpecialType.System_Double:
                    sb.AppendFormatLine("access.WriteDouble{0}({1});", IsBigEndian ? "BE" : System.String.Empty, Name);
                    break;
            }
        }
    }
}