

namespace MP.Graphics.OpenGL
{
    /// <summary>
    /// Defines different OpenGL capabilities.
    /// </summary>
    public enum GLCapability : System.UInt32
    {
        /// <summary>
        /// If enabled, draw lines with correct filtering. Otherwise, draw aliased lines. See <see cref="GL.glLineWidth"/>.
        /// </summary>
        GL_LINE_SMOOTH = 0x0B20,
        /// <summary>
        /// If enabled, draw polygons with proper filtering. <br />
        /// Otherwise, draw aliased polygons.  <br />
        /// For correct antialiased polygons, an alpha buffer is needed and the polygons must be sorted front to back.
        /// </summary>
        GL_POLYGON_SMOOTH = 0x0B41,
        /// <summary>
        /// If enabled, cull polygons based on their winding in window coordinates. See <c>glCullFace</c>.
        /// </summary>
        GL_CULL_FACE = 0x0B44,
        /// <summary>
        /// If enabled, do depth comparisons and update the depth buffer. Note that even if the depth buffer exists and the depth mask is non-zero, the depth buffer is not updated if the depth test is disabled. See <see href="https://registry.khronos.org/OpenGL-Refpages/gl4/html/glDepthFunc.xhtml">glDepthFunc</see> and <see href="https://registry.khronos.org/OpenGL-Refpages/gl4/html/glDepthRange.xhtml">glDepthRange</see>.
        /// </summary>
        GL_DEPTH_TEST = 0x0B71,
        /// <summary>
        /// If enabled, do stencil testing and update the stencil buffer. See glStencilFunc and glStencilOp.
        /// </summary>
        GL_STENCIL_TEST = 0x0B90,
        /// <summary>
        /// If enabled, dither color components or indices before they are written to the color buffer.
        /// </summary>
        GL_DITHER = 0x0BD0,
        /// <summary>
        /// If enabled, blend the computed fragment color values with the values in the color buffers. See <c>glBlendFunc</c>.
        /// </summary>
        GL_BLEND = 0x0BE2,
        /// <summary>
        /// If enabled, apply the currently selected logical operation to the computed fragment color and color buffer values. See <c>glLogicOp</c>.
        /// </summary>
        GL_COLOR_LOGIC_OP = 0x0BF2,
        /// <summary>
        /// If enabled, discard fragments that are outside the scissor rectangle. <br />
        /// See <see href="https://registry.khronos.org/OpenGL-Refpages/gl4/html/glScissor.xhtml">glScissor</see> or <see cref="GL.glScissor"/>.
        /// </summary>
        GL_SCISSOR_TEST = 0x0C11,
        /// <summary>
        /// If enabled, the fragment's coverage is ANDed with the temporary coverage value. 
        /// If <see cref="GL_SAMPLE_COVERAGE_INVERT"/> is set to <see cref="GLConstants.GL_TRUE"/>, invert the coverage value. See <see cref="GL.glSampleCoverage"/>.
        /// </summary>
        GL_SAMPLE_COVERAGE = 0x80A0,
        /// <summary>
        /// The inverted result of <see cref="GL_SAMPLE_COVERAGE"/>. <br />
        /// See <see cref="GL.glSampleCoverage"/>.
        /// </summary>
        GL_SAMPLE_COVERAGE_INVERT = 0x80AB,
        /// <summary>
        /// If enabled, use multiple fragment samples in computing the final color of a pixel. See <see cref="GL.glSampleCoverage"/>.
        /// </summary>
        GL_MULTISAMPLE = 0x809D,
        /// <summary>
        /// If enabled, compute a temporary coverage value where each bit is determined by the alpha value at the corresponding sample location. <br />
        /// The temporary coverage value is then ANDed with the fragment coverage value.
        /// </summary>
        GL_SAMPLE_ALPHA_TO_COVERAGE = 0x809E,
        /// <summary>
        /// If enabled, compute a temporary coverage value where each bit is determined by the alpha value at the corresponding sample location. <br />
        /// The temporary coverage value is then ANDed with the fragment coverage value.
        /// </summary>
        GL_SAMPLE_ALPHA_TO_ONE = 0x809F,
        /// <summary>
        /// If enabled and a vertex or geometry shader is active, then the derived point size is taken from the (potentially clipped) shader builtin gl_PointSize and clamped to the implementation-dependent point size range.
        /// </summary>
        GL_PROGRAM_POINT_SIZE = 0x8642,
        /// <summary>
        /// If enabled, the −wc≤zc≤wc plane equation is ignored by view volume clipping (effectively, there is no near or far plane clipping). See <see href="https://registry.khronos.org/OpenGL-Refpages/gl4/html/glDepthRange.xhtml">glDepthRange</see>.
        /// </summary>
        GL_DEPTH_CLAMP = 0x864F,
        /// <summary>
        /// If enabled, cubemap textures are sampled such that when linearly sampling from the border between two adjacent faces, texels from both faces are used to generate the final sample value. <br />
        /// When disabled, texels from only a single face are used to construct the final sample value.
        /// </summary>
        GL_TEXTURE_CUBE_MAP_SEAMLESS = 0x884F,
        /// <summary>
        /// If enabled, primitives are discarded after the optional transform feedback stage, but before rasterization. <br />
        /// Furthermore, when enabled, <see cref="GL.glClear"/>, glClearBufferData, glClearBufferSubData, glClearTexImage, and glClearTexSubImage are ignored.
        /// </summary>
        GL_RASTERIZER_DISCARD = 0x8C89,
        /// <summary>
        /// If enabled and the value of <c>GL_FRAMEBUFFER_ATTACHMENT_COLOR_ENCODING</c> for the framebuffer attachment corresponding to the destination buffer is GL_SRGB, the R, G, and B 
        /// destination color values (after conversion from fixed-point to floating-point) are considered to be encoded for the sRGB color space and hence are linearized prior to their use in blending.
        /// </summary>
        GL_FRAMEBUFFER_SRGB = 0x8DB9,
        /// <summary>
        /// If enabled, the sample coverage mask generated for a fragment during rasterization will be ANDed with the value of GL_SAMPLE_MASK_VALUE before shading occurs. See <see href="https://registry.khronos.org/OpenGL-Refpages/gl4/html/glSampleMaski.xhtml">glSampleMaski</see>.
        /// </summary>
        GL_SAMPLE_MASK = 0x8E51,
        /// <summary>
        /// Enables primitive restarting. <br />
        /// If enabled, any one of the draw commands which transfers a set of generic attribute array elements to the GL will restart the primitive when the index of the vertex is equal to the primitive restart index.  <br />
        /// See <see cref="GL.glPrimitiveRestartIndex"/>.
        /// </summary>
        GL_PRIMITIVE_RESTART = 0x8F9D,
        
    }
}