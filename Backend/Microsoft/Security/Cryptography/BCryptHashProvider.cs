

using MP;
using System;
using Microsoft.Win32.SafeHandles;

namespace Microsoft.Security.Cryptography
{
    // Provides a slim hash provider connection to native interop.
    // Unlike the original hash provider in System.Security.Cryptography , this does exactly the same,
    // but with less code overhead.
    internal sealed class BCryptHashProvider : HashProvider
    {
        private System.Byte[] secretkey;
        private SafeBCryptHashHandle hh;
        private SafeBCryptAlgorithmProviderHandle provider;

        public BCryptHashProvider(System.String algorithmname , System.Boolean hmac = false , System.Byte[] secretkey = null)
        {
            provider = new(algorithmname , hmac);
            hh = provider.GetHashHandle(secretkey);
            this.secretkey = secretkey;
        }

        public BCryptHashProvider(SafeBCryptAlgorithmProviderHandle hprovider , System.Byte[] secretkey = null)
        {
            provider = hprovider;
            hh = hprovider.GetHashHandle(secretkey);
            this.secretkey = secretkey;
        }

        public static System.Int32 HashDataAndReturnDirectly(System.String hashalgorithmname , ReadOnlySpan<byte> source, Span<byte> destination)
        {
            using (BCryptHashProvider hashprov = new(hashalgorithmname))
            {
                hashprov.AppendHashData(source);
                return hashprov.FinalizeHashAndReset(destination);
            }
        }

        public static System.Int32 HashDataStreamAndReturnDirectly(System.String hashalgorithmname , System.IO.Stream source , System.Span<System.Byte> destination)
        {
            using (BCryptHashProvider hashprov = new(hashalgorithmname))
            {
                System.Byte[] buffer = new System.Byte[4096];
                System.Int32 rb;
                while ((rb = source.Read(buffer , 0 , buffer.Length)) > 0)
                {
                    hashprov.AppendHashData(buffer, 0, rb);
                }
                return hashprov.FinalizeHashAndReset(destination);
            }
        }

        private static System.Int32 HashSizeInBytesInternal(SafeBCryptHashHandle hash)
        {
            System.UInt32 retv;
            var nts = Interop.BCrypt.BCryptGetProperty_UINT32(hash.Handle, Interop.BCrypt.BCRYPT_HASH_LENGTH, out retv);
            if (nts != Interop.NTSTATUS.STATUS_SUCCESS)
            {
                throw new MP.ExceptionSystem.NativeWindowsException(Interop.NtDll.RtlNtStatusToDosError(nts).ToInt32());
            }
            return retv.ToInt32();
        }

        public System.String AlgorithmName
        {
            get {
                System.String alg;
                Interop.NTSTATUS nts = Interop.BCrypt.BCryptGetProperty_String(hh.Handle , Interop.BCrypt.BCRYPT_ALGORITHM_NAME , out alg);
                if (nts != Interop.NTSTATUS.STATUS_SUCCESS)
                {
                    throw new MP.ExceptionSystem.NativeWindowsException(Interop.NtDll.RtlNtStatusToDosError(nts).ToInt32());
                }
                return alg;
            }
        }

        public override int HashSizeInBytes => HashSizeInBytesInternal(hh);

        public override void AppendHashData(ReadOnlySpan<byte> data)
        {
            Interop.NTSTATUS nts = Interop.BCrypt.BCryptHashData(hh.Handle , data);
            if (nts != Interop.NTSTATUS.STATUS_SUCCESS)
            {
                throw new MP.ExceptionSystem.NativeWindowsException(Interop.NtDll.RtlNtStatusToDosError(nts).ToInt32());
            }
        }

        public override void Dispose(bool disposing)
        {
            hh?.Dispose();
            hh = null;
            provider?.Dispose();
            provider = null;
        }

        public override int FinalizeHashAndReset(Span<byte> destination)
        {
            Interop.NTSTATUS nts = Interop.BCrypt.BCryptFinishHash(hh.Handle , destination);
            if (nts != Interop.NTSTATUS.STATUS_SUCCESS)
            {
                throw new MP.ExceptionSystem.NativeWindowsException(Interop.NtDll.RtlNtStatusToDosError(nts).ToInt32());
            }
            System.Int32 hs = HashSizeInBytesInternal(hh);
            Reset();
            return hs;
        }

        public override int GetCurrentHash(Span<byte> destination)
        {
            using (var dp = hh.Duplicate())
            {
                Interop.NTSTATUS nts = Interop.BCrypt.BCryptFinishHash(dp.Handle, destination);
                if (nts != Interop.NTSTATUS.STATUS_SUCCESS)
                {
                    throw new MP.ExceptionSystem.NativeWindowsException(Interop.NtDll.RtlNtStatusToDosError(nts).ToInt32());
                }
                return HashSizeInBytesInternal(dp);
            }
        }

        public override void Reset()
        {
            hh?.Dispose();
            hh = provider.GetHashHandle(secretkey);
        }
    }
}