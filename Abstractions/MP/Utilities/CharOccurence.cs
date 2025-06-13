

namespace MP.Utilities
{
    /// <summary>
    /// Defines a single occurence of a character in a string.
    /// </summary>
    public sealed class CharOccurence
    {
        private System.Char ch;
        private System.Int32 pos;

        internal CharOccurence(System.Char ch, System.Int32 pos)
        {
            this.ch = ch;
            this.pos = pos;
        }

        /// <summary>
        /// Gets the character that occured at <see cref="StringIndex"/>.
        /// </summary>
        public System.Char OccuringCharacter => ch;

        /// <summary>
        /// Gets the index where the <see cref="OccuringCharacter"/> occured in the string.
        /// </summary>
        public System.Int32 StringIndex => pos;
    }
}