using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MP.ExceptionSystem
{
    public sealed class VorbisWrapperException : BaseException , INativeException
    {

        public VorbisWrapperException() { }

        public VorbisWrapperException(System.String message) : base(message) { }

        internal VorbisWrapperException(Interop.VorbisFile.Errors error) 
            : this($"Error of type {error} occured on the wrapper.") { }
    }
}
