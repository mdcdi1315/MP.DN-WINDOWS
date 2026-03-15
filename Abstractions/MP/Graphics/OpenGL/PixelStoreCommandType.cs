


namespace MP.Graphics.OpenGL
{
    /// <summary>
    /// Provides constants for different glPixelStoref/i invocations.
    /// </summary>
    public enum PixelStoreCommandType : System.UInt32
    {
        /// <summary></summary>
        GL_PACK_ALIGNMENT = 0x0D05,
        /// <summary></summary>
        GL_PACK_IMAGE_HEIGHT = 0x806C,
        /// <summary></summary>
        GL_PACK_LSB_FIRST = 0x0D01,
        /// <summary></summary>
        GL_PACK_ROW_LENGTH = 0x0D02,
        /// <summary></summary>
        GL_PACK_SKIP_IMAGES = 0x806B,
        /// <summary></summary>
        GL_PACK_SKIP_PIXELS = 0x0D04,
        /// <summary></summary>
        GL_PACK_SKIP_ROWS = 0x0D03,
        /// <summary></summary>
        GL_PACK_SWAP_BYTES = 0x0D00,

        /// <summary></summary>
        GL_UNPACK_ALIGNMENT = 0x0CF5,
        /// <summary></summary>
        GL_UNPACK_IMAGE_HEIGHT = 0x806E,
        /// <summary></summary>
        GL_UNPACK_LSB_FIRST = 0x0CF1,
        /// <summary></summary>
        GL_UNPACK_ROW_LENGTH = 0x0CF2,
        /// <summary></summary>
        GL_UNPACK_SKIP_IMAGES = 0x806D,
        /// <summary></summary>
        GL_UNPACK_SKIP_PIXELS = 0x0CF4,
        /// <summary></summary>
        GL_UNPACK_SKIP_ROWS = 0x0CF3,
        /// <summary></summary>
        GL_UNPACK_SWAP_BYTES = 0x0CF0,
    }
}