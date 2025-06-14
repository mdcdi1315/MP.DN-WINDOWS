
using MP;
using Microsoft.Security.Cryptography;

namespace Microsoft.Win32.SafeHandles
{
    /// <summary>
    /// Provides the BCrypt cryptographic operations.
    /// </summary>
    public sealed class SafeBCryptAlgorithmProviderHandle : CriticalHandleZeroOrMinusOneIsInvalid
    {
        public SafeBCryptAlgorithmProviderHandle(System.String algorithm , System.Boolean usehmac = false) 
        {
            Interop.NTSTATUS nts = Interop.BCrypt.BCryptOpenAlgorithmProvider(algorithm, null, usehmac ? Interop.BCrypt.OpenAlgorithmProviderFlags.BCRYPT_ALG_HANDLE_HMAC_FLAG : Interop.BCrypt.OpenAlgorithmProviderFlags.None, out handle);
            if (nts != Interop.NTSTATUS.STATUS_SUCCESS)
            {
                throw new MP.ExceptionSystem.NativeWindowsException(Interop.NtDll.RtlNtStatusToDosError(nts).ToInt32());
            }
        }

        public SafeBCryptHashHandle GetHashHandle(System.Byte[] secretkey = null)
        {
            Interop.NTSTATUS nts = Interop.BCrypt.BCryptCreateHash(handle, out var hashalg, secretkey);
            if (nts != Interop.NTSTATUS.STATUS_SUCCESS)
            {
                throw new MP.ExceptionSystem.NativeWindowsException(Interop.NtDll.RtlNtStatusToDosError(nts).ToInt32());
            }
            return new(hashalg);
        }

        public SafeBCryptKeyHandle GetKeyHandleFromExisting(System.ReadOnlySpan<System.Byte> key)
        {
            SafeLibcMemoryHandle mem = Interop.BCrypt.BCRYPT_KEY_DATA_BLOB_HEADER.CreateKeyBlob(key);
            Interop.NTSTATUS nts = Interop.BCrypt.BCryptImportKey(handle, Interop.BCrypt.BCRYPT_KEY_DATA_BLOB, mem , out var hencdec);
            if (nts != Interop.NTSTATUS.STATUS_SUCCESS)
            {
                mem?.Dispose();
                mem = null;
                throw new MP.ExceptionSystem.NativeWindowsException(Interop.NtDll.RtlNtStatusToDosError(nts).ToInt32());
            }
            return new(hencdec);
        }

        public static SafeBCryptAlgorithmProviderHandle GetAesStandardHandle(CipherMode cipherMode, int feedback)
        {
            SafeBCryptAlgorithmProviderHandle handleret = new(Interop.BCrypt.BCRYPT_AES_ALGORITHM , false);
            handleret.SetCipherMode(cipherMode);
            if (feedback > 0 && feedback != 1)
            {
                if (CipherMode.CFB == cipherMode)
                {
                    switch (feedback)
                    {
                        case 1:
                        case 16:
                            break;
                        default:
                            throw new System.NotSupportedException();
                    }
                }
                handleret.SetFeedbackSize(feedback);
            }
            return handleret;
        }

        public void SetCipherMode(CipherMode mode)
        {
            Interop.NTSTATUS nts = Interop.BCrypt.BCryptSetProperty_String(handle, Interop.BCrypt.BCRYPT_CHAINING_MODE, mode switch {
                CipherMode.CBC => Interop.BCrypt.BCRYPT_CHAIN_MODE_CBC,
                CipherMode.ECB => Interop.BCrypt.BCRYPT_CHAIN_MODE_ECB,
                CipherMode.CFB => Interop.BCrypt.BCRYPT_CHAIN_MODE_CFB,
                _ => throw new System.ArgumentException($"Mode {mode} not supported by BCrypt.")
            });

            if (nts != Interop.NTSTATUS.STATUS_SUCCESS)
            {
                throw new MP.ExceptionSystem.NativeWindowsException(Interop.NtDll.RtlNtStatusToDosError(nts).ToInt32());
            }
        }

        public void SetFeedbackSize(System.Int32 feedbacksize)
        {
            Interop.NTSTATUS nts = Interop.BCrypt.BCryptSetProperty_Int32(handle, Interop.BCrypt.BCRYPT_MESSAGE_BLOCK_LENGTH, feedbacksize);

            if (nts != Interop.NTSTATUS.STATUS_SUCCESS)
            {
                throw new MP.ExceptionSystem.NativeWindowsException(Interop.NtDll.RtlNtStatusToDosError(nts).ToInt32());
            }
        }

        public void SetEffectiveKeyLength(System.Int32 effectivekeylen)
        {
            Interop.NTSTATUS nts = Interop.BCrypt.BCryptSetProperty_Int32(handle, Interop.BCrypt.BCRYPT_EFFECTIVE_KEY_LENGTH, effectivekeylen);

            if (nts != Interop.NTSTATUS.STATUS_SUCCESS)
            {
                throw new MP.ExceptionSystem.NativeWindowsException(Interop.NtDll.RtlNtStatusToDosError(nts).ToInt32());
            }
        }

        protected override bool ReleaseHandle()
        {
            Interop.NTSTATUS nts = Interop.BCrypt.BCryptCloseAlgorithmProvider(handle);
            return nts == Interop.NTSTATUS.STATUS_SUCCESS;
        }
    }
}