namespace MP.AudioLibrary.Codecs
{
    /// <summary>
    /// mu-law encoder
    /// based on code from:
    /// http://hazelware.luggle.com/tutorials/mulawcompression.html
    /// </summary>
    public static class MuLawEncoder
    {
        private const short cBias = 0x84;
        private const short cClip = 32635;

        private static readonly byte[] MuLawCompressTable = new byte[256] 
        {
             0,0,1,1,2,2,2,2,3,3,3,3,3,3,3,3,
             4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,
             5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,
             5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,
             6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,
             6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,
             6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,
             6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,
             7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,
             7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,
             7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,
             7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,
             7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,
             7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,
             7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,
             7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7
        };

        /// <summary>
        /// Encodes a single 16 bit sample to mu-law
        /// </summary>
        /// <param name="sample">16 bit PCM sample</param>
        /// <returns>mu-law encoded byte</returns>
        public static byte LinearToMuLawSample(short sample)
        {
            int sign = sample >> 8 & 0x80;
            if (sign != 0) { sample = (short)-sample; }
            else if (sample > cClip) { sample = cClip; }

            sample = (sample + cBias).ToInt16();
            int exponent = MuLawCompressTable[sample >> 7 & 0xFF];
            int mantissa = sample >> exponent + 3 & 0x0F;

            return (~(sign | exponent << 4 | mantissa)).ToByte();
        }
    }
}
