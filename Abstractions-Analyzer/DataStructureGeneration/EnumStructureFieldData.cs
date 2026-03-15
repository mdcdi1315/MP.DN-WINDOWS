
using Microsoft.CodeAnalysis;

namespace MP.AbstractionsLib.Analyzer.DataStructureGeneration
{
    public class EnumStructureFieldData : StructureNumericFieldData 
    {
        public ITypeSymbol EnumUnderlyingType { get; set; }
    }
}
