
using System;
using MP.ComInterop;
using System.Runtime.InteropServices;
using MP.WindowsInterop;

namespace MP.AudioLibrary.WASAPI
{
    /// <summary>
    /// <see cref="IChannelAudioVolume"/> class implementation. <br />
    /// Note that it requires a shared audio session so that this can actually work.
    /// </summary>
    public unsafe sealed class ChannelAudioVolume : IDisposable
    {
        private IChannelAudioVolume channelAudioVolume;

        internal ChannelAudioVolume(IChannelAudioVolume channelAudioVolume)
        {
            if (channelAudioVolume is null)
            {
                throw new ArgumentNullException(nameof(channelAudioVolume));
            }
            this.channelAudioVolume = channelAudioVolume;
        }

        public System.Int32 ChannelCount
        {
            get {
                System.UInt32 numberofchannels;
                channelAudioVolume.GetChannelCount(&numberofchannels).ThrowOnFailure();
                return numberofchannels.ToInt32();
            }
        }

        public System.Single GetChannelVolume(System.Int32 channelindex)
        {
            if (channelindex < 0) { throw new ArgumentOutOfRangeException(nameof(channelindex) , "Channel index must not be negative."); }
            if (channelindex >= ChannelCount) { throw new ArgumentOutOfRangeException(nameof(channelindex) , "Channel index must not be larger than the channel count."); }
            System.Single ret;
            channelAudioVolume.GetChannelVolume(channelindex.ToUInt32(), &ret).ThrowOnFailure();
            return ret;
        }

        public void SetChannelVolume(System.Int32 channelindex, System.Single volume) => SetChannelVolume(channelindex , volume , Guid.Empty);

        public void SetChannelVolume(System.Int32 channelindex, System.Single volume , System.Guid eventcontextguid)
        {
            if (channelindex < 0) { throw new ArgumentOutOfRangeException(nameof(channelindex), "Channel index must not be negative."); }
            if (channelindex >= ChannelCount) { throw new ArgumentOutOfRangeException(nameof(channelindex), "Channel index must not be larger than the channel count."); }
            if (volume < 0.0 && volume > 1.0) { throw new ArgumentOutOfRangeException("Volume must be in a range of 0 to 1." , nameof(volume)); }
            GUID guid = GUID.FromGUID(eventcontextguid);
            channelAudioVolume.SetChannelVolume(channelindex.ToUInt32() , volume , &guid).ThrowOnFailure();
        }

        public System.Single[] GetChannelVolumes()
        {
            System.UInt32 ctc = ChannelCount.ToUInt32();
            System.Single[] channelvolumes = new System.Single[ctc];
            HRESULT hr;
            fixed (System.Single* native = channelvolumes)
            {
                hr = channelAudioVolume.GetAllVolumes(ctc, native);
            }
            hr.ThrowOnFailure();
            return channelvolumes;
        }

        public void SetChannelVolumes(System.Single[] volumes , System.Guid eventcontextguid)
        {
            if (volumes is null) { throw new ArgumentNullException(nameof(volumes)); }
            System.Int32 cc = ChannelCount;
            if (volumes.Length != cc) { throw new ArgumentException($"The array length must be exactly {cc}, while found that it was {volumes.Length}.", nameof(volumes)); }
            HRESULT hr;
            fixed (System.Single* native = volumes)
            {
                var guid = GUID.FromGUID(eventcontextguid);
                hr = channelAudioVolume.SetAllVolumes(cc.ToUInt32(), native, &guid);
            }
            hr.ThrowOnFailure();
        }

        public void SetChannelVolumes(System.Single[] volumes) => SetChannelVolumes(volumes , Guid.Empty);

        /// <summary>
        /// Disposes this <see cref="ChannelAudioVolume"/> instance. <br />
        /// Note that you must call <see cref="Dispose"/> from the same thread that you created it.
        /// </summary>
        public void Dispose() 
        {
            // If the object is freed, there is no need to check for thread validity.
            if (channelAudioVolume is not null) 
            {
                ComMarshalling.ReleaseInteropObject(channelAudioVolume);
                channelAudioVolume = null;
            }
            GC.SuppressFinalize(this);
        }
    }
}