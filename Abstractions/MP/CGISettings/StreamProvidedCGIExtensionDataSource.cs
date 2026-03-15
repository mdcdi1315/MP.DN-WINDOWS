
using System.Text;

namespace MP.CGISettings
{
    internal sealed class StreamProvidedCGIExtensionDataSource
        : ICGISettingExtensionDataSource
    {
        private IO.DataStream strm;
        private Encoding encoding;
        private System.Int64 bloblen;
        private System.Int64 rdata;

        public StreamProvidedCGIExtensionDataSource(IO.DataStream strm , Encoding enc , System.Int64 bloblen)
        {
            this.strm = strm;
            encoding = enc;
            this.bloblen = bloblen;
            rdata = 0;
        }

        public System.Int64 Length => bloblen;

        public Encoding StringEncoding => encoding;

        public DataSourceType Type
        {
            get {
                if (strm.CanRead && strm.CanWrite)
                {
                    return DataSourceType.Both;
                } else if (strm.CanRead)
                {
                    return DataSourceType.Reader;
                } else if (strm.CanWrite) { 
                    return DataSourceType.Writer;
                }
                return (DataSourceType)255;
            }
        }

        public int Read(byte[] buffer, int offset, int count)
        {
            if (strm is null || count == 0) { return 0; }
            return strm.Read(buffer, offset, (rdata + count > bloblen ? bloblen - count : count).ToInt32());
        }

        public void Write(byte[] buffer, int offset, int count)
        {
            if (strm is null || count == 0) { return; }
            strm.Write(buffer , offset, count);
        }

        public void Dispose()
        {
            strm = null;
            encoding = null;
            bloblen = 0;
        }
    }
}