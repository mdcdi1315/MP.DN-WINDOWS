namespace MP.AbstractionsLib.Analyzer.DataStructureGeneration
{
    public class FixedBufferFieldData : StructureFieldData
    {
        public int FixedSize { get; set; }

        public string GenerateFixedBufferReadMethod(int method_ordinal)
        {
            System.Text.StringBuilder sb = new();

            sb.AppendTabs(1);
            sb.AppendFormatLine("private void ReadFixedBuffer_G{0}(IDataStreamAccess access)", method_ordinal);
            sb.AppendTabs(1);
            sb.AppendLine('{');

            string fs = FieldType.GetFullTypeName();

            sb.AppendTabs(2);
            sb.AppendFormatLine("System.UInt32 cpblk_size = {0}U * sizeof({1}).ToUInt32();", FixedSize, fs);
            sb.AppendTabs(2);
            sb.AppendFormatLine("System.Byte[] data = access.ReadBytes(cpblk_size);");
            sb.AppendTabs(2);
            sb.AppendLine("Unsafe.CopyBlockUnaligned(");
            sb.AppendTabs(3);
            sb.AppendFormatLine("ref Unsafe.As<{0}, System.Byte>(ref {1}[0]),", fs, Name);
            sb.AppendTabs(3);
            sb.AppendLine("ref data[0], cpblk_size");
            sb.AppendTabs(2);
            sb.AppendLine(");");

            sb.AppendTabs(1);
            sb.AppendLine('}');
            sb.AppendLine();

            return sb.ToString();
        }

        public string GenerateFixedBufferWriteMethod(int method_ordinal)
        {
            System.Text.StringBuilder sb = new();

            sb.AppendTabs(1);
            sb.AppendFormatLine("private void WriteFixedBuffer_G{0}(IDataStreamAccess access)", method_ordinal);
            sb.AppendTabs(1);
            sb.AppendLine('{');

            string fs = FieldType.GetFullTypeName();

            sb.AppendTabs(2);
            sb.AppendFormatLine("System.UInt32 cpblk_size = {0}U * sizeof({1}).ToUInt32();", FixedSize, fs);

            sb.AppendTabs(2);
            sb.AppendLine("System.Byte[] data = new System.Byte[cpblk_size];");

            sb.AppendTabs(2);
            sb.AppendLine("Unsafe.CopyBlockUnaligned(");
            sb.AppendTabs(3);
            sb.AppendLine("ref data[0],");
            sb.AppendTabs(3);
            sb.AppendFormatLine("ref Unsafe.As<{0}, System.Byte>(ref {1}[0]),", fs, Name);
            sb.AppendTabs(3);
            sb.AppendLine("cpblk_size");
            sb.AppendTabs(2);
            sb.AppendLine(");");

            sb.AppendTabs(2);
            sb.AppendLine("access.WriteBytes(data);");

            sb.AppendTabs(1);
            sb.AppendLine('}');
            sb.AppendLine();

            return sb.ToString();
        }
    }
}