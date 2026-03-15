
using System.IO;
using Microsoft.Build.Utilities;
using System.Runtime.CompilerServices;

namespace MusicPlayer.BuildTasks
{
    public static class SharedUtilities
    {
        public static void LogErrorWithCode(this TaskLoggingHelper task , System.String code , System.String format , params System.Object[] replacements)
            => task.LogError("", code, "", "", "", 0, 0, 0, 0, format, replacements);

        public static void LogWarningWithCode(this TaskLoggingHelper task , System.String code, System.String format , params System.Object[] replacements)
            => task.LogWarning("", code, "", "", "", 0, 0, 0, 0, format, replacements);

        public static void CopyToDirectory(this FileInfo fi , DirectoryInfo directory)
        {
            System.IO.FileStream fss = fi.OpenRead(), fst = null;
            try {
                fst = new(Path.Combine(directory.FullName, fi.Name), FileMode.Create);
                fss.DirectCopyToStream(fst, 2048);
            } finally {
                fss.Dispose();
                fst.Dispose();
            }
        }
  
        public static void MoveToDirectory(this FileInfo fi , DirectoryInfo directory)
        {
            System.IO.FileStream fss = fi.OpenRead(), fst = null;
            try {
                fst = new(Path.Combine(directory.FullName, fi.Name), FileMode.Create);
                fss.DirectCopyToStream(fst, 2048);
            } finally {
                fss.Dispose();
                fst.Dispose();
            }
            fi.Delete();
        }

        public static void DirectCopyToStream(this System.IO.Stream input, System.IO.Stream output, System.Int32 buffersize)
        {
            if (output is null) { throw new System.ArgumentNullException(nameof(output)); }
            if (buffersize < 1024) { throw new System.ArgumentOutOfRangeException(nameof(buffersize), "The buffersize parameter is too small and could degrade performance."); }
            System.Byte[] buffer = null;
            System.Int32 readin;
            try
            {

                buffer = System.Buffers.ArrayPool<System.Byte>.Shared.Rent(buffersize);

                while ((readin = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    output.Write(buffer, 0, readin);
                }

            }
            finally
            {
                if (buffer is not null)
                {
                    System.Buffers.ArrayPool<System.Byte>.Shared.Return(buffer);
                    buffer = null;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int ComputeStreamBufferSize(long consumed, long total, int buffer_size) => ((consumed + buffer_size) < total) ? buffer_size : (int)(total - consumed);

        public static System.Byte[] ReadBytes(this System.IO.Stream stream, long bytes_to_copy, int buffer_size)
        {
            if (bytes_to_copy < 0)
            {
                throw new System.ArgumentOutOfRangeException(nameof(bytes_to_copy), "The number of bytes to copy cannot be negative.");
            }
            else if (bytes_to_copy == 0)
            {
                return System.Array.Empty<System.Byte>();
            }
            else if (buffer_size < 1024)
            {
                throw new System.ArgumentOutOfRangeException(nameof(buffer_size), "The buffer_size parameter is too small and could degrade performance.");
            }

            System.Byte[] ret = new System.Byte[bytes_to_copy], buffer = null;

            int temp_read_bytes;

            try
            {

                buffer = System.Buffers.ArrayPool<System.Byte>.Shared.Rent(buffer_size);

                for (long consumed = 0; consumed < bytes_to_copy; consumed += temp_read_bytes)
                {
                    if ((temp_read_bytes = stream.Read(buffer, 0, ComputeStreamBufferSize(consumed, bytes_to_copy, buffer_size))) > 0) {
                        Unsafe.CopyBlockUnaligned(ref ret[consumed], ref buffer[0], (uint)temp_read_bytes);
                    } else {
                        break;
                    }
                }

            }
            finally
            {
                if (buffer is not null)
                {
                    System.Buffers.ArrayPool<System.Byte>.Shared.Return(buffer);
                    buffer = null;
                }
            }
            return ret;
        }
    }
}