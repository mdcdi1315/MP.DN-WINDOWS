
using System;
using MP.Annotations;
using System.Runtime.CompilerServices;

namespace MP.BinaryPlaylist
{
    /// <summary>
    /// Base class for binary playlist blob writers. <br />
    /// It is recommended to use the <see cref="Stream"/> property for newer designs.
    /// </summary>
    public class PlaylistBlobWriter : IDisposable
    {
        private BLOBHEADER header;
        private IO.MemoryStream mems;
        /// <summary>
        /// Gets or sets a value whether the writer has written the full and complete data , 
        /// meaning that the constructed blob is ready to be consumed.
        /// </summary>
        protected System.Boolean IsCompleted;

        /// <summary>
        /// Creates a new instance of the <see cref="PlaylistBlobWriter"/> class.
        /// </summary>
        public PlaylistBlobWriter() {
            header = new();
            IsCompleted = false;
            mems = new();
        }

        internal WriterCallState WriterCall_Write(System.IO.Stream target) 
        {
            if (mems is null) { return WriterCallState.Disposed; }
            if (IsCompleted == false) { return WriterCallState.Incomplete; }
            header.Length = mems.Length;
            target.WriteStructure(header);
            mems.Position = 0;
            mems.CopyTo(target, 2048);
            return WriterCallState.Success;
        }

        /// <summary>
        /// Writes raw blob array data that are meant to be saved to a binary playlist.
        /// </summary>
        /// <param name="bytes">The array to write to the blob.</param>
        /// <param name="offset">The offset inside the array to start writing to the blob.</param>
        /// <param name="length">The number of bytes to copy from the array.</param>
        protected void Write(System.Byte[] bytes, int offset, int length) => mems.Write(bytes, offset, length);

        /// <summary>
        /// Writes the specified <see langword="long"/> to the blob.
        /// </summary>
        /// <param name="number">The <see langword="long"/> to write.</param>
        [DeprecatedMayBeRemoved] // There are no intentions to remove it but newer designs should use the Stream property.
        protected void Write(System.Int64 number) => Write(number.GetBytes(), 0, sizeof(System.Int64));

        /// <summary>
        /// Writes the specified <see langword="int"/> to the blob.
        /// </summary>
        /// <param name="number">The <see langword="int"/> to write.</param>
        [DeprecatedMayBeRemoved] // There are no intentions to remove it but newer designs should use the Stream property.
        protected void Write(System.Int32 number) => Write(number.GetBytes(), 0, sizeof(System.Int32));

        /// <summary>
        /// Writes the specified <see langword="short"/> to the blob.
        /// </summary>
        /// <param name="number">The <see langword="short"/> to write.</param>
        [DeprecatedMayBeRemoved] // There are no intentions to remove it but newer designs should use the Stream property.
        protected void Write(System.Int16 number) => Write(number.GetBytes(), 0, sizeof(System.Int16));

        /// <summary>
        /// Writes the specified <see langword="byte"/> to the blob.
        /// </summary>
        /// <param name="number">The <see langword="byte"/> to write.</param>
        [DeprecatedMayBeRemoved] // There are no intentions to remove it but newer designs should use the Stream property.
        protected void Write(System.Byte number) => Write(new System.Byte[] { number }, 0, 1);

        /// <summary>
        /// Writes the specified <see langword="sbyte"/> to the blob.
        /// </summary>
        /// <param name="number">The <see langword="sbyte"/> to write.</param>
        [DeprecatedMayBeRemoved] // There are no intentions to remove it but newer designs should use the Stream property.
        protected void Write(System.SByte number) => Write(new System.Byte[] { number.ToByte() }, 0, 1);

        /// <summary>
        /// Writes the specified <see langword="ulong"/> to the blob.
        /// </summary>
        /// <param name="number">The <see langword="ulong"/> to write.</param>
        [DeprecatedMayBeRemoved] // There are no intentions to remove it but newer designs should use the Stream property.
        protected void Write(System.UInt64 number) => Write(number.GetBytes() , 0 , sizeof(System.UInt64));

        /// <summary>
        /// Writes the specified <see langword="uint"/> to the blob.
        /// </summary>
        /// <param name="number">The <see langword="uint"/> to write.</param>
        [DeprecatedMayBeRemoved] // There are no intentions to remove it but newer designs should use the Stream property.
        protected void Write(System.UInt32 number) => Write(number.GetBytes(), 0, sizeof(System.UInt32));

        /// <summary>
        /// Writes the specified <see langword="ushort"/> to the blob.
        /// </summary>
        /// <param name="number">The <see langword="ushort"/> to write.</param>
        [DeprecatedMayBeRemoved] // There are no intentions to remove it but newer designs should use the Stream property.
        protected void Write(System.UInt16 number) => Write(number.GetBytes(), 0, sizeof(System.UInt16));

        /// <summary>
        /// Writes the specified <see langword="char"/> to the blob.
        /// </summary>
        /// <param name="character">The UTF16-LE character to write.</param>
        [DeprecatedMayBeRemoved] // There are no intentions to remove it but newer designs should use the Stream property.
        protected void Write(System.Char character) => Write(character.GetBytes() , 0 , sizeof(System.Char));

        /// <summary>
        /// Writes the specified string to the blob.
        /// </summary>
        /// <param name="str">The string to write to the blob.</param>
        /// <param name="ascii">When the value of this parameter is <see langword="true"/>, it then writes the string as ASCII; otherwise it writes the string as UTF16-LE characters.</param>
        [DeprecatedMayBeRemoved] // There are no intentions to remove it but newer designs should use the Stream property.
        protected void Write(System.String str , System.Boolean ascii)
        {
            if (ascii) {
                for (System.Int32 I = 0; I < str.Length; I++)
                {
                    mems.WriteByte(str[I].ToByte());
                }
            } else {
                System.Int32 chsi = sizeof(System.Char);
                System.UInt32 chsize = chsi.ToUInt32();
                System.Byte[] temp = new System.Byte[str.Length * chsize];
                System.Byte[] bt;
                for (System.Int32 I = 0; I < temp.Length; I += 2)
                {
                    bt = str[I / chsi].GetBytes();
                    Unsafe.CopyBlockUnaligned(ref temp[I], ref bt[0], chsize);
                }
                bt = null;
                mems.Write(temp , 0 , temp.Length);
                temp = null;
            }
        }

        /// <summary>
        /// Writes the specified structure of type <typeparamref name="T"/> to the blob.
        /// </summary>
        /// <typeparam name="T">The type of the structure to write.</typeparam>
        /// <param name="structure">The structure instance to write.</param>
        [DeprecatedMayBeRemoved] // There are no intentions to remove it but newer designs should use the Stream property.
        protected void WriteStructure<T>(T structure) where T : struct => mems.WriteStructure(structure);

        /// <summary>
        /// Writes a string that is used for padding scenarios. <br />
        /// The string is repeated if <paramref name="pads"/> exceeds the string's length.
        /// </summary>
        /// <param name="pad">The pad string from which the pad bytes will be generated from.</param>
        /// <param name="pads">The number of pad bytes to write to the stream.</param>
        [DeprecatedMayBeRemoved] // There are no intentions to remove it but newer designs should use the Stream property.
        protected void WritePadString(System.String pad , System.Int32 pads) => mems.WritePadString(pad , pads);

        /// <summary>
        /// Gets a stream suitable for writing data to the current blob. <br />
        /// Make sure to dispose the returned object after you have finished writing any data.
        /// </summary>
        protected System.IO.Stream Stream => new IO.WrapperStream(mems);

        /// <summary>
        /// Disposes all the data used by the <see cref="PlaylistBlobWriter"/> instance. <br />
        /// You also use this method to define your own dispose code. <br />
        /// Note that when you override this method, you must call this one by using the <see langword="base"/> convention.
        /// </summary>
        /// <param name="disposing">A value whether all the resources allocated in the managed memory should also be destroyed.</param>
        protected virtual void Dispose(System.Boolean disposing) 
        {
            if (disposing) { header = default; }
            mems?.Dispose();
            mems = null;
        }

        /// <summary>
        /// Gets or sets the blob header to be written to the binary playlist.
        /// </summary>
        public BLOBHEADER Header
        {
            get => header;
            set => header = value;
        }

        /// <summary>
        /// Disposes this instance of the <see cref="PlaylistBlobWriter"/> class.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Default finalizer.
        /// </summary>
        ~PlaylistBlobWriter() => Dispose(false);
    }
}
