
using System;
using MP.ComInterop;

namespace MP.AudioLibrary.MediaFoundation
{
    /// <summary>
    /// Provides the Media Foundation's implementations of various interfaces. <br />
    /// Most of the provided methods are corresponding to exact interop calls.
    /// </summary>
    public static class MediaFoundationInterfacesFactory
    {
        /// <summary>
        /// Creates and returns an empty Media Foundation sample.
        /// </summary>
        public static IMFSample CreateSample()
        {
            Interop.MfPlat.MFCreateSample(out var ps).ThrowOnFailure();
            return ps;
        }

        /// <summary>
        /// Creates and returns an empty Media Foundation sample. <br />
        /// Instead of throwing an exception whenever on one <see cref="CreateSample()"/> call happens,
        /// this method instead returns the error code back to the user, for additional inspection.
        /// </summary>
        /// <param name="sample">The created <see cref="IMFSample"/>.</param>
        /// <returns>The error code, as reported by the native P/Invoke.</returns>
        public static HRESULT CreateSample(out IMFSample sample) => Interop.MfPlat.MFCreateSample(out sample);

        /// <summary>
        /// Creates and returns an empty Media Foundation media type.
        /// </summary>
        public static IMFMediaType CreateMediaType()
        {
            Interop.MfPlat.MFCreateMediaType(out var mt).ThrowOnFailure();
            return mt;
        }

        /// <summary>
        /// Creates and returns an empty Media Foundation media type. <br />
        /// Instead of throwing an exception whenever on one <see cref="CreateMediaType()"/> call happens,
        /// this method instead returns the error code back to the user, for additional inspection.
        /// </summary>
        /// <param name="sample">The created <see cref="IMFMediaType"/>.</param>
        /// <returns>The error code, as reported by the native P/Invoke.</returns>
        public static HRESULT CreateMediaType(out IMFMediaType mediatype) => Interop.MfPlat.MFCreateMediaType(out mediatype);

        /// <summary>
        /// Creates and returns an empty attributes store.
        /// </summary>
        /// <param name="initialsize">The initial size of the created attributes store.</param>
        /// <returns>A new instance of <see cref="IMFAttributes"/> interface.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="initialsize"/> was negative.</exception>
        public static IMFAttributes CreateAttributes(System.Int32 initialsize = 0)
        {
            if (initialsize < 0) {
                throw new ArgumentOutOfRangeException(nameof(initialsize) , "Initial attributes store size must not be negative.");
            }
            Interop.MfPlat.MFCreateAttributes(initialsize , out var attributes).ThrowOnFailure();
            return attributes;
        }

        /// <summary>
        /// Creates and returns an empty attributes store. <br />
        /// Instead of throwing an exception whenever on one <see cref="CreateAttributes(System.Int32)"/> call happens,
        /// this method instead returns the error code back to the user, for additional inspection.
        /// </summary>
        /// <param name="initialsize">The initial size of the created attributes store.</param>
        /// <param name="attributes">On successfull return, this contains a new instance of the <see cref="IMFAttributes"/> interface.</param>
        /// <returns>The error code, as reported by the native P/Invoke.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="initialsize"/> was negative.</exception>
        public static HRESULT CreateAttributes(System.Int32 initialsize, out IMFAttributes attributes)
        {
            if (initialsize < 0) {
                throw new ArgumentOutOfRangeException(nameof(initialsize), "Initial attributes store size must not be negative.");
            }
            return Interop.MfPlat.MFCreateAttributes(initialsize, out attributes);
        }

        /// <summary>
        /// Creates and returns a new Media Foundation media buffer. <br />
        /// The buffer is created in memory.
        /// </summary>
        /// <param name="maxlength">The maximum length that the returned media buffer will hold.</param>
        /// <returns>A new instance of <see cref="IMFMediaBuffer"/> interface.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="maxlength"/> was zero or negative.</exception>
        public static IMFMediaBuffer CreateMediaBuffer(System.Int32 maxlength)
        {
            if (maxlength < 1) {
                throw new ArgumentOutOfRangeException(nameof(maxlength), "Maximum media buffer length must not be zero or negative.");
            }
            Interop.MfPlat.MFCreateMemoryBuffer(maxlength, out var buffer).ThrowOnFailure();
            return buffer;
        }

        /// <summary>
        /// Creates and returns an empty attributes store. <br />
        /// Instead of throwing an exception whenever on one <see cref="CreateMediaBuffer(System.Int32)"/> call happens,
        /// this method instead returns the error code back to the user, for additional inspection.
        /// </summary>
        /// <param name="maxlength">The maximum length that the returned media buffer will hold.</param>
        /// <param name="buffer">On successfull return, this contains a new instance of the <see cref="IMFMediaBuffer"/> interface.</param>
        /// <returns>The error code, as reported by the native P/Invoke.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="maxlength"/> was zero or negative.</exception>
        public static HRESULT CreateMediaBuffer(System.Int32 maxlength, out IMFMediaBuffer buffer)
        {
            if (maxlength < 1) {
                throw new ArgumentOutOfRangeException(nameof(maxlength), "Maximum media buffer length must not be zero or negative.");
            }
            return Interop.MfPlat.MFCreateMemoryBuffer(maxlength, out buffer);
        }

        /// <summary>
        /// Creates and returns an <see cref="IMFByteStream"/> by wrapping a COM object of type <see cref="IStream"/>. <br />
        /// You may also provide your own .NET <see cref="IStream"/> implementation here.
        /// </summary>
        /// <param name="stream">The stream implementation to translate as a <see cref="IMFByteStream"/>.</param>
        /// <returns>The created <see cref="IMFByteStream"/> interface object.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="stream"/> was <see langword="null"/>.</exception>
        public static IMFByteStream CreateFromWrappingStream(IStream stream)
        {
            ArgumentNullException.ThrowIfNull(stream);
            Interop.MfPlat.MFCreateMFByteStreamOnStream(stream , out var bsm).ThrowOnFailure();
            return bsm;
        }

        /// <summary>
        /// Creates and returns an <see cref="IMFByteStream"/> by wrapping a COM object of type <see cref="IStream"/>. <br />
        /// You may also provide your own .NET <see cref="IStream"/> implementation here. <br />
        ///  Instead of throwing an exception whenever on one <see cref="EnumerateMFTs(Guid)"/> call happens,
        /// this method instead returns the error code back to the user, for additional inspection.
        /// </summary>
        /// <param name="stream">The stream implementation to translate as a <see cref="IMFByteStream"/>.</param>
        /// <param name="bytestream">On successfull return, this contains a created <see cref="IMFByteStream"/> interface object.</param>
        /// <returns>The error code, as reported by the native P/Invoke.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="stream"/> was <see langword="null"/>.</exception>
        public static HRESULT CreateFromWrappingStream(IStream stream , out IMFByteStream bytestream)
        {
            ArgumentNullException.ThrowIfNull(stream);
            return Interop.MfPlat.MFCreateMFByteStreamOnStream(stream, out bytestream);
        }

        /// <summary>
        /// Enumerates all the MFT's that can be found by Media Foundation and those registered by the current process.
        /// </summary>
        /// <param name="guidcategory">The GUID category of the MFT's to specifically enumerate.</param>
        /// <returns>An array of <see cref="IMFActivate"/> objects, representing creatable MFT's.</returns>
        public static IMFActivate[] EnumerateMFTs(Guid guidcategory)
        {
            // Enumerate MFT's that are:
            // -> Syncronous
            // -> Asyncronous, async implemented with software
            // -> Asyncronous, async implemented with hardware
            // -> Created in-process
            // -> All the above, but make sure that are approved by the system for use
            Interop.MfPlat.MFT_ENUM_FLAG flags =
                Interop.MfPlat.MFT_ENUM_FLAG.MFT_ENUM_FLAG_SYNCMFT |
                Interop.MfPlat.MFT_ENUM_FLAG.MFT_ENUM_FLAG_ASYNCMFT |
                Interop.MfPlat.MFT_ENUM_FLAG.MFT_ENUM_FLAG_HARDWARE |
                Interop.MfPlat.MFT_ENUM_FLAG.MFT_ENUM_FLAG_LOCALMFT | 
                Interop.MfPlat.MFT_ENUM_FLAG.MFT_ENUM_FLAG_SORTANDFILTER_APPROVED_ONLY;

            Interop.MfPlat.MFTEnumEx(guidcategory, flags, null, null, out var activates).ThrowOnFailure();
            return activates;
        }

        /// <summary>
        /// Enumerates all the MFT's that can be found by Media Foundation and those registered by the current process. <br />
        /// Instead of throwing an exception whenever on one <see cref="EnumerateMFTs(Guid)"/> call happens,
        /// this method instead returns the error code back to the user, for additional inspection.
        /// </summary>
        /// <param name="guidcategory">The GUID category of the MFT's to specifically enumerate.</param>
        /// <param name="activates">On successfull return, this contains an array of <see cref="IMFActivate"/> objects, representing creatable MFT's.</param>
        /// <returns>The error code, as reported by the native P/Invoke.</returns>
        public static HRESULT EnumerateMFTs(Guid guidcategory, out IMFActivate[] activates)
        {
            // Enumerate MFT's that are:
            // -> Syncronous
            // -> Asyncronous, async implemented with software
            // -> Asyncronous, async implemented with hardware
            // -> Created in-process
            // -> All the above, but make sure that are approved by the system for use
            Interop.MfPlat.MFT_ENUM_FLAG flags =
                Interop.MfPlat.MFT_ENUM_FLAG.MFT_ENUM_FLAG_SYNCMFT |
                Interop.MfPlat.MFT_ENUM_FLAG.MFT_ENUM_FLAG_ASYNCMFT |
                Interop.MfPlat.MFT_ENUM_FLAG.MFT_ENUM_FLAG_HARDWARE |
                Interop.MfPlat.MFT_ENUM_FLAG.MFT_ENUM_FLAG_LOCALMFT | 
                Interop.MfPlat.MFT_ENUM_FLAG.MFT_ENUM_FLAG_SORTANDFILTER_APPROVED_ONLY;

            return Interop.MfPlat.MFTEnumEx(guidcategory, flags, null, null, out activates);
        }
    }
}