
using Microsoft.CodeAnalysis;

namespace MP.AbstractionsLib.Analyzer.DataStructureGeneration
{
    public static class MPDataStructureGeneratorDiagnosticDescriptors
    {
        public static readonly DiagnosticDescriptor RULE_1 = new(
            DiagnosticIds.PointerTypesCannotBeHandled,
            "Pointer types cannot be handled by the MP Data Structure Generator",
            "Remove or alter the type of the field {0}",
            DiagnosticIds.DataStructuresUsageCategory,
            DiagnosticSeverity.Error,
            true
        );

        public static readonly DiagnosticDescriptor RULE_2 = new(
            DiagnosticIds.NotAnAttributeAppliedForStringType,
            "Not an instance of the FixedStringAttribute or the VariableLengthStringAttribute was specified",
            "Make the string field to specify either one of the attributes, or remove the string field with name {0}",
            DiagnosticIds.DataStructuresUsageCategory,
            DiagnosticSeverity.Error,
            true
        );

        public static readonly DiagnosticDescriptor RULE_3 = new(
            DiagnosticIds.UnrecognizedDataStructureField,
            "Field is not recognized and cannot be participating somehow at the data structure definition",
            "Either remove the field {0} or suppress this message if the field is existing for other reason far than reading it and writing it to a stream",
            DiagnosticIds.DataStructuresUsageCategory,
            DiagnosticSeverity.Warning,
            true
        );

        public static readonly DiagnosticDescriptor RULE_4 = new(
            DiagnosticIds.EnumFieldCannotBeEncodedAs7Bit,
            "Field backed by an enumeration type cannot be de/encoded as a 7-bit encoded integer",
            "The field {0} specified that it wants to be de/encoded as a 7-bit encoded integer, but this is not supported on such kind of fields",
            DiagnosticIds.DataStructuresUsageCategory,
            DiagnosticSeverity.Warning,
            true
        );

        public static readonly DiagnosticDescriptor RULE_5 = new(
            DiagnosticIds.FieldCannotBeUsedAsStringLengthStorage,
            "The specified field cannot be used as the length storage for a variable-sized string field",
            "The type of field {0} cannot be used as the length storage for the variable-sized string field {1}; change the type of the field to a supported integer type instead",
            DiagnosticIds.DataStructuresUsageCategory,
            DiagnosticSeverity.Error,
            true
        );

        public static readonly DiagnosticDescriptor RULE_6 = new(
            DiagnosticIds.DataStructureNotImplementingIDataStructure,
            "The specified class/structure does not implement the IDataStructure interface",
            "Apply the IDataStructure interface to class/structure {0}",
            DiagnosticIds.DataStructuresUsageCategory,
            DiagnosticSeverity.Error,
            true
        );
    }
}