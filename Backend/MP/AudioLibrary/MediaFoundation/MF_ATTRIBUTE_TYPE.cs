
using MP.ComInterop;

namespace MP.AudioLibrary.MediaFoundation
{
    /// <summary>
    /// MF Attributes Store value type. <br />
    /// The enum values were retrieved from the Windows 10.0.19041.0 SDK. <br />
    /// See file mfobjects.idl, line 40 for more information.
    /// </summary>
    public enum MF_ATTRIBUTE_TYPE : System.Int32
    {
        /// <summary>Represents a <see cref="System.UInt32"/> type.</summary>
        MF_ATTRIBUTE_UINT32 = VARTYPE.VT_UI4,
        /// <summary>Represents a <see cref="System.UInt64"/> type.</summary>
        MF_ATTRIBUTE_UINT64 = VARTYPE.VT_UI8,
        /// <summary>Represents a <see cref="System.Double"/> type.</summary>
        MF_ATTRIBUTE_DOUBLE = VARTYPE.VT_R8,
        /// <summary>Represents a <see cref="WindowsInterop.GUID"/> type.</summary>
        MF_ATTRIBUTE_GUID = VARTYPE.VT_CLSID,
        /// <summary>Represents a <see cref="System.String"/> type.</summary>
        MF_ATTRIBUTE_STRING = VARTYPE.VT_LPWSTR,
        /// <summary>Represents a <see cref="System.Byte[]"/> type.</summary>
        MF_ATTRIBUTE_BLOB = VARTYPE.VT_VECTOR | VARTYPE.VT_UI1,
        /// <summary>Represents a <see cref="System.__ComObject"/> type.</summary>
        MF_ATTRIBUTE_IUNKNOWN = VARTYPE.VT_UNKNOWN
    }
}