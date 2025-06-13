using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MP.GamepadBackend
{
    public static class GamepadManager
    {
        private static System.UInt32 reqs;
        private static System.Boolean run;
        private static GamepadFeatures features;
        private static List<System.UInt32> connected;

        private static void GamePadRemovedDummyFunction(System.Object obj, System.UInt32 idx) { }

        private static void GamePadAddedDummyFunction(System.Object obj, GamepadReader rdr) { }

        static GamepadManager() 
        {
            reqs = 0;
            connected = new(4);
            GamepadRemoved = new(GamePadRemovedDummyFunction);
            GamepadAdded = new(GamePadAddedDummyFunction);
            run = false;
            features = null;
        }

        /// <summary>
        /// This event informs the user whether a new gamepad is connected to the session or not.
        /// </summary>
        public static event System.EventHandler<GamepadReader> GamepadAdded;

        /// <summary>
        /// This event informs the user whether a gamepad was disconnected.
        /// </summary>
        public static event System.EventHandler<System.UInt32> GamepadRemoved;

        public static void RequestGamepads(System.Int32 count , GamepadFeatures requiredfeatures)
        {
            if (run) { return; }
            if (count < 1 || count > 4) { throw new ArgumentOutOfRangeException(nameof(count) , "There can be only requested 4 gamepads each time."); }
            requiredfeatures ??= new();
            features = requiredfeatures;
            reqs = count.ToUInt32();
            BackgroundThread();
        }

        public static void StopRequestingGamepads() => run = false;

        public static void ResumeRequestingGamepads() => BackgroundThread();

        private static void BackgroundThreadCode()
        {
            run = true;
            Interop.XInput.XINPUT_CAPABILITIES caps;
            while (run)
            {
                System.UInt32 I = 0, erc;
                System.Boolean support = true;
                while (I < Interop.XInput.XUSER_MAX_COUNT)
                {
                    support = true;
                    erc = Interop.XInput.XInputGetCapabilities(I, Interop.XInput.XINPUT_FLAG_GAMEPAD, out caps);
                    if (connected.Contains(I)) { goto G_detachedch; }
                    if (erc == Interop.Errors.ERROR_SUCCESS)
                    {
                        if (features is null) { GamepadAdded.Invoke(null, new(I)); reqs--; connected.Add(I); }
                        if (features.FFB)
                        {
                            support |= caps.Flags.HasFlag(Interop.XInput.XInputDeviceCapabilities.FFB_SUPPORTED);
                        }
                        if (features.Navigation)
                        {
                            support |= caps.Flags.HasFlag(Interop.XInput.XInputDeviceCapabilities.NO_NAVIGATION) == false;
                        }
                        if (features.VoiceDeviceSupport)
                        {
                            support |= caps.Flags.HasFlag(Interop.XInput.XInputDeviceCapabilities.VOICE_SUPPORTED);
                        }
                        if (features.PMD)
                        {
                            support |= caps.Flags.HasFlag(Interop.XInput.XInputDeviceCapabilities.PMD_SUPPORTED);
                        }
                        if (support) { GamepadAdded.Invoke(null, new(I)); reqs--; connected.Add(I); }
                    }
                G_detachedch:
                    if (erc == Interop.Errors.ERROR_DEVICE_NOT_CONNECTED && connected.Contains(I))
                    {
                        GamepadRemoved.Invoke(null, I);
                        reqs++;
                        connected.Remove(I);
                    }
                    I++;
                }
                System.Threading.Thread.Sleep(2000);
            }
        }

        private static void BackgroundThread()
        {
            var back = new System.Threading.Thread(BackgroundThreadCode);
            back.TrySetApartmentState(System.Threading.ApartmentState.STA);
            back.Name = InternalResources.MP_GAMEPAD_MANAGER_REQTHREADNAME;
            back.Priority = System.Threading.ThreadPriority.Lowest;
            back.IsBackground = true;
            back.Start();
        }

        public static IEnumerable<GamepadReader> Gamepads
        {
            get
            {
                System.UInt32 I = 0, erc;
                while (I < Interop.XInput.XUSER_MAX_COUNT)
                {
                    erc = Interop.XInput.XInputGetCapabilities(I, Interop.XInput.XINPUT_FLAG_GAMEPAD, out _);
                    if (erc == Interop.Errors.ERROR_SUCCESS) { yield return new(I); }
                    I++;
                }
            }
        }
    }
}
