
using System;
using MP.Serialization;

namespace MP.ExtensibilitySystem.Serialization
{
    /// <summary>
    /// Defines a way to de/serialize <see cref="ExtensibilitySystem.ExtensionPackage"/> instances.
    /// </summary>
    public class ExtensionPackage : ISerializableClass
    {
        /// <summary>
        /// Defines the assets to be loaded by the extensibility engine.
        /// </summary>
        [FieldName("assets")]
        [DerivedTypeBinding(typeof(ManagedAsset) , typeof(NativeAsset))]
        public Asset[] Assets;

        /// <summary>Gets a unique identifier of this package.</summary>
        [FieldName("name")]
        public System.String Name;

        /// <summary>Gets a small textual description of the package.</summary>
        [FieldName("description")]
        public System.String Description;

        /// <summary>Gets the version of this package.</summary>
        [FieldName("version")]
        public Version Version;
    }
}