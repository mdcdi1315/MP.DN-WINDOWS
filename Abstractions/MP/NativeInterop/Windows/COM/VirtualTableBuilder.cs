

using System;
using MP.Collections;
using MP.Annotations.CodeAnalysis;

namespace MP.NativeInterop.Windows.COM
{
    /// <summary>
    /// Provides a storage class for storing COM interface virtual tables.
    /// </summary>
    public sealed class VirtualTableBuilder
    {
        private bool locked;
        private readonly SingleLinkedList<IntPtr> function_pointers;

        /// <summary>
        /// Creates a default and new instance of the <see cref="VirtualTableBuilder"/> class.
        /// </summary>
        public VirtualTableBuilder()
        {
            function_pointers = new();
            locked = false;
        }

        /// <summary>
        /// Adds a function pointer to the interface's virtual table. <br />
        /// The pointer is appended to the end of the virtual table.
        /// </summary>
        /// <param name="func_pointer">The function pointer to add.</param>
        /// <exception cref="InvalidOperationException">No more functions can be registered to this instance.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="func_pointer"/> is <see langword="null"/>.</exception>
        [Throws(typeof(ArgumentNullException), typeof(InvalidOperationException))]
        public unsafe void AddFunction(void* func_pointer)
        {
            if (locked) { throw new InvalidOperationException("Cannot add more function pointers once this object becomes locked."); }
            ArgumentNullException.ThrowIfNull(func_pointer);
            function_pointers.Add(new(func_pointer));
        }

        /// <summary>
        /// Gets the number of registered function pointers in the current virtual table builder.
        /// </summary>
        public int Count => function_pointers.Count;

        /// <summary>
        /// Gets the number of bytes required to store all the function pointers in a virtual table memory.
        /// </summary>
        public unsafe int NativeSize => function_pointers.Count * sizeof(void*);

        /// <summary>
        /// Locks this instance, and modifications cannot be performed once this method completes execution.
        /// </summary>
        public void Lock() => locked = true;

        /// <summary>
        /// Fills the given unmanaged memory pointer the virtual table contained by this instance.
        /// </summary>
        /// <param name="memory">The unmanaged memory pointer.</param>
        /// <exception cref="ArgumentNullException"><paramref name="memory"/> is <see langword="null"/>.</exception>
        [Throws(typeof(ArgumentNullException))]
        public unsafe void FillVirtualTable(void* memory)
        {
            ArgumentNullException.ThrowIfNull(memory);
            void** p_virt_table = (void**)memory;
            foreach (IntPtr fp in function_pointers) {
                *p_virt_table = fp.ToPointer();
                p_virt_table++;
            }
        }

        /// <summary>
        /// Fills the given memory handle the virtual table contained by this instance.
        /// </summary>
        /// <param name="handle">The memory handle to use.</param>
        /// <exception cref="ArgumentNullException"><paramref name="handle"/> is <see langword="null"/>.</exception>
        [Throws(typeof(ArgumentNullException))]
        public unsafe void FillVirtualTable(IMemoryHandle handle)
        {
            ArgumentNullException.ThrowIfNull(handle);
            FillVirtualTable(handle.MemoryPointer);
        }
    }
}