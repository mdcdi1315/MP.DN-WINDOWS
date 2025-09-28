
using System;
using MP.Annotations.CodeAnalysis;
using System.Diagnostics.CodeAnalysis;

namespace MP.Serialization
{
    /// <summary>
    /// Defines an interface for transoding custom object fields when using the serialization services. <br />
    /// It is called transcoder because such instances transcode the data, that is, conversion from the existing primitive types. <br />
    /// Thus, this interface can extend the serialization manager as required.
    /// </summary>
    public interface ITypeTranscoder
    {
        /// <summary>
        /// The type that the <see cref="ITypeTranscoder"/> instance should read data as and write to. <br />
        /// This is the serialized result when writing or the provides the serialized data, when reading respectively.
        /// </summary>
        public SerializedFieldType SerializedType { get; }

        /// <summary>
        /// Gets the type that this type transcoder can transcode from/to. <br />
        /// The serialization manager can also decode derived instances of the passed type, if required and specified by the <see cref="StrictTypeMatch"/> property.
        /// </summary>
        public Type ActualType { get; }

        /// <summary>
        /// Gets a value whether transcoding should be only done when the type passed in <see cref="ActualType"/> property is an exact and unambiguous match. <br />
        /// If this returns <see langword="false"/>, the serialization manager will also consider that derived instances of the <see cref="ActualType"/> type property can be also transcoded with this instance.
        /// </summary>
        public System.Boolean StrictTypeMatch { get; }

        /// <summary>
        /// Encodes the specified object as of the primitive specified in the <see cref="SerializedType"/> property.
        /// </summary>
        /// <param name="value">The object to encode.</param>
        /// <returns>The serialized value.</returns>
        public System.Object Encode([DisallowNull] System.Object value);

        /// <summary>
        /// Decodes the specified object to an object of type specified in <see cref="ActualType"/> property. <br />
        /// The actual type requested to decode is also passed, so that the transcoder can finally see whether decoding into the requested field type is finally supported.
        /// </summary>
        /// <param name="value">The serialized value to decode as an instance of the <see cref="ActualType"/> property.</param>
        /// <param name="actual">The actual .NET type to return. Can be used by the transcoder to finally determine whether it can decode the specified value into the requested object, or for inspection of the object layout itself.</param>
        /// <returns>The decoded value.</returns>
        /// <exception cref="SerializationException">Decoding <paramref name="value"/> is not supported by this transcoder, or the passed data were invalid so that to be transcoded.</exception>
        [return: NotNull]
        [Throws(typeof(SerializationException))]
        public System.Object Decode([DisallowNull] System.Object value , [DisallowNull] Type actual);
    }
}
