

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace MP.AudioLibrary
{
    /// <summary>
    /// Provides base services for objects implementing the <see cref="AudioFormatConverter{T}"/> abstract class.
    /// </summary>
    public static class AudioFormatConverters
    {
        private static readonly Dictionary<Type, IAudioFormatConverterAccessor> converters;

        static AudioFormatConverters() {
            converters = new(10);
        }

        private static void RegisterConverterInternal(IAudioFormatConverterAccessor accessor)
        {
            try {
                converters.Add(accessor.Type, accessor);
            } catch (ArgumentException) {
                throw new ConverterAlreadyRegisteredException(accessor.Type);
            }
        }

        /// <summary>
        /// Registers a converter that can convert from/to the type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The incompatible audio format to be converted to an instance of the <see cref="AudioFormat"/> class.</typeparam>
        /// <param name="converter">The converter to register.</param>
        /// <exception cref="ArgumentNullException"><paramref name="converter"/> is <see langword="null"/>.</exception>
        /// <exception cref="ConverterAlreadyRegisteredException">A converter that converts to the exact same type was attempted to be registered.</exception>
        public static void RegisterConverter<T>(AudioFormatConverter<T> converter)
        {
            ArgumentNullException.ThrowIfNull(converter);
            RegisterConverterInternal(converter);
        }

        /// <summary>
        /// Attempts to convert the specified audio format to an audio format of type <typeparamref name="T"/>, if the converter exists for that type.
        /// </summary>
        /// <typeparam name="T">The type of the representation to be returned.</typeparam>
        /// <param name="format">The audio format to convert.</param>
        /// <param name="result">The converted object that equally represents the passed format in the <paramref name="format"/> parameter.</param>
        /// <returns>A value whether a converter was found for the specified type.</returns>
        public static System.Boolean TryConvert<T>(AudioFormat format, out T result)
        {
            ArgumentNullException.ThrowIfNull(format);
            if (converters.TryGetValue(typeof(T), out var converter)) {
                result = (T)converter.ConvertTo(format);
                return true;
            } else {
                result = default;
                return false;
            }
        }

        /// <summary>
        /// Attempts to convert the specified unknown audio format representation of type <typeparamref name="T"/> to an instance of the <see cref="AudioFormat"/> class, if the converter exists for that type.
        /// </summary>
        /// <typeparam name="T">The type of the representation to be converted to an <see cref="AudioFormat"/> instance.</typeparam>
        /// <param name="original">The unknown audio format representation to convert.</param>
        /// <param name="format">The converted <see cref="AudioFormat"/> instance.</param>
        /// <returns>A value whether a converter was found for the specified type.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="original"/> is <see langword="null"/>.</exception>
        public static System.Boolean TryConvert<T>(T original, [NotNullWhen(true)] out AudioFormat format)
        {
            ArgumentNullException.ThrowIfNull(original);
            if (converters.TryGetValue(typeof(T), out var converter)) {
                format = converter.ConvertFrom(original);
                return true;
            } else {
                format = null;
                return false;
            }
        }

        /// <summary>
        /// Converts the specified <see cref="AudioFormat"/> instance to a representation of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of the representation to be returned.</typeparam>
        /// <param name="format">The audio format to convert.</param>
        /// <returns>The converted audio format representation of type <typeparamref name="T"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="format"/> is <see langword="null"/>.</exception>
        /// <exception cref="ConverterNotFoundException">A converter was not found for the specified type.</exception>
        public static T Convert<T>(AudioFormat format)
        {
            ArgumentNullException.ThrowIfNull(format);
            if (converters.TryGetValue(typeof(T) , out var converter)) {
                return (T)converter.ConvertTo(format);
            } else {
                throw new ConverterNotFoundException($"There was not a suitable converter to convert from the AudioFormat class to the type {typeof(T).FullName}.");
            }
        }

        /// <summary>
        /// Converts the specified unknown audio format representation of type <typeparamref name="T"/> to an instance of the <see cref="AudioFormat"/> class.
        /// </summary>
        /// <typeparam name="T">The type of the representation to be converted to an <see cref="AudioFormat"/> instance.</typeparam>
        /// <param name="original">The unknown audio format representation to convert.</param>
        /// <returns>An instance of the <see cref="AudioFormat"/> class.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="original"/> is <see langword="null"/>.</exception>
        /// <exception cref="ConverterNotFoundException">A converter was not found for the specified type.</exception>
        public static AudioFormat Convert<T>(T original)
        {
            ArgumentNullException.ThrowIfNull(original);
            if (converters.TryGetValue(typeof(T), out var converter)) {
                return converter.ConvertFrom(original);
            } else {
                throw new ConverterNotFoundException($"There was not a suitable converter to convert from the type {typeof(T).FullName} to the AudioFormat class.");
            }
        }
    }
}