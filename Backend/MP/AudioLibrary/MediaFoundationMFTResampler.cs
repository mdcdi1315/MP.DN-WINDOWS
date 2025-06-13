

using System;
using MP.ComInterop;
using MP.WindowsInterop;
using MP.AudioLibrary.MediaFoundation;


namespace MP.AudioLibrary
{
    /// <summary>
    /// A transform by using the underlying RESAMPLEDMO.DLL library.
    /// </summary>
    public sealed class MediaFoundationMFTResampler : MediaFoundationTransform
    {
        private System.Int32 resamplingquality;
        private IMFActivate activateobject;
        private System.Single lpfbandwidth;

        private static System.Boolean IsPcmOrIeeeFloat(AudioFormat format)
            => format.CommonFormat == CommonAudioFormat.PCM || format.CommonFormat == CommonAudioFormat.IEEEFloat;

        public MediaFoundationMFTResampler(IAudioProvider provider , AudioFormat desiredformat , System.Int32 latencyinms = 100)
            : base(provider, desiredformat , latencyinms)
        {
            if (IsPcmOrIeeeFloat(provider.Format) == false) {
                throw new ArgumentException("Input audio format must be PCM or IEEE floating-point." , nameof(provider));
            }
            if (IsPcmOrIeeeFloat(desiredformat) == false) {
                throw new ArgumentException("Output audio format must be PCM or IEEE floating-point." , nameof(desiredformat));
            }

            // Set default data for the two properties, and create the appropriate IMFActivate.
            resamplingquality = 40; // max 60 , minimum 1
            lpfbandwidth = 1.0f; // max 1.0f , minimum 0.0f.
            // Attempt to create it's IMFActivate.
            // If the IMFActivate of the resampler does not exist , throw an exception.
            TryCreateActivateObject();
        }

        private void TryCreateActivateObject()
        {
            if (activateobject is not null) { return; }
            // The Resampler belongs into the Audio Effect category.
            Guid resamplerclsid = new(MediaFoundationInterfaceIds.CLSID_RESAMPLEDMO);
            foreach (var ac in MediaFoundationInterfacesFactory.EnumerateMFTs(
                new(0x11064c48, 0x3648, 0x4ed0, 0x93, 0x2e, 0x05, 0xce, 0x8a, 0xc8, 0x11, 0xb7))) 
            {
                // If is the resampler and the resampler was not previously found...
                if (activateobject is null && ac.GetAttribute<Guid>(IMFActivateAttributes.MFT_TRANSFORM_CLSID_Attribute) == resamplerclsid) {
                    // Save the object to the field and continue iteration, to dispose all the rest IMFActivates.
                    activateobject = ac;
                    continue;
                }
                ComMarshalling.ReleaseInteropObject(ac);
            }
            if (activateobject is null) {
                throw new NotSupportedException("RESAMPLEDMO.DLL is not installed in the system. It is required for playback.");
            }
        }

        /// <summary>
        /// Gets or sets the Resampler quality. <br />
        /// The desired resampling quality can only be effectively set before the first <see cref="MediaFoundationTransform.Read"/> is performed. <br />
        /// 1 is lowest quality (linear interpolation) and 60 is best quality
        /// </summary>
        public System.Int32 ResamplingQuality
        {
            get => resamplingquality;
            set
            {
                if (value < 1 || value > 60) {
                    throw new ArgumentOutOfRangeException(nameof(value) , "Resampler Quality must be between 1 and 60");
                }
                resamplingquality = value;
            }
        }

        /// <summary>
        /// Gets or sets the Resampler low-pass filter bandwidth. <br />
        /// The desired bandwidth can only be effectively set before the first <see cref="MediaFoundationTransform.Read"/> is performed. <br />
        /// 0.0 in float format is the minimum value and 1.0 is the maximum value.
        /// </summary>
        public System.Single LowPassBandwidth
        {
            get => lpfbandwidth;
            set {
                if (value < 0.0f || value > 1.0f) {
                    throw new ArgumentOutOfRangeException(nameof(value), "Resampler low-pass filter bandwidth must be a value between 0.0 and 1.0.");
                }
                lpfbandwidth = value;
            }
        }

        protected unsafe override IMFTransform CreateTransform()
        {
            // Create the Resampler object through IMFActivate, and configure the resampler
            System.Object obj;
            GUID imftransformid = GUID.FromString(MediaFoundationInterfaceIds.IID_IMFTransform);
            void* p;
            activateobject.ActivateObject(&imftransformid, &p).ThrowOnFailure();
            obj = ComMarshalling.CreateInteropObject(p , -1);

            // If we reached here, we have our MFT... Configure it.
            IMFTransform finalMFT = obj as IMFTransform;

            // Configure I/O media formats
            MediaFoundationMediaType mt = MediaFoundationMediaType.FromAudioFormat(OriginalProvider.Format);
            finalMFT.SetInputType(0, mt.NativeMediaType);
            mt.Dispose();

            mt = MediaFoundationMediaType.FromAudioFormat(Format);
            finalMFT.SetOutputType(0 , mt.NativeMediaType);
            mt.Dispose();

            // Retrieve an IPropertyStore object to modify low-pass bandwidth
            // and resampling quality

            IPropertyStore ps = obj as IPropertyStore;

            Guid resamplerguidk = new(0xaf1adc73, 0xa210, 0x4b05, 0x96, 0x6e, 0x54, 0x91, 0xcf, 0xf4, 0x8b, 0x1d);

            // Configure resampling quality first
            PROPERTYKEY pk = new(resamplerguidk , 0x01); // MFPKEY_WMRESAMP_FILTERQUALITY
            PROPVARIANT pvtemp = PROPVARIANT.FromInt(resamplingquality);

            ps.SetValue(&pk, &pvtemp); // Will always succeed.

            // Configure low-pass filter bandwidth
            pk = new(resamplerguidk , 0x03); // MFPKEY_WMRESAMP_LOWPASS_BANDWIDTH
            pvtemp = PROPVARIANT.FromSingle(lpfbandwidth);

            ps.SetValue(&pk, &pvtemp); // Will always succeed.
            // Destroy the queried object
            ps = null;

            // Note: Through IPropertyStore we can also set the channel mapping matrix, 
            // but channel mappings are already done through the set media types in the MFT side of the resampler.

            return finalMFT;
        }

        protected override void Dispose(bool disposing)
        {
            try {
                // First dispose the transform and the provider
                base.Dispose(disposing); 
            } finally {
                // In either success of failure, we must be sure that the IMFActivate will be destroyed
                if (disposing && activateobject is not null)
                {
                    activateobject.ShutdownObject(); // Now destroy the transform for sure
                    ComMarshalling.ReleaseInteropObject(activateobject); // and release the IMFActivate itself...
                }
            }
        }
    }
}
