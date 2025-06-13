using MP.ExceptionSystem;

namespace MP.BinaryPlaylist
{
    public sealed class PropertyFormatInvalidException : BaseException
    {
        public PropertyFormatInvalidException() : base("The property value end marker could not be found.") { }

        public PropertyFormatInvalidException(string message) : base(message) { }
    }

    public sealed class InvalidStringDataException : BaseException
    {
        public InvalidStringDataException() : base("The validation marker was invalid.") { }

        public InvalidStringDataException(string message) : base(message) { }
    }

    public sealed class InvalidByteArrayDataException : BaseException
    {
        public InvalidByteArrayDataException() : base("The validation marker was invalid.") { }
    }
}