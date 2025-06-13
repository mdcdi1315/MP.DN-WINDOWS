

namespace MP.ExceptionSystem
{
    public sealed class OptionalFeatureAbsentException : BaseException
    {
        private System.String feature;

        public OptionalFeatureAbsentException(System.String feature) : base($"Code execution cannot continue because an optional feature is required that is not installed.\nFeature name: {feature}") { this.feature = feature; }

        public System.String MissingFeature => feature;
    }
}