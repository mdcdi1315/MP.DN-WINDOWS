
using System;
using MP.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace MP.AudioLibrary
{
    /// <summary>
    /// Defines static and extension methods for converting audio formats to and from various native sources. <br />
    /// It allows dynamically to register a new converter delegate that does the job.
    /// </summary>
    public static class AudioFormatConverter
    {
        private sealed class AFToNativeEQComparer : IEqualityComparer<AudioFormatConverterToNativeRegistrationInfo>
        {
            private DelegateEqualityComparer<Converter<AudioFormat, System.Object>> eqc;

            public AFToNativeEQComparer() => eqc = new();

            public System.Boolean Equals(AudioFormatConverterToNativeRegistrationInfo x, AudioFormatConverterToNativeRegistrationInfo y) => 
                x.OutputType == y.OutputType && eqc.Equals(x.ConverterDelegate, y.ConverterDelegate);

            public System.Int32 GetHashCode([DisallowNull] AudioFormatConverterToNativeRegistrationInfo obj) => eqc.GetHashCode(obj.ConverterDelegate);
        }

        private sealed class NativeToAFEQComparer : IEqualityComparer<NativeToAudioFormatConverterRegistrationInfo>
        {
            private DelegateEqualityComparer<Converter<System.Object, AudioFormat>> eqc;
            
            public NativeToAFEQComparer() => eqc = new();

            public System.Boolean Equals(NativeToAudioFormatConverterRegistrationInfo x, NativeToAudioFormatConverterRegistrationInfo y) => 
                x.InputType == y.InputType && eqc.Equals(x.ConverterDelegate, y.ConverterDelegate);

            public System.Int32 GetHashCode([DisallowNull] NativeToAudioFormatConverterRegistrationInfo obj) => eqc.GetHashCode(obj.ConverterDelegate);
        }

        private static HashSet<AudioFormatConverterToNativeRegistrationInfo> converterstonative;
        private static HashSet<NativeToAudioFormatConverterRegistrationInfo> convertersfromnative;

        static AudioFormatConverter() {
            convertersfromnative = new(5, new NativeToAFEQComparer());
            converterstonative = new(5, new AFToNativeEQComparer());
        }

        /// <summary>
        /// Registers a converter delegate that can convert from an <see cref="AudioFormat"/> instance to a native representation of an object to the audio format converter.
        /// </summary>
        /// <param name="converter">The converter to provide.</param>
        /// <exception cref="ArgumentNullException"><paramref name="converter"/> information were <see langword="null"/>.</exception>
        /// <exception cref="InvalidOperationException"><paramref name="converter"/> is already registered.</exception>
        public static void RegisterConverterToNativeObject(AudioFormatConverterToNativeRegistrationInfo converter)
        {
            ArgumentNullException.ThrowIfNull(converter.OutputType , nameof(converter));
            ArgumentNullException.ThrowIfNull(converter.ConverterDelegate, nameof(converter));
            if (converterstonative.Add(converter) == false) {
                throw new InvalidOperationException("Attempted to register the same converter method instance twice.");
            }
        }

        /// <summary>
        /// Registers a converter delegate that can convert from a native representation of an object to an <see cref="AudioFormat"/> instance to the audio format converter.
        /// </summary>
        /// <param name="converter">The converter delegate to provide.</param>
        /// <exception cref="ArgumentNullException"><paramref name="converter"/> information were <see langword="null"/>.</exception>
        /// <exception cref="InvalidOperationException"><paramref name="converter"/> is already registered.</exception>
        public static void RegisterNativeObjectToConverter(NativeToAudioFormatConverterRegistrationInfo converter)
        {
            ArgumentNullException.ThrowIfNull(converter.InputType , nameof(converter));
            ArgumentNullException.ThrowIfNull(converter.ConverterDelegate, nameof(converter));
            if (convertersfromnative.Add(converter) == false) {
                throw new InvalidOperationException("Attempted to register the same converter method instance twice.");
            }
        }

        /// <summary>
        /// Gets a value whether the specified type that is provided as an input parameter to the converter delegate is supported.
        /// </summary>
        /// <param name="srcobj">The native object to see if it can be converted into an <see cref="AudioFormat"/> instance.</param>
        /// <returns>A value whether the specified type is supported by the currently registered converters.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="srcobj"/> was <see langword="null"/>.</exception>
        public static System.Boolean CanConvertFrom(Type srcobj)
        {
            ArgumentNullException.ThrowIfNull(srcobj);
            foreach (var c in convertersfromnative)
            {
                if (c.InputType == srcobj) {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Gets a value whether the specified type that is provided as an input parameter to the converter delegate is supported.
        /// </summary>
        /// <typeparam name="T">The native object type to see if it can be converted into an <see cref="AudioFormat"/> instance.</typeparam>
        /// <returns>A value whether the specified type is supported by the currently registered converters.</returns>
        public static System.Boolean CanConvertFrom<T>() => CanConvertFrom(typeof(T));

        /// <summary>
        /// Gets a value whether the specified type that is outputted from any converter delegate is supported.
        /// </summary>
        /// <param name="aft"></param>
        /// <param name="targetobj">The native object to see if it can be converted from an <see cref="AudioFormat"/> instance.</param>
        /// <returns>A value whether the specified type is supported by the currently registered converters.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="targetobj"/> was <see langword="null"/>.</exception>
        [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Just to hook this up as an extension method to AudioFormat class.")]
        public static System.Boolean CanConvertTo(this AudioFormat aft, System.Type targetobj) 
        {
            ArgumentNullException.ThrowIfNull(targetobj);
            foreach (var c in converterstonative)
            {
                if (c.OutputType == targetobj) {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Gets a value whether the specified type that is outputted from any converter delegate is supported.
        /// </summary>
        /// <typeparam name="T">The native object type to see if it can be converted from an <see cref="AudioFormat"/> instance.</typeparam>
        /// <param name="aft"></param>
        /// <returns>A value whether the specified type is supported by the currently registered converters.</returns>
        public static System.Boolean CanConvertTo<T>(this AudioFormat aft) => CanConvertTo(aft , typeof(T));

        /// <summary>
        /// Given a native object, it searches into the registered converters for an appropriate converter, 
        /// performs the conversion, and the conversion result is returned.
        /// </summary>
        /// <param name="native">The native object to be converted.</param>
        /// <exception cref="ArgumentNullException"><paramref name="native"/> is <see langword="null"/>.</exception>
        /// <exception cref="NotSupportedException">Conversion is not suppported from <paramref name="native"/>.</exception>
        public static AudioFormat ConvertFrom(System.Object native)
        {
            ArgumentNullException.ThrowIfNull(native);
            var t = native.GetType();
            foreach (var c in convertersfromnative)
            {
                if (c.InputType == t) { return c.ConverterDelegate(native); }
            }
            throw new NotSupportedException($"Conversion from {t.FullName} is not supported.");
        }

        /// <summary>
        /// Given the specified type , it searches into the registered converters for an appropriate converter,
        /// performs the conversion , and the conversion result is returned casted to <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The conversion result type of the object to be returned.</typeparam>
        /// <param name="aft">The audio format to convert from.</param>
        /// <returns>The converted native object.</returns>
        /// <exception cref="NotSupportedException">Conversion is not supported from <paramref name="aft"/> to <typeparamref name="T"/>.</exception>
        public static T ConvertTo<T>(this AudioFormat aft) 
        {
            Type t = typeof(T);
            foreach (var c in converterstonative)
            {
                if (c.OutputType == t) {
                    return (T)c.ConverterDelegate(aft);
                }
            }
            throw new NotSupportedException($"Conversion to {t.FullName} is not supported.");
        }
    }

    /// <summary>
    /// Registration information for a converter that converts from an <see cref="AudioFormat"/> to a native object.
    /// </summary>
    public struct AudioFormatConverterToNativeRegistrationInfo
    {
        /// <summary>
        /// The <see cref="Type"/> of the native object returned through <see cref="ConverterDelegate"/>.
        /// </summary>
        public Type OutputType;
        /// <summary>
        /// A conversion delegate that is invoked to perform the actual conversion.
        /// </summary>
        public Converter<AudioFormat, System.Object> ConverterDelegate;
    }

    /// <summary>
    /// Registration information for a converter that converts from a native object to an <see cref="AudioFormat"/>.
    /// </summary>
    public struct NativeToAudioFormatConverterRegistrationInfo
    {
        /// <summary>
        /// The <see cref="Type"/> of the native object passed as a parameter to <see cref="ConverterDelegate"/>.
        /// </summary>
        public Type InputType;
        /// <summary>
        /// A conversion delegate that is invoked to perform the actual conversion.
        /// </summary>
        public Converter<System.Object, AudioFormat> ConverterDelegate;
    }
}