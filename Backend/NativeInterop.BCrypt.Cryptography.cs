

using MP;
using System;
using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

partial class Interop
{
    public static unsafe partial class BCrypt
    {
        // Algorithm implementations
        public const string BCRYPT_AES_ALGORITHM = "AES";

        // Cipher modes
        public const string BCRYPT_CHAIN_MODE_CBC = "ChainingModeCBC";
        public const string BCRYPT_CHAIN_MODE_ECB = "ChainingModeECB";
        public const string BCRYPT_CHAIN_MODE_GCM = "ChainingModeGCM";
        public const string BCRYPT_CHAIN_MODE_CFB = "ChainingModeCFB";
        public const string BCRYPT_CHAIN_MODE_CCM = "ChainingModeCCM";

        // BCryptGet/SetProperty keys
        public const System.String BCRYPT_IS_REUSABLE_HASH = "IsReusableHash";
        public const System.String BCRYPT_IS_KEYED_HASH = "IsKeyedHash";
        public const System.String BCRYPT_ALGORITHM_NAME = "AlgorithmName";
        public const System.String BCRYPT_HASH_LENGTH = "HashDigestLength";
        public const System.String BCRYPT_OBJECT_LENGTH = "ObjectLength";
        public const System.String BCRYPT_KEY_DATA_BLOB = "KeyDataBlob";
        public const System.String BCRYPT_CHAINING_MODE = "ChainingMode";
        public const System.String BCRYPT_MESSAGE_BLOCK_LENGTH = "MessageBlockLength";
        public const System.String BCRYPT_EFFECTIVE_KEY_LENGTH = "EffectiveKeyLength";

        [Flags]
        public enum OpenAlgorithmProviderFlags : System.UInt32
        {
            None = 0,
            BCRYPT_ALG_HANDLE_HMAC_FLAG = 0x00000008,
            BCRYPT_HASH_REUSABLE_FLAG = 0x00000020
        }

        [Flags]
        public enum EncryptDecryptFlags : System.UInt32
        {
            None = 0,
            BCRYPT_BLOCK_PADDING = 0x00000001
        }

        [StructLayout(LayoutKind.Explicit, Pack = 4)]
        public struct BCRYPT_KEY_DATA_BLOB_HEADER
        {
            [FieldOffset(0)]
            public System.UInt32 Magic;
            [FieldOffset(4)]
            public System.UInt32 Version;
            [FieldOffset(8)]
            public System.UInt32 KeyDataLength;

            public BCRYPT_KEY_DATA_BLOB_HEADER()
            {
                Magic = BCRYPT_KEY_DATA_BLOB_MAGIC;
                Version = BCRYPT_KEY_DATA_BLOB_VERSION1;
            }

            public static SafeLibcMemoryHandle CreateKeyBlob(System.ReadOnlySpan<System.Byte> key)
            {
                if (key.IsEmpty) { throw new ArgumentNullException(nameof(key)); }
                SafeLibcMemoryHandle mem = new(sizeof(BCRYPT_KEY_DATA_BLOB_HEADER) + key.Length);
                BCRYPT_KEY_DATA_BLOB_HEADER* header = (BCRYPT_KEY_DATA_BLOB_HEADER*)mem.MemoryPointer;
                header->Magic = BCRYPT_KEY_DATA_BLOB_MAGIC;
                header->Version = BCRYPT_KEY_DATA_BLOB_VERSION1;
                header->KeyDataLength = key.Length.ToUInt32();
                fixed (System.Byte* pk = key)
                {
                    Unsafe.CopyBlockUnaligned(mem.MemoryPointer + sizeof(BCRYPT_KEY_DATA_BLOB_HEADER), pk, header->KeyDataLength);
                }
                return mem;
            }

            public const System.UInt32 BCRYPT_KEY_DATA_BLOB_MAGIC = 0x4d42444b;
            public const System.UInt32 BCRYPT_KEY_DATA_BLOB_VERSION1 = 0x1;
        }

        [DllImport(Libraries.BCrypt, EntryPoint = "BCryptOpenAlgorithmProvider", ExactSpelling = true)]
        private static extern NTSTATUS BCryptOpenAlgorithmProvider_Native(System.IntPtr* halg, System.Char* algname, System.Char* algimpl, OpenAlgorithmProviderFlags flags);

        public static NTSTATUS BCryptOpenAlgorithmProvider(System.String algname, System.String algimplementation, OpenAlgorithmProviderFlags flags, out System.IntPtr halgorithm)
        {
            if (algimplementation is not null) { algimplementation += "\0"; }
            algname += "\0";
            System.IntPtr outalg;
            fixed (System.Char* pname = algname)
            fixed (System.Char* pi = algimplementation)
            {
                NTSTATUS nts = BCryptOpenAlgorithmProvider_Native(&outalg, pname, pi, flags);
                halgorithm = outalg;
                return nts;
            }
        }

        [DllImport(Libraries.BCrypt, EntryPoint = "BCryptCloseAlgorithmProvider", ExactSpelling = true)]
        public static extern NTSTATUS BCryptCloseAlgorithmProvider(System.IntPtr halg, System.UInt32 flagsnotdefined = 0);

        [DllImport(Libraries.BCrypt, EntryPoint = "BCryptCreateHash", ExactSpelling = true)]
        private static extern NTSTATUS BCryptCreateHash_Native(System.IntPtr halg, System.IntPtr* phashalg, void* hashobjunused, System.UInt32 hashobjlen, System.Byte* psecretbuf, System.UInt32 secbuflen, OpenAlgorithmProviderFlags flags);

        public static NTSTATUS BCryptCreateHash(System.IntPtr inalg, out System.IntPtr hashalg, System.Byte[] secretkey = null)
        {
            NTSTATUS nts;
            System.IntPtr outalg;
            if (secretkey is null) {
                nts = BCryptCreateHash_Native(inalg, &outalg, null, 0, null, 0, OpenAlgorithmProviderFlags.None);
            } else {
                fixed (System.Byte* pbf = secretkey)
                {
                    nts = BCryptCreateHash_Native(inalg, &outalg, null, 0, pbf, secretkey.Length.ToUInt32(), OpenAlgorithmProviderFlags.None);
                }
            }
            hashalg = outalg;
            return nts;
        }

        [DllImport(Libraries.BCrypt, EntryPoint = "BCryptHashData", ExactSpelling = true)]
        private static extern NTSTATUS BCryptHashData_Native(System.IntPtr halg, System.Byte* pbytes, System.UInt32 pbyteslen, System.UInt32 flagssettozero = 0);

        public static NTSTATUS BCryptHashData(System.IntPtr halg, System.ReadOnlySpan<System.Byte> buffer)
        {
            fixed (System.Byte* pbytes = buffer)
            {
                return BCryptHashData_Native(halg, pbytes, buffer.Length.ToUInt32());
            }
        }

        [DllImport(Libraries.BCrypt, EntryPoint = "BCryptGetProperty", ExactSpelling = true)]
        private static extern NTSTATUS BCryptGetProperty_Native(System.IntPtr halg, System.Char* pproperty, void* pout, System.UInt32 poutlen, System.UInt32* poutwritten, System.UInt32 flags = 0);

        public static NTSTATUS BCryptGetProperty_UINT32(System.IntPtr halg, System.String prop, out System.UInt32 propvalue)
        {
            if (prop is null) { throw new ArgumentNullException(nameof(prop)); }
            prop += "\0";
            System.UInt32 pv, pw;
            NTSTATUS nts;
            fixed (System.Char* pproperty = prop)
            {
                nts = BCryptGetProperty_Native(halg, pproperty, &pv, sizeof(System.UInt32).ToUInt32(), &pw, 0);
            }
            propvalue = pv;
            return nts;
        }

        public static NTSTATUS BCryptGetProperty_String(System.IntPtr halg, System.String prop, out System.String str)
        {
            if (prop is null) { throw new ArgumentNullException(nameof(prop)); }
            prop += "\0";
            System.UInt32 pw;
            NTSTATUS nts;
            fixed (System.Char* pproperty = prop)
            {
                nts = BCryptGetProperty_Native(halg, pproperty, null, 0, &pw, 0);
                // pw parameter contains the size of the string in bytes (assuming that the native call was first succeeded)
                if (nts == NTSTATUS.STATUS_SUCCESS)
                {
                    str = new('\0', (pw / sizeof(System.Char)).ToInt32());
                    fixed (System.Char* pvalue = str)
                    {
                        System.UInt32 pw2;
                        nts = BCryptGetProperty_Native(halg, pproperty, pvalue, pw, &pw2, 0);
                    }
                } else {
                    str = null;
                }
            }
            return nts;
        }

        [DllImport(Libraries.BCrypt, EntryPoint = "BCryptFinishHash", ExactSpelling = true)]
        private static extern NTSTATUS BCryptFinishHash_Native(System.IntPtr halg, System.Byte* pdata, System.UInt32 pdlen, System.UInt32 flags = 0);

        public static NTSTATUS BCryptFinishHash(System.IntPtr halg, System.Span<System.Byte> finaldata)
        {
            fixed (System.Byte* pdata = finaldata)
            {
                return BCryptFinishHash_Native(halg, pdata, finaldata.Length.ToUInt32(), 0);
            }
        }

        [DllImport(Libraries.BCrypt, EntryPoint = "BCryptDestroyHash", ExactSpelling = true)]
        public static extern NTSTATUS BCryptDestroyHash(System.IntPtr halg);

        [DllImport(Libraries.BCrypt, EntryPoint = "BCryptDuplicateHash", ExactSpelling = true)]
        private static extern NTSTATUS BCryptDuplicateHash_Native(System.IntPtr halg, System.IntPtr* pduped, void* hobj, System.UInt32 hobjval, System.UInt32 flags = 0);

        public static NTSTATUS BCryptDuplicateHash(System.IntPtr halg, out System.IntPtr duped)
        {
            System.IntPtr dup;
            NTSTATUS nts = BCryptDuplicateHash_Native(halg, &dup, null, 0, 0);
            duped = dup;
            return nts;
        }

        [DllImport(Libraries.BCrypt, EntryPoint = "BCryptImportKey", ExactSpelling = true)]
        private static extern NTSTATUS BCryptImportKey_Native(System.IntPtr halg, System.IntPtr himportkeynotused, System.Char* blobtype, System.IntPtr* pbcryptencdec, System.Byte* pkeyobj, System.UInt32 pkeyobjsize, System.Byte* pkey, System.UInt32 pkeylen, System.UInt32 flags = 0);

        public static NTSTATUS BCryptImportKey(System.IntPtr halg, System.String blobtype, SafeLibcMemoryHandle keyobject, out System.IntPtr hencdec)
        {
            if (keyobject is null) { throw new ArgumentNullException(nameof(keyobject)); }
            blobtype += "\0";
            NTSTATUS nts;
            System.IntPtr pout;
            fixed (System.Char* pblobtype = blobtype)
            {
                nts = BCryptImportKey_Native(halg, System.IntPtr.Zero, pblobtype, &pout, null, 0, keyobject.MemoryPointer, keyobject.MemoryLength.ToUInt32());
            }
            hencdec = pout;
            return nts;
        }

        [DllImport(Libraries.BCrypt, EntryPoint = "BCryptEncrypt", ExactSpelling = true)]
        private static extern NTSTATUS BCryptEncrypt_Native(System.IntPtr hkey, System.Byte* pencrypt, System.UInt32 penlen, void* paddinginf, System.Byte* initvecbuf, System.UInt32 initvecbuflen, System.Byte* poutbuf, System.UInt32 outbuflen, System.UInt32* poutbufbytes, EncryptDecryptFlags flags);

        public static NTSTATUS BCryptEncrypt(System.IntPtr hkey, System.ReadOnlySpan<System.Byte> encryptbuf, System.ReadOnlySpan<System.Byte> initvec, System.Span<System.Byte> encryptedbuf, EncryptDecryptFlags flags, out System.Int32 writtenbytes)
        {
            NTSTATUS nts;
            System.UInt32 wb;
            fixed (System.Byte* pencrypt = encryptbuf)
            fixed (System.Byte* poutbuf = encryptedbuf)
            fixed (System.Byte* piv = initvec)
            {
                nts = BCryptEncrypt_Native(hkey, pencrypt, encryptbuf.Length.ToUInt32(), null, piv, initvec.Length.ToUInt32(), poutbuf, encryptedbuf.Length.ToUInt32(), &wb, flags);
            }
            writtenbytes = wb.ToInt32();
            return nts;
        }

        [DllImport(Libraries.BCrypt, EntryPoint = "BCryptDecrypt", ExactSpelling = true)]
        private static extern NTSTATUS BCryptDecrypt_Native(System.IntPtr hkey, System.Byte* pencrypted, System.UInt32 pencdlen, void* paddinginf, System.Byte* initvecbuf, System.UInt32 initvecbuflen, System.Byte* poutbuf, System.UInt32 decbuflen, System.UInt32* pdecbufbytes, EncryptDecryptFlags flags);

        public static NTSTATUS BCryptDecrypt(System.IntPtr hkey, System.ReadOnlySpan<System.Byte> encryptedbuf, System.ReadOnlySpan<System.Byte> initvec, System.Span<System.Byte> decryptedbuf, EncryptDecryptFlags flags, out System.Int32 writtenbytes)
        {
            NTSTATUS nts;
            System.UInt32 wb;
            fixed (System.Byte* pencrypted = encryptedbuf)
            fixed (System.Byte* poutbuf = decryptedbuf)
            fixed (System.Byte* piv = initvec)
            {
                nts = BCryptDecrypt_Native(hkey, pencrypted, encryptedbuf.Length.ToUInt32(), null, piv, initvec.Length.ToUInt32(), poutbuf, decryptedbuf.Length.ToUInt32(), &wb, flags);
            }
            writtenbytes = wb.ToInt32();
            return nts;
        }

        [DllImport(Libraries.BCrypt , EntryPoint = "BCryptGenerateSymmetricKey" , ExactSpelling = true)]
        private static extern NTSTATUS BCryptGenerateSymmetricKey_Native(System.IntPtr halg, System.IntPtr* pkeyout, System.Byte* pkeyobj, System.UInt32 keyobjlen, System.Byte* psecret, System.UInt32 secretlen , System.UInt32 flags = 0);

        public static NTSTATUS BCryptGenerateSymmetricKey(System.IntPtr halg, System.ReadOnlySpan<System.Byte> secret, out System.IntPtr hkey)
        {
            NTSTATUS nts;
            System.IntPtr ok; // abbrev for 'openedkey'.
            fixed (System.Byte* ps = secret)
            {
                nts = BCryptGenerateSymmetricKey_Native(halg, &ok, null, 0, ps, secret.Length.ToUInt32(), 0);
            }
            hkey = ok;
            return nts;
        }

        [DllImport(Libraries.BCrypt , ExactSpelling = true)]
        public static extern NTSTATUS BCryptDestroyKey(System.IntPtr hkey);

        [DllImport(Libraries.BCrypt , EntryPoint = "BCryptSetProperty" , ExactSpelling = true)]
        private static extern NTSTATUS BCryptSetProperty_Native(System.IntPtr halg, System.Char* pproperty, System.Byte* pvaluebuf, System.UInt32 buflen, System.UInt32 flags = 0);

        public static NTSTATUS BCryptSetProperty_String(System.IntPtr halg , System.String property , System.String value)
        {
            NTSTATUS nts;
            if (value is null) { throw new ArgumentNullException(nameof(value)); }
            if (property is null) { throw new ArgumentNullException(nameof(property)); }
            value += "\0";
            property += "\0";
            System.UInt32 vallen = (value.Length * sizeof(System.Char)).ToUInt32();
            fixed (System.Char* pprop = property)
            fixed (System.Char* pvalue = value)
            {
                nts = BCryptSetProperty_Native(halg, pprop, (System.Byte*)pvalue, vallen, 0);
            }
            return nts;
        }

        public static NTSTATUS BCryptSetProperty_Int32(System.IntPtr halg , System.String property , System.Int32 value)
        {
            NTSTATUS nts;
            if (property is null) { throw new ArgumentNullException(nameof(property)); }
            property += "\0";
            System.UInt32 vallen = sizeof(System.Int32).ToUInt32();
            fixed (System.Char* pprop = property)
            {
                nts = BCryptSetProperty_Native(halg, pprop, (System.Byte*)&value, vallen, 0);
            }
            return nts;
        }
    }
}