/*
 * Copyright © 2000-2018 SharpZipLib Contributors

Permission is hereby granted, free of charge, to any person obtaining a copy of this
software and associated documentation files (the "Software"), to deal in the Software
without restriction, including without limitation the rights to use, copy, modify, merge,
publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or
substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
DEALINGS IN THE SOFTWARE.

*/

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
    #define VECTORIZE_MEMORY_MOVE
#endif

using System.Threading;
using System.Threading.Tasks;
using System.IO.ManagedZip.Zip;
using System.IO.ManagedZip.Core;
using System.IO.ManagedZip.Checksum;
using Microsoft.Security.Cryptography;

namespace System.IO.ManagedZip.Encryption
{

    /// <summary>
    /// PkzipClassic embodies the classic or original encryption facilities used in Pkzip archives.
    /// While it has been superseded by more recent and more powerful algorithms, its still in use and
    /// is viable for preventing casual snooping
    /// </summary>
    public abstract class PkzipClassic : SymmetricAlgorithm
    {
        /// <summary>
        /// Generates new encryption keys based on given seed
        /// </summary>
        /// <param name="seed">The seed value to initialise keys with.</param>
        /// <returns>A new key value.</returns>
        static public byte[] GenerateKeys(byte[] seed)
        {
            if (seed == null)
            {
                throw new ArgumentNullException(nameof(seed));
            }

            if (seed.Length == 0)
            {
                throw new ArgumentException("Length is zero", nameof(seed));
            }

            uint[] newKeys = {
                0x12345678,
                0x23456789,
                0x34567890
             };

            for (int i = 0; i < seed.Length; ++i)
            {
                newKeys[0] = Crc32.ComputeCrc32(newKeys[0], seed[i]);
                newKeys[1] = newKeys[1] + (byte)newKeys[0];
                newKeys[1] = newKeys[1] * 134775813 + 1;
                newKeys[2] = Crc32.ComputeCrc32(newKeys[2], (byte)(newKeys[1] >> 24));
            }

            byte[] result = new byte[12];
            result[0] = (byte)(newKeys[0] & 0xff);
            result[1] = (byte)((newKeys[0] >> 8) & 0xff);
            result[2] = (byte)((newKeys[0] >> 16) & 0xff);
            result[3] = (byte)((newKeys[0] >> 24) & 0xff);
            result[4] = (byte)(newKeys[1] & 0xff);
            result[5] = (byte)((newKeys[1] >> 8) & 0xff);
            result[6] = (byte)((newKeys[1] >> 16) & 0xff);
            result[7] = (byte)((newKeys[1] >> 24) & 0xff);
            result[8] = (byte)(newKeys[2] & 0xff);
            result[9] = (byte)((newKeys[2] >> 8) & 0xff);
            result[10] = (byte)((newKeys[2] >> 16) & 0xff);
            result[11] = (byte)((newKeys[2] >> 24) & 0xff);
            return result;
        }
    }

    /// <summary>
    /// PkzipClassicCryptoBase provides the low level facilities for encryption
    /// and decryption using the PkzipClassic algorithm.
    /// </summary>
    internal class PkzipClassicCryptoBase
    {
        /// <summary>
        /// Transform a single byte
        /// </summary>
        /// <returns>
        /// The transformed value
        /// </returns>
        protected byte TransformByte()
        {
            uint temp = ((keys[2] & 0xFFFF) | 2);
            return (byte)((temp * (temp ^ 1)) >> 8);
        }

        /// <summary>
        /// Set the key schedule for encryption/decryption.
        /// </summary>
        /// <param name="keyData">The data use to set the keys from.</param>
        protected void SetKeys(byte[] keyData)
        {
            if (keyData == null)
            {
                throw new ArgumentNullException(nameof(keyData));
            }

            if (keyData.Length != 12)
            {
                throw new InvalidOperationException("Key length is not valid");
            }

            keys = new uint[3];
            keys[0] = (uint)((keyData[3] << 24) | (keyData[2] << 16) | (keyData[1] << 8) | keyData[0]);
            keys[1] = (uint)((keyData[7] << 24) | (keyData[6] << 16) | (keyData[5] << 8) | keyData[4]);
            keys[2] = (uint)((keyData[11] << 24) | (keyData[10] << 16) | (keyData[9] << 8) | keyData[8]);
        }

        /// <summary>
        /// Update encryption keys
        /// </summary>
        protected void UpdateKeys(byte ch)
        {
            keys[0] = Crc32.ComputeCrc32(keys[0], ch);
            keys[1] = keys[1] + (byte)keys[0];
            keys[1] = keys[1] * 134775813 + 1;
            keys[2] = Crc32.ComputeCrc32(keys[2], (byte)(keys[1] >> 24));
        }

        /// <summary>
        /// Reset the internal state.
        /// </summary>
        protected void Reset()
        {
            keys[0] = 0;
            keys[1] = 0;
            keys[2] = 0;
        }

        #region Instance Fields

        private uint[] keys;

        #endregion Instance Fields
    }

    /// <summary>
    /// PkzipClassic CryptoTransform for encryption.
    /// </summary>
    internal class PkzipClassicEncryptCryptoTransform : PkzipClassicCryptoBase, ICryptoTransform
    {
        /// <summary>
        /// Initialise a new instance of <see cref="PkzipClassicEncryptCryptoTransform"></see>
        /// </summary>
        /// <param name="keyBlock">The key block to use.</param>
        internal PkzipClassicEncryptCryptoTransform(byte[] keyBlock)
        {
            SetKeys(keyBlock);
        }

        #region ICryptoTransform Members

        /// <summary>
        /// Transforms the specified region of the specified byte array.
        /// </summary>
        /// <param name="inputBuffer">The input for which to compute the transform.</param>
        /// <param name="inputOffset">The offset into the byte array from which to begin using data.</param>
        /// <param name="inputCount">The number of bytes in the byte array to use as data.</param>
        /// <returns>The computed transform.</returns>
        public byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
        {
            byte[] result = new byte[inputCount];
            TransformBlock(inputBuffer, inputOffset, inputCount, result, 0);
            return result;
        }

        /// <summary>
        /// Transforms the specified region of the input byte array and copies
        /// the resulting transform to the specified region of the output byte array.
        /// </summary>
        /// <param name="inputBuffer">The input for which to compute the transform.</param>
        /// <param name="inputOffset">The offset into the input byte array from which to begin using data.</param>
        /// <param name="inputCount">The number of bytes in the input byte array to use as data.</param>
        /// <param name="outputBuffer">The output to which to write the transform.</param>
        /// <param name="outputOffset">The offset into the output byte array from which to begin writing data.</param>
        /// <returns>The number of bytes written.</returns>
        public int TransformBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
        {
            for (int i = inputOffset; i < inputOffset + inputCount; ++i)
            {
                byte oldbyte = inputBuffer[i];
                outputBuffer[outputOffset++] = (byte)(inputBuffer[i] ^ TransformByte());
                UpdateKeys(oldbyte);
            }
            return inputCount;
        }

        /// <summary>
        /// Gets a value indicating whether the current transform can be reused.
        /// </summary>
        public bool CanReuseTransform
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets the size of the input data blocks in bytes.
        /// </summary>
        public int InputBlockSize
        {
            get
            {
                return 1;
            }
        }

        /// <summary>
        /// Gets the size of the output data blocks in bytes.
        /// </summary>
        public int OutputBlockSize
        {
            get
            {
                return 1;
            }
        }

        /// <summary>
        /// Gets a value indicating whether multiple blocks can be transformed.
        /// </summary>
        public bool CanTransformMultipleBlocks
        {
            get
            {
                return true;
            }
        }

        #endregion ICryptoTransform Members

        #region IDisposable Members

        /// <summary>
        /// Cleanup internal state.
        /// </summary>
        public void Dispose()
        {
            Reset();
        }

        #endregion IDisposable Members
    }

    /// <summary>
    /// PkzipClassic CryptoTransform for decryption.
    /// </summary>
    internal class PkzipClassicDecryptCryptoTransform : PkzipClassicCryptoBase, ICryptoTransform
    {
        /// <summary>
        /// Initialise a new instance of <see cref="PkzipClassicDecryptCryptoTransform"></see>.
        /// </summary>
        /// <param name="keyBlock">The key block to decrypt with.</param>
        internal PkzipClassicDecryptCryptoTransform(byte[] keyBlock)
        {
            SetKeys(keyBlock);
        }

        #region ICryptoTransform Members

        /// <summary>
        /// Transforms the specified region of the specified byte array.
        /// </summary>
        /// <param name="inputBuffer">The input for which to compute the transform.</param>
        /// <param name="inputOffset">The offset into the byte array from which to begin using data.</param>
        /// <param name="inputCount">The number of bytes in the byte array to use as data.</param>
        /// <returns>The computed transform.</returns>
        public byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
        {
            byte[] result = new byte[inputCount];
            TransformBlock(inputBuffer, inputOffset, inputCount, result, 0);
            return result;
        }

        /// <summary>
        /// Transforms the specified region of the input byte array and copies
        /// the resulting transform to the specified region of the output byte array.
        /// </summary>
        /// <param name="inputBuffer">The input for which to compute the transform.</param>
        /// <param name="inputOffset">The offset into the input byte array from which to begin using data.</param>
        /// <param name="inputCount">The number of bytes in the input byte array to use as data.</param>
        /// <param name="outputBuffer">The output to which to write the transform.</param>
        /// <param name="outputOffset">The offset into the output byte array from which to begin writing data.</param>
        /// <returns>The number of bytes written.</returns>
        public int TransformBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
        {
            for (int i = inputOffset; i < inputOffset + inputCount; ++i)
            {
                var newByte = (byte)(inputBuffer[i] ^ TransformByte());
                outputBuffer[outputOffset++] = newByte;
                UpdateKeys(newByte);
            }
            return inputCount;
        }

        /// <summary>
        /// Gets a value indicating whether the current transform can be reused.
        /// </summary>
        public bool CanReuseTransform
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets the size of the input data blocks in bytes.
        /// </summary>
        public int InputBlockSize
        {
            get
            {
                return 1;
            }
        }

        /// <summary>
        /// Gets the size of the output data blocks in bytes.
        /// </summary>
        public int OutputBlockSize
        {
            get
            {
                return 1;
            }
        }

        /// <summary>
        /// Gets a value indicating whether multiple blocks can be transformed.
        /// </summary>
        public bool CanTransformMultipleBlocks
        {
            get
            {
                return true;
            }
        }

        #endregion ICryptoTransform Members

        #region IDisposable Members

        /// <summary>
        /// Cleanup internal state.
        /// </summary>
        public void Dispose()
        {
            Reset();
        }

        #endregion IDisposable Members
    }

    /// <summary>
    /// Defines a wrapper object to access the Pkzip algorithm.
    /// This class cannot be inherited.
    /// </summary>
    public sealed class PkzipClassicManaged : PkzipClassic
    {
        /// <summary>
        /// Get / set the applicable block size in bits.
        /// </summary>
        /// <remarks>The only valid block size is 8.</remarks>
        public override int BlockSize
        {
            get
            {
                return 8;
            }

            set
            {
                if (value != 8)
                {
                    throw new CryptographicException("Block size is invalid");
                }
            }
        }

        /// <summary>
        /// Get an array of legal <see cref="KeySizes">key sizes.</see>
        /// </summary>
        public override KeySizes[] LegalKeySizes
        {
            get
            {
                KeySizes[] keySizes = new KeySizes[1];
                keySizes[0] = new KeySizes(12 * 8, 12 * 8, 0);
                return keySizes;
            }
        }

        /// <summary>
        /// Generate an initial vector.
        /// </summary>
        public override void GenerateIV()
        {
            // Do nothing.
        }

        /// <summary>
        /// Get an array of legal <see cref="KeySizes">block sizes</see>.
        /// </summary>
        public override KeySizes[] LegalBlockSizes
        {
            get
            {
                KeySizes[] keySizes = new KeySizes[1];
                keySizes[0] = new KeySizes(1 * 8, 1 * 8, 0);
                return keySizes;
            }
        }

        /// <summary>
        /// Get / set the key value applicable.
        /// </summary>
        public override byte[] Key
        {
            get
            {
                if (key_ == null)
                {
                    GenerateKey();
                }

                return (byte[])key_.Clone();
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                if (value.Length != 12)
                {
                    throw new CryptographicException("Key size is illegal");
                }

                key_ = (byte[])value.Clone();
            }
        }

        /// <summary>
        /// Generate a new random key.
        /// </summary>
        public override void GenerateKey()
        {
            key_ = new byte[12];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(key_);
            }
        }

        /// <summary>
        /// Create an encryptor.
        /// </summary>
        /// <param name="rgbKey">The key to use for this encryptor.</param>
        /// <param name="rgbIV">Initialisation vector for the new encryptor.</param>
        /// <returns>Returns a new PkzipClassic encryptor</returns>
        public override ICryptoTransform CreateEncryptor(
            byte[] rgbKey,
            byte[] rgbIV)
        {
            key_ = rgbKey;
            return new PkzipClassicEncryptCryptoTransform(Key);
        }

        /// <summary>
        /// Create a decryptor.
        /// </summary>
        /// <param name="rgbKey">Keys to use for this new decryptor.</param>
        /// <param name="rgbIV">Initialisation vector for the new decryptor.</param>
        /// <returns>Returns a new decryptor.</returns>
        public override ICryptoTransform CreateDecryptor(
            byte[] rgbKey,
            byte[] rgbIV)
        {
            key_ = rgbKey;
            return new PkzipClassicDecryptCryptoTransform(Key);
        }

        #region Instance Fields

        private byte[] key_;

        #endregion Instance Fields
    }

    /// <summary>
    /// Encrypts and decrypts AES ZIP
    /// </summary>
    /// <remarks>
    /// Based on information from http://www.winzip.com/aes_info.htm
    /// and http://www.gladman.me.uk/cryptography_technology/fileencrypt/
    /// </remarks>
    internal class ZipAESStream : CryptoStream
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="stream">The stream on which to perform the cryptographic transformation.</param>
        /// <param name="transform">Instance of ZipAESTransform</param>
        /// <param name="mode">Read or Write</param>
        public ZipAESStream(Stream stream, ZipAESTransform transform, CryptoStreamMode mode)
            : base(stream, transform, mode)
        {
            _stream = stream;
            _transform = transform;
            _slideBuffer = new byte[1024];

            // mode:
            //  CryptoStreamMode.Read means we read from "stream" and pass decrypted to our Read() method.
            //  Write bypasses this stream and uses the Transform directly.
            if (mode != CryptoStreamMode.Read)
            {
                throw new Exception("ZipAESStream only for read");
            }
        }

        // The final n bytes of the AES stream contain the Auth Code.
        public const int AUTH_CODE_LENGTH = 10;

        // Blocksize is always 16 here, even for AES-256 which has transform.InputBlockSize of 32.
        private const int CRYPTO_BLOCK_SIZE = 16;

        // total length of block + auth code
        private const int BLOCK_AND_AUTH = CRYPTO_BLOCK_SIZE + AUTH_CODE_LENGTH;

        private Stream _stream;
        private ZipAESTransform _transform;
        private byte[] _slideBuffer;
        private int _slideBufStartPos;
        private int _slideBufFreePos;

        // Buffer block transforms to enable partial reads
        private byte[] _transformBuffer = null;// new byte[CRYPTO_BLOCK_SIZE];
        private int _transformBufferFreePos;
        private int _transformBufferStartPos;

        // Do we have some buffered data available?
        private bool HasBufferedData => _transformBuffer != null && _transformBufferStartPos < _transformBufferFreePos;

        /// <summary>
        /// Reads a sequence of bytes from the current CryptoStream into buffer,
        /// and advances the position within the stream by the number of bytes read.
        /// </summary>
        public override int Read(byte[] buffer, int offset, int count)
        {
            // Nothing to do
            if (count == 0)
                return 0;

            // If we have buffered data, read that first
            int nBytes = 0;
            if (HasBufferedData)
            {
                nBytes = ReadBufferedData(buffer, offset, count);

                // Read all requested data from the buffer
                if (nBytes == count)
                    return nBytes;

                offset += nBytes;
                count -= nBytes;
            }

            // Read more data from the input, if available
            if (_slideBuffer != null)
                nBytes += ReadAndTransform(buffer, offset, count);

            return nBytes;
        }

        /// <inheritdoc/>
        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            var readCount = Read(buffer, offset, count);
            return Task.FromResult(readCount);
        }

        // Read data from the underlying stream and decrypt it
        private int ReadAndTransform(byte[] buffer, int offset, int count)
        {
            int nBytes = 0;
            while (nBytes < count)
            {
                int bytesLeftToRead = count - nBytes;

                // Calculate buffer quantities vs read-ahead size, and check for sufficient free space
                int byteCount = _slideBufFreePos - _slideBufStartPos;

                // Need to handle final block and Auth Code specially, but don't know total data length.
                // Maintain a read-ahead equal to the length of (crypto block + Auth Code).
                // When that runs out we can detect these final sections.
                int lengthToRead = BLOCK_AND_AUTH - byteCount;
                if (_slideBuffer.Length - _slideBufFreePos < lengthToRead)
                {
                    // Shift the data to the beginning of the buffer
                    int iTo = 0;
                    for (int iFrom = _slideBufStartPos; iFrom < _slideBufFreePos; iFrom++, iTo++)
                    {
                        _slideBuffer[iTo] = _slideBuffer[iFrom];
                    }
                    _slideBufFreePos -= _slideBufStartPos;      // Note the -=
                    _slideBufStartPos = 0;
                }
                int obtained = StreamUtils.ReadRequestedBytes(_stream, _slideBuffer, _slideBufFreePos, lengthToRead);
                _slideBufFreePos += obtained;

                // Recalculate how much data we now have
                byteCount = _slideBufFreePos - _slideBufStartPos;
                if (byteCount >= BLOCK_AND_AUTH)
                {
                    var read = TransformAndBufferBlock(buffer, offset, bytesLeftToRead, CRYPTO_BLOCK_SIZE);
                    nBytes += read;
                    offset += read;
                }
                else
                {
                    // Last round.
                    if (byteCount > AUTH_CODE_LENGTH)
                    {
                        // At least one byte of data plus auth code
                        int finalBlock = byteCount - AUTH_CODE_LENGTH;
                        nBytes += TransformAndBufferBlock(buffer, offset, bytesLeftToRead, finalBlock);
                    }
                    else if (byteCount < AUTH_CODE_LENGTH)
                        throw new ZipException("Internal error missed auth code"); // Coding bug
                                                                                   // Final block done. Check Auth code.
                    byte[] calcAuthCode = _transform.GetAuthCode();
                    for (int i = 0; i < AUTH_CODE_LENGTH; i++)
                    {
                        if (calcAuthCode[i] != _slideBuffer[_slideBufStartPos + i])
                        {
                            throw new ZipException("AES Authentication Code does not match. This is a super-CRC check on the data in the file after compression and encryption. \r\n"
                                + "The file may be damaged.");
                        }
                    }

                    // don't need this any more, so use it as a 'complete' flag
                    _slideBuffer = null;

                    break;  // Reached the auth code
                }
            }
            return nBytes;
        }

        // read some buffered data
        private int ReadBufferedData(byte[] buffer, int offset, int count)
        {
            int copyCount = Math.Min(count, _transformBufferFreePos - _transformBufferStartPos);

            Array.Copy(_transformBuffer, _transformBufferStartPos, buffer, offset, copyCount);
            _transformBufferStartPos += copyCount;

            return copyCount;
        }

        // Perform the crypto transform, and buffer the data if less than one block has been requested.
        private int TransformAndBufferBlock(byte[] buffer, int offset, int count, int blockSize)
        {
            // If the requested data is greater than one block, transform it directly into the output
            // If it's smaller, do it into a temporary buffer and copy the requested part
            bool bufferRequired = (blockSize > count);

            if (bufferRequired && _transformBuffer == null)
                _transformBuffer = new byte[CRYPTO_BLOCK_SIZE];

            var targetBuffer = bufferRequired ? _transformBuffer : buffer;
            var targetOffset = bufferRequired ? 0 : offset;

            // Transform the data
            _transform.TransformBlock(_slideBuffer,
                                      _slideBufStartPos,
                                      blockSize,
                                      targetBuffer,
                                      targetOffset);

            _slideBufStartPos += blockSize;

            if (!bufferRequired)
            {
                return blockSize;
            }
            else
            {
                Array.Copy(_transformBuffer, 0, buffer, offset, count);
                _transformBufferStartPos = count;
                _transformBufferFreePos = blockSize;

                return count;
            }
        }

        /// <summary>
        /// Writes a sequence of bytes to the current stream and advances the current position within this stream by the number of bytes written.
        /// </summary>
        /// <param name="buffer">An array of bytes. This method copies count bytes from buffer to the current stream. </param>
        /// <param name="offset">The byte offset in buffer at which to begin copying bytes to the current stream. </param>
        /// <param name="count">The number of bytes to be written to the current stream. </param>
        public override void Write(byte[] buffer, int offset, int count)
        {
            // ZipAESStream is used for reading but not for writing. Writing uses the ZipAESTransform directly.
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Transforms stream using AES in CTR mode
    /// </summary>
    internal class ZipAESTransform : ICryptoTransform
    {
        private const int PWD_VER_LENGTH = 2;

        // WinZip use iteration count of 1000 for PBKDF2 key generation
        private const int KEY_ROUNDS = 1000;

        // For 128-bit AES (16 bytes) the encryption is implemented as expected.
        // For 256-bit AES (32 bytes) WinZip do full 256 bit AES of the nonce to create the encryption
        // block but use only the first 16 bytes of it, and discard the second half.
        private const int ENCRYPT_BLOCK = 16;

        private int _blockSize;
        private readonly ICryptoTransform _encryptor;
        private readonly byte[] _counterNonce;
        private byte[] _encryptBuffer;
        private int _encrPos;
        private byte[] _pwdVerifier;
        private IncrementalHash _hmacsha1;
        private byte[] _authCode = null;

        private bool _writeMode;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="key">Password string</param>
        /// <param name="saltBytes">Random bytes, length depends on encryption strength.
        /// 128 bits = 8 bytes, 192 bits = 12 bytes, 256 bits = 16 bytes.</param>
        /// <param name="blockSize">The encryption strength, in bytes eg 16 for 128 bits.</param>
        /// <param name="writeMode">True when creating a zip, false when reading. For the AuthCode.</param>
        ///
        public ZipAESTransform(string key, byte[] saltBytes, int blockSize, bool writeMode)
        {
            if (blockSize != 16 && blockSize != 32) // 24 valid for AES but not supported by Winzip
                throw new Exception("Invalid blocksize " + blockSize + ". Must be 16 or 32.");
            if (saltBytes.Length != blockSize / 2)
                throw new Exception("Invalid salt len. Must be " + blockSize / 2 + " for blocksize " + blockSize);
            // initialise the encryption buffer and buffer pos
            _blockSize = blockSize;
            _encryptBuffer = new byte[_blockSize];
            _encrPos = ENCRYPT_BLOCK;

            // Performs the equivalent of derive_key in Dr Brian Gladman's pwd2key.c
            var pdb = new Rfc2898DeriveBytes(key, saltBytes, KEY_ROUNDS, HashAlgorithmName.SHA1);
            var rm = Aes.Create();
            rm.Mode = CipherMode.ECB;           // No feedback from cipher for CTR mode
            _counterNonce = new byte[_blockSize];
            byte[] key1bytes = pdb.GetBytes(_blockSize);
            byte[] key2bytes = pdb.GetBytes(_blockSize);

            // Use empty IV for AES
            _encryptor = rm.CreateEncryptor(key1bytes, new byte[16]);
            _pwdVerifier = pdb.GetBytes(PWD_VER_LENGTH);
            //
            _hmacsha1 = IncrementalHash.CreateHMAC(HashAlgorithmName.SHA1, key2bytes);
            _writeMode = writeMode;
        }

        /// <summary>
        /// Implement the ICryptoTransform method.
        /// </summary>
        public int TransformBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
        {
            // Pass the data stream to the hash algorithm for generating the Auth Code.
            // This does not change the inputBuffer. Do this before decryption for read mode.
            if (!_writeMode)
            {
                _hmacsha1.AppendData(inputBuffer, inputOffset, inputCount);
            }
            // Encrypt with AES in CTR mode. Regards to Dr Brian Gladman for this.
            int ix = 0;
            while (ix < inputCount)
            {
                if (_encrPos == ENCRYPT_BLOCK)
                {
                    /* increment encryption nonce   */
                    int j = 0;
                    while (++_counterNonce[j] == 0)
                    {
                        ++j;
                    }
                    /* encrypt the nonce to form next xor buffer    */
                    _encryptor.TransformBlock(_counterNonce, 0, _blockSize, _encryptBuffer, 0);
                    _encrPos = 0;
                }
                outputBuffer[ix + outputOffset] = (byte)(inputBuffer[ix + inputOffset] ^ _encryptBuffer[_encrPos++]);
                //
                ix++;
            }
            if (_writeMode)
            {
                // This does not change the buffer.
                _hmacsha1.AppendData(outputBuffer, outputOffset, inputCount);
            }
            return inputCount;
        }

        /// <summary>
        /// Returns the 2 byte password verifier
        /// </summary>
        public byte[] PwdVerifier => _pwdVerifier;

        /// <summary>
        /// Returns the 10 byte AUTH CODE to be checked or appended immediately following the AES data stream.
        /// </summary>
        public byte[] GetAuthCode() => _authCode ?? (_authCode = _hmacsha1.GetHashAndReset());

        #region ICryptoTransform Members

        /// <summary>
        /// Transform final block and read auth code
        /// </summary>
        public byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
        {
            var buffer = Array.Empty<byte>();

            // FIXME: When used together with `ZipAESStream`, the final block handling is done inside of it instead
            // This should not be necessary anymore, and the entire `ZipAESStream` class should be replaced with a plain `CryptoStream`
            if (inputCount != 0)
            {
                if (inputCount > ZipAESStream.AUTH_CODE_LENGTH)
                {
                    // At least one byte of data is preceeding the auth code
                    int finalBlock = inputCount - ZipAESStream.AUTH_CODE_LENGTH;
                    buffer = new byte[finalBlock];
                    TransformBlock(inputBuffer, inputOffset, finalBlock, buffer, 0);
                }
                else if (inputCount < ZipAESStream.AUTH_CODE_LENGTH)
                    throw new Zip.ZipException("Auth code missing from input stream");

                // Read the authcode from the last 10 bytes
                _authCode = _hmacsha1.GetHashAndReset();
            }


            return buffer;
        }

        /// <summary>
        /// Gets the size of the input data blocks in bytes.
        /// </summary>
        public int InputBlockSize => _blockSize;

        /// <summary>
        /// Gets the size of the output data blocks in bytes.
        /// </summary>
        public int OutputBlockSize => _blockSize;

        /// <summary>
        /// Gets a value indicating whether multiple blocks can be transformed.
        /// </summary>
        public bool CanTransformMultipleBlocks => true;

        /// <summary>
        /// Gets a value indicating whether the current transform can be reused.
        /// </summary>
        public bool CanReuseTransform => true;

        /// <summary>
        /// Cleanup internal state.
        /// </summary>
        public void Dispose() => _encryptor.Dispose();

        #endregion ICryptoTransform Members
    }
}
