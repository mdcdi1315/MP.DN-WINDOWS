
using System;
using MP.Utilities;
using System.Runtime.CompilerServices;
using System.Numerics;
using MP.TagReading.MP4;

// The below warning is disabled because all calls that evolve it are sizeof's which only get sizes of unmanaged types.
#pragma warning disable 8500 // Declares a pointer to , takes the address of , or gets the size of a managed type

/* NOTE NOTE NOTE
      'AggressiveInlining' attributes should be only declared to methods that are consisting of only primitive instructions
      (such as additions and divisions) and methods that contain only primitive instructions.
      Furthermore, these techniques can be also applied:
      -> Most of the Unsafe class methods do also have only primitive instructions (ldobj , cpblk , etc) so these can be used too.
      -> Small array creation and disposal should be also considered as primitive instructions too (newarr instruction).
      -> stackalloc calls should be also considered fast primitive instructions since they allocate from the stack (localloc instruction).
      Anything else will possibly have perf impact.
 */

namespace MP
{
    /// <summary>Provides extension methods for 'unsafely' manipulating information.</summary>
    public static unsafe partial class UnsafeMethods
    {
        // The unsafe calls allow us to bypass definite runtime checks for conversions.
        // These are also implemented in pure MSIL - which means that these can work in any possible .NET platform.
        // All these operations work on checked mode but note that actually work as if the conversion was performed in unchecked mode.

        // Note also that we will have an additional perf gain in .NET 10 - where there the 'stackalloc' is gaining further optimizations.

        // NOTE: The method implementation is the same as the Unsafe.BitCast class method.
        [System.Diagnostics.DebuggerHidden]
        // Keep this method with no additional optimizations (Seems to lose a tick or so)
        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static TTo LinearConversion<TFrom, TTo>(TFrom source)
            where TFrom : struct
            where TTo : struct
        {
            if (sizeof(TFrom) != sizeof(TTo)) { throw new NotSupportedException("The structures must have the same size so that the linear conversion can be performed."); }
            return Unsafe.ReadUnaligned<TTo>(ref Unsafe.As<TFrom, System.Byte>(ref source));
        }

        // Reinterprets the given unmanaged type to an equivalent and recreatable representation 
        // in a plain byte array.
        [System.Diagnostics.DebuggerHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static System.Byte[] GetBytesTemplate<T>(T input) where T : unmanaged
        {
            // We cannot introduce stackalloc here because we might have a large unmanaged type (> 100 bytes).
            System.Byte[] bytes = new System.Byte[sizeof(T)];
            Unsafe.As<System.Byte, T>(ref bytes[0]) = input;
            return bytes;
        }

        // Converts the given plain data to a new structure with type T.
        // Be noted that the method only requires the minimum amount of bytes in order to recreate the structure;
        // the method does not test against exact size.
        [System.Diagnostics.DebuggerHidden]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2208",
            Justification = "This method is always hidden by the debugger and any methods that use this pass the sidx parameter as StartIndex.")]
        private static T GetFromBytesTemplate<T>(System.Byte[] bytes , System.Int32 sidx) where T : unmanaged
        {
            if (sidx < 0 || (sidx + sizeof(T)) > bytes.Length) {
                throw new ArgumentOutOfRangeException("StartIndex" , "StartIndex must be more or equal to zero and smaller than the array length plus the size of the structure.");
            } else {
                return Unsafe.ReadUnaligned<T>(ref bytes[sidx]);
            }
        }

        /// <summary>
        /// Gets the managed reference of the first array element in <paramref name="array"/>.
        /// </summary>
        /// <typeparam name="T">The array type.</typeparam>
        /// <param name="array">The array to retrieve the first element from.</param>
        /// <returns>The reference of the first element in <paramref name="array"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T GetFirstArrayElementRef<T>(this T[] array) where T : notnull
        {
            if (array is null || array.Length < 1) { 
                return ref Unsafe.NullRef<T>(); 
            } else {
                return ref array[0];
            }
        }

        /// <summary>
        /// Copies the value of the primitive type <typeparamref name="T"/> to a new instance of <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The primitive type to copy.</typeparam>
        /// <param name="input">The input value type to copy.</param>
        /// <returns>A copied version of <paramref name="input"/>.</returns>
        public static T Copy<T>(this T input) where T : unmanaged
        {
            int size = sizeof(T);
            System.Byte[] copied = new System.Byte[size];
            // Instead of allocating a new array, cook the input reference instead.
            Unsafe.CopyBlockUnaligned(ref copied[0], ref Unsafe.As<T, System.Byte>(ref input), size.ToUInt32());
            return Unsafe.ReadUnaligned<T>(ref copied[0]);
        }

        /// <summary>
        /// Copies bytes from the current array to the target array. <br />
        /// The copy is performed using unsafe schemes.
        /// </summary>
        /// <param name="src">The source array to copy from.</param>
        /// <param name="srcidx">The index inside <paramref name="src"/> to start copying from.</param>
        /// <param name="dest">The array that will recieve the data from <paramref name="src"/>.</param>
        /// <param name="destidx">The index inside <paramref name="dest"/> to start writing from.</param>
        /// <param name="count">The number of bytes to copy.</param>
        /// <exception cref="ArgumentNullException"><paramref name="src"/> and/or <paramref name="dest"/> were null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="destidx"/> , <paramref name="count"/> or <paramref name="srcidx"/> were out of their array bounds.</exception>
        /// <exception cref="ArgumentException"><paramref name="destidx"/> and/or <paramref name="srcidx"/> were not into their array bounds.</exception>
        public static void Copy(this System.Byte[] src, System.Int32 srcidx,
            System.Byte[] dest, System.Int32 destidx, System.UInt32 count)
        {
            if (src is null) { throw new ArgumentNullException(nameof(src)); }
            if (dest is null) { throw new ArgumentNullException(nameof(dest)); }
            if (srcidx < 0 || destidx < 0) { throw new ArgumentOutOfRangeException(nameof(src), "srcidx and destidx must be greater than or equal to zero."); }
            if (srcidx >= src.Length)
            {
                throw new ArgumentException("srcidx must be less than the array length.");
            }
            if (destidx >= dest.Length)
            {
                throw new ArgumentException("destidx must be less than the array length.");
            }
            if (src.Length - srcidx < count || dest.Length - destidx < count)
            {
                throw new ArgumentOutOfRangeException(nameof(src) , "The number of elements to be copied are not available. Either change start index or enlarge the arrays.");
            }
            Unsafe.CopyBlockUnaligned(ref dest[destidx], ref src[srcidx], count);
        }

        /// <summary>
        /// Copies bytes from the current array to the target array. <br />
        /// The copy is performed using unsafe schemes.
        /// </summary>
        /// <param name="src">The source array to copy from.</param>
        /// <param name="srcidx">The index inside <paramref name="src"/> to start copying from.</param>
        /// <param name="dest">The array that will recieve the data from <paramref name="src"/>.</param>
        /// <param name="destidx">The index inside <paramref name="dest"/> to start writing from.</param>
        /// <param name="count">The number of bytes to copy.</param>
        /// <exception cref="ArgumentNullException"><paramref name="src"/> and/or <paramref name="dest"/> were null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="destidx"/> , <paramref name="count"/> or <paramref name="srcidx"/> were out of their array bounds.</exception>
        /// <exception cref="ArgumentException"><paramref name="destidx"/> and/or <paramref name="srcidx"/> were not into their array bounds.</exception>
        public static void Copy(this System.Byte[] src , System.Int64 srcidx , 
            System.Byte[] dest , System.Int64 destidx , System.UInt32 count)
        {
            ArgumentNullException.ThrowIfNull(src);
            ArgumentNullException.ThrowIfNull(dest);
            if (srcidx < 0 || destidx < 0) { throw new ArgumentOutOfRangeException(nameof(src), "srcidx and destidx must be greater than or equal to zero."); }
            if (srcidx >= src.Length)
            {
                throw new ArgumentException("srcidx must be less than the array length.");
            }
            if (destidx >= dest.Length)
            {
                throw new ArgumentException("destidx must be less than the array length.");
            }
            if (src.Length - srcidx < count || dest.Length - destidx < count)
            {
                throw new ArgumentOutOfRangeException("", "The number of elements to be copied are not available. Either change start index or enlarge the arrays.");
            }
            Unsafe.CopyBlockUnaligned(ref dest[destidx], ref src[srcidx], count);
        }

        /// <summary>
        /// Gets a slice out of the current byte array, just like it happens with the <see cref="Span{T}.Slice(int, int)"/> method.
        /// </summary>
        /// <param name="source">The source byte array.</param>
        /// <param name="index">The index to start copying elements to the new array.</param>
        /// <param name="count">The number of elements that the newly returned array will have.</param>
        /// <returns>The slice formed by copying <paramref name="count"/> bytes starting from <paramref name="index"/>.</returns>
        /// <exception cref="ArgumentException"><paramref name="count"/> and <paramref name="index"/> excced the source array length.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count"/> and/or <paramref name="index"/> are negative values.</exception>
        public static System.Byte[] GetBytes(this System.Byte[] source, System.Int32 index, System.Int32 count)
        {
            if (count < 0) { 
                throw new ArgumentOutOfRangeException(nameof(count), "Count cannot be a negative value.");
            } else if (index < 0) { 
                throw new ArgumentOutOfRangeException(nameof(index), "Index cannot be a negative value.");
            } else if (count - index > source.Length) { 
                throw new ArgumentException("The source array does not have so many elements to copy!");
            } else {
                System.Byte[] result = new System.Byte[count];
                Unsafe.CopyBlockUnaligned(ref result[0], ref source[index], count.ToUInt32());
                return result;
            }
        }

        /// <summary>
        /// Reads a managed array or native memory from the <paramref name="input"/> managed pointer and copies the 
        /// result to a new managed array.
        /// </summary>
        /// <typeparam name="T">The managed type to read from the native or managed memory.</typeparam>
        /// <param name="input">The managed pointer to read data from.</param>
        /// <param name="length">The data length. The number given here will be the length of the returned array.</param>
        /// <returns>A new array of <typeparamref name="T"/> copied from <paramref name="input"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="input"/> represents a null pointer.</exception>
        public static T[] ReadCopy<T>(this ref T input, System.UInt32 length) where T : struct
        {
            if (Unsafe.IsNullRef(ref input)) { throw new ArgumentNullException(nameof(input)); }
            T[] result = new T[length];
            for (System.Int32 I = 0; I < length; I++)
            {
                result[I] = Unsafe.Add(ref input, I);
            }
            return result;
        }

        /// <summary>
        /// Reverses endianess for the primitive type <typeparamref name="T"/>. <br />
        /// If the <typeparamref name="T"/> is <see cref="System.Byte"/> , then it performs endianess swap in bit level.
        /// </summary>
        /// <typeparam name="T">The primitive type to reverse endianess for.</typeparam>
        /// <param name="input">The input type to reverse.</param>
        /// <returns>The reversed endianess result of <paramref name="input"/>.</returns>
        // The method does special-case the byte type so that endianess reversal can be performed to it.
        public static T ReverseEndianess<T>(this T input) where T : unmanaged
        {
            if (input is System.Byte b) {
                // Endianess swap in byte level is now possible because a decomposal method to binary was found.
                b = ReverseEndianess_Byte(b);
                return Unsafe.As<System.Byte , T>(ref b);
            } else {
                // Since this does not modify the original input because input is passed by value, we are possibly OK
                ref T reference = ref input;
                ReverseInner_Byte(ref Unsafe.As<T, System.Byte>(ref reference), new System.UIntPtr(sizeof(T).ToUInt32()));
                return reference;
            }
        }

        private static System.Byte ReverseEndianess_Byte(System.Byte b)
        {
            System.Boolean[] booleans = b.ToBinary();
            // Reverse bin data
            booleans.Reverse();
            System.Byte result = 0;
            // Then assign the reversed result
            for (System.Int32 I = 0; I < booleans.Length; I++) {
                if (booleans[I]) { result.SetBit(I, true); }
            }
            return result;
        }

        /// <summary>
        /// Reverses the entire sequence of the array elements.
        /// </summary>
        /// <param name="array">The array to be reversed.</param>
        /// <typeparam name="T">The type of a single element inside the array , the array type.</typeparam>
        public static void Reverse<T>(this T[] array) where T : notnull
        {
            if (array is null) { return; }
            ReverseInner(ref GetFirstArrayElementRef(array), new System.UIntPtr(array.GetLength(0).ToUInt32()));
        }

        // Acquired from https://github.com/dotnet/runtime/blob/main/src/libraries/System.Private.CoreLib/src/System/SpanHelpers.cs
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ReverseInner<T>(ref T elements, nuint length)
        {
            if (length < 2) { return; }

            ref T first = ref elements;
            ref T last = ref Unsafe.Subtract(ref Unsafe.Add(ref first, length), 1);
            do
            {
                T temp = first;
                first = last;
                last = temp;
                first = ref Unsafe.Add(ref first, 1);
                last = ref Unsafe.Subtract(ref last, 1);
            } while (Unsafe.IsAddressLessThan(ref first, ref last));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ReverseInner_Byte(ref System.Byte elements, nuint length)
        {
            if (length < 2) { return; }

            ref System.Byte first = ref elements;
            ref System.Byte last = ref Unsafe.Subtract(ref Unsafe.Add(ref first, length), 1);
            do {
                System.Byte temp = first;
                first = last;
                last = temp;
                first = ref Unsafe.Add(ref first, 1);
                last = ref Unsafe.Subtract(ref last, 1);
            } while (Unsafe.IsAddressLessThan(ref first, ref last));
        }

        // ReadStructure and WriteStructure methods are safer alternatives to Unsafe.ReadUnaligned and Unsafe.WriteUnaligned,
        // for cases that you have to read or write a structure to an array.

        /// <summary>
        /// Reads a structure of type <typeparamref name="T"/> from the specified byte array.
        /// </summary>
        /// <typeparam name="T">The structure type to read.</typeparam>
        /// <param name="data">The byte array to read the structure from.</param>
        /// <param name="startindex">The starting index to read from.</param>
        /// <returns>The read structure , returned as <typeparamref name="T"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="startindex"/> was negative , or it's current value is not enough to read a structure of type <typeparamref name="T"/>.</exception>
        public static T ReadStructure<T>(this System.Byte[] data, System.Int32 startindex) where T : struct
        {
            System.Int32 ssize = sizeof(T);
            if (startindex < 0) { 
                throw new ArgumentOutOfRangeException(nameof(startindex), "Start index cannot be a negative value.");
            } else if (data.Length - startindex < ssize) { 
                throw new ArgumentOutOfRangeException(nameof(startindex), $"The array is not large enough in order to read a structure of type {typeof(T).FullName}.");
            } else {
                T ret = new(); // We can execute 'new' in a struct because a struct must always define the empty ctor
                Unsafe.CopyBlockUnaligned(ref Unsafe.As<T, System.Byte>(ref ret), ref data[startindex], ssize.ToUInt32());
                return ret;
            }
        }

        /// <summary>
        /// Writes a structure of type <typeparamref name="T"/> to the specified byte array.
        /// </summary>
        /// <typeparam name="T">The structure type to write.</typeparam>
        /// <param name="data">The byte array to read the structure to.</param>
        /// <param name="startindex">The starting index to start writing from.</param>
        /// <param name="structure">The structure to write.</param>
        /// <returns>The number of written bytes to <paramref name="data"/>. Useful for streaming methods.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="startindex"/> was negative , or it's current value is not enough to write a structure of type <typeparamref name="T"/>.</exception>
        public static int WriteStructure<T>(this System.Byte[] data, System.Int32 startindex, T structure) where T : struct
        {
            System.Int32 ssize = sizeof(T);
            if (startindex < 0) { throw new ArgumentOutOfRangeException(nameof(startindex)); }
            // NRE is tested by the below statement , so leave it as is
            if (data.Length - startindex < ssize) { throw new ArgumentOutOfRangeException(nameof(startindex), $"The array is not large enough in order to write a structure of type {typeof(T).FullName}."); }
            Unsafe.CopyBlockUnaligned(ref data[startindex], ref Unsafe.As<T, System.Byte>(ref structure), ssize.ToUInt32());
            return ssize;
        }

        /// <summary>
        /// Like <see cref="WriteStructure{T}(byte[], int, T)"/>, this method does instead create the array where the structure will be written to.
        /// </summary>
        /// <typeparam name="T">The structure type to write to a new array.</typeparam>
        /// <param name="structure">The instance to write.</param>
        /// <returns>The written structure, decomposed to a new byte array.</returns>
        public static System.Byte[] WriteStructureToNewArray<T>(this T structure) where T : struct
        {
            System.Int32 size = sizeof(T);
            System.Byte[] ret = new System.Byte[size];
            Unsafe.CopyBlockUnaligned(ref ret[0], ref Unsafe.As<T, System.Byte>(ref structure), size.ToUInt32());
            return ret;
        }
        
        /// <summary>
        /// Reinterprets a structure of type <typeparamref name="TFrom"/> to a structure of type <typeparamref name="TTo"/>. <br />
        /// If <typeparamref name="TFrom"/> is a primitive structure and the platform is little-endian, the data are reversed before they are loaded.
        /// </summary>
        /// <typeparam name="TFrom">The input structure to be reinterpreted.</typeparam>
        /// <typeparam name="TTo">The desired structure type.</typeparam>
        /// <param name="from">The input to be reinterpreted.</param>
        /// <returns>The resulting structure of type <typeparamref name="TTo"/>.</returns>
        /// <exception cref="ArgumentException"><typeparamref name="TFrom"/> and <typeparamref name="TTo"/> do not have the same size.</exception>
        public static TTo ReinterpretStructure<TFrom, TTo>(this TFrom from)
            where TFrom : struct
            where TTo : struct
        {
            int tts = sizeof(TTo) , ttf = sizeof(TFrom);
            if (ttf != tts) {
                throw new ArgumentException("Recieving and output types must have the same size.");
            }
            ref TTo ret = ref Unsafe.As<TFrom, TTo>(ref from);
            if (IsLittleEndian && typeof(TFrom).IsPrimitive) {
                ReverseInner_Byte(ref Unsafe.As<TTo, byte>(ref ret), (nuint)tts);
            }
            return ret;
        }

        /// <summary>
        /// Reinterprets a numeric type to a structure type.
        /// </summary>
        /// <typeparam name="TFrom">The input numeric type to be reinterpreted.</typeparam>
        /// <typeparam name="TTo">The desired structure type.</typeparam>
        /// <param name="from">The input to be reinterpreted.</param>
        /// <returns>The resulting structure of type <typeparamref name="TTo"/>.</returns>
        /// <exception cref="ArgumentException"><typeparamref name="TFrom"/> and <typeparamref name="TTo"/> do not have the same size.</exception>
        public static TTo ReinterpretAsStructure<TFrom, TTo>(this TFrom from)
            where TTo : struct
            where TFrom : struct, INumber<TFrom>
        {
            int tts = sizeof(TTo) , ttf = sizeof(TFrom);
            if (ttf != tts) {
                throw new ArgumentException("Recieving and output types must have the same size.");
            }
            ref TTo ret = ref Unsafe.As<TFrom, TTo>(ref from);
            if (IsLittleEndian) {
                ReverseInner_Byte(ref Unsafe.As<TTo, byte>(ref ret), (nuint)tts);
            }
            return ret;
        }

        /// <summary>
        /// Copies data from a <paramref name="source"/> memory block to a <paramref name="destination"/> memory block.
        /// </summary>
        /// <param name="source">The source pointer.</param>
        /// <param name="destination">The destination pointer.</param>
        /// <param name="byte_length">Number of bytes to copy from <paramref name="source"/> to <paramref name="destination"/>.</param>
        public static void MemoryCopy(void* source, void* destination, ulong byte_length)
        {
            const System.UInt32 ONE_MB = 1048576U;
            if (source is null || destination is null) {
                return;
            } else if (byte_length > ONE_MB) {
                byte* temp_source = (byte*)source, 
                    temp_dest = (byte*)destination;
                for (ulong consumed = 0; consumed < byte_length; consumed += ONE_MB)
                {
                    Unsafe.CopyBlockUnaligned(temp_dest, temp_source, MathHelpers.ComputeBufferSize(consumed, byte_length, ONE_MB));
                    temp_dest += ONE_MB;
                    temp_source += ONE_MB;
                }
            } else {
                Unsafe.CopyBlockUnaligned(destination, source, byte_length.ToUInt32());
            }
        }

        #region Conversions to Int64
        /// <summary>
        /// Uses unsafe schemes to convert a <see cref="System.Int32"/> to a <see cref="System.Int64"/>. 
        /// The conversion is only performed with less checks during runtime.
        /// </summary>
        /// <param name="number">The number to convert.</param>
        public static System.Int64 ToInt64(this System.Int32 number)
        {
            System.Int64* ppt = stackalloc System.Int64[1];
            *((System.Int32*)ppt) = number; // The input data type fits into the result type , and it is smaller than that
            return *ppt;
        }

        /// <summary>
        /// Uses unsafe schemes to convert a <see cref="System.UInt64"/> to a <see cref="System.Int64"/>. 
        /// The conversion is only performed with less checks during runtime.
        /// </summary>
        /// <param name="number">The number to convert.</param>
        public static System.Int64 ToInt64(this System.UInt64 number) => Unsafe.As<System.UInt64, System.Int64>(ref number);

        /// <summary>
        /// Uses unsafe schemes to convert a <see cref="System.Char"/> to a <see cref="System.Int64"/>. 
        /// The conversion is only performed with less checks during runtime.
        /// </summary>
        /// <param name="character">The character to convert.</param>
        public static System.Int64 ToInt64(this System.Char character)
        {
            System.Int64* ppt = stackalloc System.Int64[1];
            *((System.Char*)ppt) = character;
            return *ppt;
        }

        /// <summary>
        /// Uses unsafe schemes to convert a <see cref="System.UInt32"/> to a <see cref="System.Int64"/>. 
        /// The conversion is only performed with less checks during runtime.
        /// </summary>
        /// <param name="number">The number to convert.</param>
        public static System.Int64 ToInt64(this System.UInt32 number)
        {
            System.Int64* ppt = stackalloc System.Int64[1];
            *((System.UInt32*)ppt) = number;
            return *ppt;
        }

        /// <summary>
        /// Uses unsafe schemes to convert a <see cref="System.UInt16"/> to a <see cref="System.Int64"/>. 
        /// The conversion is only performed with less checks during runtime.
        /// </summary>
        /// <param name="number">The number to convert.</param>
        public static System.Int64 ToInt64(this System.UInt16 number)
        {
            System.Int64* ppt = stackalloc System.Int64[1];
            *((System.UInt16*)ppt) = number;
            return *ppt;
        }

        /// <summary>
        /// Uses unsafe schemes to convert a <see cref="System.Int16"/> to a <see cref="System.Int64"/>. 
        /// The conversion is only performed with less checks during runtime.
        /// </summary>
        /// <param name="number">The number to convert.</param>
        public static System.Int64 ToInt64(this System.Int16 number)
        {
            System.Int64* ppt = stackalloc System.Int64[1];
            *((System.Int16*)ppt) = number;
            return *ppt;
        }
        #endregion

        #region Conversions to UInt64
        /// <summary>
        /// Uses unsafe schemes to convert a <see cref="System.Int64"/> to a <see cref="System.UInt64"/>. 
        /// The conversion is only performed with less checks during runtime.
        /// </summary>
        /// <param name="number">The number to convert.</param>
        public static System.UInt64 ToUInt64(this System.Int64 number) => Unsafe.As<System.Int64, System.UInt64>(ref number);

        /// <summary>
        /// Uses unsafe schemes to convert a <see cref="System.UInt32"/> to a <see cref="System.UInt64"/>. 
        /// The conversion is only performed with less checks during runtime.
        /// </summary>
        /// <param name="number">The number to convert.</param>
        public static System.UInt64 ToUInt64(this System.UInt32 number)
        {
            System.UInt64* ppt = stackalloc System.UInt64[1];
            *((System.UInt32*)ppt) = number;
            return *ppt;
        }

        /// <summary>
        /// Uses unsafe schemes to convert a <see cref="System.Int32"/> to a <see cref="System.UInt64"/>. 
        /// The conversion is only performed with less checks during runtime.
        /// </summary>
        /// <param name="number">The number to convert.</param>
        public static System.UInt64 ToUInt64(this System.Int32 number)
        {
            System.UInt64* ppt = stackalloc System.UInt64[1];
            *((System.Int32*)ppt) = number;
            return *ppt;
        }

        /// <summary>
        /// Uses unsafe schemes to convert a <see cref="System.Int16"/> to a <see cref="System.UInt64"/>. 
        /// The conversion is only performed with less checks during runtime.
        /// </summary>
        /// <param name="number">The number to convert.</param>
        public static System.UInt64 ToUInt64(this System.Int16 number)
        {
            System.UInt64* ppt = stackalloc System.UInt64[1];
            *((System.Int16*)ppt) = number;
            return *ppt;
        }
        #endregion

        #region Conversions to Int32
        /// <summary>
        /// Uses unsafe schemes to convert a <see cref="System.Int16"/> to a <see cref="System.Int32"/>. 
        /// The conversion is only performed with less checks during runtime.
        /// </summary>
        /// <param name="number">The number to convert.</param>
        public static System.Int32 ToInt32(this System.Int16 number)
        {
            System.Int32* ppt = stackalloc System.Int32[1];
            *((System.Int16*)ppt) = number;
            return *ppt;
        }

        /// <summary>
        /// Uses unsafe schemes to convert a <see cref="System.Int64"/> to a <see cref="System.Int32"/>. 
        /// The conversion is only performed with less checks during runtime.
        /// </summary>
        /// <param name="number">The number to convert.</param>
        public static System.Int32 ToInt32(this System.Int64 number)
        {
            System.Int64* ppt = stackalloc System.Int64[1];
            *ppt = number;
            return *((System.Int32*)ppt);
        }

        /// <summary>
        /// Uses unsafe schemes to convert a <see cref="System.UInt32"/> to a <see cref="System.Int32"/>. 
        /// The conversion is only performed with less checks during runtime.
        /// </summary>
        /// <param name="number">The number to convert.</param>
        public static System.Int32 ToInt32(this System.UInt32 number) => Unsafe.As<System.UInt32, System.Int32>(ref number);

        /// <summary>
        /// Uses unsafe schemes to convert a <see cref="System.Char"/> to a <see cref="System.Int32"/>. 
        /// The conversion is only performed with less checks during runtime.
        /// </summary>
        /// <param name="character">The character to convert.</param>
        public static System.Int32 ToInt32(this System.Char character)
        {
            System.Int32* ppt = stackalloc System.Int32[1];
            *((System.Char*)ppt) = character;
            return *ppt;
        }

        /// <summary>
        /// Uses unsafe schemes to convert a <see cref="System.UInt64"/> to a <see cref="System.Int32"/>. 
        /// The conversion is only performed with less checks during runtime.
        /// </summary>
        /// <param name="number">The number to convert.</param>
        public static System.Int32 ToInt32(this System.UInt64 number)
        {
            System.UInt64* ppt = stackalloc System.UInt64[1];
            *ppt = number;
            return *((System.Int32*)ppt);
        }
        #endregion

        #region Conversions to UInt32
        /// <summary>
        /// Uses unsafe schemes to convert a <see cref="System.Int32"/> to a <see cref="System.UInt32"/>. 
        /// The conversion is only performed with less checks during runtime.
        /// </summary>
        /// <param name="number">The number to convert.</param>
        public static System.UInt32 ToUInt32(this System.Int32 number) => Unsafe.As<System.Int32, System.UInt32>(ref number);

        /// <summary>
        /// Uses unsafe schemes to convert a <see cref="System.UInt64"/> to a <see cref="System.UInt32"/>. 
        /// The conversion is only performed with less checks during runtime.
        /// </summary>
        /// <param name="number">The number to convert.</param>
        public static System.UInt32 ToUInt32(this System.UInt64 number)
        {
            System.UInt64* ppt = stackalloc System.UInt64[1];
            *ppt = number;
            return *((System.UInt32*)ppt);
        }

        /// <summary>
        /// Uses unsafe schemes to convert a <see cref="System.Int64"/> to a <see cref="System.UInt32"/>. 
        /// The conversion is only performed with less checks during runtime.
        /// </summary>
        /// <param name="number">The number to convert.</param>
        public static System.UInt32 ToUInt32(this System.Int64 number)
        {
            System.Int64* ppt = stackalloc System.Int64[1];
            *ppt = number;
            return *((System.UInt32*)ppt);
        }

        /// <summary>
        /// Uses unsafe schemes to convert a <see cref="System.UInt16"/> to a <see cref="System.UInt32"/>. 
        /// The conversion is only performed with less checks during runtime.
        /// </summary>
        /// <param name="number">The number to convert.</param>
        public static System.UInt32 ToUInt32(this System.UInt16 number)
        {
            System.UInt32* ppt = stackalloc System.UInt32[1];
            *((System.UInt16*)ppt) = number;
            return *ppt;
        }

        /// <summary>
        /// Uses unsafe schemes to convert a <see cref="System.Int16"/> to a <see cref="System.UInt32"/>. 
        /// The conversion is only performed with less checks during runtime.
        /// </summary>
        /// <param name="number">The number to convert.</param>
        public static System.UInt32 ToUInt32(this System.Int16 number)
        {
            System.UInt32* ppt = stackalloc System.UInt32[1];
            *((System.Int16*)ppt) = number;
            return *ppt;
        }
        #endregion

        #region Conversions to Int16
        /// <summary>
        /// Uses unsafe schemes to convert a <see cref="System.Byte"/> to a <see cref="System.Int16"/>. 
        /// The conversion is only performed with less checks during runtime.
        /// </summary>
        /// <param name="number">The number to convert.</param>
        public static System.Int16 ToInt16(this System.Byte number)
        {
            System.Int16* ppt = stackalloc System.Int16[1];
            *((System.Byte*)ppt) = number;
            return *ppt;
        }

        /// <summary>
        /// Uses unsafe schemes to convert a <see cref="System.UInt16"/> to a <see cref="System.Int16"/>. 
        /// The conversion is only performed with less checks during runtime.
        /// </summary>
        /// <param name="number">The number to convert.</param>
        public static System.Int16 ToInt16(this System.UInt16 number)
        {
            System.Int16* ppt = stackalloc System.Int16[1];
            *((System.UInt16*)ppt) = number;
            return *ppt;
        }

        /// <summary>
        /// Uses unsafe schemes to convert a <see cref="System.Int64"/> to a <see cref="System.Int16"/>. 
        /// The conversion is only performed with less checks during runtime.
        /// </summary>
        /// <param name="number">The number to convert.</param>
        public static System.Int16 ToInt16(this System.Int64 number)
        {
            System.Int64* ppt = stackalloc System.Int64[1];
            *ppt = number;
            return *((System.Int16*)ppt);
        }

        /// <summary>
        /// Uses unsafe schemes to convert a <see cref="System.Int32"/> to a <see cref="System.Int16"/>. 
        /// The conversion is only performed with less checks during runtime.
        /// </summary>
        /// <param name="number">The number to convert.</param>
        public static System.Int16 ToInt16(this System.Int32 number)
        {
            System.Int32* ppt = stackalloc System.Int32[1];
            *ppt = number;
            return *((System.Int16*)ppt);
        }

        /// <summary>
        /// Uses unsafe schemes to convert a <see cref="System.UInt64"/> to a <see cref="System.Int16"/>. 
        /// The conversion is only performed with less checks during runtime.
        /// </summary>
        /// <param name="number">The number to convert.</param>
        public static System.Int16 ToInt16(this System.UInt64 number)
        {
            System.UInt64* ppt = stackalloc System.UInt64[1];
            *ppt = number;
            return *((System.Int16*)ppt);
        }

        /// <summary>
        /// Uses unsafe schemes to convert a <see cref="System.Char"/> to a <see cref="System.Int16"/>. 
        /// The conversion is only performed with less checks during runtime.
        /// </summary>
        /// <param name="character">The character to convert.</param>
        public static System.Int16 ToInt16(this System.Char character)
        {
            System.Int16* ppt = stackalloc System.Int16[1];
            *((System.Char*)ppt) = character;
            return *ppt;
        }
        #endregion

        #region Conversions to UInt16
        /// <summary>
        /// Uses unsafe schemes to convert a <see cref="System.Int16"/> to a <see cref="System.UInt16"/>. 
        /// The conversion is only performed with less checks during runtime.
        /// </summary>
        /// <param name="number">The number to convert.</param>
        public static System.UInt16 ToUInt16(this System.Int16 number)
        {
            System.UInt16* ppt = stackalloc System.UInt16[1];
            *((System.Int16*)ppt) = number;
            return *ppt;
        }

        /// <summary>
        /// Uses unsafe schemes to convert a <see cref="System.UInt32"/> to a <see cref="System.UInt16"/>. 
        /// The conversion is only performed with less checks during runtime.
        /// </summary>
        /// <param name="number">The number to convert.</param>
        public static System.UInt16 ToUInt16(this System.UInt32 number)
        {
            System.UInt32* ppt = stackalloc System.UInt32[1];
            *ppt = number;
            return *((System.UInt16*)ppt);
        }

        /// <summary>
        /// Uses unsafe schemes to convert a <see cref="System.Int64"/> to a <see cref="System.UInt16"/>. 
        /// The conversion is only performed with less checks during runtime.
        /// </summary>
        /// <param name="number">The number to convert.</param>
        public static System.UInt16 ToUInt16(this System.Int64 number)
        {
            System.Int64* ppt = stackalloc System.Int64[1];
            *ppt = number;
            return *((System.UInt16*)ppt);
        }

        /// <summary>
        /// Uses unsafe schemes to convert a <see cref="System.UInt64"/> to a <see cref="System.Int16"/>. 
        /// The conversion is only performed with less checks during runtime.
        /// </summary>
        /// <param name="number">The number to convert.</param>
        public static System.UInt16 ToUInt16(this System.UInt64 number)
        {
            System.UInt64* ppt = stackalloc System.UInt64[1];
            *ppt = number;
            return *((System.UInt16*)ppt);
        }

        /// <summary>
        /// Uses unsafe schemes to convert a <see cref="System.Int32"/> to a <see cref="System.UInt16"/>. 
        /// The conversion is only performed with less checks during runtime.
        /// </summary>
        /// <param name="number">The number to convert.</param>
        public static System.UInt16 ToUInt16(this System.Int32 number)
        {
            System.Int32* ppt = stackalloc System.Int32[1];
            *ppt = number;
            return *((System.UInt16*)ppt);
        }
        #endregion

        #region Conversions to Byte
        /// <summary>
        /// Uses unsafe schemes to convert a <see cref="System.Int16"/> to a <see cref="System.Byte"/>. 
        /// The conversion is only performed with less checks during runtime.
        /// </summary>
        /// <param name="number">The number to convert.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.Byte ToByte(this System.Int16 number) => unchecked((System.Byte)(number & 255));

        /// <summary>
        /// Uses unsafe schemes to convert a <see cref="System.Int32"/> to a <see cref="System.Byte"/>. 
        /// The conversion is only performed with less checks during runtime.
        /// </summary>
        /// <param name="number">The number to convert.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.Byte ToByte(this System.Int32 number) => unchecked((System.Byte)(number & 255));

        /// <summary>
        /// Uses unsafe schemes to convert a <see cref="System.Int64"/> to a <see cref="System.Byte"/>. 
        /// The conversion is only performed with less checks during runtime.
        /// </summary>
        /// <param name="number">The number to convert.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.Byte ToByte(this System.Int64 number) => unchecked((System.Byte)(number & 255));

        /// <summary>
        /// Uses unsafe schemes to convert a <see cref="System.Char"/> to a <see cref="System.Byte"/>. 
        /// The conversion is only performed with less checks during runtime.
        /// </summary>
        /// <param name="character">The character to convert.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.Byte ToByte(this System.Char character) => unchecked((System.Byte)(character & 255));

        /// <summary>
        /// Uses unsafe schemes to convert a <see cref="System.UInt16"/> to a <see cref="System.Byte"/>. 
        /// The conversion is only performed with less checks during runtime.
        /// </summary>
        /// <param name="number">The number to convert.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.Byte ToByte(this System.UInt16 number) => unchecked((System.Byte)(number & 255));

        /// <summary>
        /// Uses unsafe schemes to convert a <see cref="System.UInt32"/> to a <see cref="System.Byte"/>. 
        /// The conversion is only performed with less checks during runtime.
        /// </summary>
        /// <param name="number">The number to convert.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.Byte ToByte(this System.UInt32 number) => unchecked((System.Byte)(number & 255));

        /// <summary>
        /// Uses unsafe schemes to convert a <see cref="System.UInt64"/> to a <see cref="System.Byte"/>. 
        /// The conversion is only performed with less checks during runtime.
        /// </summary>
        /// <param name="number">The number to convert.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.Byte ToByte(this System.UInt64 number) => unchecked((System.Byte)(number & 255));

        /// <summary>
        /// Uses unsafe schemes to convert a <see cref="System.SByte"/> to a <see cref="System.Byte"/>. 
        /// The conversion is only performed with less checks during runtime.
        /// </summary>
        /// <param name="number">The number to convert.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.Byte ToByte(this System.SByte number) => unchecked((System.Byte)number);
        #endregion

        #region Conversions to Char
        /// <summary>
        /// Uses unsafe schemes to convert a <see cref="System.Int64"/> to a <see cref="System.Char"/>. 
        /// The conversion is only performed with less checks during runtime.
        /// </summary>
        /// <param name="number">The number to convert.</param>
        public static System.Char ToChar(this System.Int64 number)
        {
            System.Int64* ppt = stackalloc System.Int64[1];
            *ppt = number;
            return *((System.Char*)ppt);
        }

        /// <summary>
        /// Uses unsafe schemes to convert a <see cref="System.Int32"/> to a <see cref="System.Char"/>. 
        /// The conversion is only performed with less checks during runtime.
        /// </summary>
        /// <param name="number">The number to convert.</param>
        public static System.Char ToChar(this System.Int32 number)
        {
            System.Int32* ppt = stackalloc System.Int32[1];
            *ppt = number;
            return *((System.Char*)ppt);
        }

        /// <summary>
        /// Uses unsafe schemes to convert a <see cref="System.UInt32"/> to a <see cref="System.Char"/>. 
        /// The conversion is only performed with less checks during runtime.
        /// </summary>
        /// <param name="number">The number to convert.</param>
        public static System.Char ToChar(this System.UInt32 number)
        {
            System.UInt32* ppt = stackalloc System.UInt32[1];
            *ppt = number;
            return *((System.Char*)ppt);
        }

        /// <summary>
        /// Uses unsafe schemes to convert a <see cref="System.Int16"/> to a <see cref="System.Char"/>. 
        /// The conversion is only performed with less checks during runtime.
        /// </summary>
        /// <param name="number">The number to convert.</param>
        public static System.Char ToChar(this System.Int16 number) => Unsafe.As<System.Int16, System.Char>(ref number);

        /// <summary>
        /// Uses unsafe schemes to convert a <see cref="System.UInt16"/> to a <see cref="System.Char"/>. 
        /// The conversion is only performed with less checks during runtime.
        /// </summary>
        /// <param name="number">The number to convert.</param>
        public static System.Char ToChar(this System.UInt16 number) => Unsafe.As<System.UInt16, System.Char>(ref number);

        /// <summary>
        /// Uses unsafe schemes to convert a <see cref="System.Byte"/> to a <see cref="System.Char"/>. 
        /// The conversion is only performed with less checks during runtime.
        /// </summary>
        /// <param name="number">The number to convert.</param>
        public static System.Char ToChar(this System.Byte number)
        {
            System.Char* ppt = stackalloc System.Char[1];
            *((System.Byte*)ppt) = number;
            return *ppt;
        }
        #endregion

        #region Conversions to SByte
        /// <summary>
        /// Uses unsafe schemes to convert a <see cref="System.Byte"/> to a <see cref="System.SByte"/>. 
        /// The conversion is only performed with less checks during runtime.
        /// </summary>
        /// <param name="number">The number to convert.</param>
        public static System.SByte ToSByte(this System.Byte number)
        {
            System.Byte* ppt = stackalloc System.Byte[1];
            *ppt = number;
            return *((System.SByte*)ppt);
        }

        /// <summary>
        /// Uses unsafe schemes to convert a <see cref="System.Int32"/> to a <see cref="System.SByte"/>. 
        /// The conversion is only performed with less checks during runtime.
        /// </summary>
        /// <param name="number">The number to convert.</param>
        public static System.SByte ToSByte(this System.Int32 number)
        {
            System.Int32* ppt = stackalloc System.Int32[1];
            *ppt = number;
            return *((System.SByte*)ppt);
        }
        #endregion

        #region Get Bytes from numeric types
        /// <summary>
        /// Returns the equivalent byte array representation of this number.
        /// </summary>
        /// <param name="number">The number to convert.</param>
        /// <returns>The equivalent array representation of <paramref name="number"/>.</returns>
        public static System.Byte[] GetBytes(this System.Int64 number) => GetBytesTemplate(number);

        /// <summary>
        /// Returns the equivalent byte array representation of this number.
        /// </summary>
        /// <param name="number">The number to convert.</param>
        /// <returns>The equivalent array representation of <paramref name="number"/>.</returns>
        public static System.Byte[] GetBytes(this System.UInt64 number) => GetBytesTemplate(number);

        /// <summary>
        /// Returns the equivalent byte array representation of this number.
        /// </summary>
        /// <param name="number">The number to convert.</param>
        /// <returns>The equivalent array representation of <paramref name="number"/>.</returns>
        public static System.Byte[] GetBytes(this System.UInt32 number) => GetBytesTemplate(number);

        /// <summary>
        /// Returns the equivalent byte array representation of this number.
        /// </summary>
        /// <param name="number">The number to convert.</param>
        /// <returns>The equivalent array representation of <paramref name="number"/>.</returns>
        public static System.Byte[] GetBytes(this System.UInt16 number) => GetBytesTemplate(number);

        /// <summary>
        /// Returns the equivalent byte array representation of this number.
        /// </summary>
        /// <param name="number">The number to convert.</param>
        /// <returns>The equivalent array representation of <paramref name="number"/>.</returns>
        public static System.Byte[] GetBytes(this System.Int16 number) => GetBytesTemplate(number);

        /// <summary>
        /// Returns the equivalent byte array representation of this number.
        /// </summary>
        /// <param name="number">The number to convert.</param>
        /// <returns>The equivalent array representation of <paramref name="number"/>.</returns>
        public static System.Byte[] GetBytes(this System.Int32 number) => GetBytesTemplate(number);

        /// <summary>
        /// Returns the equivalent byte array representation of this number.
        /// </summary>
        /// <param name="number">The number to convert.</param>
        /// <returns>The equivalent array representation of <paramref name="number"/>.</returns>
        public static System.Byte[] GetBytes(this System.Single number) => GetBytesTemplate(
                LinearConversion<System.Single, System.Int32>(number)
        );

        /// <summary>
        /// Returns the equivalent byte array representation of this number.
        /// </summary>
        /// <param name="number">The number to convert.</param>
        /// <returns>The equivalent array representation of <paramref name="number"/>.</returns>
        public static System.Byte[] GetBytes(this System.Double number) => GetBytesTemplate(
                LinearConversion<System.Double, System.Int64>(number)
        );

        /// <summary>
        /// Returns the equivalent byte array representation of this number.
        /// </summary>
        /// <param name="number">The number to convert.</param>
        /// <returns>The equivalent array representation of <paramref name="number"/>.</returns>
        public static System.Byte[] GetBytes(this System.Decimal number) => GetBytesTemplate(number);

        /// <summary>
        /// Returns the equivalent byte array representation of this Unicode character.
        /// </summary>
        /// <param name="ch">The number to convert.</param>
        /// <returns>The equivalent array representation of <paramref name="ch"/>.</returns>
        public static System.Byte[] GetBytes(this System.Char ch) => GetBytesTemplate(ch);
        #endregion

        #region Convert to numeric types from bytes
        /// <summary>
        /// Gets a <see cref="System.Single"/> from a byte array returned from <c>GetBytes</c> method.
        /// </summary>
        /// <param name="array">The array that contains the information to create a <see cref="System.Single"/>.</param>
        /// <param name="StartIndex">The zero-based index position to start reading from.</param>
        /// <returns>The read number.</returns>
        public static System.Single ToSingle(this System.Byte[] array , System.Int32 StartIndex)
        {
            System.Int32 rn = GetFromBytesTemplate<System.Int32>(array , StartIndex);
            return LinearConversion<System.Int32 , System.Single>(rn);
        }

        /// <summary>
        /// Gets a <see cref="System.Double"/> from a byte array returned from <c>GetBytes</c> method.
        /// </summary>
        /// <param name="array">The array that contains the information to create a <see cref="System.Double"/>.</param>
        /// <param name="StartIndex">The zero-based index position to start reading from.</param>
        /// <returns>The read number.</returns>
        public static System.Double ToDouble(this System.Byte[] array , System.Int32 StartIndex)
        {
            System.Int64 rn = GetFromBytesTemplate<System.Int64>(array , StartIndex);
            return LinearConversion<System.Int64 , System.Double>(rn);
        }

        /// <summary>
        /// Gets a <see cref="System.Decimal"/> from a byte array returned from <c>GetBytes</c> method.
        /// </summary>
        /// <param name="array">The array that contains the information to create a <see cref="System.Decimal"/>.</param>
        /// <param name="StartIndex">The zero-based index position to start reading from.</param>
        /// <returns>The read number.</returns>
        public static System.Decimal ToDecimal(this System.Byte[] array, System.Int32 StartIndex)
        {
            if (StartIndex < 0 || (StartIndex + sizeof(System.Decimal)) > array.Length) {
                throw new ArgumentOutOfRangeException(nameof(StartIndex), "StartIndex must be more or equal to zero and smaller than the array length plus the size of the structure.");
            } else {
                return Unsafe.ReadUnaligned<System.Decimal>(ref array[StartIndex]);
            }
        }

        /// <summary>
        /// Gets a <see cref="System.Int16"/> from a byte array returned from <c>GetBytes</c> method.
        /// </summary>
        /// <param name="array">The array that contains the information to create a <see cref="System.Int16"/>.</param>
        /// <param name="StartIndex">The zero-based index position to start reading from.</param>
        /// <returns>The read number.</returns>
        public static System.Int16 ToInt16(this System.Byte[] array, System.Int32 StartIndex)
        {
            if (StartIndex < 0 || (StartIndex + sizeof(System.Int16)) > array.Length) {
                throw new ArgumentOutOfRangeException(nameof(StartIndex), "StartIndex must be more or equal to zero and smaller than the array length plus the size of the structure.");
            } else {
                return Unsafe.ReadUnaligned<System.Int16>(ref array[StartIndex]);
            }
        }

        /// <summary>
        /// Gets a <see cref="System.Int32"/> from a byte array returned from <c>GetBytes</c> method.
        /// </summary>
        /// <param name="array">The array that contains the information to create a <see cref="System.Int32"/>.</param>
        /// <param name="StartIndex">The zero-based index position to start reading from.</param>
        /// <returns>The read number.</returns>
        public static System.Int32 ToInt32(this System.Byte[] array, System.Int32 StartIndex)
        {
            if (StartIndex < 0 || (StartIndex + sizeof(System.Int32)) > array.Length) {
                throw new ArgumentOutOfRangeException(nameof(StartIndex), "StartIndex must be more or equal to zero and smaller than the array length plus the size of the structure.");
            } else {
                return Unsafe.ReadUnaligned<System.Int32>(ref array[StartIndex]);
            }
        }

        /// <summary>
        /// Gets a <see cref="System.Int64"/> from a byte array returned from <c>GetBytes</c> method.
        /// </summary>
        /// <param name="array">The array that contains the information to create a <see cref="System.Int64"/>.</param>
        /// <param name="StartIndex">The zero-based index position to start reading from.</param>
        /// <returns>The read number.</returns>
        public static System.Int64 ToInt64(this System.Byte[] array, System.Int32 StartIndex)
        {
            if (StartIndex < 0 || (StartIndex + sizeof(System.Int64)) > array.Length) {
                throw new ArgumentOutOfRangeException(nameof(StartIndex), "StartIndex must be more or equal to zero and smaller than the array length plus the size of the structure.");
            } else {
                return Unsafe.ReadUnaligned<System.Int64>(ref array[StartIndex]);
            }
        }

        /// <summary>
        /// Gets a <see cref="System.UInt64"/> from a byte array returned from <c>GetBytes</c> method.
        /// </summary>
        /// <param name="array">The array that contains the information to create a <see cref="System.UInt64"/>.</param>
        /// <param name="StartIndex">The zero-based index position to start reading from.</param>
        /// <returns>The read number.</returns>
        public static System.UInt64 ToUInt64(this System.Byte[] array, System.Int32 StartIndex)
        {
            if (StartIndex < 0 || (StartIndex + sizeof(System.UInt64)) > array.Length) {
                throw new ArgumentOutOfRangeException(nameof(StartIndex), "StartIndex must be more or equal to zero and smaller than the array length plus the size of the structure.");
            } else {
                return Unsafe.ReadUnaligned<System.UInt64>(ref array[StartIndex]);
            }
        }

        /// <summary>
        /// Gets a <see cref="System.UInt32"/> from a byte array returned from <c>GetBytes</c> method.
        /// </summary>
        /// <param name="array">The array that contains the information to create a <see cref="System.UInt32"/>.</param>
        /// <param name="StartIndex">The zero-based index position to start reading from.</param>
        /// <returns>The read number.</returns>
        public static System.UInt32 ToUInt32(this System.Byte[] array, System.Int32 StartIndex)
        {
            if (StartIndex < 0 || (StartIndex + sizeof(System.UInt32)) > array.Length) {
                throw new ArgumentOutOfRangeException(nameof(StartIndex), "StartIndex must be more or equal to zero and smaller than the array length plus the size of the structure.");
            } else {
                return Unsafe.ReadUnaligned<System.UInt32>(ref array[StartIndex]);
            }
        }

        /// <summary>
        /// Gets a <see cref="System.UInt16"/> from a byte array returned from <c>GetBytes</c> method.
        /// </summary>
        /// <param name="array">The array that contains the information to create a <see cref="System.UInt16"/>.</param>
        /// <param name="StartIndex">The zero-based index position to start reading from.</param>
        /// <returns>The read number.</returns>
        public static System.UInt16 ToUInt16(this System.Byte[] array, System.Int32 StartIndex)
        {
            if (StartIndex < 0 || (StartIndex + sizeof(System.UInt16)) > array.Length) {
                throw new ArgumentOutOfRangeException(nameof(StartIndex), "StartIndex must be more or equal to zero and smaller than the array length plus the size of the structure.");
            } else {
                return Unsafe.ReadUnaligned<System.UInt16>(ref array[StartIndex]);
            }
        }

        /// <summary>
        /// Gets a <see cref="System.Char"/> from a byte array returned from <c>GetBytes</c> method.
        /// </summary>
        /// <param name="array">The array that contains the information to create a <see cref="System.UInt16"/>.</param>
        /// <param name="StartIndex">The zero-based index position to start reading from.</param>
        /// <returns>The read number.</returns>
        public static System.Char ToChar(this System.Byte[] array , System.Int32 StartIndex)
        {
            if (StartIndex < 0 || (StartIndex + sizeof(System.Char)) > array.Length) {
                throw new ArgumentOutOfRangeException(nameof(StartIndex), "StartIndex must be more or equal to zero and smaller than the array length plus the size of the structure.");
            } else {
                return Unsafe.ReadUnaligned<System.Char>(ref array[StartIndex]);
            }
        }

        #endregion

        #region Floating-point precision conversions
        /// <summary>
        /// Converts the double-precision floating <paramref name="number"/> given to it's equivalent 64 bits , stored in a <see cref="System.Int64"/>. 
        /// </summary>
        /// <param name="number">The double-precision floating <paramref name="number"/> to convert.</param>
        /// <returns>The equivalent 64 bits returned as a <see cref="System.Int64"/>.</returns>
        public static System.Int64 ToInt64Bits(this System.Double number) => LinearConversion<System.Double, System.Int64>(number);

        /// <summary>
        /// Converts the single-precision floating <paramref name="number"/> given to it's equivalent 32 bits , stored in a <see cref="System.Int32"/>. 
        /// </summary>
        /// <param name="number">The single-precision floating <paramref name="number"/> to convert.</param>
        /// <returns>The equivalent 32 bits returned as a <see cref="System.Int32"/>.</returns>
        public static System.Int32 ToInt32Bits(this System.Single number) => LinearConversion<System.Single, System.Int32>(number);

        /// <summary>
        /// Converts the given <paramref name="number"/> bits back to it's equivalent single-precision floating point value.
        /// </summary>
        /// <param name="number">The converted number bits acquired from <see cref="ToInt32Bits(float)"/>.</param>
        /// <returns>The original single-precision floating point value.</returns>
        public static System.Single FromInt32Bits(this System.Int32 number) => LinearConversion<System.Int32 , System.Single>(number);

        /// <summary>
        /// Converts the given <paramref name="number"/> bits back to it's equivalent double-precision floating-point value.
        /// </summary>
        /// <param name="number">The converted number bits acquired from <see cref="ToInt32Bits(float)"/>.</param>
        /// <returns>The original double-precision floating-point value.</returns>
        public static System.Double FromInt64Bits(this System.Int64 number) => LinearConversion<System.Int64, System.Double>(number);
        #endregion

        #region Bit Manipulations
        // Parts of bit conversion code do belong from referencesource.microsoft.com/en-us !.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static System.Int32 GetBitValue(System.Int32 bitidx) => 1 << (bitidx & 7);

        /// <summary>
        /// Gets the bit on the specified index inside the byte. <br />
        /// The index is a zero-based number up to 7.
        /// </summary>
        /// <param name="bt">The byte to retrieve the bit from.</param>
        /// <param name="bitindex">The bit index to retrieve.</param>
        /// <returns>The bit value. <see langword="true"/> means that the bit is set.</returns>
        public static System.Boolean GetBit(this System.Byte bt , System.Int32 bitindex)
        {
            System.Int32 bitval = GetBitValue(bitindex);
            return (bt & bitval) == bitval;
        }

        /// <summary>
        /// Sets the bit at the specified index of the specified byte. The byte given must be a reference to it
        /// so that the operation can succeed.
        /// </summary>
        /// <param name="bt">The byte to set the bit to.</param>
        /// <param name="bitindex">The bit index inside the byte to set.</param>
        /// <param name="value">The value to set on the specified bit (<see langword="true"/> means 1 , and <see langword="false"/> means 0).</param>
        [System.Security.SecuritySafeCritical] // This operation is undefined if the pointer is invalid
        public static void SetBit(this ref System.Byte bt , System.Int32 bitindex , System.Boolean value)
        {
            // GetBitValue will be used in both cases so why not computing it before the code paths do split out?
            System.Int32 bv = GetBitValue(bitindex);
            bt = (value ? (bt | bv) : (bt & ~bv)).ToByte();
        }

        /// <summary>
        /// Returns the byte as a fully recreatable and representable bit array of <see cref="System.Boolean"/>s.
        /// </summary>
        /// <param name="bt">The byte to convert.</param>
        /// <returns>It's binary representation.</returns>
        public static System.Boolean[] ToBinary(this System.Byte bt)
        {
            // Create a Boolean array that it's number of elements are equal to the bitsize size of a byte.
            System.Boolean[] result = new System.Boolean[8];
            for (System.Int32 I = 0; I < 8; I++) { result[I] = GetBit(bt , I); }
            return result;
        }

        /// <summary>
        /// Converts the given number to a binary string in the .NET Core format.
        /// </summary>
        /// <typeparam name="T">The number type to convert.</typeparam>
        /// <param name="number">The number to convert.</param>
        /// <returns>The converted binary string that is equal to <paramref name="number"/>.</returns>
        public static System.String ToBinaryString<T>(this T number) where T : unmanaged
        {
            System.Text.StringBuilder sb = new(1024);
            System.Byte[] data = GetBytesTemplate(number);
            for (System.Int32 I = 0; I < data.Length; I++)
            {
                var b = data[I];
                for (System.Int32 J = 0; J < 8; J++)
                {
                    sb.Append(b.GetBit(J) ? '1' : '0');
                }
                if (I+1 < data.Length) { sb.Append('_'); }
            }
            return sb.ToString();
        }
        
        /// <summary>
        /// Reads a number in bits. The read number is converted to an Int32.
        /// </summary>
        /// <param name="data">The byte array to read the number from.</param>
        /// <param name="startindex">The starting index inside the array to start reading from.</param>
        /// <param name="bits">The number of bits to write.</param>
        /// <returns>The read bit-field number.</returns>
        /// <exception cref="ArgumentException">The range of bits was not in 0..32.</exception>
        public static System.Int32 ReadBitLevelNumber(this System.Byte[] data , System.Int32 startindex , System.Int32 bits)
        {
            System.Byte[] resultbytes = new System.Byte[sizeof(System.Int32)];
            if (bits > 32 || bits < 0) { throw new ArgumentException("Bits must be from 0..32 range."); }
            System.Int32 bi = 0 , byteidx , bitordinal;
            while (bi < bits)
            {
                byteidx = bi >> 3;
                bitordinal = bi % 8;
                if (data[byteidx + startindex].GetBit(bitordinal)) {
                    resultbytes[byteidx].SetBit(bitordinal , true);
                }
                bi++;
            }
            return resultbytes.ToInt32(0);
        }

        /// <summary>
        /// Decomposes the current byte by keeping only the bits specified in <paramref name="bits"/> parameter and starting by the bit index denoted by the <paramref name="index"/> parameter.
        /// </summary>
        /// <param name="bt">The byte to decompose.</param>
        /// <param name="index">The index to start reading bits from.</param>
        /// <param name="bits">The number of bits that the returned byte will have from the current one.</param>
        /// <returns>The decomposed byte.</returns>
        /// <exception cref="ArgumentException"><paramref name="bits"/> was not in the range 0..8 and/or <paramref name="index"/> was not in the range 0..7.</exception>
        public static System.Byte ReadBitLevelByte(this System.Byte bt , System.Byte index , System.Byte bits)
        {
            if (bits > 8 || bits < 0) { throw new ArgumentException("Bits must be from 0..8 range."); }
            if (index < 0 || index > 7) { throw new ArgumentException("Bits index must be from 0..7 range."); }
            System.Byte ret = 0;
            System.Int32 bit = index , cord = 0;
            while (cord < bits)
            {
                if (bt.GetBit(bit)) {
                    ret.SetBit(cord, true);
                }
                bit++;
                cord++;
            }
            return ret;
        }

        /// <summary>
        /// Decomposes the current byte by keeping only the bits specified in <paramref name="bits"/> parameter.
        /// </summary>
        /// <param name="bt">The byte to decompose.</param>
        /// <param name="bits">The number of bits that the returned byte will have from the current one.</param>
        /// <returns>The decomposed byte.</returns>
        /// <exception cref="ArgumentException"><paramref name="bits"/> was not in the range 0..8.</exception>
        public static System.Byte ReadBitLevelByte(this System.Byte bt , System.Byte bits) => ReadBitLevelByte(bt , 0 , bits);
        #endregion

        /// <summary>
        /// Data alignment helper. Used by many classes that need to pad data in the streams.
        /// </summary>
        /// <param name="align">The alignment, in bytes, to apply in the data. Must be multiples of 2</param>
        /// <param name="length">The length, in bytes, of the data where the alignment will be applied to.</param>
        /// <returns>The number of bytes to add in order to achieve the alignment specified by <paramref name="align"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="align"/> was not a number fully divisible by 2.</exception>
        public static System.Int32 PadAlignment(System.Int32 align, System.Int32 length)
        {
            if ((align & 1) == 1) { 
                throw new ArgumentOutOfRangeException(nameof(align), "The align parameter must always be multiples of 2."); 
            } else {
                System.Int32 misalignment = length & (align - 1);
                return (misalignment == 0) ? 0 : align - misalignment;
            }
        }

        /// <summary>
        /// Data alignment helper. Used by many classes that need to pad data in the streams.
        /// </summary>
        /// <param name="align">The alignment, in bytes, to apply in the data. Must be multiples of 2</param>
        /// <param name="length">The length, in bytes, of the data where the alignment will be applied to.</param>
        /// <returns>The number of bytes to add in order to achieve the alignment specified by <paramref name="align"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="align"/> was not a number fully divisible by 2.</exception>
        public static System.Int64 PadAlignment(System.Int64 align, System.Int64 length)
        {
            if ((align & 1L) == 1L) { 
                throw new ArgumentOutOfRangeException(nameof(align), "The align parameter must always be multiples of 2."); 
            } else {
                System.Int64 misalignment = length & (align - 1L);
                return (misalignment == 0L) ? 0L : align - misalignment;
            }
        }

        /// <summary>
        /// Data alignment helper. Used by many classes that need to pad data in the streams.
        /// </summary>
        /// <param name="align">The alignment, in bytes, to apply in the data. Must be multiples of 2</param>
        /// <param name="length">The length, in bytes, of the data where the alignment will be applied to.</param>
        /// <returns>The number of bytes to add in order to achieve the alignment specified by <paramref name="align"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="align"/> was not a number fully divisible by 2.</exception>
        public static System.UInt64 PadAlignment(System.UInt64 align, System.UInt64 length)
        {
            if ((align & 1UL) == 1UL) {
                throw new ArgumentOutOfRangeException(nameof(align), "The align parameter must always be multiples of 2.");
            } else {
                System.UInt64 misalignment = length & (align - 1UL);
                return (misalignment == 0UL) ? 0UL : align - misalignment;
            }
        }
    }
}
