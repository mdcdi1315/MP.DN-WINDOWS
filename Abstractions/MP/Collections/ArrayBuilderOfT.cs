
using MP.Annotations.CodeAnalysis;
using System.Diagnostics.CodeAnalysis;

namespace MP.Collections
{
    /// <summary>
    /// Provides a builder for creating arrays quickly. <br />
    /// Implements the <see cref="ICollectionObjectBuilder{T, TElement}"/> interface. <br />
    /// Each add operation has a complexity of O(1), and building the array is a O(<see cref="Count"/>) operation.
    /// </summary>
    /// <typeparam name="T">The element type of the array.</typeparam>
    public sealed class ArrayBuilder<T> : ICollectionObjectBuilder<T[], T>
    {
        private sealed class Node
        {
            public readonly T Value;
            
            public Node Next;

            public Node(T value)
            {
                Value = value;
                Next = null;
            }
        }

        private long count;
        private Node root, current;

        /// <summary>
        /// Initializes a new and empty instance of the <see cref="ArrayBuilder{T}"/> class.
        /// </summary>
        [MustNotReportException]
        public ArrayBuilder()
        {
            root = null;
            count = 0L;
            current = null;
        }

        /// <summary>
        /// Gets the number of elements currently registered so far.
        /// </summary>
        public long Count => count;

        /// <summary>
        /// Adds an element to this <see cref="ArrayBuilder{T}"/> instance.
        /// </summary>
        /// <param name="element">The element to be added.</param>
        public void Add([AllowNull] T element)
        {
            if (root is null) {
                root = current = new Node(element);
            } else {
                Node n = new(element);
                current.Next = n;
                current = n;
            }
            count++;
        }

        /// <summary>Builds the array, returning the built array.</summary>
        /// <returns>The built array instance.</returns>
        [return: NotNull]
        public T[] Build()
        {
            long I = 0L, 
                tc = count; // Avoids thread safety issues.
            T[] a = new T[tc];
            Node g = root;
            while (I < tc && g is not null) {
                a[I++] = g.Value; 
                g = g.Next;
            }
            return a;
        }

        /// <summary>
        /// Removes all the elements added so far.
        /// </summary>
        [MustNotReportException]
        public void Clear()
        {
            root = null;
            count = 0L;
            current = null;
        }
    }
}