
using System.Runtime.InteropServices;

namespace MP.Graphics.OpenGL
{
    /// <summary>
    /// glViewport native delegate definition.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    [OpenGLFunctionName("glViewport")]
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void GLVIEWPORT(int x , int y, int width, int height);

    /// <summary>
    /// glClear native delegate definition.
    /// </summary>
    /// <param name="bits"></param>
    [OpenGLFunctionName("glClear")]
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void GLCLEAR(BufferBits bits);

    /// <summary>
    /// glClearColor native delegate definition.
    /// </summary>
    /// <param name="red"></param>
    /// <param name="green"></param>
    /// <param name="blue"></param>
    /// <param name="alpha"></param>
    [OpenGLFunctionName("glClearColor")]
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void GLCLEARCOLOR(float red , float green , float blue , float alpha);

    /// <summary>
    /// glGenBuffers native delegate definition.
    /// </summary>
    /// <param name="buffers"></param>
    /// <param name="pbuffers"></param>
    [OpenGLFunctionName("glGenBuffers")]
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void GLGENBUFFERS(int buffers, BufferObject* pbuffers);

    /// <summary>
    /// glBindBuffer native delegate definition.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="buffer"></param>
    [OpenGLFunctionName("glBindBuffer")]
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void GLBINDBUFFER(BufferObjectType type, BufferObject buffer);

    /// <summary>
    /// glBufferData native delegate definition.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="datasize"></param>
    /// <param name="pData"></param>
    /// <param name="usage"></param>
    [OpenGLFunctionName("glBufferData")]
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void GLBUFFERDATA(BufferObjectType type , int datasize , void* pData , DataStoreUsagePattern usage);

    /// <summary>
    /// glCreateShader native delegate function.
    /// </summary>
    /// <param name="type">The type of the newly created shader.</param>
    /// <returns>The created and empty shader object.</returns>
    [OpenGLFunctionName("glCreateShader")]
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate ShaderObject GLCREATESHADER(ShaderType type);

    /// <summary>
    /// glShaderSource native delegate function.
    /// </summary>
    /// <param name="shader"></param>
    /// <param name="count"></param>
    /// <param name="stringdata"></param>
    /// <param name="lengths"></param>
    [OpenGLFunctionName("glShaderSource")]
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void GLSHADERSOURCE(ShaderObject shader, int count, byte** stringdata, int* lengths);

    /// <summary>
    /// glShaderCompile native delegate function.
    /// </summary>
    /// <param name="shader">The shader object to compile</param>
    [OpenGLFunctionName("glShaderCompile")]
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void GLSHADERCOMPILE(ShaderObject shader);

    /// <summary>
    /// glGetShaderInfoLog native delegate function.
    /// </summary>
    /// <param name="shader"></param>
    /// <param name="maxlen"></param>
    /// <param name="pused"></param>
    /// <param name="errordata"></param>
    [OpenGLFunctionName("glGetShaderInfoLog")]
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void GLGETSHADERINFOLOG(ShaderObject shader , int maxlen , int* pused , byte* errordata);

    /// <summary>
    /// glCreateProgram native delegate function.
    /// </summary>
    /// <returns></returns>
    [OpenGLFunctionName("glCreateProgram")]
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate ProgramObject GLCREATEPROGRAM();

    /// <summary>
    /// glAttachShader native delegate function.
    /// </summary>
    /// <param name="program"></param>
    /// <param name="shader"></param>
    [OpenGLFunctionName("glAttachShader")]
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void GLATTACHSHADER(ProgramObject program , ShaderObject shader);

}