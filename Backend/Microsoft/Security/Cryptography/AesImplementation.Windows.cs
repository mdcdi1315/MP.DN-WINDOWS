// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using Microsoft.Win32.SafeHandles;

namespace Microsoft.Security.Cryptography
{
    internal sealed partial class AesImplementation
    {
        private static UniversalCryptoTransform CreateTransformCore(
            CipherMode cipherMode,
            PaddingMode paddingMode,
            byte[] key,
            byte[] iv,
            int blockSize,
            int paddingSize,
            int feedbackSize,
            bool encrypting)
        {
            var algorithm = SafeBCryptAlgorithmProviderHandle.GetAesStandardHandle(cipherMode, feedbackSize);

            BasicSymmetricCipher cipher = new BasicSymmetricCipherBCrypt(algorithm, cipherMode, blockSize, paddingSize, key, true, iv, encrypting);
            return UniversalCryptoTransform.Create(paddingMode, cipher, encrypting);
        }

        private static BasicSymmetricCipherLiteBCrypt CreateLiteCipher(
            CipherMode cipherMode,
            ReadOnlySpan<byte> key,
            ReadOnlySpan<byte> iv,
            int blockSize,
            int paddingSize,
            int feedbackSize,
            bool encrypting)
        {
            var algorithm = SafeBCryptAlgorithmProviderHandle.GetAesStandardHandle(cipherMode, feedbackSize);

            return new BasicSymmetricCipherLiteBCrypt(
                algorithm,
                blockSize,
                paddingSize,
                key,
                ownsParentHandle: true,
                iv,
                encrypting);
        }
    }
}
