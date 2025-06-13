
namespace MP.ExtSystemApi
{
    public sealed class InvalidPackageJsonFormatException : ExceptionSystem.BaseException
    {
        public InvalidPackageJsonFormatException(System.String msg) : base(msg) { }
    }
}