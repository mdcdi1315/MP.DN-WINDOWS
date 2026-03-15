

namespace MP.SettingsTree
{
    /// <summary>
    /// Has the values for a setting that provides the <see cref="SettingsTreeValidValuesAttribute{T}"/>.
    /// </summary>
    public sealed class SettingTreeSettingValidRange
    {
        private System.Object minval , maxval;

        /// <summary>
        /// Creates a new instance of the <see cref="SettingTreeSettingValidRange"/> class 
        /// with the specified minimum and maximum values.
        /// </summary>
        /// <param name="minimum"></param>
        /// <param name="maximum"></param>
        public SettingTreeSettingValidRange(System.Object minimum, System.Object maximum)
        {
            minval = minimum;
            maxval = maximum;
        }

        /// <summary>
        /// Gets the smaller value that is the minimum inclusive bound of acceptable values.
        /// </summary>
        public System.Object MinimumBound => minval;

        /// <summary>
        /// Gets the greater value that is the maximum inclusive bound of acceptable values.
        /// </summary>
        public System.Object MaximumBound => maxval;
    }

}