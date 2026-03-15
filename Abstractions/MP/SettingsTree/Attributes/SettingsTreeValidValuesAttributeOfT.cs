
using System;

namespace MP.SettingsTree
{
    /// <summary>
    /// Fields or properties marked with this attribute denote the valid range of values 
    /// that the denoted field or property can accept. <br />
    /// The <typeparamref name="T"/> type indicate the setting type that is defined on the field. <br />
    /// You should use this attribute for numeric values.
    /// </summary>
    /// <typeparam name="T">The type of the setting to mark it's valid range of values.</typeparam>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property , AllowMultiple = false)]
    public sealed class SettingsTreeValidValuesAttribute<T> : Attribute
    {
        private T minval;
        private T maxval;

        /// <summary>
        /// Constructs a new instance of the <see cref="SettingsTreeValidValuesAttribute{T}"/> class by specifying the desired numeric range.
        /// </summary>
        /// <param name="minimum">The smaller number that is the minimum inclusive bound of acceptable values.</param>
        /// <param name="maximum">The greater number that is the maximum inclusive bound of acceptable values.</param>
        public SettingsTreeValidValuesAttribute(T minimum, T maximum)
        {
            minval = minimum;
            maxval = maximum;
        }
        
        /// <summary>
        /// Gets the smaller value that is the minimum inclusive bound of acceptable values.
        /// </summary>
        public T MinimumValue => minval;

        /// <summary>
        /// Gets the greater value that is the maximum inclusive bound of acceptable values.
        /// </summary>
        public T MaximumValue => maxval;
    }
}