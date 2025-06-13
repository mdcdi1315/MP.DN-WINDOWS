

using MP.ComInterop;
using MP.WindowsInterop;
using System;
using System.Runtime.InteropServices;

namespace MP.AudioLibrary.MediaFoundation
{
    public sealed class MediaFoundationMediaType : IEquatable<MediaFoundationMediaType>
    {
        private IMFMediaType medtype;

        /// <summary>
        /// This is the <see cref="MajorType"/> GUID for audio data.
        /// </summary>
        public static Guid AudioMajorType => new(0x73647561, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xAA, 0x00, 0x38, 0x9B, 0x71);

        private MediaFoundationMediaType(IMFMediaType medtype) => this.medtype = medtype;

        public static MediaFoundationMediaType DeserializeFromBuffer(System.Byte[] buffer)
        {
            ArgumentNullException.ThrowIfNull(buffer);
            if (buffer.LongLength == 0) {
                throw new ArgumentException("Buffer cannot be empty." , nameof(buffer));
            }
            // Create the media type, we will otherwise need it, see below why.
            IMFMediaType mtype; 
            HRESULT hr = Interop.MfPlat.MFCreateMediaType(out mtype);
            hr.ThrowOnFailure();
            hr = Interop.MfPlat.MFInitAttributesFromBlob(mtype , buffer);
            if (hr.FAILED) {
                // Destroy mtype, we are going to throw exception
                ComMarshalling.ReleaseInteropObject(mtype);
                hr.ThrowOnFailure();
            }
            return new MediaFoundationMediaType(mtype);
        }

        public static MediaFoundationMediaType FromExisting(IMFMediaType medtype) => new(medtype);

        public static unsafe MediaFoundationMediaType FromPointer(void* ptype)
        {
            ArgumentNullException.ThrowIfNull(ptype);
            return new(ComMarshalling.CreateInteropObject(ptype) as IMFMediaType);
        }

        public static unsafe MediaFoundationMediaType FromHandle(System.IntPtr handle)
        {
            if (handle == IntPtr.Zero) { 
                throw new ArgumentNullException(nameof(handle));
            }
            return new(ComMarshalling.CreateInteropObject(handle) as IMFMediaType);
        }

        public static MediaFoundationMediaType FromExtensible(WAVEFORMATEXTENSIBLE ext)
        {
            HRESULT hr = Interop.MfPlat.MFCreateMediaType(out var imf);
            hr.ThrowOnFailure();
            hr = Interop.MfPlat.MFInitMediaTypeFromWaveFormatEx(imf, ext);
            if (hr.FAILED) {
                // Destroy mtype, we are going to throw exception
                ComMarshalling.ReleaseInteropObject(imf);
                hr.ThrowOnFailure();
                return null; // This instruction will never be reached , it just exists here so as to mock the C# compiler that all code paths are covered
            }
            return new MediaFoundationMediaType(imf);
        }

        public static MediaFoundationMediaType FromAudioFormat(AudioFormat af)
        {
            ArgumentNullException.ThrowIfNull(af);
            return FromExtensible(af.ConvertTo<WAVEFORMATEXTENSIBLE>());
        }

        public static MediaFoundationMediaType CreateEmpty()
        {
            IMFMediaType mt;
            Interop.MfPlat.MFCreateMediaType(out mt).ThrowOnFailure();
            return new MediaFoundationMediaType(mt); 
        }

        public static Guid CreateMediaSubTypeGUID(System.UInt32 code) => new(code, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        public static Guid CreateMediaSubTypeGUIDFromFormatTag(WAVEFORMATTAG tag) => CreateMediaSubTypeGUID((System.UInt32)tag);

        /// <summary>
        /// Determines whether two <see cref="MediaFoundationMediaType"/> objects represent the same media types.
        /// </summary>
        /// <param name="other">The other media type to compare to this one.</param>
        /// <returns>A value whether the two media types are equal.</returns>
        /// <exception cref="ObjectDisposedException">This media type is disposed.</exception>
        public unsafe System.Boolean Equals(MediaFoundationMediaType other)
        {
            ObjectDisposedException.ThrowIf(medtype is null, this);
            if (other is null) { return false; }
            if (other.medtype is null) { return false; }
            MF_MEDIATYPE_EQUALITY_FLAGS eqf;
            switch (medtype.IsEqual(Marshal.GetIUnknownForObject(other.medtype).ToPointer(), &eqf))
            {
                case CommonHResults.S_OK:
                    return eqf.HasFlag(MF_MEDIATYPE_EQUALITY_FLAGS.MF_MEDIATYPE_EQUAL_MAJOR_TYPES | 
                        MF_MEDIATYPE_EQUALITY_FLAGS.MF_MEDIATYPE_EQUAL_FORMAT_TYPES | 
                        MF_MEDIATYPE_EQUALITY_FLAGS.MF_MEDIATYPE_EQUAL_FORMAT_DATA);
                case CommonHResults.S_FALSE:
                    return false;
                default:
                    return false;
            }
        }

        public System.Boolean IsAudioFormat => MajorType == AudioMajorType;

        public Guid MajorType
        {
            get => medtype.GetAttribute<Guid>(IMFMediaTypeAttributes.MAJOR_TYPE);
            set => medtype.SetAttribute(IMFMediaTypeAttributes.MAJOR_TYPE, value);
        }

        public Guid SubType
        {
            get => medtype.GetAttribute<Guid>(IMFMediaTypeAttributes.SUBTYPE);
            set => medtype.SetAttribute(IMFMediaTypeAttributes.SUBTYPE , value);
        }

        public WAVEFORMATTAG AudioFormatTag
        {
            get {
                if (IsAudioFormat == false) {
                    throw new InvalidOperationException("The current media type does not represent an audio format.");
                }
                return (WAVEFORMATTAG)SubType.ToByteArray().ToUInt16(0);
            }
        }

        public System.UInt32 SampleSize
        {
            get {
                ObjectDisposedException.ThrowIf(medtype is null, this);
                if ((BOOL)medtype.GetAttribute<System.UInt32>(IMFMediaTypeAttributes.FIXED_SIZE_SAMPLES) == BOOL.FALSE)
                {
                    throw new InvalidOperationException("The current media type does not consist of fixed size samples");
                }
                return medtype.GetAttribute<System.UInt32>(IMFMediaTypeAttributes.SAMPLE_SIZE);
            }
            set {
                ObjectDisposedException.ThrowIf(medtype is null, this);
                if ((BOOL)medtype.GetAttribute<System.UInt32>(IMFMediaTypeAttributes.FIXED_SIZE_SAMPLES) == BOOL.FALSE)
                {
                    throw new InvalidOperationException("The current media type does not consist of fixed size samples");
                }
            }
        }

        public System.UInt32 SampleRate
        {
            get {
                ObjectDisposedException.ThrowIf(medtype is null, this);
                if (IsAudioFormat == false)
                {
                    throw new InvalidOperationException("The current media type does not represent an audio format.");
                }
                return medtype.GetAttribute<System.UInt32>(IMFMediaTypeAttributes.AUDIO_SAMPLES_PER_SECOND);
            }
            set {
                ObjectDisposedException.ThrowIf(medtype is null, this);
                if (IsAudioFormat == false) {
                    throw new InvalidOperationException("The current media type does not represent an audio format.");
                }
                medtype.SetAttribute(IMFMediaTypeAttributes.AUDIO_SAMPLES_PER_SECOND , value);
            }
        }

        public System.UInt32 BitRate
        {
            get {
                ObjectDisposedException.ThrowIf(medtype is null, this);
                if (IsAudioFormat == false) {
                    throw new InvalidOperationException("The current media type does not represent an audio format.");
                }
                return medtype.GetAttribute<System.UInt32>(IMFMediaTypeAttributes.AUDIO_BITS_PER_SAMPLE);
            }
            set {
                ObjectDisposedException.ThrowIf(medtype is null, this);
                if (IsAudioFormat == false) {
                    throw new InvalidOperationException("The current media type does not represent an audio format.");
                }
                medtype.SetAttribute(IMFMediaTypeAttributes.AUDIO_BITS_PER_SAMPLE , value);
            }
        }

        public System.UInt32 Channels
        {
            get {
                ObjectDisposedException.ThrowIf(medtype is null, this);
                if (IsAudioFormat == false) {
                    throw new InvalidOperationException("The current media type does not represent an audio format.");
                }
                return medtype.GetAttribute<System.UInt32>(IMFMediaTypeAttributes.AUDIO_NUM_CHANNELS);
            }
            set {
                ObjectDisposedException.ThrowIf(medtype is null, this);
                if (IsAudioFormat == false) {
                    throw new InvalidOperationException("The current media type does not represent an audio format.");
                }
                medtype.SetAttribute(IMFMediaTypeAttributes.AUDIO_NUM_CHANNELS , value);
            }
        }

        public SPEAKERASSIGNMENT ChannelAssignment
        {
            get {
                ObjectDisposedException.ThrowIf(medtype is null, this);
                if (IsAudioFormat == false) {
                    throw new InvalidOperationException("The current media type does not represent an audio format.");
                }
                return (SPEAKERASSIGNMENT)medtype.GetAttribute<System.UInt32>(IMFMediaTypeAttributes.AUDIO_CHANNEL_MASK);
            }
            set {
                ObjectDisposedException.ThrowIf(medtype is null, this);
                if (IsAudioFormat == false) {
                    throw new InvalidOperationException("The current media type does not represent an audio format.");
                }
                medtype.SetAttribute(IMFMediaTypeAttributes.AUDIO_CHANNEL_MASK, (System.UInt32)value);
            }
        }

        public System.UInt32 BlockAlignment
        {
            get {
                ObjectDisposedException.ThrowIf(medtype is null, this);
                if (IsAudioFormat == false) {
                    throw new InvalidOperationException("The current media type does not represent an audio format.");
                }
                return medtype.GetAttribute<System.UInt32>(IMFMediaTypeAttributes.AUDIO_BLOCK_ALIGNMENT);
            }
            set {
                ObjectDisposedException.ThrowIf(medtype is null, this);
                if (IsAudioFormat == false) {
                    throw new InvalidOperationException("The current media type does not represent an audio format.");
                }
                medtype.SetAttribute(IMFMediaTypeAttributes.AUDIO_BLOCK_ALIGNMENT, value);
            }
        }

        public System.UInt32 ValidBitsPerSample
        {
            get {
                ObjectDisposedException.ThrowIf(medtype is null, this);
                if (IsAudioFormat == false) {
                    throw new InvalidOperationException("The current media type does not represent an audio format.");
                }
                return medtype.GetAttribute<System.UInt32>(IMFMediaTypeAttributes.AUDIO_VALID_BITS_PER_SAMPLE);
            }
            set {
                ObjectDisposedException.ThrowIf(medtype is null, this);
                if (IsAudioFormat == false) {
                    throw new InvalidOperationException("The current media type does not represent an audio format.");
                }
                if (BitRate > value) {
                    throw new ArgumentException("The number of valid bits per sample must be at least the bit rate.");
                }
                medtype.SetAttribute(IMFMediaTypeAttributes.AUDIO_VALID_BITS_PER_SAMPLE, value);
            }
        }

        public WAVEFORMATEXTENSIBLE GetExtensible()
        {
            ObjectDisposedException.ThrowIf(medtype is null, this);
            if (IsAudioFormat == false) {
                throw new InvalidOperationException("The current media type does not represent an audio format.");
            }
            Interop.MfPlat.MFCreateWaveFormatExFromMFMediaType(medtype, out var wex).ThrowOnFailure();
            return wex;
        }

        public AudioFormat ToAudioFormat() => AudioFormatConverter.ConvertFrom(GetExtensible());

        /// <summary>
        /// Serializes the current media type so that it can be created on other thread with the help of <see cref="DeserializeFromBuffer(byte[])"/>.
        /// </summary>
        /// <returns>The serialized media type. Do not optimize the size of this buffer.</returns>
        public System.Byte[] Serialize()
        {
            ObjectDisposedException.ThrowIf(medtype is null, this);
            HRESULT hr = Interop.MfPlat.MFGetAttributesAsBlob(medtype, out var bf);
            hr.ThrowOnFailure();
            return bf;
        }

        /// <summary>
        /// Returns the native media type - use this when you directly need to interact with Media Foundation API's
        /// </summary>
        public IMFMediaType NativeMediaType
        {
            get {
                ObjectDisposedException.ThrowIf(medtype is null, this);
                return medtype;
            }
        }

        /// <summary>
        /// Use this when you do not own this media type
        /// </summary>
        public void SetAsInvalid() => medtype = null;

        /// <summary>
        /// Disposes this <see cref="MediaFoundationMediaType"/> instance.
        /// </summary>
        public void Dispose() 
        {
            if (medtype is not null)
            {
                ComMarshalling.ReleaseInteropObject(medtype);
                medtype = null;
            }
        }
    }
}