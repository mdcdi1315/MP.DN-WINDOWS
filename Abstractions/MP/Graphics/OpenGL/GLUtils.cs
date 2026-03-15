
using System;
using MP.Collections;
using MP.Annotations;
using MP.NativeInterop;
using System.Collections.Generic;
using MP.Annotations.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace MP.Graphics.OpenGL
{
    /// <summary>
    /// Defines common utilities useful for when working with OpenGL contexts.
    /// </summary>
    public static unsafe class GLUtils
    {
        /// <summary>
        /// Transforms a normalized coordinate point to a window point, provided that you know the window size in pixels. <br />
        /// This is roughly the transformation that <see cref="GL.glViewport"/> does.
        /// </summary>
        /// <param name="normalized">The normalized device coordinate to transform.</param>
        /// <param name="windowsize">The window size.</param>
        /// <returns>The exact position as a window coordinate.</returns>
        // Obtained from https://registry.khronos.org/OpenGL-Refpages/gl4/html/glViewport.xhtml
        [MethodImpl(MethodImplOptions.AggressiveInlining)] // We can aggressively inline this
        public static Point NormalizedToWindowCoordinates(PointF normalized , Size windowsize) => new(
            (System.Int32)((normalized.X + 1) * (windowsize.Width / 2)),
            (System.Int32)((normalized.Y + 1) * (windowsize.Height / 2))
        );

        /// <summary>
        /// Transforms a window coordinate point to an OpenGL normalized point, provided that you know the window size in pixels. <br />
        /// This is the exact reverse transformation that <see cref="GL.glViewport"/> does, useful for passing it to shader objects.
        /// </summary>
        /// <param name="window">The window coordinate to transform.</param>
        /// <param name="windowsize">The window size in pixels.</param>
        /// <returns>The exact normalized device position.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)] // We can aggressively inline this
        public static PointF WindowToNormalizedCoordinates(Point window, Size windowsize) => new(
            ((window.X * 2f) / windowsize.Width) - 1f,
            ((window.Y * 2f) / windowsize.Height) - 1f
        );

        /// <summary>
        /// Transforms an OpenGL NDC size to a window size , provided that the exact window size is known. <br />
        /// Useful for translating controls to a window size.
        /// </summary>
        /// <param name="normalized">The normalized size instance to convert to a window size.</param>
        /// <param name="windowsize">The window size in pixels.</param>
        /// <returns>The exact window size of the specified OpenGL NDC size.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Size NormalizedToWindowSize(SizeF normalized, Size windowsize) => new(
            (System.Int32)((normalized.Width + 1) * (windowsize.Width / 2)),
            (System.Int32)((normalized.Height + 1) * (windowsize.Height / 2))
        );

        /// <summary>
        /// Translates a window size to an OpenGL NDC size , provided that the exact window size is known. <br />
        /// Useful for translating window sizes to NDC sizes at a given time.
        /// </summary>
        /// <param name="window">The window size value to convert to an OpenGL NDC size.</param>
        /// <param name="windowsize">The window size in pixels.</param>
        /// <returns>The exact OpenGL NDC size of the specified window size.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SizeF WindowToNormalizedSize(Size window, Size windowsize) => new(
            ((window.Width * 2f) / windowsize.Width) - 1f,
            ((window.Height * 2f) / windowsize.Height) - 1f
        );

        /// <summary>
        /// A utility method assisting for passing .NET strings to <see cref="GL.glShaderSource"/> function.
        /// </summary>
        /// <param name="shader">The shader object to provide the source for.</param>
        /// <param name="stringcollection">A string collection (can be just also an array of strings as well).</param>
        /// <exception cref="ArgumentNullException"><paramref name="stringcollection"/> was <see langword="null"/>.</exception>
        /// <exception cref="OutOfMemoryException">Not enough memory to create all the shader lines.</exception>
        [RequiresNativeLayer]
        [Throws(typeof(ArgumentNullException) , typeof(OutOfMemoryException))]
        public static void GLShaderSource(ShaderObject shader, ICollection<System.String> stringcollection)
        {
            ArgumentNullException.ThrowIfNull(stringcollection);
            INativeMemoryManager mhf = SystemInfo.GetDefaultMemoryManager();
            // First processing stage - identify memory handles as well as how many they are and create memory handles for all the strings.
            int[] lengths = new int[stringcollection.Count];
            IMemoryHandle[] handles = new IMemoryHandle[stringcollection.Count];
            try {
                int I = 0;
                foreach (System.String s in stringcollection)
                {
                    lengths[I] = s.Length;
                    handles[I++] = mhf.CreateUTF8StringHandle(s, false);
                }
            } catch (OutOfMemoryException) {
                // OOM occured due to the many byte* pointers we create - delete at least those we created before
                handles.DisposeAll();
                throw;
            }
            // Second processing stage - create a .NET array with all the byte pointers and pin it to make the call.
            // Finally, call in the GL function.
            try {
                fixed (int* lenpointer = lengths)
                fixed (byte** all = TranslateAsBytePointerArray(handles))
                {
                    // Call in the GL function
                    GL.glShaderSource(shader, handles.Length, all, lenpointer);
                }
            } finally {
                // Dispose EVERYTHING!!!
                handles.DisposeAll();
            }
        }

        private static byte*[] TranslateAsBytePointerArray(IMemoryHandle[] handles)
        {
            byte*[] pointers = new byte*[handles.Length];
            for (int I = 0; I < handles.Length; I++) {
                pointers[I] = handles[I].MemoryPointer;
            }
            return pointers;
        }

        /// <summary>
        /// A utility method assisting for getting the shader information log.
        /// </summary>
        /// <param name="shader">The shader to get it's information log.</param>
        /// <returns>The information log, or an empty string if no such data exist.</returns>
        public static System.String GLGetShaderInfoLog(ShaderObject shader)
        {
            int leninternal, length = GL.GLGetShaderIV(shader, ShaderParameter.GL_INFO_LOG_LENGTH);
            if (length == 0) { return System.String.Empty; }
            fixed (System.Byte* p = new System.Byte[length])
            {
                GL.glGetShaderInfoLog(shader, length, &leninternal, p);
                return new((System.SByte*)p, 0, leninternal);
            }
        }

        /// <summary>
        /// A utility method assisting for getting the program information log.
        /// </summary>
        /// <param name="program">The program to get it's information log.</param>
        /// <returns>The information log, or an empty string if no such data exist.</returns>
        public static System.String GLGetProgramInfoLog(ProgramObject program)
        {
            int leninternal, length = GL.GLGetProgramIV(program, ProgramParameter.GL_INFO_LOG_LENGTH);
            if (length == 0) { return System.String.Empty; }
            fixed (System.Byte* p = new System.Byte[length])
            {
                GL.glGetProgramInfoLog(program, length, &leninternal, p);
                return new((System.SByte*)p, 0, leninternal);
            }
        }
    
        /// <summary>
        /// A utility method for getting a program uniform variable location.
        /// </summary>
        /// <param name="program">The program to get the desired uniform.</param>
        /// <param name="name">The name of the uniform variable to get it's exact location.</param>
        /// <returns>The location of the uniform.</returns>
        public static int GLGetUniformLocation(ProgramObject program , String name)
        {
            ArgumentException.ThrowIfNullOrEmpty(name);
            int namelen = name.Length;
            System.Byte[] utf8string = new System.Byte[namelen + 1]; // NULL terminator, that's why +1.
            for (int I = 0; I < namelen; I++)
            {
                utf8string[I] = name[I].ToByte();
            }
            fixed (System.Byte* pdata = utf8string) {
                return GL.glGetUniformLocation(program, pdata);
            }
        }

        /// <summary>
        /// Passes buffer data by binding the buffer to <see cref="BufferObjectType.GL_ARRAY_BUFFER"/> and then passing them. <br />
        /// After the call returns, the buffer object is still bound.
        /// </summary>
        /// <typeparam name="T">The type of the elements to pass.</typeparam>
        /// <param name="buffer">The named buffer object.</param>
        /// <param name="dsu">The buffer usage.</param>
        /// <param name="data">The actual data to pass to OpenGL.</param>
        public static void GLBufferData<T>(BufferObject buffer, DataStoreUsagePattern dsu, T[] data)
            where T : unmanaged 
            => GLBufferData(buffer, BufferObjectType.GL_ARRAY_BUFFER , dsu, data);

        /// <summary>
        /// Passes buffer data by binding the buffer to the type specified by the <paramref name="type"/> parameter and then passing them. <br />
        /// After the call returns, the buffer object is still bound.
        /// </summary>
        /// <typeparam name="T">The type of the elements to pass.</typeparam>
        /// <param name="buffer">The named buffer object.</param>
        /// <param name="dsu">The buffer usage.</param>
        /// <param name="type">The type of the buffer to create and bind to.</param>
        /// <param name="data">The actual data to pass to OpenGL.</param>
        public static void GLBufferData<T>(BufferObject buffer, BufferObjectType type, DataStoreUsagePattern dsu, T[] data)
            where T : unmanaged
        {
            GL.glBindBuffer(type, buffer);
            fixed (T* pdata = data) {
                GL.glBufferData(type, data.Length * sizeof(T), pdata, dsu);
            }
        }

        /// <summary>
        /// Updates buffer data by binding the buffer to <see cref="BufferObjectType.GL_ARRAY_BUFFER"/> and then passing them. <br />
        /// After the call returns, the buffer object is still bound.
        /// </summary>
        /// <typeparam name="T">The type of the elements to pass.</typeparam>
        /// <param name="buffer">The named buffer object.</param>
        /// <param name="data">The actual data to pass to OpenGL.</param>
        public static void GLBufferSubData<T>(BufferObject buffer, T[] data)
            where T : unmanaged
            => GLBufferSubData(buffer , BufferObjectType.GL_ARRAY_BUFFER , data);

        /// <summary>
        /// Updates buffer data by binding the buffer to the type specified by the <paramref name="type"/> parameter and then passing them. <br />
        /// After the call returns, the buffer object is still bound.
        /// </summary>
        /// <typeparam name="T">The type of the elements to pass.</typeparam>
        /// <param name="buffer">The named buffer object.</param>
        /// <param name="type">The type of the buffer that <paramref name="buffer"/> is so as binding can succeed.</param>
        /// <param name="data">The actual data to pass to OpenGL.</param>
        public static void GLBufferSubData<T>(BufferObject buffer, BufferObjectType type, T[] data)
            where T : unmanaged
        {
            GL.glBindBuffer(type, buffer);
            fixed (T* pdata = data) {
                GL.glBufferSubData(type, 0, data.Length, pdata);
            }
        }

        /// <summary>
        /// Updates buffer data by binding the buffer to <see cref="BufferObjectType.GL_ARRAY_BUFFER"/> and then passing them. <br />
        /// After the call returns, the buffer object is still bound.
        /// </summary>
        /// <typeparam name="T">The type of the elements to pass.</typeparam>
        /// <param name="buffer">The named buffer object.</param>
        /// <param name="byteoffset">The byte offset in the existing buffer data to start overwriting from.</param>
        /// <param name="data">The actual data to pass to OpenGL.</param>
        public static void GLBufferSubData<T>(BufferObject buffer, int byteoffset, T[] data)
            where T : unmanaged
            => GLBufferSubData(buffer, BufferObjectType.GL_ARRAY_BUFFER , byteoffset , data);

        /// <summary>
        /// Updates buffer data by binding the buffer to the type specified by the <paramref name="type"/> parameter and then passing them. <br />
        /// After the call returns, the buffer object is still bound.
        /// </summary>
        /// <typeparam name="T">The type of the elements to pass.</typeparam>
        /// <param name="buffer">The named buffer object.</param>
        /// <param name="type">The type of the buffer that <paramref name="buffer"/> is so as binding can succeed.</param>
        /// <param name="byteoffset">The byte offset in the existing buffer data to start overwriting from.</param>
        /// <param name="data">The actual data to pass to OpenGL.</param>
        public static void GLBufferSubData<T>(BufferObject buffer, BufferObjectType type, int byteoffset, T[] data)
            where T : unmanaged
        {
            GL.glBindBuffer(type, buffer);
            fixed (T* pdata = data) {
                GL.glBufferSubData(type, byteoffset, data.Length, pdata);
            }
        }

        /// <summary>
        /// Passes buffer data by binding the buffer to <see cref="BufferObjectType.GL_ARRAY_BUFFER"/> and then passing them. <br />
        /// Before the call returns, the buffer object unbinds from OpenGL.
        /// </summary>
        /// <typeparam name="T">The type of the elements to pass.</typeparam>
        /// <param name="buffer">The named buffer object.</param>
        /// <param name="dsu">The buffer usage.</param>
        /// <param name="data">The actual data to pass to OpenGL.</param>
        public static void GLBufferDataUnbindAtEnd<T>(BufferObject buffer, DataStoreUsagePattern dsu, T[] data)
            where T : unmanaged
            => GLBufferDataUnbindAtEnd(buffer, BufferObjectType.GL_ARRAY_BUFFER, dsu, data);

        /// <summary>
        /// Passes buffer data by binding the buffer to the type specified by the <paramref name="type"/> parameter and then passing them. <br />
        /// Before the call returns, the buffer object unbinds from OpenGL.
        /// </summary>
        /// <typeparam name="T">The type of the elements to pass.</typeparam>
        /// <param name="buffer">The named buffer object.</param>
        /// <param name="dsu">The buffer usage.</param>
        /// <param name="type">The type of the buffer to create and bind to.</param>
        /// <param name="data">The actual data to pass to OpenGL.</param>
        public static void GLBufferDataUnbindAtEnd<T>(BufferObject buffer, BufferObjectType type, DataStoreUsagePattern dsu, T[] data)
            where T : unmanaged
        {
            GL.glBindBuffer(type, buffer);
            fixed (T* pdata = data) {
                GL.glBufferData(type, data.Length * sizeof(T), pdata, dsu);
            }
            GL.glBindBuffer(type, BufferObject.Empty); // Unbinds the buffer. See https://registry.khronos.org/OpenGL-Refpages/gl4/html/glBindBuffer.xhtml for more information.
        }

        /// <summary>
        /// Updates buffer data by binding the buffer to <see cref="BufferObjectType.GL_ARRAY_BUFFER"/> and then passing them. <br />
        /// Before the call returns, the buffer object unbinds from OpenGL.
        /// </summary>
        /// <typeparam name="T">The type of the elements to pass.</typeparam>
        /// <param name="buffer">The named buffer object.</param>
        /// <param name="data">The actual data to pass to OpenGL.</param>
        public static void GLBufferSubDataUnbindAtEnd<T>(BufferObject buffer, T[] data)
            where T : unmanaged => GLBufferSubDataUnbindAtEnd(buffer, BufferObjectType.GL_ARRAY_BUFFER, data);

        /// <summary>
        /// Updates buffer data by binding the buffer to the type specified by the <paramref name="type"/> parameter and then passing them. <br />
        /// Before the call returns, the buffer object unbinds from OpenGL.
        /// </summary>
        /// <typeparam name="T">The type of the elements to pass.</typeparam>
        /// <param name="buffer">The named buffer object.</param>
        /// <param name="data">The actual data to pass to OpenGL.</param>
        /// <param name="type">The type of the buffer that <paramref name="buffer"/> is so as binding can succeed.</param>
        public static void GLBufferSubDataUnbindAtEnd<T>(BufferObject buffer, BufferObjectType type, T[] data)
            where T : unmanaged
        {
            GL.glBindBuffer(type, buffer);
            fixed (T* pdata = data) {
                GL.glBufferSubData(type, 0, data.Length * sizeof(T), pdata);
            }
            GL.glBindBuffer(type, BufferObject.Empty); // Unbinds the buffer. See https://registry.khronos.org/OpenGL-Refpages/gl4/html/glBindBuffer.xhtml for more information.
        }

        /// <summary>
        /// Updates buffer data by binding the buffer to <see cref="BufferObjectType.GL_ARRAY_BUFFER"/> and then passing them. <br />
        /// Before the call returns, the buffer object unbinds from OpenGL.
        /// </summary>
        /// <typeparam name="T">The type of the elements to pass.</typeparam>
        /// <param name="buffer">The named buffer object.</param>
        /// <param name="byteoffset">The byte offset in the existing buffer data to start overwriting from.</param>
        /// <param name="data">The actual data to pass to OpenGL.</param>
        public static void GLBufferSubDataUnbindAtEnd<T>(BufferObject buffer, int byteoffset, T[] data)
            where T : unmanaged
            => GLBufferSubDataUnbindAtEnd(buffer, BufferObjectType.GL_ARRAY_BUFFER, byteoffset , data);

        /// <summary>
        /// Updates buffer data by binding the buffer to the type specified by the <paramref name="type"/> parameter and then passing them. <br />
        /// Before the call returns, the buffer object unbinds from OpenGL.
        /// </summary>
        /// <typeparam name="T">The type of the elements to pass.</typeparam>
        /// <param name="buffer">The named buffer object.</param>
        /// <param name="byteoffset">The byte offset in the existing buffer data to start overwriting from.</param>
        /// <param name="data">The actual data to pass to OpenGL.</param>
        /// <param name="type">The type of the buffer that <paramref name="buffer"/> is so as binding can succeed.</param>
        public static void GLBufferSubDataUnbindAtEnd<T>(BufferObject buffer, BufferObjectType type, int byteoffset, T[] data)
            where T : unmanaged
        {
            GL.glBindBuffer(type, buffer);
            fixed (T* pdata = data)
            {
                GL.glBufferSubData(type, byteoffset, data.Length * sizeof(T), pdata);
            }
            GL.glBindBuffer(type, BufferObject.Empty);  // Unbinds the buffer. See https://registry.khronos.org/OpenGL-Refpages/gl4/html/glBindBuffer.xhtml for more information.
        }

    }
}