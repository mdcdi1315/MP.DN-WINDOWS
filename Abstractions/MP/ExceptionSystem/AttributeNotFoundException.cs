

namespace MP.ExceptionSystem
{
    /// <summary>
    /// Exception class that is thrown when a specified attribute class that implements the
    /// <see cref="IAttributeable"/> interface has called the <see cref="IAttributeableExtensions.GetAttribute(IAttributeable, string)"/> but the 
    /// requested attribute was not found.
    /// </summary>
    public sealed class AttributeNotFoundException : BaseException
    {
        private readonly System.String attributename;

        /// <summary>
        /// Creates a new instance of the <see cref="AttributeNotFoundException"/> class
        /// with the specified attribute that the lookup failed.
        /// </summary>
        /// <param name="attributename">The name of the attribute for which the lookup was failed.</param>
        public AttributeNotFoundException(System.String attributename)
            : base ($"The specified attribute was not found. \nAttribute name: {attributename}")
        {
            this.attributename = attributename;
        }

        /// <summary>
        /// Gets the name of the attribute that the lookup was failed with.
        /// </summary>
        public System.String Name => attributename;
    }
}