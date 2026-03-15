

namespace MP.AbstractionsLib.Analyzer
{
    public static class DiagnosticIds
    {
        public const System.String ExceptionGeneric = "MPAL00000";
        public const System.String PreliminaryAttributedClassUsed = "MPAL00001";
        public const System.String DeprecatedMayBeRemovedAttributedClassUsed = "MPAL00002";
        public const System.String ClassExtendingAbstractPlatformLayerButDoesNotHaveNativeLayerAttribute = "MPAL00003";
        public const System.String DoesNotSpecifyRequiresNativeLayer = "MPAL00006";

        #region COM Generator Diagnostic ID's
        
        public const System.String MisformattedCOMGUIDUsed = "MPAL00004";
        public const System.String ExceptionGeneratingCOMInterface = "MPAL00005";
        public const System.String UnresolvableBaseTypeMayCauseIssues = "MPAL00014";

        #endregion

        #region Data Structure Generator Diagnostic ID's

        public const System.String PointerTypesCannotBeHandled = "MPAL00008";

        public const System.String NotAnAttributeAppliedForStringType = "MPAL00009";

        public const System.String UnrecognizedDataStructureField = "MPAL00010";

        public const System.String EnumFieldCannotBeEncodedAs7Bit = "MPAL00011";

        public const System.String FieldCannotBeUsedAsStringLengthStorage = "MPAL00012";

        public const System.String DataStructureNotImplementingIDataStructure = "MPAL00013";

        #endregion

        #region Diagnostic Categories

        public const System.String ApiUsageCategory = "MDCDI1315.APIUSAGE";

        public const System.String COMGenerationCategory = "MDCDI1315.COMGEN";

        public const System.String DataStructuresUsageCategory = "MDCDI1315.DUSAGE";

        #endregion
    }
}