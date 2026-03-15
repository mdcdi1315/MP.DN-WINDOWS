using System;

namespace MP.AudioLibrary
{
    /// <summary>
    /// Internal interface for statically accessing and storing <see cref="AudioFormatConverter{T}"/> instances to the <see cref="AudioFormatConverters"/> class.
    /// </summary>
    internal interface IAudioFormatConverterAccessor
    {
        Type Type { get; } // This will be called once per instance to store it in dictionaries

        public System.Object ConvertTo(AudioFormat audio_format);

        public AudioFormat ConvertFrom(System.Object custom);
    }

    /// <summary>
    /// Provides a converter for converting instances of type <typeparamref name="T"/> to <see cref="AudioFormat"/> classes and vice-versa.
    /// </summary>
    /// <typeparam name="T">The type that this converter will convert to.</typeparam>
    public abstract class AudioFormatConverter<T> : IAudioFormatConverterAccessor
    {
        /// <summary>
        /// Converts an <see cref="AudioFormat"/> to an audio format of type <typeparamref name="T"/>.
        /// </summary>
        /// <param name="format">The audio format to convert.</param>
        /// <returns>The converted audio format.</returns>
        public abstract T ConvertTo(AudioFormat format);

        /// <summary>
        /// Converts an audio format of type <typeparamref name="T"/> to an <see cref="AudioFormat"/> instance.
        /// </summary>
        /// <param name="custom_format">The audio format of type <typeparamref name="T"/> to convert.</param>
        /// <returns>The converted audio format.</returns>
        public abstract AudioFormat ConvertFrom(T custom_format);

        Type IAudioFormatConverterAccessor.Type => typeof(T);

        System.Object IAudioFormatConverterAccessor.ConvertTo(AudioFormat audio_format) => ConvertTo(audio_format);

        AudioFormat IAudioFormatConverterAccessor.ConvertFrom(object custom) => ConvertFrom((T)custom);
    }
}