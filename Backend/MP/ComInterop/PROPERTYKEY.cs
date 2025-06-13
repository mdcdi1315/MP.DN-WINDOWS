using MP.WindowsInterop;
using System;
using System.Runtime.InteropServices;

namespace MP.ComInterop
{
    /// <summary>
    /// PROPERTYKEY is defined in wtypes.h
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public struct PROPERTYKEY
    {
        /// <summary>Format ID</summary>
        [FieldOffset(0)]
        public GUID FORMATID;
        /// <summary>Property ID</summary>
        [FieldOffset(16)]
        public System.UInt32 PROPERTYID;
        
        /// <summary>
        /// Creates a new property key , based on the key's format ID and it's property ID.
        /// </summary>
        /// <param name="formatId">The format ID of the key</param>
        /// <param name="propertyId">The property ID of the key</param>
        public PROPERTYKEY(Guid formatId, System.Int32 propertyId) : this(GUID.FromGUID(formatId), propertyId.ToUInt32()) { }

        /// <summary>
        /// Creates a new property key , based on the key's format ID and it's property ID.
        /// </summary>
        /// <param name="formatid">The format ID of the key</param>
        /// <param name="propid">The property ID of the key</param>
        public PROPERTYKEY(GUID formatid , System.Int32 propid) : this(formatid , propid.ToUInt32()) { }

        /// <summary>
        /// Creates a new property key , based on the key's format ID and it's property ID.
        /// </summary>
        /// <param name="formatid">The format ID of the key</param>
        /// <param name="propid">The property ID of the key</param>
        public PROPERTYKEY(GUID formatid , System.UInt32 propid)
        {
            FORMATID = formatid;
            PROPERTYID = propid;
        }

        public static PROPERTYKEY FromPropertyKeyString(System.String pkey)
        {
            if (pkey is null) { throw new System.ArgumentNullException(nameof(pkey)); }
            PROPERTYKEY pret;
            HRESULT hrt = Interop.PropSys.PSPropertyKeyFromString(pkey , out pret);
            if (hrt.SUCCEEDED) {
                return pret;
            } else {
                throw hrt.MappingException;
            }
        }

        public readonly System.String PropertyName
        {
            get {
                System.String ret;
                HRESULT hrt = Interop.PropSys.PSGetNameFromPropertyKey(this , out ret);
                if (hrt.SUCCEEDED) {
                    return ret;
                } else {
                    throw hrt.MappingException;
                }
            }
        }

        public readonly System.String PropertyStorageString
        {
            get {
                System.String ret;
                HRESULT hrt = Interop.PropSys.PSStringFromPropertyKey(this , out ret);
                if (hrt.SUCCEEDED) {
                    return ret;
                } else {
                    throw hrt.MappingException;
                }
            }
        }

        public readonly override System.String ToString()
        {
            try {
                return PropertyStorageString;
            } catch {
                return $"{{{FORMATID}}} {PROPERTYID}";
            }
        }
    }
}
