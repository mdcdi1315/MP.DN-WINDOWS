
using System;
using MP.Annotations;
using MP.Graphics.Imaging;
using MP.Graphics.OpenGL;
using MP.Graphics.Windowing.Input;
using System.Runtime.InteropServices;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace MP.Graphics.Windowing
{
    public unsafe class GLFWWindow : Window, IOpenGLFunctionLoader
    {
        private GLFWImage image;
        private GLFWCursor cursor;
        private GCHandle handletothis;
        private readonly GLFWInputState state;
        private Interop.GLFW.GLFW_WINDOW window;
        private Interop.GLFW.GLFW_CURSOR cursor_native;

        private sealed class GLFWInputState : InputDataState
        {
            private Point cursor_pos;
            private MouseButtonData mouse_data;
            private KeyboardKeyData keyboard_data;

            public void UpdateMouseState(MouseButtonData d) => mouse_data = d;

            public void UpdateKBDState(KeyboardKeyData d) => keyboard_data = d;

            public void UpdateCursorPos(double x, double y) => cursor_pos = new((int)x, (int)y);

            public override KeyboardKeyData LastKeyboardKeyState => keyboard_data;

            public override MouseButtonData LastMouseButtonState => mouse_data;

            public override Point CursorLocation => cursor_pos;
        }

        public GLFWWindow() : base()
        {
            state = new();
            cursor = null;
            window = default;
            cursor_native = default;
            Damaged = new(OnDamagedDummyMethod);
            handletothis = GCHandle.Alloc(this, GCHandleType.Normal);
        }

        public override Size Size
        {
            get {
                if (window.IsNull) {
                    return default;
                } else {
                    Interop.GLFW.glfwGetWindowSize(window, out int w, out int h);
                    return new Size(w, h);
                }
            }
        }

        public override string Title 
        { 
            get {
                if (window.IsNull) {
                    return base.Title;
                } else {
                    return base.Title = Interop.GLFW.glfwGetWindowTitle(window);
                }
            }
            set {
                base.Title = value;
                if (!window.IsNull) {
                    Interop.GLFW.glfwSetWindowTitle(window, value);
                }
            }
        }

        public void SetSize(Size s)
        {
            if (window.IsNull) { return; }
            Interop.GLFW.glfwSetWindowSize(window, s.Width, s.Height);
        }

        public Size FrameBufferSize
        {
            get {
                if (window.IsNull) { return default; }
                Interop.GLFW.glfwGetFramebufferSize(window, out int width, out int height);
                return new Size(width, height);
            }
        }

        private static Interop.GLFW.GLFW_IMAGE CreateImage(IImage img) => new() {
            ImageData = img.NativePointer,
            Height = img.Size.Height,
            Width = img.Size.Width,
        };

        public override Cursor Cursor
        { 
            get => cursor; 
            set {
                ArgumentNullException.ThrowIfNull(value);
                cursor = new(value);
                if (!cursor_native.IsNull) {
                    Interop.GLFW.glfwDestroyCursor(cursor_native);
                }
                cursor_native = Interop.GLFW.glfwCreateCursor(CreateImage(cursor), cursor.Hotspot.X, cursor.Hotspot.Y);
                if (!window.IsNull) {
                    Interop.GLFW.glfwSetCursor(window, cursor_native);
                }
            }
        }

        public override IImage Icon 
        { 
            get => image; 
            set {
                ArgumentNullException.ThrowIfNull(value);
                image = new(value);
                if (!window.IsNull) {
                    Interop.GLFW.glfwSetWindowIcon(window, new[] { CreateImage(image) });
                }
            }
        }

        public override nint Handle => new(window.Pointer);

        public override InputDataState InputState => state;

        public override void Hide()
        {
            if (window.IsNull) { return; }
            Interop.GLFW.glfwHideWindow(window);
        }

        public override void Show()
        {
            if (window.IsNull) { return; }
            Interop.GLFW.glfwShowWindow(window);
        }

        protected sealed override void Create()
        {
            try {
                GLFWLibrary.AddReference();
                Interop.GLFW.glfwWindowHint(Interop.GLFW.GLFW_WINDOW_HINT.GLFW_SRGB_CAPABLE, (int)Interop.GLFW.GLFW_BOOLEAN.GLFW_TRUE);
                GLFWWindowCreationParameters p = new();
                Create(p);
                Interop.GLFW.glfwWindowHint(Interop.GLFW.GLFW_WINDOW_HINT.GLFW_CONTEXT_VERSION_MAJOR, p.MajorContextVersion);
                Interop.GLFW.glfwWindowHint(Interop.GLFW.GLFW_WINDOW_HINT.GLFW_CONTEXT_VERSION_MINOR, p.MinorContextVersion);
                Interop.GLFW.glfwWindowHint(Interop.GLFW.GLFW_WINDOW_HINT.GLFW_CLIENT_API, (int)(p.API switch {
                    GLFWBackendAPI.OpenGL => Interop.GLFW.GLFW_API.GLFW_OPENGL_API,
                    GLFWBackendAPI.OpenGL_ES => Interop.GLFW.GLFW_API.GLFW_OPENGL_ES_API,
                    _ => Interop.GLFW.GLFW_API.GLFW_NO_API,
                }));
                Interop.GLFW.glfwWindowHint(Interop.GLFW.GLFW_WINDOW_HINT.GLFW_OPENGL_PROFILE, (int)(p.OpenGLProfile switch {
                    GLFWOpenGLProfile.OpenGLCore => Interop.GLFW.GLFW_OPENGL_PROFILE_TYPE.GLFW_OPENGL_CORE_PROFILE,
                    GLFWOpenGLProfile.OpenGLCompatibility => Interop.GLFW.GLFW_OPENGL_PROFILE_TYPE.GLFW_OPENGL_COMPAT_PROFILE,
                    _ => Interop.GLFW.GLFW_OPENGL_PROFILE_TYPE.GLFW_OPENGL_ANY_PROFILE
                }));
                Interop.GLFW.glfwWindowHint(Interop.GLFW.GLFW_WINDOW_HINT.GLFW_CONTEXT_DEBUG, (int)(p.EnableContextDebugging ? Interop.GLFW.GLFW_BOOLEAN.GLFW_TRUE : Interop.GLFW.GLFW_BOOLEAN.GLFW_FALSE));
                Interop.GLFW.glfwWindowHint(Interop.GLFW.GLFW_WINDOW_HINT.GLFW_OPENGL_FORWARD_COMPAT, (int)(p.EnableForwardCompatibility ? Interop.GLFW.GLFW_BOOLEAN.GLFW_TRUE : Interop.GLFW.GLFW_BOOLEAN.GLFW_FALSE));
                window = Interop.GLFW.glfwCreateWindow(p.Size.Width, p.Size.Height, Title ?? System.String.Empty, default, (Parent is GLFWWindow w) ? w.window : default);
                if (window.IsNull) {
                    string error;
                    Interop.GLFW.glfwGetError(out error);
                    throw new ExceptionSystem.GLFWException(error);
                } else {
                    if (!cursor_native.IsNull) {
                        Interop.GLFW.glfwSetCursor(window, cursor_native);
                    }
                    if (image is not null) {
                        Interop.GLFW.glfwSetWindowIcon(window, new[] { CreateImage(image) });
                    }
                    Interop.GLFW.glfwSetWindowUserPointer(window, GCHandle.ToIntPtr(handletothis).ToPointer());
                    Interop.GLFW.glfwSetKeyCallback(window, &DispatchKeyPressedEvent);
                    Interop.GLFW.glfwSetCursorPosCallback(window, &DispatchCursorMovedEvent);
                    Interop.GLFW.glfwSetWindowCloseCallback(window, &DispatchWindowClosingEvent);
                    Interop.GLFW.glfwSetScrollCallback(window, &DispatchScrollWheelChangedEvent);
                    Interop.GLFW.glfwSetWindowRefreshCallback(window, &DispatchWindowDamageEvent);
                    Interop.GLFW.glfwSetMouseButtonCallback(window, &DispatchMouseButtonPressedEvent);
                    Interop.GLFW.glfwMakeContextCurrent(window);
                    OnWindowReady();
                }
            } catch {
                GLFWLibrary.RemoveReference();
                if (!window.IsNull) {
                    Interop.GLFW.glfwDestroyWindow(window);
                }
                throw;
            }
        }

        protected virtual void OnWindowReady() { }

        protected virtual void Create([DisallowNull] GLFWWindowCreationParameters parameters) { }

        [RequiresNativeLayer]
        protected override void Dispose(bool disposing)
        {
            if (!disposing) { return; }
            if (handletothis.IsAllocated) { handletothis.Free(); }
            if (window.IsNull) { return; }
            Interop.GLFW.glfwDestroyWindow(window);
            if (image is not null) {
                image.Dispose();
                image = null;
            }
            if (!cursor_native.IsNull) {
                Interop.GLFW.glfwDestroyCursor(cursor_native);
            }
            GLFWLibrary.RemoveReference();
            window = default;
        }

        protected override void OnAfterDispatching()
        {
            Interop.GLFW.glfwSwapBuffers(window);
            Interop.GLFW.glfwPollEvents();
        }

        protected override bool ShouldClose() => window.IsNull || Interop.GLFW.glfwWindowShouldClose(window) == 1;

        public void* GetFunction(string function)
        {
            ArgumentNullException.ThrowIfNull(function);
            void* pf = Interop.GLFW.glfwGetProcAddress(function);
            if (pf is null) {
                throw new UnloadableOpenGLFunctionException(function);
            } else {
                return pf;
            }
        }

        #region Events and Native to Managed Event Handlers

        public event Action Damaged;

        private static void OnDamagedDummyMethod() { }

        private static GLFWWindow ResolveGLFWWindow(Interop.GLFW.GLFW_WINDOW window) => GCHandle.FromIntPtr(new(Interop.GLFW.glfwGetWindowUserPointer(window))).Target as GLFWWindow;

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        private static void DispatchWindowClosingEvent(Interop.GLFW.GLFW_WINDOW w) => ResolveGLFWWindow(w).Close();

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        private static void DispatchWindowDamageEvent(Interop.GLFW.GLFW_WINDOW w)
        {
            ResolveGLFWWindow(w).Damaged.Invoke();
            Interop.GLFW.glfwSwapBuffers(w);
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        private static void DispatchCursorMovedEvent(Interop.GLFW.GLFW_WINDOW w, double x, double y)
        {
            GLFWWindow wrapped = ResolveGLFWWindow(w);
            wrapped.OnMouseMoved(x, y);
            wrapped.state.UpdateCursorPos(x, y);
        }
        
        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        private static void DispatchScrollWheelChangedEvent(Interop.GLFW.GLFW_WINDOW w, double x, double y) => ResolveGLFWWindow(w).OnMouseScrolled(x, y);

        private static KeyboardKeyCode ToKeyboardKeyCode(Interop.GLFW.GLFW_KEY key) => key switch {
            Interop.GLFW.GLFW_KEY.GLFW_KEY_0 => KeyboardKeyCode.Zero,
            Interop.GLFW.GLFW_KEY.GLFW_KEY_1 => KeyboardKeyCode.One,
            Interop.GLFW.GLFW_KEY.GLFW_KEY_2 => KeyboardKeyCode.Two,
            Interop.GLFW.GLFW_KEY.GLFW_KEY_3 => KeyboardKeyCode.Three,
            Interop.GLFW.GLFW_KEY.GLFW_KEY_4 => KeyboardKeyCode.Four,
            Interop.GLFW.GLFW_KEY.GLFW_KEY_5 => KeyboardKeyCode.Five,
            Interop.GLFW.GLFW_KEY.GLFW_KEY_6 => KeyboardKeyCode.Six,
            Interop.GLFW.GLFW_KEY.GLFW_KEY_7 => KeyboardKeyCode.Seven,
            Interop.GLFW.GLFW_KEY.GLFW_KEY_8 => KeyboardKeyCode.Eight,
            Interop.GLFW.GLFW_KEY.GLFW_KEY_9 => KeyboardKeyCode.Nine,
            Interop.GLFW.GLFW_KEY.GLFW_KEY_APOSTROPHE => KeyboardKeyCode.Apostrophe,
            Interop.GLFW.GLFW_KEY.GLFW_KEY_B => KeyboardKeyCode.B,
            Interop.GLFW.GLFW_KEY.GLFW_KEY_BACKSLASH => KeyboardKeyCode.BackSlash,
            Interop.GLFW.GLFW_KEY.GLFW_KEY_BACKSPACE => KeyboardKeyCode.BackSpace,
            Interop.GLFW.GLFW_KEY.GLFW_KEY_C => KeyboardKeyCode.C,
            Interop.GLFW.GLFW_KEY.GLFW_KEY_COMMA => KeyboardKeyCode.Comma,
            Interop.GLFW.GLFW_KEY.GLFW_KEY_D => KeyboardKeyCode.D,
            Interop.GLFW.GLFW_KEY.GLFW_KEY_DELETE => KeyboardKeyCode.Delete,
            Interop.GLFW.GLFW_KEY.GLFW_KEY_DOWN => KeyboardKeyCode.ArrowDown,
            Interop.GLFW.GLFW_KEY.GLFW_KEY_E => KeyboardKeyCode.E,
            Interop.GLFW.GLFW_KEY.GLFW_KEY_END => KeyboardKeyCode.End,
            Interop.GLFW.GLFW_KEY.GLFW_KEY_ENTER => KeyboardKeyCode.Enter,
            Interop.GLFW.GLFW_KEY.GLFW_KEY_EQUAL => KeyboardKeyCode.Equal,
            Interop.GLFW.GLFW_KEY.GLFW_KEY_ESCAPE => KeyboardKeyCode.Escape,
            Interop.GLFW.GLFW_KEY.GLFW_KEY_F => KeyboardKeyCode.F,
            Interop.GLFW.GLFW_KEY.GLFW_KEY_F1 => KeyboardKeyCode.F1,
            Interop.GLFW.GLFW_KEY.GLFW_KEY_F2 => KeyboardKeyCode.F2,
            Interop.GLFW.GLFW_KEY.GLFW_KEY_F3 => KeyboardKeyCode.F3,
            Interop.GLFW.GLFW_KEY.GLFW_KEY_F4 => KeyboardKeyCode.F4,
            Interop.GLFW.GLFW_KEY.GLFW_KEY_F5 => KeyboardKeyCode.F5,
            Interop.GLFW.GLFW_KEY.GLFW_KEY_F6 => KeyboardKeyCode.F6,
            Interop.GLFW.GLFW_KEY.GLFW_KEY_F7 => KeyboardKeyCode.F7,
            Interop.GLFW.GLFW_KEY.GLFW_KEY_F8 => KeyboardKeyCode.F8,
            Interop.GLFW.GLFW_KEY.GLFW_KEY_F9 => KeyboardKeyCode.F9,
            Interop.GLFW.GLFW_KEY.GLFW_KEY_F10 => KeyboardKeyCode.F10,
            Interop.GLFW.GLFW_KEY.GLFW_KEY_F11 => KeyboardKeyCode.F11,
            Interop.GLFW.GLFW_KEY.GLFW_KEY_F12 => KeyboardKeyCode.F12,
            Interop.GLFW.GLFW_KEY.GLFW_KEY_F13 => KeyboardKeyCode.F13,
            Interop.GLFW.GLFW_KEY.GLFW_KEY_F14 => KeyboardKeyCode.F14,
            Interop.GLFW.GLFW_KEY.GLFW_KEY_F15 => KeyboardKeyCode.F15,
            Interop.GLFW.GLFW_KEY.GLFW_KEY_F16 => KeyboardKeyCode.F16,
            Interop.GLFW.GLFW_KEY.GLFW_KEY_F17 => KeyboardKeyCode.F17,
            Interop.GLFW.GLFW_KEY.GLFW_KEY_F18 => KeyboardKeyCode.F18,
            Interop.GLFW.GLFW_KEY.GLFW_KEY_F19 => KeyboardKeyCode.F19,
            Interop.GLFW.GLFW_KEY.GLFW_KEY_F20 => KeyboardKeyCode.F20,
            Interop.GLFW.GLFW_KEY.GLFW_KEY_F21 => KeyboardKeyCode.F21,
            Interop.GLFW.GLFW_KEY.GLFW_KEY_F22 => KeyboardKeyCode.F22,
            Interop.GLFW.GLFW_KEY.GLFW_KEY_F23 => KeyboardKeyCode.F23,
            Interop.GLFW.GLFW_KEY.GLFW_KEY_G => KeyboardKeyCode.G,
            Interop.GLFW.GLFW_KEY.GLFW_KEY_GRAVE_ACCENT => KeyboardKeyCode.GraveAccent,
            Interop.GLFW.GLFW_KEY.GLFW_KEY_H => KeyboardKeyCode.H,
            Interop.GLFW.GLFW_KEY.GLFW_KEY_HOME => KeyboardKeyCode.Home,
            Interop.GLFW.GLFW_KEY.GLFW_KEY_I => KeyboardKeyCode.I,
            Interop.GLFW.GLFW_KEY.GLFW_KEY_INSERT => KeyboardKeyCode.Insert,
            Interop.GLFW.GLFW_KEY.GLFW_KEY_J => KeyboardKeyCode.J,
            Interop.GLFW.GLFW_KEY.GLFW_KEY_K => KeyboardKeyCode.K,
            Interop.GLFW.GLFW_KEY.GLFW_KEY_L => KeyboardKeyCode.L,
            Interop.GLFW.GLFW_KEY.GLFW_KEY_LEFT => KeyboardKeyCode.ArrowLeft,
            Interop.GLFW.GLFW_KEY.GLFW_KEY_LEFT_ALT => KeyboardKeyCode.LeftAlt,
            Interop.GLFW.GLFW_KEY.GLFW_KEY_LEFT_BRACKET => KeyboardKeyCode.LeftBracket,
            Interop.GLFW.GLFW_KEY.GLFW_KEY_LEFT_CONTROL => KeyboardKeyCode.LeftControl,
            Interop.GLFW.GLFW_KEY.GLFW_KEY_LEFT_SHIFT => KeyboardKeyCode.LeftShift,
            Interop.GLFW.GLFW_KEY.GLFW_KEY_M => KeyboardKeyCode.M,
            Interop.GLFW.GLFW_KEY.GLFW_KEY_MINUS => KeyboardKeyCode.Minus,
            Interop.GLFW.GLFW_KEY.GLFW_KEY_N => KeyboardKeyCode.N,
            Interop.GLFW.GLFW_KEY.GLFW_KEY_O => KeyboardKeyCode.O,
            Interop.GLFW.GLFW_KEY.GLFW_KEY_P => KeyboardKeyCode.P,
            Interop.GLFW.GLFW_KEY.GLFW_KEY_PAGE_DOWN => KeyboardKeyCode.PageDown,
            Interop.GLFW.GLFW_KEY.GLFW_KEY_PAGE_UP => KeyboardKeyCode.PageUp,
            Interop.GLFW.GLFW_KEY.GLFW_KEY_PAUSE => KeyboardKeyCode.Pause,
            Interop.GLFW.GLFW_KEY.GLFW_KEY_PERIOD => KeyboardKeyCode.Period,
            Interop.GLFW.GLFW_KEY.GLFW_KEY_PRINT_SCREEN => KeyboardKeyCode.PrintScreen,
            Interop.GLFW.GLFW_KEY.GLFW_KEY_Q => KeyboardKeyCode.Q,
            Interop.GLFW.GLFW_KEY.GLFW_KEY_R => KeyboardKeyCode.R,
            Interop.GLFW.GLFW_KEY.GLFW_KEY_RIGHT => KeyboardKeyCode.ArrowRight,
            Interop.GLFW.GLFW_KEY.GLFW_KEY_RIGHT_ALT => KeyboardKeyCode.RightAlt,
            Interop.GLFW.GLFW_KEY.GLFW_KEY_RIGHT_BRACKET => KeyboardKeyCode.RightBracket,
            Interop.GLFW.GLFW_KEY.GLFW_KEY_RIGHT_CONTROL => KeyboardKeyCode.RightControl,
            Interop.GLFW.GLFW_KEY.GLFW_KEY_RIGHT_SHIFT => KeyboardKeyCode.RightShift,
            Interop.GLFW.GLFW_KEY.GLFW_KEY_S => KeyboardKeyCode.S,
            Interop.GLFW.GLFW_KEY.GLFW_KEY_SEMICOLON => KeyboardKeyCode.SemiColon,
            Interop.GLFW.GLFW_KEY.GLFW_KEY_SLASH => KeyboardKeyCode.ForwardSlash,
            Interop.GLFW.GLFW_KEY.GLFW_KEY_SPACE => KeyboardKeyCode.Space,
            Interop.GLFW.GLFW_KEY.GLFW_KEY_T => KeyboardKeyCode.T,
            Interop.GLFW.GLFW_KEY.GLFW_KEY_TAB => KeyboardKeyCode.Tab,
            Interop.GLFW.GLFW_KEY.GLFW_KEY_U => KeyboardKeyCode.U,
            Interop.GLFW.GLFW_KEY.GLFW_KEY_UP => KeyboardKeyCode.ArrowUp,
            Interop.GLFW.GLFW_KEY.GLFW_KEY_V => KeyboardKeyCode.V,
            Interop.GLFW.GLFW_KEY.GLFW_KEY_W => KeyboardKeyCode.W,
            Interop.GLFW.GLFW_KEY.GLFW_KEY_X => KeyboardKeyCode.X,
            Interop.GLFW.GLFW_KEY.GLFW_KEY_Y => KeyboardKeyCode.Y,
            Interop.GLFW.GLFW_KEY.GLFW_KEY_Z => KeyboardKeyCode.Z,
            _ => KeyboardKeyCode.Unmapped
        };

        private static KeyState ToKeyState(Interop.GLFW.GLFW_KEY_PRESS_INFO kp) => kp switch { 
            Interop.GLFW.GLFW_KEY_PRESS_INFO.GLFW_PRESS => KeyState.Pressed,
            Interop.GLFW.GLFW_KEY_PRESS_INFO.GLFW_REPEAT => KeyState.Held,
            _ => KeyState.Released,
        };

        private static MouseButtonCode ToMouseButtonCode(Interop.GLFW.GLFW_MOUSE_BUTTON button) => button switch {
            Interop.GLFW.GLFW_MOUSE_BUTTON.GLFW_MOUSE_BUTTON_1 => MouseButtonCode.Left,
            Interop.GLFW.GLFW_MOUSE_BUTTON.GLFW_MOUSE_BUTTON_2 => MouseButtonCode.Right,
            Interop.GLFW.GLFW_MOUSE_BUTTON.GLFW_MOUSE_BUTTON_3 => MouseButtonCode.Middle,
            Interop.GLFW.GLFW_MOUSE_BUTTON.GLFW_MOUSE_BUTTON_4 => MouseButtonCode.Extra0,
            Interop.GLFW.GLFW_MOUSE_BUTTON.GLFW_MOUSE_BUTTON_5 => MouseButtonCode.Extra1,
            Interop.GLFW.GLFW_MOUSE_BUTTON.GLFW_MOUSE_BUTTON_6 => MouseButtonCode.Extra2,
            Interop.GLFW.GLFW_MOUSE_BUTTON.GLFW_MOUSE_BUTTON_7 => MouseButtonCode.Extra3,
            Interop.GLFW.GLFW_MOUSE_BUTTON.GLFW_MOUSE_BUTTON_8 => MouseButtonCode.Extra4,
            _ => MouseButtonCode.Undefined,
        };
 
        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        private static void DispatchKeyPressedEvent(Interop.GLFW.GLFW_WINDOW native, Interop.GLFW.GLFW_KEY key, int scancode, Interop.GLFW.GLFW_KEY_PRESS_INFO p, Interop.GLFW.GLFW_MODIFIER_KEY_FLAGS modifiers)
        {
            if (scancode > System.Char.MaxValue) { return; }
            GLFWWindow wrapped = ResolveGLFWWindow(native);
            KeyboardKeyData data = new(ToKeyboardKeyCode(key), ToKeyState(p), scancode.ToChar());
            try {
                wrapped.OnKeyboardKeyStateChanged(data);
            } finally {
                wrapped.state.UpdateKBDState(data);
            }
        }
        
        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        private static void DispatchMouseButtonPressedEvent(Interop.GLFW.GLFW_WINDOW native, Interop.GLFW.GLFW_MOUSE_BUTTON button, Interop.GLFW.GLFW_KEY_PRESS_INFO p, Interop.GLFW.GLFW_MODIFIER_KEY_FLAGS modifiers)
        {
            GLFWWindow wrapped = ResolveGLFWWindow(native);
            MouseButtonData mbd = new(ToMouseButtonCode(button), p != Interop.GLFW.GLFW_KEY_PRESS_INFO.GLFW_RELEASE);
            try {
                wrapped.OnMouseButtonStateChanged(mbd);
            } finally {
                wrapped.state.UpdateMouseState(mbd);
            }
        }

        #endregion
    }
}