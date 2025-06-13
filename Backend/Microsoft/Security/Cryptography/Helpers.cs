

namespace Microsoft.Security.Cryptography
{
    internal static class Helpers
    {
        public static System.Byte[] CloneByteArray(this System.Byte[] data) => data.Clone() as System.Byte[];

        public static bool UsesIv(this CipherMode cipherMode)
        {
            return cipherMode != CipherMode.ECB;
        }

        public static byte[] GetCipherIv(this CipherMode cipherMode, byte[] iv)
        {
            if (cipherMode.UsesIv())
            {
                if (iv is null) {
                    throw new CryptographicException(System.SR.Cryptography_MissingIV);
                }

                return iv;
            }

            return null;
        }

        public static int GetPaddingSize(this SymmetricAlgorithm algorithm, CipherMode mode, int feedbackSizeInBits)
        {
            return (mode == CipherMode.CFB ? feedbackSizeInBits : algorithm.BlockSize) / 8;
        }
    }
}