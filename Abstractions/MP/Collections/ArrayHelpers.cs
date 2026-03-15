
using System;
using System.Collections.Generic;
using MP.Annotations.CodeAnalysis;
using System.Diagnostics.CodeAnalysis;

namespace MP.Collections
{
    /// <summary>
    /// Contains some helper methods around arrays.
    /// </summary>
    public static class ArrayHelpers
    {
        /// <summary>
        /// Finds the specified index of the specified item. The specified <paramref name="comparer"/> is used for comparing the elements.
        /// </summary>
        /// <typeparam name="T">The type of the elements to be compared.</typeparam>
        /// <param name="array">The array to be enumerated.</param>
        /// <param name="start_index">The index inside <paramref name="array"/> to start enumerating elements.</param>
        /// <param name="count">The number of elements to be iterated in <paramref name="array"/>.</param>
        /// <param name="item">The item to be found in <paramref name="array"/>.</param>
        /// <param name="comparer">The comparer that is used to compare elements. Can be <see langword="null"/>.</param>
        /// <returns>An index of the specified item in the specified array. If the specified item is not found, -1 is returned.</returns>
        [Throws(typeof(ArgumentNullException), typeof(ArgumentOutOfRangeException))]
        public static long FindIndex<T>(T[] array, long start_index, long count, [AllowNull] T item , [AllowNull] IEqualityComparer<T> comparer)
        {
            ArgumentNullException.ThrowIfNull(array);

            // Do not generate the default equality comparer for type T if we have not validated everything 
            
            if ((uint)start_index > (uint)array.Length) {
                throw new ArgumentOutOfRangeException(nameof(start_index), "Index must be less than or equal to the array's length.");
            }

            if ((uint)count > (uint)(array.Length - start_index)) {
                throw new ArgumentOutOfRangeException(nameof(count), "Specified count exceeded the array's bounds.");    
            }

            comparer ??= EqualityComparer<T>.Default;

            long bound = start_index + count;
            for (long I = start_index; I < bound; I++)
            {
                if (comparer.Equals(array[I] , item)) { return I; }
            }
            return -1L;
        }

        /// <summary>
        /// Finds the specified index of the specified item. The specified <paramref name="comparer"/> is used for comparing the elements.
        /// </summary>
        /// <typeparam name="T">The type of the elements to be compared.</typeparam>
        /// <param name="array">The array to be enumerated.</param>
        /// <param name="start_index">The index inside <paramref name="array"/> to start enumerating elements.</param>
        /// <param name="count">The number of elements to be iterated in <paramref name="array"/>.</param>
        /// <param name="item">The item to be found in <paramref name="array"/>.</param>
        /// <param name="comparer">The comparer that is used to compare elements. Can be <see langword="null"/>.</param>
        /// <returns>An index of the specified item in the specified array. If the specified item is not found, -1 is returned.</returns>
        [Throws(typeof(ArgumentNullException), typeof(ArgumentOutOfRangeException))]
        public static int FindIndex<T>(T[] array, int start_index, int count, [AllowNull] T item , [AllowNull] IEqualityComparer<T> comparer)
        {
            ArgumentNullException.ThrowIfNull(array);

            // Do not generate the default equality comparer for type T if we have not validated everything 
            
            if ((uint)start_index > (uint)array.Length) {
                throw new ArgumentOutOfRangeException(nameof(start_index), "Index must be less than or equal to the array's length.");
            }

            if ((uint)count > (uint)(array.Length - start_index)) {
                throw new ArgumentOutOfRangeException(nameof(count), "Specified count exceeded the array's bounds.");    
            }

            comparer ??= EqualityComparer<T>.Default; 

            int bound = start_index + count;
            for (int I = start_index; I < bound; I++)
            {
                if (comparer.Equals(array[I] , item)) { return I; }
            }
            return -1;
        }

        /// <summary>
        /// Changes the number of elements in a one-dimensional array to the specified value.
        /// </summary>
        /// <typeparam name="T">The type of the elements that the array retains.</typeparam>
        /// <param name="array">The array to be resized.</param>
        /// <param name="size">The number of elements that <paramref name="array"/> will contain on return.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="size"/> is negative.</exception>
        [Throws(typeof(ArgumentOutOfRangeException))]
        public static void Resize<T>([AllowNull] ref T[] array, long size) 
            where T : notnull
        {
            if (size < 0L) {
                throw new ArgumentOutOfRangeException(nameof(size), "Array size cannot be a negative value.");
            } else {
                T[] local = array; // Local copy of array
                if (local is null) {
                    array = new T[size];
                } else if (local.LongLength != size) {
                    T[] temp = new T[size];
                    Array.Copy(local, 0, temp, 0, local.LongLength);
                    array = temp;
                }
            }
        }

        /*
            // This is not implemented yet. The below is valid but it will be used for small arrays in size.
            public static bool FindAll<T>(T[] array, long start_index, long count, T[] required , long required_array_start_index, long required_array_count, [AllowNull] IEqualityComparer<T> comparer)
            {
                ArgumentNullException.ThrowIfNull(array);

                comparer ??= EqualityComparer<T>.Default;

                long req_bound = required_array_start_index + required_array_count, bound = start_index + count , found = 0L;
                for (long I = start_index; I < bound && found != required_array_count; I++)
                {
                    for (long G = required_array_start_index; G < req_bound; G++) {
                        if (comparer.Equals(array[I], required[G])) { found++; break; }
                    }
                }

                return found == required_array_count;
            
                return false;
            }
        */
    }
}