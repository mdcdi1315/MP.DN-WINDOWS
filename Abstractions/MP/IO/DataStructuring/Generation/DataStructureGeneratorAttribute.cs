
using System;

namespace MP.IO.DataStructuring.Generation
{
    /// <summary>Indicates to the MP Data Structure Generator that it should generate boilerplate code for reading/writing the specified class/structure.</summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
    public sealed class DataStructureGeneratorAttribute : Attribute { }
}