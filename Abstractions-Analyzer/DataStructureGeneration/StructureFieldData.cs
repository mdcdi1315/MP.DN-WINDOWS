
using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace MP.AbstractionsLib.Analyzer.DataStructureGeneration
{
    public class StructureFieldData
    {
        private const System.String ATTRIBUTES_NS_PREFIX = "MP.IO.DataStructuring.Generation.";

        public string Name { get; set; }

        public ITypeSymbol FieldType { get; set; }

        public static List<StructureFieldData> GetValidFields(INamedTypeSymbol nts, DiagnosticReporter reporter)
        {
            List<StructureFieldData> fields = new();
            foreach (ISymbol member in nts.GetMembers())
            {
                if (member is IFieldSymbol fs)
                {
                    if (fs.IsConst || fs.IsStatic) {
                        continue;
                    } else if (fs.IsFixedSizeBuffer) {
                        fields.Add(new FixedBufferFieldData() {
                            FieldType = (fs.Type as IPointerTypeSymbol).PointedAtType,
                            Name = fs.Name,
                            FixedSize = fs.FixedSize
                        });
                    } else if (fs.Type is IPointerTypeSymbol) {
                        reporter(Diagnostic.Create(MPDataStructureGeneratorDiagnosticDescriptors.RULE_1, null, fs.Name));
                        return null;
                    } else if (AnalyzerUtils.IsValidNumericType(fs.Type.SpecialType) || fs.Type.TypeKind == TypeKind.Enum) {
                        System.Boolean big_endian = false, bit7encoding = false;
                        foreach (AttributeData ad in fs.GetAttributes())
                        {
                            if (ad.AttributeClass is null) { continue; }
                            switch (ad.AttributeClass.GetFullTypeName())
                            {
                                case $"{ATTRIBUTES_NS_PREFIX}FieldEndianessAttribute":
                                    big_endian = ad.ConstructorArguments[0].Value.ToString() == "Big";
                                    break;
                                case $"{ATTRIBUTES_NS_PREFIX}EncodeAs7BitIfPossibleAttribute":
                                    bit7encoding = true;
                                    break;
                            }
                        }
                        if (fs.Type.TypeKind == TypeKind.Enum) {
                            if (bit7encoding) { reporter(Diagnostic.Create(MPDataStructureGeneratorDiagnosticDescriptors.RULE_4, null, fs.Name)); }
                            fields.Add(new EnumStructureFieldData() {
                                EnumUnderlyingType = (fs.Type as INamedTypeSymbol).EnumUnderlyingType,
                                FieldType = fs.Type,
                                Name = fs.Name,
                                IsBigEndian = big_endian,
                                Use7BitEncoding = bit7encoding,
                            });
                        } else {
                            fields.Add(new StructureNumericFieldData() {
                                FieldType = fs.Type,
                                Name = fs.Name,
                                IsBigEndian = big_endian,
                                Use7BitEncoding = bit7encoding,
                            });
                        }
                    } else if (fs.Type.SpecialType == SpecialType.System_String) {
                        if (AnalyzerUtils.HasAttributeWithMetadataName(fs, $"{ATTRIBUTES_NS_PREFIX}FixedStringAttribute", out AttributeData fixed_str)) {
                            fields.Add(new StructureFixedStringFieldData() {
                                Name = fs.Name,
                                FieldType = fs.Type,
                                Length = (int)fixed_str.ConstructorArguments[0].Value,
                                Encoding = StringFieldEncodingHelpers.ParseFromTC(fixed_str.ConstructorArguments[1])
                            });
                        } else if (AnalyzerUtils.HasAttributeWithMetadataName(fs, $"{ATTRIBUTES_NS_PREFIX}VariableLengthStringAttribute", out AttributeData v_str)) {
                            fields.Add(new StructureVariableStringFieldData() {
                                Name = fs.Name,
                                FieldType = fs.Type,
                                FieldLengthProvider = v_str.ConstructorArguments[0].Value as System.String,
                                Encoding = StringFieldEncodingHelpers.ParseFromTC(v_str.ConstructorArguments[1])
                            });
                        } else {
                            reporter(Diagnostic.Create(MPDataStructureGeneratorDiagnosticDescriptors.RULE_2, null, fs.Name));
                            return null;
                        }
                    } else if (AnalyzerUtils.ImplementsInterface(fs.Type, "MP.IO.DataStructuring.IDataStructure")) {
                        fields.Add(new StructureFieldData() { FieldType = fs.Type, Name = fs.Name });
                    } else {
                        reporter(Diagnostic.Create(MPDataStructureGeneratorDiagnosticDescriptors.RULE_3, null, fs.Name));
                    }
                }
            }
            return fields;
        }
    }
}