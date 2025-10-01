
using MP.Annotations;

namespace MP
{
    /// <summary>
    /// Defines a single string property as it is saved in a <see cref="SavedDataTag"/> instance.
    /// </summary>
    [DeprecatedMayBeRemoved("2.0.0.0")]
    public sealed class SavedDataTagProperty
    {
        private System.String name, value;

        /// <summary>
        /// Creates a new and empty instance of the <see cref="SavedDataTagProperty"/> class.
        /// </summary>
        public SavedDataTagProperty()
        {
            name = null;
            value = null;
        }

        /// <summary>
        /// Creates a new instance of <see cref="SavedDataTagProperty"/> class
        /// from the specified property name and value.
        /// </summary>
        /// <param name="name">The new property name. In the <see cref="SavedDataTag"/> usually this is a hard-coded value.</param>
        /// <param name="value">The property's value.</param>
        public SavedDataTagProperty(System.String name, System.String value) 
        {
            this.name = name;
            this.value = value;
        }

        /// <summary>
        /// Gets or sets the name of the current property.
        /// </summary>
        public System.String Name
        {
            get => name;
            set => name = value;
        }

        /// <summary>
        /// Gets or sets the value of the current property.
        /// </summary>
        public System.String Value
        {
            get => value;
            set => this.value = value;
        }
    }
}