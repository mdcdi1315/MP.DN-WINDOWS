
using System;

namespace MP
{
    /// <summary>
    /// Provides the base interface for a service. <br /> 
    /// Services are descriptions of how something specific should be done. <br />
    /// Instances of this interface are loaded through the <see cref="ServiceLoader{T}"/> abstract class.
    /// </summary>
    public interface IService : IDisposable { }
}