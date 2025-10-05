

/* 
 * CROSS-PLATFORM CROSS-PLATFORM CROSS-PLATFORM CROSS-PLATFORM
 
    This source file is cross-platform because GLFW is supported for a multiple of platforms.

    For new OS backends, this file must be copied to them as-is.

    Changes happening to this file should be ported to the other backends as well.

    DO NOT IMPORT MP namespaces!!

    If you wish to use them, just fully qualify the name instead.

 */

using System;
using System.Threading;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

partial class Interop
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "GLFW interop signatures")]
    public unsafe static class GLFW
    {
        private static GLFWkeyfun currentkeyfunction;
        private static GLFWerrorfun currenterrorfunction;
        private static GLFWmonitorfun currentmonitorfunction;
        private static GLFWjoystickfun currentjoystickfunction;
        private static GLFWscrollfun currentmousescrollfunction;
        private static GLFWwindowposfun currentwindowposfunction;
        private static GLFWwindowsizefun currentwindowsizefunction;
        private static GLFWwindowclosefun currentwindowclosefunction;
        private static GLFWmousebuttonfun currentmousebuttonfunction;
        private static GLFWwindowiconifyfun currentwindowiconifyfunction;
        private static GLFWwindowcontentscalefun currentwindowcontentscalefunction;
        private static GLFWframebuffersizefun currentwindowframebuffersizefunction;

        /// <summary>Do not care special value.</summary>
        public const System.Int32 GLFW_DONT_CARE = -1;

        /// <summary>Also a coded constant to indicate that any position can be used.</summary>
        public const System.Int32 GLFW_ANY_POSITION = unchecked((System.Int32)0x80000000);

        /// <summary>Shared between the window hints and attributes.</summary>
        private const System.Int32
            GLFW_DOUBLEBUFFER = 0x00021010,
            GLFW_CLIENT_API = 0x00022001,
            GLFW_CONTEXT_VERSION_MAJOR = 0x00022002,
            GLFW_CONTEXT_VERSION_MINOR = 0x00022003,
            GLFW_CONTEXT_REVISION = 0x00022004,
            GLFW_CONTEXT_ROBUSTNESS = 0x00022005,
            GLFW_OPENGL_FORWARD_COMPAT = 0x00022006,
            GLFW_CONTEXT_DEBUG = 0x00022007,
            GLFW_OPENGL_PROFILE = 0x00022008,
            GLFW_CONTEXT_RELEASE_BEHAVIOR = 0x00022009,
            GLFW_CONTEXT_NO_ERROR = 0x0002200A,
            GLFW_CONTEXT_CREATION_API = 0x0002200B;

        /// <summary>
        /// Defines GLFW common error codes.
        /// </summary>
        public enum Errors : System.Int32
        {
            /// <summary>No error has occurred.</summary>
            GLFW_NO_ERROR = 0,
            /// <summary>This occurs if a GLFW function was called that must not be called unless the library is initialized.</summary>
            GLFW_NOT_INITIALIZED = 0x00010001,
            /// <summary>This occurs if a GLFW function was called that needs and operates on the current OpenGL or OpenGL ES context but no context is current on the calling thread. One such function is glfwSwapInterval.</summary>
            GLFW_NO_CURRENT_CONTEXT = 0x00010002,
            /// <summary>One of the arguments to the function was an invalid enum value, for example requesting GLFW_RED_BITS with glfwGetWindowAttrib.</summary>
            GLFW_INVALID_ENUM = 0x00010003,
            /// <summary>
            /// One of the arguments to the function was an invalid value, for example requesting a non-existent OpenGL or OpenGL ES version like 2.7. <br />
            /// Requesting a valid but unavailable OpenGL or OpenGL ES version will instead result in a <see cref="GLFW_VERSION_UNAVAILABLE"/> error.
            /// </summary>
            GLFW_INVALID_VALUE = 0x00010004,
            /// <summary>A memory allocation failed.</summary>
            GLFW_OUT_OF_MEMORY = 0x00010005,
            /// <summary>GLFW could not find support for the requested API on the system.</summary>
            GLFW_API_UNAVAILABLE = 0x00010006,
            /// <summary>The requested OpenGL or OpenGL ES version (including any requested context or framebuffer hints) is not available on this machine.</summary>
            GLFW_VERSION_UNAVAILABLE = 0x00010007,
            /// <summary>A platform-specific error occurred that does not match any of the more specific categories.</summary>
            GLFW_PLATFORM_ERROR = 0x00010008,
            /// <summary>
            /// If emitted during window creation, the requested pixel format is not supported. <br />
            /// If emitted when querying the clipboard, the contents of the clipboard could not be converted to the requested format.
            /// </summary>
            GLFW_FORMAT_UNAVAILABLE = 0x00010009,
            /// <summary>A window that does not have an OpenGL or OpenGL ES context was passed to a function that requires it to have one.</summary>
            GLFW_NO_WINDOW_CONTEXT = 0x0001000A,
            /// <summary>The specified standard cursor shape is not available, either because the current platform cursor theme does not provide it or because it is not available on the platform.</summary>
            GLFW_CURSOR_UNAVAILABLE = 0x0001000B,
            /// <summary>The requested feature is not provided by the platform, so GLFW is unable to implement it. The documentation for each function notes if it could emit this error.</summary>
            GLFW_FEATURE_UNAVAILABLE = 0x0001000C,
            /// <summary>The requested feature has not yet been implemented in GLFW for this platform.</summary>
            GLFW_FEATURE_UNIMPLEMENTED = 0x0001000D,
            /// <summary>
            /// If emitted during initialization, no matching platform was found. If the GLFW_PLATFORM init hint was set to GLFW_ANY_PLATFORM, GLFW could not detect any of the platforms supported by this library binary, except for the Null platform.
            /// If the init hint was set to a specific platform, it is either not supported by this library binary or GLFW was not able to detect it.
            /// </summary>
            GLFW_PLATFORM_UNAVAILABLE = 0x0001000E
        }

        /// <summary>
        /// This is not an actual exported type from the native library, it is just used for comparing booleans with ease.
        /// </summary>
        public enum GLFW_BOOLEAN : System.Int32
        {
            GLFW_TRUE = 1,
            GLFW_FALSE = 0
        }

        /// <summary>
        /// This is not an actual exported enumeration type from the native library, it is just to group all the library initialization hints.
        /// </summary>
        public enum GLFW_INIT_HINT : System.Int32
        {
            GLFW_JOYSTICK_HAT_BUTTONS = 0x00050001,
            GLFW_ANGLE_PLATFORM_TYPE = 0x00050002,
            GLFW_PLATFORM = 0x00050003,
            GLFW_COCOA_CHDIR_RESOURCES = 0x00051001,
            GLFW_COCOA_MENUBAR = 0x00051002,
            GLFW_X11_XCB_VULKAN_SURFACE = 0x00052001,
            GLFW_WAYLAND_LIBDECOR  = 0x00053001,
        }

        /// <summary>
        /// This is not an actual exported enumeration type from the native library, it is just to group all the window initialization hints.
        /// </summary>
        public enum GLFW_WINDOW_HINT : System.Int32
        {
            GLFW_RED_BITS = 0x00021001,
            GLFW_GREEN_BITS = 0x00021002,
            GLFW_BLUE_BITS = 0x00021003,
            GLFW_ALPHA_BITS = 0x00021004,
            GLFW_DEPTH_BITS = 0x00021005,
            GLFW_STENCIL_BITS = 0x00021006,
            GLFW_ACCUM_RED_BITS = 0x00021007,
            GLFW_ACCUM_GREEN_BITS = 0x00021008,
            GLFW_ACCUM_BLUE_BITS = 0x00021009,
            GLFW_ACCUM_ALPHA_BITS = 0x0002100A,
            GLFW_AUX_BUFFERS = 0x0002100B,
            GLFW_STEREO = 0x0002100C,
            GLFW_SAMPLES = 0x0002100D,
            GLFW_SRGB_CAPABLE = 0x0002100E,
            GLFW_REFRESH_RATE = 0x0002100F,

            GLFW_CLIENT_API = GLFW.GLFW_CLIENT_API,
            GLFW_CONTEXT_VERSION_MAJOR = GLFW.GLFW_CONTEXT_VERSION_MAJOR,
            GLFW_CONTEXT_VERSION_MINOR = GLFW.GLFW_CONTEXT_VERSION_MINOR,
            GLFW_CONTEXT_REVISION = GLFW.GLFW_CONTEXT_REVISION,
            GLFW_CONTEXT_ROBUSTNESS = GLFW.GLFW_CONTEXT_ROBUSTNESS,
            GLFW_OPENGL_FORWARD_COMPAT = GLFW.GLFW_OPENGL_FORWARD_COMPAT,
            GLFW_CONTEXT_DEBUG = GLFW.GLFW_CONTEXT_DEBUG,
            GLFW_OPENGL_PROFILE = GLFW.GLFW_OPENGL_PROFILE,
            GLFW_CONTEXT_RELEASE_BEHAVIOR = GLFW.GLFW_CONTEXT_RELEASE_BEHAVIOR,
            GLFW_DOUBLEBUFFER = GLFW.GLFW_DOUBLEBUFFER,
            GLFW_CONTEXT_NO_ERROR = GLFW.GLFW_CONTEXT_NO_ERROR,
            GLFW_CONTEXT_CREATION_API = GLFW.GLFW_CONTEXT_CREATION_API,

            // TODO: Investiage whether the below are 
            GLFW_SCALE_TO_MONITOR = 0x0002200C,
            GLFW_SCALE_FRAMEBUFFER = 0x0002200D,
            GLFW_COCOA_RETINA_FRAMEBUFFER = 0x00023001,
            GLFW_COCOA_FRAME_NAME = 0x00023002,
            GLFW_COCOA_GRAPHICS_SWITCHING = 0x00023003,
            GLFW_X11_CLASS_NAME = 0x00024001,
            GLFW_X11_INSTANCE_NAME = 0x00024002,
            GLFW_WIN32_KEYBOARD_MENU = 0x00025001,
            GLFW_WIN32_SHOWDEFAULT = 0x00025002,
            GLFW_WAYLAND_APP_ID = 0x00026001,
            GLFW_NO_API = 0,
            GLFW_OPENGL_API = 0x00030001,
            GLFW_OPENGL_ES_API = 0x00030002,
        }

        /// <summary>
        /// This is not an actual exported enumeration type from the native library, it is just to group all the window attribute hints.
        /// </summary>
        public enum GLFW_WINDOW_ATTRIBUTE : System.Int32
        {
            GLFW_CLIENT_API = GLFW.GLFW_CLIENT_API,
            GLFW_CONTEXT_VERSION_MAJOR = GLFW.GLFW_CONTEXT_VERSION_MAJOR,
            GLFW_CONTEXT_VERSION_MINOR = GLFW.GLFW_CONTEXT_VERSION_MINOR,
            GLFW_CONTEXT_REVISION = GLFW.GLFW_CONTEXT_REVISION,
            GLFW_CONTEXT_ROBUSTNESS = GLFW.GLFW_CONTEXT_ROBUSTNESS,
            GLFW_OPENGL_FORWARD_COMPAT = GLFW.GLFW_OPENGL_FORWARD_COMPAT,
            GLFW_CONTEXT_DEBUG = GLFW.GLFW_CONTEXT_DEBUG,
            GLFW_OPENGL_PROFILE = GLFW.GLFW_OPENGL_PROFILE,
            GLFW_CONTEXT_RELEASE_BEHAVIOR = GLFW.GLFW_CONTEXT_RELEASE_BEHAVIOR,
            GLFW_DOUBLEBUFFER = GLFW.GLFW_DOUBLEBUFFER,
            GLFW_CONTEXT_NO_ERROR = GLFW.GLFW_CONTEXT_NO_ERROR,
            GLFW_CONTEXT_CREATION_API = GLFW.GLFW_CONTEXT_CREATION_API,

            GLFW_MOUSE_PASSTHROUGH = 0x0002000D,

            GLFW_POSITION_X = 0x0002000E,
            GLFW_POSITION_Y = 0x0002000F,

            GLFW_FOCUSED = 0x00020001,
            GLFW_ICONIFIED = 0x00020002,
            GLFW_RESIZABLE = 0x00020003,
            GLFW_VISIBLE = 0x00020004,
            GLFW_DECORATED = 0x00020005,
            GLFW_AUTO_ICONIFY = 0x00020006,
            GLFW_FLOATING = 0x00020007,
            GLFW_MAXIMIZED = 0x00020008,
            GLFW_CENTER_CURSOR = 0x00020009,
            GLFW_TRANSPARENT_FRAMEBUFFER = 0x0002000A,
            GLFW_HOVERED = 0x0002000B,
            GLFW_FOCUS_ON_SHOW = 0x0002000C
        }

        /// <summary>
        /// This is not an actual exported enumeration type from the native library, it is just to group all the platforms that GLFW supports.
        /// </summary>
        public enum GLFW_PLATFORM : System.Int32
        {
            GLFW_ANY_PLATFORM = 0x00060000,
            GLFW_PLATFORM_WIN32 = 0x00060001,
            GLFW_PLATFORM_COCOA = 0x00060002,
            GLFW_PLATFORM_WAYLAND = 0x00060003, 
            GLFW_PLATFORM_X11 = 0x00060004,
            GLFW_PLATFORM_NULL = 0x00060005,
        }

        /// <summary>
        /// This is not an actual exported enumeration type from the native library, it is just to group the events happening during monitor callback calls.
        /// </summary>
        public enum GLFW_MONITOR_CALLBACK_EVENT_TYPE : System.Int32
        {
            GLFW_CONNECTED = 0x00040001,
            GLFW_DISCONNECTED = 0x00040002
        }

        /// <summary>
        /// This is not an actual exported enumeration type from the native library, it is just to group the key press information together.
        /// </summary>
        public enum GLFW_KEY_PRESS_INFO : System.Int32
        {
            GLFW_KEY_UNKNOWN = -1,
            GLFW_RELEASE = 0,
            GLFW_PRESS = 1,
            GLFW_REPEAT = 2
        }

        /// <summary>
        /// This is not an actual exported enumeration type from the native library, it is just to group the different mouse buttons.
        /// </summary>
        public enum GLFW_MOUSE_BUTTON : System.Int32
        {
            GLFW_MOUSE_BUTTON_1 = 0,
            GLFW_MOUSE_BUTTON_2 = 1,
            GLFW_MOUSE_BUTTON_3 = 2,
            GLFW_MOUSE_BUTTON_4 = 3,
            GLFW_MOUSE_BUTTON_5 = 4,
            GLFW_MOUSE_BUTTON_6 = 5,
            GLFW_MOUSE_BUTTON_7 = 6,
            GLFW_MOUSE_BUTTON_8 = 7,
            GLFW_MOUSE_BUTTON_LAST = GLFW_MOUSE_BUTTON_8,
            GLFW_MOUSE_BUTTON_LEFT = GLFW_MOUSE_BUTTON_1,
            GLFW_MOUSE_BUTTON_RIGHT = GLFW_MOUSE_BUTTON_2,
            GLFW_MOUSE_BUTTON_MIDDLE = GLFW_MOUSE_BUTTON_3,
        }

        /// <summary>
        /// This is not an actual exported enumeration type from the native library, it is just to group the different modifier key flags.
        /// </summary>
        [Flags]
        public enum GLFW_MODIFIER_KEY_FLAGS : System.Int32
        {
            GLFW_MOD_SHIFT = 0x0001,
            GLFW_MOD_CTRL = 0x0002,
            GLFW_MOD_ALT = 0x0004,
            GLFW_MOD_SUPER = 0x0008,
            GLFW_MOD_CAPS_LOCK = 0x0010,
            GLFW_MOD_NUM_LOCK = 0x0020
        }

        /// <summary>
        /// This is not an actual exported enumeration type from the native library, it is just to group the different keyboard keys.
        /// </summary>
        public enum GLFW_KEY : System.Int32
        {
            GLFW_KEY_SPACE = 32,
 
 	        GLFW_KEY_APOSTROPHE = 39, /* ' */
 
 	        GLFW_KEY_COMMA = 44, /* , */
 
 	        GLFW_KEY_MINUS = 45, /* - */
 
 	        GLFW_KEY_PERIOD = 46, /* . */
 
 	        GLFW_KEY_SLASH = 47, /* / */
 
 	        GLFW_KEY_0 = 48,
 
 	        GLFW_KEY_1 = 49,
 
 	        GLFW_KEY_2 = 50,
 
 	        GLFW_KEY_3 = 51,
 
 	        GLFW_KEY_4 = 52,
 
 	        GLFW_KEY_5 = 53,
 
 	        GLFW_KEY_6 = 54,
 
 	        GLFW_KEY_7 = 55,
 
 	        GLFW_KEY_8 = 56,
 
 	        GLFW_KEY_9 = 57,
 
 	        GLFW_KEY_SEMICOLON = 59, /* ; */
 
 	        GLFW_KEY_EQUAL = 61, /* = */
 
 	        GLFW_KEY_A = 65,
 
 	        GLFW_KEY_B = 66,
 
 	        GLFW_KEY_C = 67,
 
 	        GLFW_KEY_D = 68,
 
 	        GLFW_KEY_E = 69,
 
 	        GLFW_KEY_F = 70,
 
 	        GLFW_KEY_G = 71,
 
 	        GLFW_KEY_H = 72,
 
 	        GLFW_KEY_I = 73,
 
 	        GLFW_KEY_J = 74,
 
 	        GLFW_KEY_K = 75,
 
 	        GLFW_KEY_L = 76,
 
 	        GLFW_KEY_M = 77,
 
 	        GLFW_KEY_N = 78,
 
 	        GLFW_KEY_O = 79,
 
 	        GLFW_KEY_P = 80,
 
 	        GLFW_KEY_Q = 81,
 
 	        GLFW_KEY_R = 82,
 
 	        GLFW_KEY_S = 83,
 
 	        GLFW_KEY_T = 84,
 
 	        GLFW_KEY_U = 85,
 
 	        GLFW_KEY_V = 86,
 
 	        GLFW_KEY_W = 87,
 
 	        GLFW_KEY_X = 88,
 
 	        GLFW_KEY_Y = 89,
 
 	        GLFW_KEY_Z = 90,
 
 	        GLFW_KEY_LEFT_BRACKET = 91, /* [ */
 
 	        GLFW_KEY_BACKSLASH = 92, /* \ */
 
 	        GLFW_KEY_RIGHT_BRACKET = 93, /* ] */
 
 	        GLFW_KEY_GRAVE_ACCENT = 96, /* ` */
 
 	        GLFW_KEY_WORLD_1 = 161, /* non-US #1 */
 
 	        GLFW_KEY_WORLD_2 = 162, /* non-US #2 */
 
 	        GLFW_KEY_ESCAPE = 256,
 
 	        GLFW_KEY_ENTER = 257,
 
 	        GLFW_KEY_TAB = 258,
 
 	        GLFW_KEY_BACKSPACE = 259,
 
 	        GLFW_KEY_INSERT = 260,
 
 	        GLFW_KEY_DELETE = 261,
 
 	        GLFW_KEY_RIGHT = 262,
 
 	        GLFW_KEY_LEFT = 263,
 
 	        GLFW_KEY_DOWN = 264,
 
 	        GLFW_KEY_UP = 265,
 
 	        GLFW_KEY_PAGE_UP = 266,
 
 	        GLFW_KEY_PAGE_DOWN = 267,
 
 	        GLFW_KEY_HOME = 268,
 
 	        GLFW_KEY_END = 269,
 
 	        GLFW_KEY_CAPS_LOCK = 280,
 
 	        GLFW_KEY_SCROLL_LOCK = 281,
 
 	        GLFW_KEY_NUM_LOCK = 282,
 
 	        GLFW_KEY_PRINT_SCREEN = 283,
 
 	        GLFW_KEY_PAUSE = 284,
 
 	        GLFW_KEY_F1 = 290,
 
 	        GLFW_KEY_F2 = 291,
 
 	        GLFW_KEY_F3 = 292,
 
 	        GLFW_KEY_F4 = 293,
 
 	        GLFW_KEY_F5 = 294,
 
 	        GLFW_KEY_F6 = 295,
 
 	        GLFW_KEY_F7 = 296,
 
 	        GLFW_KEY_F8 = 297,
 
 	        GLFW_KEY_F9 = 298,
 
 	        GLFW_KEY_F10 = 299,
 
 	        GLFW_KEY_F11 = 300,
 
 	        GLFW_KEY_F12 = 301,
 
 	        GLFW_KEY_F13 = 302,
 
 	        GLFW_KEY_F14 = 303,
 
 	        GLFW_KEY_F15 = 304,
 
 	        GLFW_KEY_F16 = 305,
 
 	        GLFW_KEY_F17 = 306,
 
 	        GLFW_KEY_F18 = 307,
 
 	        GLFW_KEY_F19 = 308,
 
 	        GLFW_KEY_F20 = 309,
 
 	        GLFW_KEY_F21 = 310,
 
 	        GLFW_KEY_F22 = 311,
 
 	        GLFW_KEY_F23 = 312,
 
 	        GLFW_KEY_F24 = 313,
 
 	        GLFW_KEY_F25 = 314,
 
 	        GLFW_KEY_KP_0 = 320,
 
 	        GLFW_KEY_KP_1 = 321,
 
 	        GLFW_KEY_KP_2 = 322,
 
 	        GLFW_KEY_KP_3 = 323,
 
 	        GLFW_KEY_KP_4 = 324,
 
 	        GLFW_KEY_KP_5 = 325,
 
 	        GLFW_KEY_KP_6 = 326,
 
 	        GLFW_KEY_KP_7 = 327,
 
 	        GLFW_KEY_KP_8 = 328,
 
 	        GLFW_KEY_KP_9 = 329,
 
 	        GLFW_KEY_KP_DECIMAL = 330,
 
 	        GLFW_KEY_KP_DIVIDE = 331,
 
 	        GLFW_KEY_KP_MULTIPLY = 332,
 
 	        GLFW_KEY_KP_SUBTRACT = 333,
 
 	        GLFW_KEY_KP_ADD = 334,
 
 	        GLFW_KEY_KP_ENTER = 335,
 
 	        GLFW_KEY_KP_EQUAL = 336,
 
 	        GLFW_KEY_LEFT_SHIFT = 340,
 
 	        GLFW_KEY_LEFT_CONTROL = 341,
 
 	        GLFW_KEY_LEFT_ALT = 342,
 
 	        GLFW_KEY_LEFT_SUPER = 343,
 
 	        GLFW_KEY_RIGHT_SHIFT = 344,
 
 	        GLFW_KEY_RIGHT_CONTROL = 345,
 
 	        GLFW_KEY_RIGHT_ALT = 346,
 
 	        GLFW_KEY_RIGHT_SUPER = 347,
 
 	        GLFW_KEY_MENU = 348,
 
 	        GLFW_KEY_LAST =  GLFW_KEY_MENU
        }

        /// <summary>
        /// This is not an actual exported enumeration type from the native library, it is just to group the device connection events. <br />
        /// This is currently used only by the Joystick API, but more API's in the future may include relevant functionality that uses this type.
        /// </summary>
        public enum GLFW_CONNECTION_EVENT : System.Int32
        {
            GLFW_CONNECTED = 0x00040001,
            GLFW_DISCONNECTED = 0x00040002
        }

        /// <summary>
        /// This is not an actual exported enumeration type from the native library, it is just to describe the input modifier options.
        /// </summary>
        public enum GLFW_INPUT_MODIFIER_OPTIONS : System.Int32
        {
            GLFW_CURSOR = 0x00033001,
            GLFW_STICKY_KEYS = 0x00033002,
            GLFW_STICKY_MOUSE_BUTTONS = 0x00033003,
            GLFW_LOCK_KEY_MODS = 0x00033004,
            GLFW_RAW_MOUSE_MOTION = 0x00033005,
        }

        /// <summary>
        /// This is not an actual exported enumeration type from the native library, it is just to describe the different states that the cursor can be into.
        /// </summary>
        public enum GLFW_CURSOR_STATE : System.Int32
        {
            GLFW_CURSOR_NORMAL = 0x00034001,
            GLFW_CURSOR_HIDDEN = 0x00034002,
            GLFW_CURSOR_DISABLED = 0x00034003,
            GLFW_CURSOR_CAPTURED = 0x00034004
        }

        /// <summary>
        /// This is not an actual exported enumeration type from the native library, it is just to describe the different system-provided cursor shapes.
        /// </summary>
        public enum GLFW_STANDARD_CURSOR_SHAPE : System.Int32
        {
            GLFW_ARROW_CURSOR = 0x00036001,
            GLFW_IBEAM_CURSOR = 0x00036002,
            GLFW_CROSSHAIR_CURSOR = 0x00036003,
            GLFW_POINTING_HAND_CURSOR = 0x00036004,
            GLFW_RESIZE_EW_CURSOR = 0x00036005,
            GLFW_RESIZE_NS_CURSOR = 0x00036006,
            GLFW_RESIZE_NWSE_CURSOR = 0x00036007,
            GLFW_RESIZE_NESW_CURSOR = 0x00036008,
            GLFW_RESIZE_ALL_CURSOR = 0x00036009,
            GLFW_NOT_ALLOWED_CURSOR = 0x0003600A
        }

        public struct GLFW_ALLOCATOR
        {
            /// <summary>
            /// typedef void* (* GLFWallocatefun) (size_t size, void* user) <br />
            /// Should return NULL at failure
            /// </summary>
            public delegate* unmanaged[Cdecl]<IntPtr, void*, void*> Allocate;

            /// <summary>
            /// typedef void *(* GLFWreallocatefun) (void *block, size_t size, void *user) <br />
            /// Should return NULL at failure, all parameters are not NULL
            /// </summary>
            public delegate* unmanaged[Cdecl]<void*, IntPtr, void*, void*> Reallocate;

            /// <summary>
            /// typedef void (* GLFWdeallocatefun) (void* block, void* user)
            /// Must be valid up to GLFWterminate
            /// </summary>
            public delegate* unmanaged[Cdecl]<void*, void*, void> Free;

            /// <summary>
            /// User data.
            /// </summary>
            public void* User;
        }

        /// <summary>
        /// This corresponds to the GLFWmonitor structure but note that it contains the actual pointer. <br />
        /// So a GLFWmonitor* definition will become <see cref="GLFW_MONITOR"/> in .NET.
        /// </summary>
        public struct GLFW_MONITOR : MP.Utilities.INullable
        {
            public void* Pointer;

            public readonly System.Boolean IsNull => Pointer is null;
        }

        /// <summary>
        /// This corresponds bit perfectly to the GLFWvidmode structure.
        /// </summary>
        [StructLayout(LayoutKind.Explicit, Pack = 1 , Size = 24)]
        public struct GLFW_VIDEO_MODE
        {
            [FieldOffset(0)]
            public int Width;
            [FieldOffset(4)]
            public int Height;
            [FieldOffset(8)]
            public int RedBits;
            [FieldOffset(12)]
            public int GreenBits;
            [FieldOffset(16)]
            public int BlueBits;
            [FieldOffset(20)]
            public int RefreshRate;
        }

        /// <summary>
        /// This corresponds to the GLFWgammaramp structure.
        /// </summary>
        public struct GLFW_GAMMA_RAMP
        {
            public System.UInt16* Red;
            public System.UInt16* Green;
            public System.UInt16* Blue;
            public System.UInt32 Size; // array size of each component
        }

        /// <summary>
        /// This corresponds to the GLFWwindow structure but note that it contains the actual pointer. <br />
        /// So a GLFWwindow* definition will become <see cref="GLFW_WINDOW"/> in .NET.
        /// </summary>
        public struct GLFW_WINDOW : MP.Utilities.INullable
        {
            public void* Pointer;

            public GLFW_WINDOW() => Pointer = null;

            public readonly System.Boolean IsNull => Pointer is null;
        }

        /// <summary>
        /// This corresponds to the GLFWimage structure. <br />
        /// Note that the length of the memory pointer provided here must be <see cref="Width"/> * <see cref="Height"/> * 4. <br />
        /// The image data in the pointer must be arranged as RGBA, left-to-right, top-to-bottom.
        /// </summary>
        public struct GLFW_IMAGE
        {
            public int Width;
            public int Height;
            public System.Byte* ImageData;
        }

        /// <summary>
        /// This corresponds to the GLFWcursor structure but note that it contains the actual pointer. <br />
        /// So a GLFWcursor* definition will become <see cref="GLFW_CURSOR"/> in .NET.
        /// </summary>
        public struct GLFW_CURSOR : MP.Utilities.INullable
        {
            public void* Pointer;

            public GLFW_CURSOR() => Pointer = null;

            public readonly bool IsNull => Pointer is null;
        }

        /// <summary>
        /// This corresponds bit perfectly to the GLFWgamepadstate structure.
        /// </summary>
        public struct GLFW_GAMEPAD_STATE
        {
            public fixed System.Byte Buttons[15]; // Cast each member to GLFW_BOOLEAN to learn whether a joystick button was used or not.
            public fixed System.Single Axes[6]; // All the array elements are values playing from -1 to 1
        }

        public delegate void GLFWmousebuttonfun(GLFW_WINDOW window, GLFW_MOUSE_BUTTON button, GLFW_KEY_PRESS_INFO action, GLFW_MODIFIER_KEY_FLAGS modifiers);

        // If you have a question about what this is, just consider that it provides the offset of the mouse's scroll wheel.
        public delegate void GLFWscrollfun(GLFW_WINDOW window, double xoffset, double yoffset);

        public delegate void GLFWkeyfun(GLFW_WINDOW window, GLFW_KEY key, int scancode, GLFW_KEY_PRESS_INFO action, GLFW_MODIFIER_KEY_FLAGS modifiers);

        public delegate void GLFWjoystickfun(int jid, GLFW_CONNECTION_EVENT connection_event);

        public delegate void GLFWwindowclosefun(GLFW_WINDOW window);

        public delegate void GLFWwindowiconifyfun(GLFW_WINDOW window , GLFW_BOOLEAN iconified);

        public delegate void GLFWframebuffersizefun(GLFW_WINDOW window , int widthpixels , int heightpixels);

        public delegate void GLFWwindowcontentscalefun(GLFW_WINDOW window, float xscale, float yscale);

        public delegate void GLFWwindowposfun(GLFW_WINDOW window, int x, int y);

        public delegate void GLFWwindowsizefun(GLFW_WINDOW window, int width, int height);

        public delegate void GLFWmonitorfun(GLFW_MONITOR monitor , GLFW_MONITOR_CALLBACK_EVENT_TYPE event_type);

        public delegate void GLFWerrorfun(Errors error, String description);

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        private static void GLFWErrorFunTranslation(Errors err , byte* desc)
        {
            if (currenterrorfunction is not null) {
                currenterrorfunction(err, MP.UnsafeMethods.CreateASCIINullTerminated(desc));
            }
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        private static void GLFWMonitorFunTranslation(GLFW_MONITOR monitor , GLFW_MONITOR_CALLBACK_EVENT_TYPE cevent)
        {
            if (currentmonitorfunction is not null) {
                currentmonitorfunction(monitor, cevent);
            }
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        private static void GLFWWindowPosFunTranslation(GLFW_WINDOW window, int x , int y)
        {
            if (currentwindowposfunction is not null) {
                currentwindowposfunction(window, x, y);
            }
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        private static void GLFWWindowSizeFunTranslation(GLFW_WINDOW window, int width, int height)
        {
            if (currentwindowsizefunction is not null) {
                currentwindowsizefunction(window, width, height);
            }
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        private static void GLFWWindowClosingFunTranslation(GLFW_WINDOW window)
        {
            if (currentwindowclosefunction is not null){
                currentwindowclosefunction(window);
            }
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        private static void GLFWWindowIconifyFunTranslation(GLFW_WINDOW window, GLFW_BOOLEAN iconified)
        {
            if (currentwindowiconifyfunction is not null) {
                currentwindowiconifyfunction(window, iconified);
            }
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        private static void GLFWWindowFrameBufferSizeFunTranslation(GLFW_WINDOW window, int width, int height)
        {
            if (currentwindowframebuffersizefunction is not null) {
                currentwindowframebuffersizefunction(window, width, height);
            }
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        private static void GLFWWindowContentsScaleFunTranslation(GLFW_WINDOW window, float xs, float ys)
        {
            if (currentwindowcontentscalefunction is not null) { 
                currentwindowcontentscalefunction(window , xs, ys);
            }
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        private static void GLFWMouseButtonFunTranslation(GLFW_WINDOW window, GLFW_MOUSE_BUTTON button, GLFW_KEY_PRESS_INFO action, GLFW_MODIFIER_KEY_FLAGS modifiers)
        {
            if (currentmousebuttonfunction is not null) {
                currentmousebuttonfunction(window, button, action, modifiers);
            }
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        private static void GLFWMouseScrollFunTranslation(GLFW_WINDOW window, double xoffset, double yoffset)
        {
            if (currentmousescrollfunction is not null) {
                currentmousescrollfunction(window, xoffset, yoffset);
            }
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        private static void GLFWKeyFunTranslation(GLFW_WINDOW window, GLFW_KEY key, int scancode, GLFW_KEY_PRESS_INFO action, GLFW_MODIFIER_KEY_FLAGS modifiers)
        {
            if (currentkeyfunction is not null) { 
                currentkeyfunction(window , key , scancode, action, modifiers);
            }
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        private static void GLFWControllerEventFunTranslation(int jid, GLFW_CONNECTION_EVENT connection_event)
        {
            if (currentjoystickfunction is not null) {
                currentjoystickfunction(jid, connection_event);
            }
        }

        private static MP.IMemoryHandle ToUTF8String(System.String value)
        {
            var utf8 = System.Text.Encoding.UTF8;
            int bc = utf8.GetByteCount(value) + utf8.GetByteCount("\0");
            MP.IMemoryHandle mem = MP.SystemInfo.CreateNativeMemory(bc);
            try {
                fixed (System.Char* p = value) {
                    utf8.GetBytes(p , value.Length + 1 , mem.MemoryPointer , mem.MemoryLength);
                }
            } catch {
                mem.Dispose();
                throw;
            }
            return mem;
        }

        [DllImport(Libraries.GLFW , CallingConvention = CallingConvention.Cdecl , EntryPoint = "glfwInitAllocator", SetLastError = false , ExactSpelling = true)]
        private static extern void glfwInitAllocator_Native(GLFW_ALLOCATOR* allocator);

        public static void glfwInitAllocator(GLFW_ALLOCATOR allc) => glfwInitAllocator_Native(&allc);

        [DllImport(Libraries.GLFW , CallingConvention = CallingConvention.Cdecl, SetLastError = false, ExactSpelling = true)]
        public static extern GLFW_BOOLEAN glfwInit();

        [DllImport(Libraries.GLFW, CallingConvention = CallingConvention.Cdecl, SetLastError = false, ExactSpelling = true)]
        public static extern void glfwTerminate();

        [DllImport(Libraries.GLFW, CallingConvention = CallingConvention.Cdecl, SetLastError = false, ExactSpelling = true)]
        public static extern void glfwInitHint(GLFW_INIT_HINT inithint, int value);

        [DllImport(Libraries.GLFW, EntryPoint = "glfwGetVersion", CallingConvention = CallingConvention.Cdecl, SetLastError = false, ExactSpelling = true)]
        private static extern void glfwGetVersion_Native(int* major, int* minor, int* revision);

        public static Version glfwGetVersion()
        {
            int major, minor, revision;
            glfwGetVersion_Native(&major, &minor, &revision);
            return new Version(major, minor, 0, revision);
        }

        [DllImport(Libraries.GLFW , EntryPoint = "glfwGetVersionString", CallingConvention = CallingConvention.Cdecl, SetLastError = false, ExactSpelling = true)]
        private static extern byte* glfwGetVersionString_Native();

        public static System.String glfwGetVersionString() => MP.UnsafeMethods.CreateASCIINullTerminated(glfwGetVersionString_Native());

        [DllImport(Libraries.GLFW, EntryPoint = "glfwGetError", CallingConvention = CallingConvention.Cdecl, SetLastError = false, ExactSpelling = true)]
        private static extern Errors glfwGetError_Native(byte** descstring);

        public static Errors glfwGetError(out System.String description)
        {
            byte* pstring;
            Errors err = glfwGetError_Native(&pstring);
            description = MP.UnsafeMethods.CreateASCIINullTerminated(pstring);
            return err;
        }

        [DllImport(Libraries.GLFW, EntryPoint = "glfwSetErrorCallback", CallingConvention = CallingConvention.Cdecl, SetLastError = false, ExactSpelling = true)]
        private static extern delegate* unmanaged[Cdecl]<Errors, byte*, void> glfwSetErrorCallback_Native(delegate* unmanaged[Cdecl]<Errors, byte*, void> callback);

        public static GLFWerrorfun glfwSetErrorCallback(GLFWerrorfun error)
        {
            if (currenterrorfunction is null) {
                glfwSetErrorCallback_Native(&GLFWErrorFunTranslation);
            }
            return Interlocked.Exchange(ref currenterrorfunction, error);
        }

        [DllImport(Libraries.GLFW, CallingConvention = CallingConvention.Cdecl, SetLastError = false, ExactSpelling = true)]
        public static extern GLFW_PLATFORM glfwGetPlatform();

        [DllImport(Libraries.GLFW, CallingConvention = CallingConvention.Cdecl, SetLastError = false, ExactSpelling = true)]
        public static extern GLFW_BOOLEAN glfwPlatformSupported(GLFW_PLATFORM platform);

        [DllImport(Libraries.GLFW, EntryPoint = "glfwGetMonitors", CallingConvention = CallingConvention.Cdecl, SetLastError = false, ExactSpelling = true)]
        private static extern GLFW_MONITOR* glfwGetMonitors_Native(int* count);

        public static GLFW_MONITOR[] glfwGetMonitors()
        {
            int count;
            GLFW_MONITOR* pmonitors = glfwGetMonitors_Native(&count);
            GLFW_MONITOR[] ret = new GLFW_MONITOR[count];
            fixed (GLFW_MONITOR* pretmonitors = ret) {
                Unsafe.CopyBlockUnaligned(pretmonitors, pmonitors, MP.UnsafeMethods.ToUInt32(count * sizeof(GLFW_MONITOR)));
            }
            return ret;
        }

        [DllImport(Libraries.GLFW, EntryPoint = "glfwGetMonitorWorkarea", CallingConvention = CallingConvention.Cdecl, SetLastError = false, ExactSpelling = true)]
        private static extern void glfwGetMonitorWorkarea_Native(GLFW_MONITOR monitor , int* xpos , int* ypos, int* width , int* height);

        public static void glfwGetMonitorWorkarea(GLFW_MONITOR monitor , out int xpos , out int ypos , out int width , out int height)
        {
            int xp, yp, wd, ht;
            glfwGetMonitorWorkarea_Native(monitor, &xp, &yp, &wd, &ht);
            xpos = xp;
            ypos = yp;
            width = wd;
            height = ht;
        }

        [DllImport(Libraries.GLFW, EntryPoint = "glfwGetMonitorPhysicalSize", CallingConvention = CallingConvention.Cdecl, SetLastError = false, ExactSpelling = true)]
        private static extern void glfwGetMonitorPhysicalSize_Native(GLFW_MONITOR monitor, int* widthMM, int* heightMM);

        public static void glfwGetMonitorPhysicalSize(GLFW_MONITOR monitor, out int widthMM, out int heightMM) 
        {
            int wd, ht;
            glfwGetMonitorPhysicalSize_Native(monitor, &wd, &ht);
            widthMM = wd;
            heightMM = ht;
        }

        [DllImport(Libraries.GLFW, EntryPoint = "glfwGetMonitorContentScale", CallingConvention = CallingConvention.Cdecl, SetLastError = false, ExactSpelling = true)]
        private static extern void glfwGetMonitorContentScale_Native(GLFW_MONITOR monitor , float* xscale , float* yscale);

        public static void glfwGetMonitorContentScale(GLFW_MONITOR monitor , out float xscale , out float yscale)
        {
            float xs, ys;
            glfwGetMonitorContentScale_Native(monitor, &xs, &ys);
            xscale = xs;
            yscale = ys;
        }

        [DllImport(Libraries.GLFW, EntryPoint = "glfwGetMonitorName", CallingConvention = CallingConvention.Cdecl, SetLastError = false, ExactSpelling = true)]
        private static extern byte* glfwGetMonitorName_Native(GLFW_MONITOR monitor);

        public static System.String glfwGetMonitorName(GLFW_MONITOR monitor)
        {
            byte* p = glfwGetMonitorName_Native(monitor);
            if (p is null) { return null; }
            return MP.UnsafeMethods.CreateUtf8NullTerminated(p);
        }

        [DllImport(Libraries.GLFW, EntryPoint = "glfwSetMonitorCallback", CallingConvention = CallingConvention.Cdecl, SetLastError = false, ExactSpelling = true)]
        private static extern delegate* unmanaged[Cdecl]<GLFW_MONITOR, GLFW_MONITOR_CALLBACK_EVENT_TYPE, void> glfwSetMonitorCallback_Native(delegate* unmanaged[Cdecl]<GLFW_MONITOR, GLFW_MONITOR_CALLBACK_EVENT_TYPE, void> newdelegate);

        public static GLFWmonitorfun glfwSetMonitorCallback(GLFWmonitorfun monitor)
        {
            if (currentmonitorfunction is null) {
                glfwSetMonitorCallback_Native(&GLFWMonitorFunTranslation);
            }
            return Interlocked.Exchange(ref currentmonitorfunction, monitor);
        }

        [DllImport(Libraries.GLFW, EntryPoint = "glfwGetVideoModes", CallingConvention = CallingConvention.Cdecl, SetLastError = false, ExactSpelling = true)]
        private static extern GLFW_VIDEO_MODE* glfwGetVideoModes_Native(GLFW_MONITOR monitor, int* count);

        public static GLFW_VIDEO_MODE[] glfwGetVideoModes(GLFW_MONITOR monitor)
        {
            int count;
            GLFW_VIDEO_MODE* gvm = glfwGetVideoModes_Native(monitor, &count);
            GLFW_VIDEO_MODE[] ret = new GLFW_VIDEO_MODE[count];
            fixed (GLFW_VIDEO_MODE* pg = ret) {
                Unsafe.CopyBlockUnaligned(pg, gvm, MP.UnsafeMethods.ToUInt32(count * sizeof(GLFW_VIDEO_MODE)));
            }
            return ret;
        }

        [DllImport(Libraries.GLFW, EntryPoint = "glfwGetVideoMode", CallingConvention = CallingConvention.Cdecl, SetLastError = false, ExactSpelling = true)]
        private static extern GLFW_VIDEO_MODE* glfwGetVideoMode_Native(GLFW_MONITOR monitor);

        public static GLFW_VIDEO_MODE glfwGetVideoMode(GLFW_MONITOR monitor)
        {
            GLFW_VIDEO_MODE* vm = glfwGetVideoMode_Native(monitor);
            return vm is null ? default : *vm;
        }

        [DllImport(Libraries.GLFW, CallingConvention = CallingConvention.Cdecl, SetLastError = false, ExactSpelling = true)]
        public static extern void glfwSetGamma(GLFW_MONITOR monitor, float gamma);

        [DllImport(Libraries.GLFW, EntryPoint = "glfwGetGammaRamp", CallingConvention = CallingConvention.Cdecl, SetLastError = false, ExactSpelling = true)]
        private static extern GLFW_GAMMA_RAMP* glfwGetGammaRamp_Native(GLFW_MONITOR monitor);

        public static GLFW_GAMMA_RAMP glfwGetGammaRamp(GLFW_MONITOR monitor)
        {
            GLFW_GAMMA_RAMP* p = glfwGetGammaRamp_Native(monitor);
            return p is null ? default : *p;
        }

        [DllImport(Libraries.GLFW, EntryPoint = "glfwSetGammaRamp", CallingConvention = CallingConvention.Cdecl, SetLastError = false, ExactSpelling = true)]
        private static extern void glfwSetGammaRamp_Native(GLFW_MONITOR monitor, GLFW_GAMMA_RAMP* pramp);

        public static void glfwSetGammaRamp(GLFW_MONITOR monitor, GLFW_GAMMA_RAMP gamma_ramp) => glfwSetGammaRamp_Native(monitor, &gamma_ramp);

        [DllImport(Libraries.GLFW, EntryPoint = "glfwSetWindowPosCallback", CallingConvention = CallingConvention.Cdecl, SetLastError = false, ExactSpelling = true)]
        private static extern delegate* unmanaged[Cdecl]<GLFW_WINDOW, int, int, void> glfwSetWindowPosCallback_Native(delegate* unmanaged[Cdecl]<GLFW_WINDOW, int, int, void> replacewith);

        public static GLFWwindowposfun glfwSetWindowPosCallback(GLFWwindowposfun pf)
        {
            if (currentwindowposfunction is null) {
                glfwSetWindowPosCallback_Native(&GLFWWindowPosFunTranslation);
            }
            return Interlocked.Exchange(ref currentwindowposfunction, pf);
        }

        [DllImport(Libraries.GLFW, EntryPoint = "glfwSetWindowSizeCallback", CallingConvention = CallingConvention.Cdecl, SetLastError = false, ExactSpelling = true)]
        private static extern delegate* unmanaged[Cdecl]<GLFW_WINDOW, int, int, void> glfwSetWindowSizeCallback_Native(delegate* unmanaged[Cdecl]<GLFW_WINDOW, int, int, void> replacewith);

        public static GLFWwindowsizefun glfwSetWindowSizeCallback(GLFWwindowsizefun size)
        {
            if (currentwindowsizefunction is null) {
                glfwSetWindowSizeCallback_Native(&GLFWWindowSizeFunTranslation);
            }
            return Interlocked.Exchange(ref currentwindowsizefunction , size);
        }

        [DllImport(Libraries.GLFW, EntryPoint = "glfwSetWindowCloseCallback", CallingConvention = CallingConvention.Cdecl, SetLastError = false, ExactSpelling = true)]
        private static extern delegate* unmanaged[Cdecl]<GLFW_WINDOW, void> glfwSetWindowCloseCallback_Native(delegate* unmanaged[Cdecl]<GLFW_WINDOW, void> replacewith);

        public static GLFWwindowclosefun glfwSetWindowCloseCallback(GLFWwindowclosefun close) 
        {
            if (currentwindowclosefunction is null) {
                glfwSetWindowCloseCallback_Native(&GLFWWindowClosingFunTranslation);
            }
            return Interlocked.Exchange(ref currentwindowclosefunction , close);
        }

        [DllImport(Libraries.GLFW, EntryPoint = "glfwSetWindowIconifyCallback", CallingConvention = CallingConvention.Cdecl, SetLastError = false, ExactSpelling = true)]
        private static extern delegate* unmanaged[Cdecl]<GLFW_WINDOW, GLFW_BOOLEAN, void> glfwSetWindowIconifyCallback_Native(delegate* unmanaged[Cdecl]<GLFW_WINDOW, GLFW_BOOLEAN, void> replacewith);

        public static GLFWwindowiconifyfun glfwSetWindowIconifyCallback(GLFWwindowiconifyfun iconify) 
        {
            if (currentwindowiconifyfunction is null) {
                glfwSetWindowIconifyCallback_Native(&GLFWWindowIconifyFunTranslation);
            }
            return Interlocked.Exchange(ref currentwindowiconifyfunction , iconify);
        }

        [DllImport(Libraries.GLFW, EntryPoint = "glfwSetFramebufferSizeCallback", CallingConvention = CallingConvention.Cdecl, SetLastError = false, ExactSpelling = true)]
        private static extern delegate* unmanaged[Cdecl]<GLFW_WINDOW, int, int, void> glfwSetFramebufferSizeCallback_Native(delegate* unmanaged[Cdecl]<GLFW_WINDOW, int, int, void> replacewith);

        public static GLFWframebuffersizefun glfwSetFramebufferSizeCallback(GLFWframebuffersizefun framebuffersize)
        {
            if (currentwindowframebuffersizefunction is null) {
                glfwSetFramebufferSizeCallback_Native(&GLFWWindowFrameBufferSizeFunTranslation);
            }
            return Interlocked.Exchange(ref currentwindowframebuffersizefunction , framebuffersize);
        }

        [DllImport(Libraries.GLFW, EntryPoint = "glfwSetWindowContentScaleCallback", CallingConvention = CallingConvention.Cdecl, SetLastError = false, ExactSpelling = true)]
        private static extern delegate* unmanaged[Cdecl]<GLFW_WINDOW, float, float, void> glfwSetWindowContentScaleCallback_Native(delegate* unmanaged[Cdecl]<GLFW_WINDOW, float, float, void> replacewith);

        public static GLFWwindowcontentscalefun glfwSetWindowContentScaleCallback(GLFWwindowcontentscalefun windowcontentscale)
        {
            if (currentwindowcontentscalefunction is null) {
                glfwSetWindowContentScaleCallback_Native(&GLFWWindowContentsScaleFunTranslation);
            }    
            return Interlocked.Exchange(ref currentwindowcontentscalefunction , windowcontentscale);
        }

        [DllImport(Libraries.GLFW, CallingConvention = CallingConvention.Cdecl, SetLastError = false, ExactSpelling = true)]
        public static extern void glfwDefaultWindowHints();

        [DllImport(Libraries.GLFW, CallingConvention = CallingConvention.Cdecl, SetLastError = false, ExactSpelling = true)]
        public static extern void glfwWindowHint(GLFW_WINDOW_HINT hint, int value);

        [DllImport(Libraries.GLFW, CallingConvention = CallingConvention.Cdecl, SetLastError = false, ExactSpelling = true)]
        private static extern void glfwWindowHintString_Native(GLFW_WINDOW_HINT hint, byte* value);

        public static void glfwWindowHintString(GLFW_WINDOW_HINT hint , System.String value)
        {
            int size = value.Length + 1;
            MP.IMemoryHandle mem = ToUTF8String(value);
            glfwWindowHintString_Native(hint, mem.MemoryPointer);
            mem.Dispose();
        }

        [DllImport(Libraries.GLFW, EntryPoint = "glfwCreateWindow", CallingConvention = CallingConvention.Cdecl, SetLastError = false, ExactSpelling = true)]
        private static extern GLFW_WINDOW glfwCreateWindow_Native(int width, int height, byte* title, GLFW_MONITOR monitor, GLFW_WINDOW share);

        public static GLFW_WINDOW glfwCreateWindow(int width , int height, System.String title,  GLFW_MONITOR monitor, GLFW_WINDOW share)
        {
            MP.IMemoryHandle mh = ToUTF8String(title);
            try {
                return glfwCreateWindow_Native(width, height, mh.MemoryPointer, monitor, share);
            } finally {
                mh.Dispose();
            }
        }

        [DllImport(Libraries.GLFW, CallingConvention = CallingConvention.Cdecl, SetLastError = false, ExactSpelling = true)]
        public static extern void glfwDestroyWindow(GLFW_WINDOW window);

        [DllImport(Libraries.GLFW, CallingConvention = CallingConvention.Cdecl, SetLastError = false, ExactSpelling = true)]
        public static extern int glfwWindowShouldClose(GLFW_WINDOW window);

        [DllImport(Libraries.GLFW, CallingConvention = CallingConvention.Cdecl, SetLastError = false, ExactSpelling = true)]
        public static extern void glfwSetWindowShouldClose(GLFW_WINDOW window , int value);

        [DllImport(Libraries.GLFW, EntryPoint = "glfwGetWindowTitle", CallingConvention = CallingConvention.Cdecl, SetLastError = false, ExactSpelling = true)]
        private static extern byte* glfwGetWindowTitle_Native(GLFW_WINDOW window);

        public static System.String glfwGetWindowTitle(GLFW_WINDOW window) => MP.UnsafeMethods.CreateUtf8NullTerminated(glfwGetWindowTitle_Native(window));

        [DllImport(Libraries.GLFW, EntryPoint = "glfwSetWindowTitle", CallingConvention = CallingConvention.Cdecl, SetLastError = false, ExactSpelling = true)]
        private static extern void glfwSetWindowTitle_Native(GLFW_WINDOW window, byte* title);

        public static void glfwSetWindowTitle(GLFW_WINDOW window, System.String title)
        {
            MP.IMemoryHandle mh = ToUTF8String(title);
            glfwSetWindowTitle_Native(window, mh.MemoryPointer);
            mh.Dispose();
        }

        [DllImport(Libraries.GLFW, EntryPoint = "glfwSetWindowIcon", CallingConvention = CallingConvention.Cdecl, SetLastError = false, ExactSpelling = true)]
        private static extern void glfwSetWindowIcon_Native(GLFW_WINDOW window, int images_count, GLFW_IMAGE* images);

        public static void glfwSetWindowIcon(GLFW_WINDOW window , GLFW_IMAGE[] images)
        {
            fixed (GLFW_IMAGE* pi = images) {
                glfwSetWindowIcon_Native(window, images.Length , pi);
            }
        }

        [DllImport(Libraries.GLFW, EntryPoint = "glfwGetWindowPos", CallingConvention = CallingConvention.Cdecl, SetLastError = false, ExactSpelling = true)]
        private static extern void glfwGetWindowPos_Native(GLFW_WINDOW window, int* x, int* y);

        public static void glfwGetWindowPos(GLFW_WINDOW window, out int x, out int y)
        {
            int xp, yp;
            glfwGetWindowPos_Native(window, &xp, &yp);
            x = xp;
            y = yp;
        }

        [DllImport(Libraries.GLFW, CallingConvention = CallingConvention.Cdecl, SetLastError = false, ExactSpelling = true)]
        public static extern void glfwSetWindowPos(GLFW_WINDOW window, int x, int y);

        [DllImport(Libraries.GLFW, EntryPoint = "glfwGetWindowSize", CallingConvention = CallingConvention.Cdecl, SetLastError = false, ExactSpelling = true)]
        private static extern void glfwGetWindowSize_Native(GLFW_WINDOW window , int* width , int* height);

        public static void glfwGetWindowSize(GLFW_WINDOW window , out int width , out int height)
        {
            int w, h;
            glfwGetWindowSize_Native(window, &w, &h);
            width = w;
            height = h;
        }
        
        [DllImport(Libraries.GLFW, CallingConvention = CallingConvention.Cdecl, SetLastError = false, ExactSpelling = true)]
        public static extern void glfwSetWindowSizeLimits(GLFW_WINDOW window , int min_width, int min_height, int max_width, int max_height);

        [DllImport(Libraries.GLFW, CallingConvention = CallingConvention.Cdecl, SetLastError = false, ExactSpelling = true)]
        public static extern void glfwSetWindowAspectRatio(GLFW_WINDOW window, int numerator, int denominator);

        [DllImport(Libraries.GLFW, CallingConvention = CallingConvention.Cdecl, SetLastError = false, ExactSpelling = true)]
        public static extern void glfwSetWindowSize(GLFW_WINDOW window, int width, int height);

        [DllImport(Libraries.GLFW, EntryPoint = "glfwGetFramebufferSize", CallingConvention = CallingConvention.Cdecl, SetLastError = false, ExactSpelling = true)]
        private static extern void glfwGetFramebufferSize_Native(GLFW_WINDOW window, int* width, int* height);

        public static void glfwGetFramebufferSize(GLFW_WINDOW window , out int width, out int height)
        {
            int w, h;
            glfwGetFramebufferSize_Native(window, &w, &h);
            width = w;
            height = h;
        }

        [DllImport(Libraries.GLFW, EntryPoint = "glfwGetWindowFrameSize", CallingConvention = CallingConvention.Cdecl, SetLastError = false, ExactSpelling = true)]
        private static extern void glfwGetWindowFrameSize_Native(GLFW_WINDOW window , int* left, int* top, int* right, int* bottom);

        public static void glfwGetWindowFrameSize(GLFW_WINDOW window, out int left , out int top , out int right, out int bottom)
        {
            int l, t, r, b;
            glfwGetWindowFrameSize_Native(window, &l, &t, &r, &b);
            left = l;
            top = t;
            right = r;
            bottom = b;
        }

        [DllImport(Libraries.GLFW, EntryPoint = "glfwGetWindowContentScale", CallingConvention = CallingConvention.Cdecl, SetLastError = false, ExactSpelling = true)]
        private static extern void glfwGetWindowContentScale_Native(GLFW_WINDOW window, float* xscale, float* yscale);

        public static void glfwGetWindowContentScale(GLFW_WINDOW window, out float xscale, out float yscale) 
        {
            float xs, ys;
            glfwGetWindowContentScale_Native(window, &xs, &ys);
            xscale = xs;
            yscale = ys;
        }

        [DllImport(Libraries.GLFW, CallingConvention = CallingConvention.Cdecl, SetLastError = false, ExactSpelling = true)]
        public static extern float glfwGetWindowOpacity(GLFW_WINDOW window);

        [DllImport(Libraries.GLFW, CallingConvention = CallingConvention.Cdecl, SetLastError = false, ExactSpelling = true)]
        public static extern void glfwSetWindowOpacity(GLFW_WINDOW window, float opacity);

        [DllImport(Libraries.GLFW, CallingConvention = CallingConvention.Cdecl, SetLastError = false, ExactSpelling = true)]
        public static extern void glfwIconifyWindow(GLFW_WINDOW window);

        [DllImport(Libraries.GLFW, CallingConvention = CallingConvention.Cdecl, SetLastError = false, ExactSpelling = true)]
        public static extern void glfwRestoreWindow(GLFW_WINDOW window);

        [DllImport(Libraries.GLFW, CallingConvention = CallingConvention.Cdecl, SetLastError = false, ExactSpelling = true)]
        public static extern void glfwMaximizeWindow(GLFW_WINDOW window);

        [DllImport(Libraries.GLFW, CallingConvention = CallingConvention.Cdecl, SetLastError = false, ExactSpelling = true)]
        public static extern void glfwShowWindow(GLFW_WINDOW window);

        [DllImport(Libraries.GLFW, CallingConvention = CallingConvention.Cdecl, SetLastError = false, ExactSpelling = true)]
        public static extern void glfwHideWindow(GLFW_WINDOW window);

        [DllImport(Libraries.GLFW, CallingConvention = CallingConvention.Cdecl, SetLastError = false, ExactSpelling = true)]
        public static extern void glfwFocusWindow(GLFW_WINDOW window);

        [DllImport(Libraries.GLFW, CallingConvention = CallingConvention.Cdecl, SetLastError = false, ExactSpelling = true)]
        public static extern void glfwRequestWindowAttention(GLFW_WINDOW window);

        [DllImport(Libraries.GLFW, CallingConvention = CallingConvention.Cdecl, SetLastError = false, ExactSpelling = true)]
        public static extern GLFW_MONITOR glfwGetWindowMonitor(GLFW_WINDOW window);

        [DllImport(Libraries.GLFW, CallingConvention = CallingConvention.Cdecl, SetLastError = false, ExactSpelling = true)]
        public static extern void glfwSetWindowMonitor(GLFW_WINDOW window , GLFW_MONITOR monitor, int xpos , int ypos , int width , int height, int refreshrate);

        [DllImport(Libraries.GLFW, CallingConvention = CallingConvention.Cdecl, SetLastError = false, ExactSpelling = true)]
        public static extern int glfwGetWindowAttrib(GLFW_WINDOW window, GLFW_WINDOW_ATTRIBUTE attribute);

        [DllImport(Libraries.GLFW, CallingConvention = CallingConvention.Cdecl, SetLastError = false, ExactSpelling = true)]
        public static extern void glfwSetWindowAttrib(GLFW_WINDOW window , GLFW_WINDOW_ATTRIBUTE attribute, int value);

        [DllImport(Libraries.GLFW, CallingConvention = CallingConvention.Cdecl, SetLastError = false, ExactSpelling = true)]
        public static extern void glfwPollEvents();

        [DllImport(Libraries.GLFW, CallingConvention = CallingConvention.Cdecl, SetLastError = false, ExactSpelling = true)]
        public static extern void glfwWaitEvents();

        [DllImport(Libraries.GLFW, CallingConvention = CallingConvention.Cdecl, SetLastError = false, ExactSpelling = true)]
        public static extern void glfwWaitEventsTimeout(double timeout);

        [DllImport(Libraries.GLFW, CallingConvention = CallingConvention.Cdecl, SetLastError = false, ExactSpelling = true)]
        public static extern void glfwPostEmptyEvent();

        [DllImport(Libraries.GLFW, CallingConvention = CallingConvention.Cdecl, SetLastError = false, ExactSpelling = true)]
        public static extern void glfwSwapBuffers(GLFW_WINDOW window);

        [DllImport(Libraries.GLFW, CallingConvention = CallingConvention.Cdecl, SetLastError = false, ExactSpelling = true)]
        public static extern int glfwGetInputMode(GLFW_WINDOW window, GLFW_INPUT_MODIFIER_OPTIONS options);

        [DllImport(Libraries.GLFW, CallingConvention = CallingConvention.Cdecl, SetLastError = false, ExactSpelling = true)]
        public static extern void glfwSetInputMode(GLFW_WINDOW window , GLFW_INPUT_MODIFIER_OPTIONS options , int value);

        [DllImport(Libraries.GLFW, CallingConvention = CallingConvention.Cdecl, SetLastError = false, ExactSpelling = true)]
        public static extern GLFW_BOOLEAN glfwRawMouseMotionSupported();

        [DllImport(Libraries.GLFW, CallingConvention = CallingConvention.Cdecl, SetLastError = false, ExactSpelling = true)]
        public static extern GLFW_KEY_PRESS_INFO glfwGetKey(GLFW_WINDOW window , GLFW_KEY key);

        [DllImport(Libraries.GLFW, CallingConvention = CallingConvention.Cdecl, SetLastError = false, ExactSpelling = true)]
        public static extern GLFW_KEY_PRESS_INFO glfwGetMouseButton(GLFW_WINDOW window , GLFW_MOUSE_BUTTON button);

        [DllImport(Libraries.GLFW, EntryPoint = "glfwGetCursorPos", CallingConvention = CallingConvention.Cdecl, SetLastError = false, ExactSpelling = true)]
        private static extern void glfwGetCursorPos_Native(GLFW_WINDOW window , double* xpos , double* ypos);

        public static void glfwGetCursorPos(GLFW_WINDOW window , out double xp , out double yp)
        {
            double xpos, ypos;
            glfwGetCursorPos_Native(window, &xpos, &ypos);
            xp = xpos;
            yp = ypos;
        }

        [DllImport(Libraries.GLFW, CallingConvention = CallingConvention.Cdecl, SetLastError = false, ExactSpelling = true)]
        public static extern void glfwSetCursorPos(GLFW_WINDOW window , double xpos , double ypos);

        [DllImport(Libraries.GLFW, EntryPoint = "glfwCreateCursor", CallingConvention = CallingConvention.Cdecl, SetLastError = false, ExactSpelling = true)]
        private static extern GLFW_CURSOR glfwCreateCursor_Native(GLFW_IMAGE* image , int hotspot_x, int hotspot_y);

        public static GLFW_CURSOR glfwCreateCursor(GLFW_IMAGE image , int hotspot_x, int hotspot_y) => glfwCreateCursor_Native(&image, hotspot_x, hotspot_y);

        [DllImport(Libraries.GLFW, CallingConvention = CallingConvention.Cdecl, SetLastError = false, ExactSpelling = true)]
        public static extern GLFW_CURSOR glfwCreateStandardCursor(GLFW_STANDARD_CURSOR_SHAPE shape);

        [DllImport(Libraries.GLFW, CallingConvention = CallingConvention.Cdecl, SetLastError = false, ExactSpelling = true)]
        public static extern void glfwDestroyCursor(GLFW_CURSOR cursor);

        [DllImport(Libraries.GLFW, CallingConvention = CallingConvention.Cdecl, SetLastError = false, ExactSpelling = true)]
        public static extern void glfwSetCursor(GLFW_WINDOW window , GLFW_CURSOR cursor);

        [DllImport(Libraries.GLFW, EntryPoint = "glfwSetKeyCallback", CallingConvention = CallingConvention.Cdecl, SetLastError = false, ExactSpelling = true)]
        private static extern delegate* unmanaged[Cdecl]<GLFW_WINDOW, GLFW_KEY, int, GLFW_KEY_PRESS_INFO, GLFW_MODIFIER_KEY_FLAGS, void> glfwSetKeyCallback(GLFW_WINDOW window , delegate* unmanaged[Cdecl]<GLFW_WINDOW, GLFW_KEY, int, GLFW_KEY_PRESS_INFO, GLFW_MODIFIER_KEY_FLAGS, void> replacewith);

        public static GLFWkeyfun glfwSetKeyCallback(GLFW_WINDOW window , GLFWkeyfun replacewith)
        {
            if (currentkeyfunction is null) {
                glfwSetKeyCallback(window, &GLFWKeyFunTranslation);
            }
            return Interlocked.Exchange(ref currentkeyfunction, replacewith);
        }

        [DllImport(Libraries.GLFW, EntryPoint = "glfwSetMouseButtonCallback", CallingConvention = CallingConvention.Cdecl, SetLastError = false, ExactSpelling = true)]
        private static extern delegate* unmanaged[Cdecl]<GLFW_WINDOW, GLFW_MOUSE_BUTTON, GLFW_KEY_PRESS_INFO, GLFW_MODIFIER_KEY_FLAGS, void> glfwSetMouseButtonCallback_Native(GLFW_WINDOW window, delegate* unmanaged[Cdecl]<GLFW_WINDOW, GLFW_MOUSE_BUTTON, GLFW_KEY_PRESS_INFO, GLFW_MODIFIER_KEY_FLAGS, void> replacewith);

        public static GLFWmousebuttonfun glfwSetMouseButtonCallback(GLFW_WINDOW window,  GLFWmousebuttonfun replacewith)
        {
            if (currentmousebuttonfunction is null) {
                glfwSetMouseButtonCallback_Native(window , &GLFWMouseButtonFunTranslation);
            }
            return Interlocked.Exchange(ref currentmousebuttonfunction, replacewith);
        }

        [DllImport(Libraries.GLFW, EntryPoint = "glfwSetScrollCallback", CallingConvention = CallingConvention.Cdecl, SetLastError = false, ExactSpelling = true)]
        private static extern delegate* unmanaged[Cdecl]<GLFW_WINDOW, double, double, void> glfwSetScrollCallback_Native(GLFW_WINDOW window, delegate* unmanaged[Cdecl]<GLFW_WINDOW, double, double, void> replacewith);

        public static GLFWscrollfun glfwSetScrollCallback(GLFW_WINDOW window , GLFWscrollfun scroll)
        {
            if (currentmousescrollfunction is null){
                glfwSetScrollCallback_Native(window, &GLFWMouseScrollFunTranslation);
            }
            return Interlocked.Exchange(ref currentmousescrollfunction, scroll);
        }

        [DllImport(Libraries.GLFW, EntryPoint = "glfwSetJoystickCallback", CallingConvention = CallingConvention.Cdecl, SetLastError = false, ExactSpelling = true)]
        private static extern delegate* unmanaged[Cdecl]<int, GLFW_CONNECTION_EVENT, void> glfwSetJoystickCallback_Native(delegate* unmanaged[Cdecl]<int, GLFW_CONNECTION_EVENT, void> replacewith);

        public static GLFWjoystickfun glfwSetJoystickCallback(GLFWjoystickfun replacewith)
        {
            if (currentjoystickfunction is null) {
                glfwSetJoystickCallback_Native(&GLFWControllerEventFunTranslation);
            }
            return Interlocked.Exchange(ref currentjoystickfunction , replacewith);
        }

        [DllImport(Libraries.GLFW, EntryPoint = "glfwGetJoystickAxes", CallingConvention = CallingConvention.Cdecl, SetLastError = false, ExactSpelling = true)]
        private static extern float* glfwGetJoystickAxes_Native(int jid, int* count);

        public static float[] glfwGetJoystickAxes(int jid)
        {
            int count;
            float* pf = glfwGetJoystickAxes_Native(jid, &count);
            if (pf is null) { return null; }
            float[] data = new float[count];
            fixed (float* p = data) {
                Unsafe.CopyBlockUnaligned(p, pf, MP.UnsafeMethods.ToUInt32(count * sizeof(float)));
            }
            return data;
        }

        [DllImport(Libraries.GLFW, EntryPoint = "glfwGetJoystickButtons", CallingConvention = CallingConvention.Cdecl, SetLastError = false, ExactSpelling = true)]
        private static extern byte* glfwGetJoystickButtons_Native(int jid, int* count);

        public static byte[] glfwGetJoystickButtons(int jid)
        {
            int count;
            byte* ps = glfwGetJoystickButtons_Native(jid, &count);
            if (ps is null) { return null; }
            byte[] data = new byte[count];
            fixed (byte* p = data) {
                Unsafe.CopyBlockUnaligned(p, ps, MP.UnsafeMethods.ToUInt32(count));
            }
            return data;
        }

        [DllImport(Libraries.GLFW, EntryPoint = "glfwGetJoystickName", CallingConvention = CallingConvention.Cdecl, SetLastError = false, ExactSpelling = true)]
        private static extern byte* glfwGetJoystickName_Native(int jid);

        public static System.String glfwGetJoystickName(int jid) => MP.UnsafeMethods.CreateUtf8NullTerminated(glfwGetJoystickName_Native(jid));

        [DllImport(Libraries.GLFW, EntryPoint = "glfwGetJoystickGUID", CallingConvention = CallingConvention.Cdecl, SetLastError = false, ExactSpelling = true)]
        private static extern byte* glfwGetJoystickGUID_Native(int jid);

        public static System.String glfwGetJoystickGUID(int jid) => MP.UnsafeMethods.CreateUtf8NullTerminated(glfwGetJoystickGUID_Native(jid));

        [DllImport(Libraries.GLFW, CallingConvention = CallingConvention.Cdecl, SetLastError = false, ExactSpelling = true)]
        public static extern GLFW_BOOLEAN glfwJoystickIsGamepad(int jid);

        [DllImport(Libraries.GLFW, EntryPoint = "glfwUpdateGamepadMappings", CallingConvention = CallingConvention.Cdecl, SetLastError = false, ExactSpelling = true)]
        private static extern GLFW_BOOLEAN glfwUpdateGamepadMappings_Native(byte* mappingsstring);

        public static GLFW_BOOLEAN glfwUpdateGamepadMappings(System.String mappings)
        {
            MP.IMemoryHandle mem = ToUTF8String(mappings);
            try {
                return glfwUpdateGamepadMappings_Native(mem.MemoryPointer);
            } finally {
                mem.Dispose();
            }
        }

        [DllImport(Libraries.GLFW, EntryPoint = "glfwGetGamepadName", CallingConvention = CallingConvention.Cdecl, SetLastError = false, ExactSpelling = true)]
        private static extern byte* glfwGetGamepadName_Native(int jid);

        public static System.String glfwGetGamepadName(int jid) => MP.UnsafeMethods.CreateUtf8NullTerminated(glfwGetGamepadName_Native(jid));

        [DllImport(Libraries.GLFW, EntryPoint = "glfwGetGamepadState", CallingConvention = CallingConvention.Cdecl, SetLastError = false, ExactSpelling = true)]
        private static extern GLFW_BOOLEAN glfwGetGamepadState_Native(int jid , GLFW_GAMEPAD_STATE* state);

        public static GLFW_BOOLEAN glfwGetGamepadState(int jid,  out GLFW_GAMEPAD_STATE state)
        {
            GLFW_GAMEPAD_STATE s;
            GLFW_BOOLEAN ret = glfwGetGamepadState_Native(jid, &s);
            state = s;
            return ret;
        }

        [DllImport(Libraries.GLFW, EntryPoint = "glfwSetClipboardString", CallingConvention = CallingConvention.Cdecl, SetLastError = false, ExactSpelling = true)]
        private static extern void glfwSetClipboardString_Native(GLFW_WINDOW window, byte* clipboardstring);

        // window parameter can be set to NULL.
        public static void glfwSetClipboardString(GLFW_WINDOW window, System.String clipboardstring)
        {
            MP.IMemoryHandle mem = ToUTF8String(clipboardstring);
            try {
                glfwSetClipboardString_Native(window, mem.MemoryPointer);
            } finally {
                mem.Dispose();
            }
        }

        [DllImport(Libraries.GLFW, EntryPoint = "glfwGetClipboardString", CallingConvention = CallingConvention.Cdecl, SetLastError = false, ExactSpelling = true)]
        private static extern byte* glfwGetClipboardString_Native(GLFW_WINDOW window);

        // window parameter can be set to NULL.
        public static System.String glfwGetClipboardString(GLFW_WINDOW window) => MP.UnsafeMethods.CreateUtf8NullTerminated(glfwGetClipboardString_Native(window));

        // It returns the number of seconds since the library was initialized with glfwInit.
        // The platform-specific time sources used typically have micro- or nanosecond resolution.
        [DllImport(Libraries.GLFW, CallingConvention = CallingConvention.Cdecl, SetLastError = false, ExactSpelling = true)]
        public static extern double glfwGetTime();

        [DllImport(Libraries.GLFW, CallingConvention = CallingConvention.Cdecl, SetLastError = false, ExactSpelling = true)]
        public static extern void glfwSetTime(double time);

        [DllImport(Libraries.GLFW, CallingConvention = CallingConvention.Cdecl, SetLastError = false, ExactSpelling = true)]
        public static extern System.UInt64 glfwGetTimerValue();

        [DllImport(Libraries.GLFW, CallingConvention = CallingConvention.Cdecl, SetLastError = false, ExactSpelling = true)]
        public static extern System.UInt64 glfwGetTimerFrequency();
    }
}