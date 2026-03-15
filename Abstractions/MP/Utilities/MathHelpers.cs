
using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace MP.Utilities
{
    /// <summary>
    /// Mathematical utilities found useful at times.
    /// </summary>
    public static class MathHelpers
    {
        /// <summary>
        /// Maps a value in the range [0..1] to the range [<paramref name="start"/>..<paramref name="end"/>].
        /// </summary>
        /// <param name="value">The value to map.</param>
        /// <param name="start">The lower bound of the range to map the value to.</param>
        /// <param name="end">The upper bound of the range to map the value to.</param>
        /// <returns>The mapped value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Lerp(double value , double start , double end) => (start + value) * (end - start);

        /// <summary>
        /// Maps a value in the range [0..1] to the range [<paramref name="start"/>..<paramref name="end"/>].
        /// </summary>
        /// <param name="value">The value to map.</param>
        /// <param name="start">The lower bound of the range to map the value to.</param>
        /// <param name="end">The upper bound of the range to map the value to.</param>
        /// <returns>The mapped value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Lerp(float value, float start, float end) => (start + value) * (end - start);

        /// <summary>Clamps the specified value into the specified range.</summary>
        /// <param name="value">The value to be clamped.</param>
        /// <param name="min">The lower bound that is the minimum value that can be returned by this method.</param>
        /// <param name="max">The upper bound that is the maximum value that can be returned by this method.</param>
        /// <returns>A value in the exact range [<paramref name="min"/>..<paramref name="max"/>].</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Clamp(double value, double min, double max) => value > max ? max : Math.Max(value, min);

        /// <summary>Clamps the specified value into the specified range.</summary>
        /// <param name="value">The value to be clamped.</param>
        /// <param name="min">The lower bound that is the minimum value that can be returned by this method.</param>
        /// <param name="max">The upper bound that is the maximum value that can be returned by this method.</param>
        /// <returns>A value in the exact range [<paramref name="min"/>..<paramref name="max"/>].</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Clamp(float value, float min, float max) => value > max ? max : Math.Max(value, min);

        /// <summary>
        /// Returns the smallest integral value that is greater than or equal to <paramref name="value"/>.
        /// </summary>
        /// <param name="value">A single-precision floating-point number.</param>
        /// <returns>The smallest integral value greater than or equal to <paramref name="value"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Ceiling(float value)
        {
            int v = (int)value, p1 = v + 1;
            return (p1 > value) ? p1 : v; 
        }

        /// <summary>
        /// Returns the smallest integral value that is greater than or equal to <paramref name="value"/>.
        /// </summary>
        /// <param name="value">A double-precision floating-point number.</param>
        /// <returns>The smallest integral value greater than or equal to <paramref name="value"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Ceiling(double value)
        {
            long v = (long)value, p1 = v + 1L;
            return (p1 > value) ? p1 : v;
        }

        /// <summary>
        /// Returns the largest integral value that is less than or equal to <paramref name="value"/>.
        /// </summary>
        /// <param name="value">A double-precision floating-point number.</param>
        /// <returns>The largest integral value less than or equal to <paramref name="value"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Floor(float value)
        {
            int v = (int)value, m1 = v - 1;
            return (m1 > value) ? m1 : v;
        }

        /// <summary>
        /// Returns the largest integral value that is less than or equal to <paramref name="value"/>.
        /// </summary>
        /// <param name="value">A single-precision floating-point number.</param>
        /// <returns>The largest integral value less than or equal to <paramref name="value"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Floor(double value)
        {
            long v = (long)value, m1 = v - 1L;
            return (m1 > value) ? m1 : v;
        }

        /// <summary>
        /// Converts an unsigned short integer to a byte, respecting the limits of both numeric types. <br />
        /// However, the arithmetic precision is downgraded by 257 values.
        /// </summary>
        /// <param name="value">The value to convert to a byte range.</param>
        /// <returns>The converted value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte ToByteRange(ushort value) => (byte)(value / 257);

        /// <summary>
        /// Converts an unsigned integer to a byte, respecting the limits of both numeric types. <br />
        /// However, the arithmetic precision is downgraded by 16843009 values.
        /// </summary>
        /// <param name="value">The value to convert to a byte range.</param>
        /// <returns>The converted value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte ToByteRange(uint value) => (byte)(value / 16843009U);

        /// <summary>
        /// Converts an unsigned long integer to a byte, respecting the limits of both numeric types. <br />
        /// However, the arithmetic precision is downgraded by 72340172838076673 values.
        /// </summary>
        /// <param name="value">The value to convert to a byte range.</param>
        /// <returns>The converted value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte ToByteRange(ulong value) => (byte)(value / 72340172838076673UL);

        /// <summary>
        /// Converts an unsigned integer to an unsigned short integer, respecting the limits of both numeric types. <br />
        /// However, the arithmetic precision is downgraded by 65537 values.
        /// </summary>
        /// <param name="value">The value to convert to a byte range.</param>
        /// <returns>The converted value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort ToUShortRange(uint value) => (ushort)(value / 65537U);

        /// <summary>
        /// Converts an unsigned long integer to an unsigned short integer, respecting the limits of both numeric types. <br />
        /// However, the arithmetic precision is downgraded by 281479271743489 values.
        /// </summary>
        /// <param name="value">The value to convert to a byte range.</param>
        /// <returns>The converted value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort ToUShortRange(ulong value) => (ushort)(value / 281479271743489UL);

        /// <summary>
        /// Maps a value from the range [<paramref name="inputlowerbound"/>..<paramref name="inputupperbound"/>] to the range [<paramref name="outputlowerbound"/>..<paramref name="outputupperbound"/>].
        /// The value is clamped, if necessary.
        /// </summary>
        /// <param name="input">The input value to map.</param>
        /// <param name="inputlowerbound">The lower bound of the input value range.</param>
        /// <param name="inputupperbound">The upper bound of the input value range.</param>
        /// <param name="outputlowerbound">The lower bound of the output value range.</param>
        /// <param name="outputupperbound">The upper bound of the output value range.</param>
        /// <returns>The mapped value, clamped to [<paramref name="outputlowerbound"/>..<paramref name="outputupperbound"/>].</returns>
        /*
             * What does it do:
             * What it could be written as a math expression (assuming that 'input' is in range):
             * ToNormalRange(input , inputlowerbound , inputupperbound) * (outputupperbound - outputlowerbound)) + outputlowerbound
             * That is , I am expressing input in the range (0..1).
             * Then, I can transform such a range to any possible number range thus multiplying with (outputupperbound - outputlowerbound) would give us a range of [0..(outputupperbound - outputlowerbound)].
             * Finally add outputlowerbound to the result to bring it into [outputlowerbound..outputupperbound] range. 
        */
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double ClampedMapToRange(double input, double inputlowerbound, double inputupperbound, double outputlowerbound, double outputupperbound) 
            => (ToNormalRange(Clamp(input, inputlowerbound, inputupperbound), inputlowerbound, inputupperbound) * (outputupperbound - outputlowerbound)) + outputlowerbound;

        /// <summary>
        /// Maps a value from the range [<paramref name="inputlowerbound"/>..<paramref name="inputupperbound"/>] to the range [<paramref name="outputlowerbound"/>..<paramref name="outputupperbound"/>].
        /// The value is clamped, if necessary.
        /// </summary>
        /// <param name="input">The input value to map.</param>
        /// <param name="inputlowerbound">The lower bound of the input value range.</param>
        /// <param name="inputupperbound">The upper bound of the input value range.</param>
        /// <param name="outputlowerbound">The lower bound of the output value range.</param>
        /// <param name="outputupperbound">The upper bound of the output value range.</param>
        /// <returns>The mapped value, clamped to [<paramref name="outputlowerbound"/>..<paramref name="outputupperbound"/>].</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ClampedMapToRange(float input, float inputlowerbound, float inputupperbound, float outputlowerbound, float outputupperbound)
            => (ToNormalRange(Clamp(input , inputlowerbound , inputupperbound), inputlowerbound, inputupperbound) * (outputupperbound - outputlowerbound)) + outputlowerbound;

        /// <summary>
        /// Maps a number of any range to the normal range. <br />
        /// That is, a conversion happens from (<paramref name="min"/>..<paramref name="max"/>) to (0..1) range respectively.
        /// </summary>
        /// <param name="v">The number in the (<paramref name="min"/>..<paramref name="max"/>) range to be converted to the (0..1) range.</param>
        /// <param name="min">The lower bound of the values that the number in <paramref name="v"/> can be into,</param>
        /// <param name="max">The upper bound of the values that the number in <paramref name="v"/> can be into,</param>
        /// <returns>A mapped number in the range (0..1).</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double ToNormalRange(double v, double min, double max) => Math.Abs(v - min) / Math.Abs(min - max);
        
        /// <summary>
        /// Maps a number of any range to the normal range. <br />
        /// That is, a conversion happens from (<paramref name="min"/>..<paramref name="max"/>) to (0..1) range respectively.
        /// </summary>
        /// <param name="v">The number in the (<paramref name="min"/>..<paramref name="max"/>) range to be converted to the (0..1) range.</param>
        /// <param name="min">The lower bound of the values that the number in <paramref name="v"/> can be into,</param>
        /// <param name="max">The upper bound of the values that the number in <paramref name="v"/> can be into,</param>
        /// <returns>A mapped number in the range (0..1).</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ToNormalRange(float v, float min, float max) => Math.Abs(v - min) / Math.Abs(min - max);

        /// <summary>
        /// Computes the buffer sizes when dispatching requests regarding methods related to data 
        /// streams that are instructed to read a fixed number of bytes, and are using temporary buffers. <br />
        /// You pass in the actual number of bytes read or to write, the total number of bytes to read or write, and the actual size of the buffer. <br />
        /// Example: <br /> <br />
        /// <code>
        /// // An example of how to use it
        /// System.IO.Stream stream; // A data stream to read from. Assumed that it is properly initialized before.
        /// System.Byte[] temp = new System.Byte[2048];
        /// 
        /// long total_bytes = 300000;
        /// 
        /// int bytes_read;
        /// 
        /// for (long c = 0; c &lt; total_bytes; c += bytes_read)
        /// {
        ///     bytes_read = stream.Read(temp , 0 , ComputeStreamBufferSize(c , total_bytes, 2048));
        ///     
        ///     // Do something with the data now...
        /// }
        /// </code>
        /// </summary>
        /// <param name="consumed">The number of bytes already processed.</param>
        /// <param name="total">The total number of bytes that are to be read/written.</param>
        /// <param name="buffer_size">The temporary buffer size.</param>
        /// <returns>A computed value, so that the value is in range [0..<paramref name="buffer_size"/>].</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ComputeBufferSize(long consumed, long total, int buffer_size)
            => ((consumed + buffer_size) < total) ? buffer_size : (int)(total - consumed);

        /// <summary>
        /// Computes the buffer sizes when dispatching requests regarding methods related to data 
        /// streams that are instructed to read a fixed number of bytes, and are using temporary buffers. <br />
        /// You pass in the actual number of bytes read or to write, the total number of bytes to read or write, and the actual size of the buffer. <br />
        /// For an detailed example, see the <see cref="ComputeBufferSize(long, long, int)"/> method.
        /// </summary>
        /// <param name="consumed">The number of bytes already processed.</param>
        /// <param name="total">The total number of bytes that are to be read/written.</param>
        /// <param name="buffer_size">The temporary buffer size.</param>
        /// <returns>A computed value, so that the value is in range [0..<paramref name="buffer_size"/>].</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ComputeBufferSize(long consumed, long total, uint buffer_size)
            => ((consumed + buffer_size) < total) ? buffer_size : (uint)(total - consumed);

        /// <summary>
        /// Computes the buffer sizes when dispatching requests regarding methods related to data 
        /// streams that are instructed to read a fixed number of bytes, and are using temporary buffers. <br />
        /// You pass in the actual number of bytes read or to write, the total number of bytes to read or write, and the actual size of the buffer. <br />
        /// For an detailed example, see the <see cref="ComputeBufferSize(long, long, int)"/> method.
        /// </summary>
        /// <param name="consumed">The number of bytes already processed.</param>
        /// <param name="total">The total number of bytes that are to be read/written.</param>
        /// <param name="buffer_size">The temporary buffer size.</param>
        /// <returns>A computed value, so that the value is in range [0..<paramref name="buffer_size"/>].</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ComputeBufferSize(ulong consumed, ulong total, int buffer_size)
            => ((consumed + buffer_size.ToUInt32()) < total) ? buffer_size : (int)(total - consumed);

        /// <summary>
        /// Computes the buffer sizes when dispatching requests regarding methods related to data 
        /// streams that are instructed to read a fixed number of bytes, and are using temporary buffers. <br />
        /// You pass in the actual number of bytes read or to write, the total number of bytes to read or write, and the actual size of the buffer. <br />
        /// For an detailed example, see the <see cref="ComputeBufferSize(long, long, int)"/> method.
        /// </summary>
        /// <param name="consumed">The number of bytes already processed.</param>
        /// <param name="total">The total number of bytes that are to be read/written.</param>
        /// <param name="buffer_size">The temporary buffer size.</param>
        /// <returns>A computed value, so that the value is in range [0..<paramref name="buffer_size"/>].</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ComputeBufferSize(ulong consumed, ulong total, uint buffer_size)
            => ((consumed + buffer_size) < total) ? buffer_size : (uint)(total - consumed);

        /// <summary>
        /// Computes the buffer sizes when dispatching requests regarding methods related to data 
        /// streams that are instructed to read a fixed number of bytes, and are using temporary buffers. <br />
        /// You pass in the actual number of bytes read or to write, the total number of bytes to read or write, and the actual size of the buffer. <br />
        /// For an detailed example, see the <see cref="ComputeBufferSize(long, long, int)"/> method.
        /// </summary>
        /// <param name="consumed">The number of bytes already processed.</param>
        /// <param name="total">The total number of bytes that are to be read/written.</param>
        /// <param name="buffer_size">The temporary buffer size.</param>
        /// <returns>A computed value, so that the value is in range [0..<paramref name="buffer_size"/>].</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ComputeBufferSize(int consumed, int total, int buffer_size)
            => ((consumed + buffer_size) < total) ? buffer_size : total - consumed;
    }
}
