
using System;
using System.Diagnostics.CodeAnalysis;

namespace MP.NativeInterop.OSX.ObjectiveC
{
    /// <summary>
    /// Indicates an Objective-C runtime object. <br />
    /// Typically, objects of such type imported into .NET have a 
    /// native poitner providing the actual representation of the object 
    /// as well as a method to destroy the object.
    /// </summary>
    public unsafe interface IObjectiveCObject : IDisposable
    {
        /// <summary>
        /// Gets the pointer to the natively referenced Objective-C object.
        /// </summary>
        [NotNull]
        public void* Native { get; }

        /// <summary>
        /// Gets an instance to the Objective-C type that the current object projects.
        /// </summary>
        /// <returns>The Objective-C type projected by the current instance.</returns>
        public ObjectiveCType GetObjectiveCType();
    }
}