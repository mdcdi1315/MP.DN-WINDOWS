
using System;

namespace MP.Annotations
{
    /// <summary>
    /// Defines a service implementation that may be used by service loaders. <br />
    /// The type parameter <typeparamref name="T"/> specifies the class to be loaded as a service.
    /// </summary>
    /// <typeparam name="T">The type to act as a service. Must have declared a public parameterless constructor.</typeparam>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true, Inherited = false)]
    public sealed class ServiceLoaderServiceAttribute<T> : Attribute 
        where T : notnull, IService, new()
    { }
}