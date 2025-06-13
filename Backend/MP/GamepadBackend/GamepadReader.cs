using System;
using System.Runtime.CompilerServices;

namespace MP.GamepadBackend
{
    public sealed class GamepadReader : IDisposable , ICloneable
    {
        private System.UInt32 userindex;
        private System.Boolean run, running , vibrationrun , vbenabled;
        private System.UInt16 delay;
        private GamepadFeatures features;
        private System.Int16 trigprecision, thumbprecision;
        private System.Threading.Thread backthread;

        private static void GamepadActionDummyFunction(System.Object obj, GamepadEventEventArgs e) { }

        internal GamepadReader(System.UInt32 index)
        {
            userindex = index;
            run = false;
            running = false;
            vibrationrun = false;
            vbenabled = false;
            delay = 120;
            trigprecision = 10;
            thumbprecision = 1500;
            GamepadAction = new(GamepadActionDummyFunction);
            if (MarshalException(Interop.XInput.XInputGetCapabilities(userindex, Interop.XInput.XINPUT_FLAG_GAMEPAD, out var caps)) == false)
            {
                throw new ExceptionSystem.GamepadDisconnectedException("The controller is disconnected.");
            }
            features = new() {
                FFB = caps.Flags.HasFlag(Interop.XInput.XInputDeviceCapabilities.FFB_SUPPORTED),
                PMD = caps.Flags.HasFlag(Interop.XInput.XInputDeviceCapabilities.PMD_SUPPORTED),
                Navigation = caps.Flags.HasFlag(Interop.XInput.XInputDeviceCapabilities.NO_NAVIGATION) == false,
                VoiceDeviceSupport = caps.Flags.HasFlag(Interop.XInput.XInputDeviceCapabilities.VOICE_SUPPORTED)
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static System.Int16 FastAbs(System.Int16 value) => (value < 0 ? -value : value).ToInt16();

        private void BackgroundThread_RunCode()
        {
            Interop.XInput.XINPUT_STATE state;
            running = true;
            while (run)
            {
                if (Interop.XInput.XInputGetState(userindex, out state) == Interop.Errors.ERROR_DEVICE_NOT_CONNECTED)
                {
                    break;
                }
                if (state.Gamepad.Buttons != 0) { InvokeButtonAction(state.Gamepad.Buttons); }
                if (FastAbs(state.Gamepad.LeftTrigger) >= trigprecision) { InvokeTriggerAction(false, state.Gamepad.LeftTrigger); }
                if (FastAbs(state.Gamepad.RightTrigger) >= trigprecision) { InvokeTriggerAction(true, state.Gamepad.RightTrigger); }
                if (FastAbs(state.Gamepad.LeftThumbstickX) >= thumbprecision ||
                    FastAbs(state.Gamepad.LeftThumbstickY) >= thumbprecision) { InvokeThumbstickAction(false, state.Gamepad.LeftThumbstickX, state.Gamepad.LeftThumbstickY); }
                if (FastAbs(state.Gamepad.RightThumbstickX) >= thumbprecision ||
                    FastAbs(state.Gamepad.RightThumbstickY) >= thumbprecision) { InvokeThumbstickAction(true, state.Gamepad.RightThumbstickX, state.Gamepad.RightThumbstickY); }
                System.Threading.Thread.Sleep(delay);
            }
            running = false;
        }

        private void BackgroundThread()
        {
            if (backthread is not null && backthread.ThreadState == System.Threading.ThreadState.Running) { return; }
            backthread = new(BackgroundThread_RunCode);
            backthread.TrySetApartmentState(System.Threading.ApartmentState.STA);
            backthread.Priority = System.Threading.ThreadPriority.Lowest;
            backthread.Name = "[MP-XINPUT] Gamepad Events Listener";
            backthread.IsBackground = true;
            backthread.Start();
        }

        // true means that the native call is successfull.
        [System.Diagnostics.DebuggerHidden]
        private System.Boolean MarshalException(System.UInt32 errorcode)
        {
            switch (errorcode)
            {
                case Interop.Errors.ERROR_SUCCESS: // Success , no need to throw any exceptions.
                    return true;
                case Interop.Errors.ERROR_DEVICE_NOT_CONNECTED: // For disconnected devices , just terminate the thread.
                    run = false;
                    return false;
                default:
                    throw new ExceptionSystem.NativeWindowsException(errorcode.ToInt32());
            }
        }

        private void InvokeThumbstickAction(System.Boolean right, System.Int16 x, System.Int16 y)
            => GamepadAction.Invoke(this, new() {
                Type = right ? GamepadMode.RightThumbstick : GamepadMode.LeftThumbstick,
                X = x,
                Y = y,
            });

        private void InvokeTriggerAction(System.Boolean right, System.Int16 val)
            => GamepadAction.Invoke(this, new() {
                Type = right ? GamepadMode.RightTrigger : GamepadMode.LeftTrigger,
                X = val
            });

        private void InvokeButtonAction(Interop.XInput.XInputGamepadButtons bt) => GamepadAction.Invoke(this, new() { Type = GamepadMode.Button, 
            Button = bt switch {
                Interop.XInput.XInputGamepadButtons.B => GamepadButton.B,
                Interop.XInput.XInputGamepadButtons.A => GamepadButton.A,
                Interop.XInput.XInputGamepadButtons.X => GamepadButton.X,
                Interop.XInput.XInputGamepadButtons.Y => GamepadButton.Y,
                Interop.XInput.XInputGamepadButtons.START => GamepadButton.Menu,
                Interop.XInput.XInputGamepadButtons.BACK => GamepadButton.View,
                Interop.XInput.XInputGamepadButtons.LEFT_SHOULDER => GamepadButton.LeftShoulder,
                Interop.XInput.XInputGamepadButtons.RIGHT_SHOULDER => GamepadButton.RightShoulder,
                Interop.XInput.XInputGamepadButtons.LEFT_THUMB => GamepadButton.LeftThumbstick,
                Interop.XInput.XInputGamepadButtons.RIGHT_THUMB => GamepadButton.RightThumbstick,
                Interop.XInput.XInputGamepadButtons.DPAD_DOWN => GamepadButton.DPadDown,
                Interop.XInput.XInputGamepadButtons.DPAD_LEFT => GamepadButton.DPadLeft,
                Interop.XInput.XInputGamepadButtons.DPAD_RIGHT => GamepadButton.DPadRight,
                Interop.XInput.XInputGamepadButtons.DPAD_UP => GamepadButton.DPadUp,
                _ => 0
            }});

        /// <summary>
        /// When an action was read through the gamepad , this event is fired and provides the information.
        /// </summary>
        public event System.EventHandler<GamepadEventEventArgs> GamepadAction;

        /// <summary>
        /// Controls whether this instance should listen to gamepad events or not. <br />
        /// Setting this to false the instance stops listening to any incoming events.
        /// </summary>
        public System.Boolean Listen
        {
            get => run;
            set { if (run = value) { BackgroundThread(); } }
        }

        /// <summary>
        /// Gets a value whether this instance is listening to gamepad events.
        /// </summary>
        public System.Boolean Running => running;

        /// <summary>
        /// Gets or sets a value whether the <see cref="Vibrate(GamepadVibrationTimeLine)"/> and <see cref="VibrateWithTimeline(GamepadVibrationTimeLine[])"/>
        /// invocations are applied to the current controller. <br />
        /// By default this property is <see langword="false"/>.
        /// </summary>
        public System.Boolean VibrationsEnabled
        {
            get => vbenabled;
            set => vbenabled = value;
        }

        /// <summary>
        /// Gets a value whether this instance is also currently handling vibration events.
        /// </summary>
        public System.Boolean VibrationsRunning => vibrationrun;

        /// <summary>
        /// Gets or sets a precision value that is the threshold before a new thumbstick event is fired on the <see cref="GamepadAction"/> event.
        /// </summary>
        public System.Int16 ThumbstickEventPrecision { get => thumbprecision; set => thumbprecision = value; }

        /// <summary>
        /// Gets or sets a precision value that is the threshold before a new trigger event is fired on the <see cref="GamepadAction"/> event.
        /// </summary>
        public System.Int16 TriggerEventPrecision { get => trigprecision; set => trigprecision = value; }

        /// <summary>
        /// Defines the data refresh delay between iterations. This value does define the resposiveness of the app from the controller requests. <br />
        /// The value given here is time interval counted in milliseconds.
        /// </summary>
        public System.UInt16 UpdateDelay 
        { 
            get => delay; 
            set { 
                if (delay < 50) { delay = 180; } 
                delay = value; 
            } 
        }

        /// <summary>
        /// Gets the associated XINPUT user index that this controller is being currently referenced.
        /// </summary>
        public System.UInt32 UserIndex => userindex;

        /// <summary>
        /// Gets the controller or the headset battery information. <br />
        /// By setting the <paramref name="headset"/> parameter to <see langword="true"/> the 
        /// headset information is attempted to be retrieved.
        /// </summary>
        /// <param name="headset">A value whether to retrieve information for the headset or for the controller.</param>
        /// <returns>The given battery info queried for this object.</returns>
        /// <exception cref="ExceptionSystem.GamepadDisconnectedException">The controller or the headset are disconnected.</exception>
        public GamepadBatteryInfo GetBatteryInfo(System.Boolean headset)
        {
            if (Interop.XInput.XInputGetBatteryInformation(userindex, 
                headset ? Interop.XInput.XInputBatteryDeviceType.HEADSET : 
                Interop.XInput.XInputBatteryDeviceType.GAMEPAD , out var bi) == Interop.Errors.ERROR_DEVICE_NOT_CONNECTED)
            {
                throw new ExceptionSystem.GamepadDisconnectedException(headset ? "The headset is not connected." : "The controller is disconnected.");
            }
            return new(bi);
        }

        /// <summary>
        /// Gets the Windows CoreAudio device ID's for the render and the capture device of the connected headset.
        /// </summary>
        /// <param name="renderid">Gets the render device id.</param>
        /// <param name="captureid">Gets the capture device id.</param>
        /// <returns>A value whether the method succeeded or not. If not succeeded , it means that a controller headset is not connected.</returns>
        /// <remarks>
        /// You should not depend on the return value of this method to ensure that the devices are existing. <br />
        /// Instead , check also whether the <paramref name="captureid"/> and <paramref name="renderid"/> parameters
        /// are null or empty.
        /// </remarks>
        public System.Boolean TryGetAudioInterfaceIds(out System.String renderid, out System.String captureid)
            => Interop.XInput.XInputGetAudioDeviceIds(userindex, out renderid, out captureid) == Interop.Errors.ERROR_SUCCESS;

        /// <summary>
        /// Gets the headset's output device as a new <see cref="AudioBackend.CoreAudioApi.MMDevice"/> instance. <br />
        /// Returns null when the XInput call fails.
        /// </summary>
        public AudioLibrary.MMDevice.MMDevice GetHeadsetOutputDevice()
        {
            AudioLibrary.MMDevice.MMDeviceEnumerator enumerator = null;
            try
            {
                enumerator = new();
                if (TryGetAudioInterfaceIds(out var renderid, out var _) && System.String.IsNullOrEmpty(renderid) == false)
                {
                    return enumerator.GetDevice(renderid);
                }
                return null;
            } finally {
                enumerator?.Dispose();
                enumerator = null;
            }
        }

        /// <summary>
        /// Gets the gamepad features that are supported in this session.
        /// </summary>
        public GamepadFeatures Features => features;

        /// <summary>
        /// Vibrates the current controller. The vibration duration and individual motor speeds
        /// are set through the provided <see cref="GamepadVibrationTimeLine"/> instance. <br /> <br />
        /// Passing <see langword="null"/> to <paramref name="vbdata"/> parameter causes the vibration scheme
        /// to be reset (that is no vibration actually).
        /// </summary>
        /// <param name="vbdata">The instance that holds the vibration information required.</param>
        public void Vibrate(GamepadVibrationTimeLine vbdata) => VibrationTimeLineThreadCode([vbdata]);

        /// <summary>
        /// Vibrates the current controller by using the specified array of controller vibrations. <br />
        /// The vibration data are applied the one after the another , allowing you to create vibration patterns.
        /// </summary>
        /// <param name="vbdata">The vibrations to execute the one after the other.</param>
        public void VibrateWithTimeline(params GamepadVibrationTimeLine[] vbdata) => VibrationTimeLineThreadCode(vbdata);

        private void VibrationTimeLineThreadCode(GamepadVibrationTimeLine[] data)
        {
            if (vbenabled == false) { return; }
            if (data is not null && data.Length == 1 && data[0] is null) {
                Interop.XInput.XInputSetState(userindex, new() { LeftMotorSpeed = 0, RightMotorSpeed = 0 });
                return;
            }
            var td = new System.Threading.Thread((System.Object obj) => {
                if (obj is not GamepadVibrationTimeLine[] arr) { return; }
                vibrationrun = true;
                System.Int32 I = 0;
                GamepadVibrationTimeLine temp;
                while (running && vbenabled && I < arr.Length)
                {
                    if ((temp = arr[I]) is not null) {
                        Interop.XInput.XInputSetState(userindex , new() { LeftMotorSpeed = temp.LeftMotorSpeed , RightMotorSpeed = temp.RightMotorSpeed });
                        System.Threading.Thread.Sleep(temp.Duration);
                        temp = null;
                    }
                    I++;
                }
                vibrationrun = false;
                if (running) {
                    Interop.XInput.XInputSetState(userindex, new() { LeftMotorSpeed = 0, RightMotorSpeed = 0 });
                }
            });
            td.Priority = System.Threading.ThreadPriority.Lowest;
            td.Name = "[MP] Vibration Timeline worker thread";
            td.IsBackground = true;
            td.Start(data);
        }

        public GamepadReader Clone()
        {
            try { return new(userindex); } catch (ExceptionSystem.GamepadDisconnectedException) { return null; }
        }

        System.Object ICloneable.Clone() => Clone();

        public override string ToString() => $"GamepadReader*XInput: {{ Running={Running} , UpdateDelay={UpdateDelay} , TriggerEventPrecision={TriggerEventPrecision} , ThumbstickEventPrecision={ThumbstickEventPrecision} , UserIndex={UserIndex} , VibrationsEnabled={VibrationsEnabled} }}";

        public void Dispose()
        {
            run = false;
            if (backthread is not null)
            {
                backthread.Join(3000);
                backthread = null;
            }
            features = null;
        }
    }
}
