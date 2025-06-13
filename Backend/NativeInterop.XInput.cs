using MP;
using System;
using System.Runtime.InteropServices;

partial class Interop
{
    public static unsafe class XInput
    {
        public const System.Int32 XUSER_MAX_COUNT = 4;
        // INDEX_ANY is a bit dangerous since it would select any gamepad it finds. Do not use it in real life.
        public const System.Int32 XUSER_INDEX_ANY = 0x000000FF;
        public const System.UInt32 XINPUT_FLAG_GAMEPAD = 0x00000001;

        // The prefix XINPUT_DEVTYPE_ is omitted for brevity.
        public enum XInputDeviceType : System.Byte
        {
            GAMEPAD = 0x01
        }

        // The prefix BATTERY_DEVTYPE_ is omitted for brevity.
        public enum XInputBatteryDeviceType : System.Byte 
        {
            GAMEPAD = 0x00,
            HEADSET = 0x01,
        }

        // The prefix BATTERY_TYPE_ is omitted for brevity.
        public enum XInputBatteryType : System.Byte
        {
            /// <summary>This device is not connected</summary>
            DISCONNECTED = 0x00,
            /// <summary>Wired device, no battery</summary>
            WIRED = 0x01,
            /// <summary>Alkaline battery source</summary>
            ALKALINE = 0x02,
            /// <summary>Nickel Metal Hydride battery source</summary>
            NIMH = 0x03,
            /// <summary>Cannot determine the battery type</summary>
            UNKNOWN = 0xFF 
        }

        // The prefix XINPUT_KEYSTROKE_ is omitted for brevity.
        [Flags]
        public enum XInputKeystrokeType : System.UInt16
        {
            KEYDOWN = 0x0001,
            KEYUP = 0x0002,
            REPEAT = 0x0004,
        }

        /// <summary>
        /// These are only valid for wireless, connected devices, with known battery types <br />
        /// The amount of use time remaining depends on the type of device.
        /// </summary>
        // The prefix BATTERY_LEVEL_ is omitted for brevity.
        public enum XInputBatteryLevel : System.Byte
        {
            EMPTY = 0x00,
            LOW = 0x01,
            MEDIUM = 0x02,
            FULL = 0x03
        }

        // The prefix XINPUT_DEVSUBTYPE_ is omitted for brevity.
        public enum XInputDeviceSubType : System.Byte 
        {
            UNKNOWN = 0x00,
            GAMEPAD = 0x01,
            WHEEL = 0x02,
            ARCADE_STICK = 0x03,
            FLIGHT_STICK = 0x04,
            DANCE_PAD = 0x05,
            GUITAR = 0x06,
            GUITAR_ALTERNATE = 0x07,
            DRUM_KIT = 0x08,
            GUITAR_BASS = 0x0B,
            ARCADE_PAD = 0x13
        }

        // The prefix XINPUT_CAPS_ is omitted for brevity.
        [Flags] // One or more capabilities may be set , support this case.
        public enum XInputDeviceCapabilities : System.UInt16
        {
            /// <summary>
            /// FFB features are supported for this device. Most features of it are not implemented on Windows.
            /// </summary>
            FFB_SUPPORTED = 0x0001,
            /// <summary>The device is connected with wire-less methods , such as Bluetooth.</summary>
            WIRELESS = 0x0002,
            /// <summary>The device has an integrated voice device.</summary>
            VOICE_SUPPORTED = 0x0004,
            /// <summary>
            /// PMD features are supported for this device. Most features of it are not implemented on Windows.
            /// </summary>
            PMD_SUPPORTED = 0x0008,
            /// <summary>
            /// Special capability that defines that the gamepad does not support the navigation buttons.
            /// </summary>
            NO_NAVIGATION = 0x0010
        }

        // The prefix XINPUT_GAMEPAD_ is omitted for brevity.
        [Flags] // One or more controller buttons may have been pressed , support this case.
        public enum XInputGamepadButtons : System.UInt16
        {
            DPAD_UP = 0x0001,
            DPAD_DOWN = 0x0002,
            DPAD_LEFT = 0x0004,
            DPAD_RIGHT = 0x0008,
            START = 0x0010,
            BACK = 0x0020,
            LEFT_THUMB = 0x0040,
            RIGHT_THUMB = 0x0080,
            LEFT_SHOULDER = 0x0100,
            RIGHT_SHOULDER = 0x0200,
            A = 0x1000,
            B = 0x2000,
            X = 0x4000,
            Y = 0x8000
        }

        [StructLayout(LayoutKind.Explicit , Size = 12)]
        public struct XINPUT_GAMEPAD
        {
            [FieldOffset(0)]
            public XInputGamepadButtons Buttons;

            [FieldOffset(2)]
            public System.Byte LeftTrigger;

            [FieldOffset(3)]
            public System.Byte RightTrigger;

            [FieldOffset(4)]
            public System.Int16 LeftThumbstickX;

            [FieldOffset(6)]
            public System.Int16 LeftThumbstickY;

            [FieldOffset(8)]
            public System.Int16 RightThumbstickX;

            [FieldOffset(10)]
            public System.Int16 RightThumbstickY;
        }

        [StructLayout(LayoutKind.Explicit , Size = 16)]
        public struct XINPUT_STATE
        {
            [FieldOffset(0)]
            public System.UInt32 PacketNumber;

            [FieldOffset(4)]
            public XINPUT_GAMEPAD Gamepad;
        }

        [StructLayout(LayoutKind.Explicit , Size = 4 , Pack = 2)]
        public struct XINPUT_VIBRATION
        {
            [FieldOffset(0)]
            public System.UInt16 LeftMotorSpeed;

            [FieldOffset(2)]
            public System.UInt16 RightMotorSpeed;
        }

        [StructLayout(LayoutKind.Explicit , Size = 20)]
        public struct XINPUT_CAPABILITIES
        {
            [FieldOffset(0)]
            public XInputDeviceType Type;

            [FieldOffset(1)]
            public XInputDeviceSubType SubType;

            [FieldOffset(2)]
            public XInputDeviceCapabilities Flags;

            [FieldOffset(4)]
            public XINPUT_GAMEPAD Gamepad;

            [FieldOffset(16)]
            public XINPUT_VIBRATION Vibration;
        }

        [StructLayout(LayoutKind.Explicit , Size = 2 , Pack = 1)]
        public struct XINPUT_BATTERY_INFORMATION
        {
            [FieldOffset(0)]
            public XInputBatteryType BatteryType;

            [FieldOffset(1)]
            public XInputBatteryLevel BatteryLevel;
        }

        [StructLayout(LayoutKind.Explicit , Size = 8)]
        public struct XINPUT_KEYSTROKE
        {
            [FieldOffset(0)]
            public System.UInt16 VirtualKey;

            [FieldOffset(2)]
            public System.Char UnicodeNotUsed;

            [FieldOffset(4)]
            public XInputKeystrokeType Flags;

            [FieldOffset(6)]
            public System.Byte UserIndex;

            [FieldOffset(7)]
            public System.Byte HIDCode;
        }

        [DllImport(Libraries.XInput , EntryPoint = "XInputGetState")]
        private static extern System.UInt32 XInputGetState_Native(System.UInt32 UserIndex, XINPUT_STATE* state);

        [DllImport(Libraries.XInput , EntryPoint = "XInputSetState")]
        private static extern System.UInt32 XInputSetState_Native(System.UInt32 UserIndex, XINPUT_VIBRATION* vibration);
        
        [DllImport(Libraries.XInput , EntryPoint = "XInputGetCapabilities")]
        private static extern System.UInt32 XInputGetCapabilities_Native(System.UInt32 UserIndex, System.UInt32 DeviceType, XINPUT_CAPABILITIES* caps);

        // Note that this method will return a valid Win32 error code if the error code is not the one that is documented!
        [DllImport(Libraries.XInput , EntryPoint = "XInputGetAudioDeviceIds")]
        private static extern System.UInt32 XInputGetAudioDeviceIds_Native(
            System.UInt32 UserIndex, 
            System.Char* ptrrenderdevid, 
            System.UInt32* sizerenderdevid,  
            System.Char* ptrcapturedevid,
            System.UInt32* sizecapturedevid);

        [DllImport(Libraries.XInput , EntryPoint = "XInputGetBatteryInformation")]
        private static extern System.UInt32 XInputGetBatteryInformation_Native(System.UInt32 UserIndex, XInputBatteryDeviceType Type, XINPUT_BATTERY_INFORMATION* bi);

        [DllImport(Libraries.XInput , EntryPoint = "XInputGetKeystroke")]
        private static extern System.UInt32 XInputGetKeystroke_Native(System.UInt32 UserIndex, System.UInt32 RSVD, XINPUT_KEYSTROKE* keystroke);

        public static System.UInt32 XInputGetState(System.UInt32 UserIndex , out XINPUT_STATE state)
        {
            state = new();
            fixed (XINPUT_STATE* ptr = &state) {
                return XInputGetState_Native(UserIndex, ptr);
            }
        }

        public static System.UInt32 XInputSetState(System.UInt32 UserIndex , XINPUT_VIBRATION vibration) 
            => XInputSetState_Native(UserIndex, &vibration);

        public static System.UInt32 XInputGetCapabilities(System.UInt32 UserIndex , System.UInt32 DeviceType , out XINPUT_CAPABILITIES caps)
        {
            caps = new();
            fixed (XINPUT_CAPABILITIES* ptr = &caps) {
                return XInputGetCapabilities_Native(UserIndex, DeviceType, ptr);
            }
        }

        public static System.UInt32 XInputGetAudioDeviceIds(System.UInt32 UserIndex , out System.String renderid , out System.String captureid)
        {
            renderid = null;
            captureid = null;
            System.UInt32 rendersize = 500 , capturesize = 500 , ret = 0;
            System.Char[] renderbuf = new System.Char[rendersize] , 
                capturebuf = new System.Char[capturesize];
            fixed (System.Char* capture = capturebuf) 
            {
                fixed (System.Char* render = renderbuf)
                {
                    ret = XInputGetAudioDeviceIds_Native(UserIndex, render, &rendersize, capture, &capturesize);
                }
            }
            if (ret == Errors.ERROR_SUCCESS) {
                renderid = new(renderbuf, 0, rendersize.ToInt32());
                captureid = new(capturebuf , 0 , capturesize.ToInt32());
            }
            // Clean temp buffers
            renderbuf = null;
            capturebuf = null;
            return ret;
        }

        public static System.UInt32 XInputGetBatteryInformation(System.UInt32 UserIndex , XInputBatteryDeviceType Type , out XINPUT_BATTERY_INFORMATION batteryinfo)
        {
            batteryinfo = new();
            fixed (XINPUT_BATTERY_INFORMATION* info = &batteryinfo)
            {
                return XInputGetBatteryInformation_Native(UserIndex, Type, info);
            }
        }

        public static System.UInt32 XInputGetKeystroke(System.UInt32 UserIndex , out XINPUT_KEYSTROKE keystroke)
        {
            keystroke = new();
            fixed (XINPUT_KEYSTROKE* info = &keystroke) {
                return XInputGetKeystroke_Native(UserIndex , 0 , info);
            }
        }
    }
}