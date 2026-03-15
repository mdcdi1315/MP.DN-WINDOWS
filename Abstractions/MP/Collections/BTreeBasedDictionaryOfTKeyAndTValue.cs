
using System;
using System.Threading;
using System.Collections;
using System.Diagnostics;
using System.Collections.Generic;
using MP.Annotations.CodeAnalysis;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace MP.Collections
{
    /// <summary>
    /// Defines a <see cref="IDictionary{TKey, TValue}"/> implementation by using a stable hash table and B-Trees for storing additional values in the dictionary. <br />
    /// Keys cannot be explicitly repeated on this implementation.
    /// </summary>
    /// <typeparam name="TKey">The type of key to be stored in the B-Tree dictionary.</typeparam>
    /// <typeparam name="TValue">The type of value to be stored in the B-Tree dictionary.</typeparam>
    public class BTreeBasedDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IGettableSettable<TKey, TValue>
    {
        private int count;
        private readonly BTreeNode[] HashTable;

        [NotNull]
        private readonly IEqualityComparer<TKey> comparer;

        private class BTreeNode
        {
            public TKey Key;
            public TValue Value;
            public int HashCode;

            public BTreeNode Parent;
            public BTreeNode LeftChild;
            public BTreeNode RightChild;

            public BTreeNode(TKey key, TValue value)
            {
                Key = key;
                Value = value;
                Parent = LeftChild = RightChild = null;
            }

            public KeyValuePair<TKey, TValue> ToKeyValuePair() => new(Key, Value);
        }

        private sealed class DeletedBTreeNode : BTreeNode
        {
            public DeletedBTreeNode(BTreeNode node) : base(default, default)
            {
                Parent = node.Parent;
                LeftChild = node.LeftChild;
                RightChild = node.RightChild;
                HashCode = node.HashCode;
            }

            public BTreeNode ToNode() => new(Key, Value)
            {
                Parent = Parent,
                LeftChild = LeftChild,
                RightChild = RightChild,
                HashCode = HashCode,
            };

            public override string ToString() => $"Deleted node: {LeftChild} {RightChild} {Parent}";
        }

        private sealed class Enumerator : GenericDictionaryEnumerator<TKey, TValue>
        {
            private int hh_index;
            private BTreeNode[] hh;
            private BTreeNode current;
            private SingleLinkedListBasedQueue<BTreeNode> nodes_to_enumerate;

            public Enumerator(BTreeNode[] ht)
            {
                hh = ht;
                hh_index = -1;
                current = null;
                nodes_to_enumerate = new();
            }

            private bool MoveToNextBTree()
            {
                BTreeNode temp = null;
                while (++hh_index < hh.Length && (temp = hh[hh_index]) is null) ;
                
                if (temp is null) {
                    return false;
                } else {
                    nodes_to_enumerate.Enqueue(temp);
                    return true;
                }
            }

            private void EnqNodeSafe(BTreeNode node)
            {
                if (node is null) { return; }
                nodes_to_enumerate.Enqueue(node);
            }

            public override KeyValuePair<TKey, TValue> Current => current.ToKeyValuePair();

            public override void Dispose()
            {
                base.Dispose();
                hh = null;
                current = null;
                nodes_to_enumerate = null;
            }

            protected override bool MoveNextImpl()
            {
            g_begin:
                while (nodes_to_enumerate.TryDequeue(out current))
                {
                    EnqNodeSafe(current.LeftChild);
                    EnqNodeSafe(current.RightChild);
                    if (current is not DeletedBTreeNode) { break; }
                }
                if (current is not null) {
                    return true;
                } else if (MoveToNextBTree()) {
                    goto g_begin;
                } else {
                    return false;
                }
            }

            protected override void ResetImpl()
            {
                hh_index = -1;
                current = null;
                nodes_to_enumerate.Clear();
            }
        }

        private sealed class Syncronized : BTreeBasedDictionary<TKey, TValue>, ISyncronizedByObject
        {
            private readonly object sync_object;

            public Syncronized() : base() => sync_object = new();

            public Syncronized(int h_table_size) : base(h_table_size) => sync_object = new();

            public Syncronized(IEqualityComparer<TKey> eqc) : base(eqc) => sync_object = new();

            public Syncronized(int h_table_size, IEqualityComparer<TKey> eqc) : base(h_table_size, eqc) => sync_object = new();

            public object SyncObject => sync_object;

            public override void Add(TKey key, [AllowNull] TValue value)
            {
                Monitor.Enter(sync_object);
                try {
                    base.Add(key, value);
                } finally {
                    Monitor.Exit(sync_object);
                }
            }

            public override void Clear()
            {
                Monitor.Enter(sync_object);
                try {
                    base.Clear();
                } finally {
                    Monitor.Exit(sync_object);
                }
            }

            public override bool ContainsKey(TKey key)
            {
                Monitor.Enter(sync_object);
                try {
                    return base.ContainsKey(key);
                } finally {
                    Monitor.Exit(sync_object);
                }
            }

            public override bool Remove(TKey key)
            {
                Monitor.Enter(sync_object);
                try {
                    return base.Remove(key);
                } finally {
                    Monitor.Exit(sync_object);
                }
            }

            public override TValue this[TKey key] 
            { 
                get {
                    Monitor.Enter(sync_object);
                    try {
                        return base[key];
                    } finally {
                        Monitor.Exit(sync_object);
                    }
                } 
                set {
                    Monitor.Enter(sync_object);
                    try {
                        base[key] = value;
                    } finally {
                        Monitor.Exit(sync_object);
                    }
                }
            }

            public override bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
            {
                Monitor.Enter(sync_object);
                try {
                    return base.TryGetValue(key, out value);
                } finally {
                    Monitor.Exit(sync_object);
                }
            }
        }

        #region Construction

        /// <summary>
        /// Constructs a new instance of the <see cref="BTreeBasedDictionary{TKey, TValue}"/> class, 
        /// specifying 15 B-Trees and using the default equality comparer for comparing keys.
        /// </summary>
        public BTreeBasedDictionary()
        {
            count = 0;
            HashTable = new BTreeNode[10];
            comparer = EqualityComparer<TKey>.Default;
        }

        /// <summary>
        /// Constructs a new instance of the <see cref="BTreeBasedDictionary{TKey, TValue}"/> class and specifying the number of B-Trees that can be internally allocated for this dictionary.
        /// </summary>
        /// <remarks>
        /// While it is possible to define a value of 1 for <paramref name="hash_table_size"/>, 
        /// this means that you are explicitly using a B-Tree only for managing the entirety of the dictionary
        /// and this can have adverse effects on CPU performance. A size of 10 is typically good enough for most applications.
        /// </remarks>
        /// <param name="hash_table_size">The internal hash table size. As larger this value is, the more and smaller B-Trees can be stored.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="hash_table_size"/> is a negative value.</exception>
        public BTreeBasedDictionary(int hash_table_size)
        {
            if (hash_table_size < 0) {
                throw new ArgumentOutOfRangeException(nameof(hash_table_size), "Hash table size cannot be a negative value.");
            } else {
                count = 0;
                HashTable = new BTreeNode[hash_table_size];
                comparer = EqualityComparer<TKey>.Default;
            }
        }

        /// <summary>
        /// Constructs a new instance of the <see cref="BTreeBasedDictionary{TKey, TValue}"/> class and specifying the equality comparer to use for comparing the dictionary's keys.
        /// </summary>
        /// <remarks>
        /// Generally try to provide an equality comparer that is non-collisible, at least for the use case you need this. <br />
        /// This class depends on hash codes in order to properly re-order the keys in the B-Trees.
        /// </remarks>
        /// <param name="comparer">An <see cref="IEqualityComparer{T}"/> implementation for comparing the keys of this dictionary. Can be <see langword="null"/>.</param>
        public BTreeBasedDictionary([AllowNull] IEqualityComparer<TKey> comparer)
        {
            count = 0;
            HashTable = new BTreeNode[10];
            this.comparer = comparer ?? EqualityComparer<TKey>.Default;
        }

        /// <summary>
        /// Constructs a new instance of the <see cref="BTreeBasedDictionary{TKey, TValue}"/> class by specifying the number of B-Trees that can be internally allocated for this dictionary,
        /// as well as the equality comparer to use for comparing the dictionary's keys.
        /// </summary>
        /// <param name="hash_table_size">The internal hash table size. As larger this value is, the more and smaller B-Trees can be stored.</param>
        /// <param name="comparer">An <see cref="IEqualityComparer{T}"/> implementation for comparing the keys of this dictionary. Can be <see langword="null"/>.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="hash_table_size"/> is a negative value.</exception>
        public BTreeBasedDictionary(int hash_table_size, [AllowNull] IEqualityComparer<TKey> comparer)
        {
            if (hash_table_size < 0) {
                throw new ArgumentOutOfRangeException(nameof(hash_table_size), "Hash table size cannot be a negative value.");
            } else {
                count = 0;
                HashTable = new BTreeNode[hash_table_size];
                this.comparer = comparer ?? EqualityComparer<TKey>.Default;
            }
        }

        /// <summary>
        /// Constructs a new instance of the <see cref="BTreeBasedDictionary{TKey, TValue}"/> class, 
        /// specifying 15 B-Trees and using the default equality comparer for comparing keys. <br />
        /// The object that is returned from this method is thread-safe.
        /// </summary>
        public static BTreeBasedDictionary<TKey, TValue> CreateSyncronized() => new Syncronized();

        /// <summary>
        /// Constructs a new instance of the <see cref="BTreeBasedDictionary{TKey, TValue}"/> class and specifying the number of B-Trees that can be internally allocated for this dictionary. <br />
        /// The object that is returned from this method is thread-safe.
        /// </summary>
        /// <remarks>
        /// While it is possible to define a value of 1 for <paramref name="hash_table_size"/>, 
        /// this means that you are explicitly using a B-Tree only for managing the entirety of the dictionary
        /// and this can have adverse effects on CPU performance. A size of 10 is typically good enough for most applications.
        /// </remarks>
        /// <param name="hash_table_size">The internal hash table size. As larger this value is, the more and smaller B-Trees can be stored.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="hash_table_size"/> is a negative value.</exception>
        public static BTreeBasedDictionary<TKey, TValue> CreateSyncronized(int hash_table_size) => new Syncronized(hash_table_size);

        /// <summary>
        /// Constructs a new instance of the <see cref="BTreeBasedDictionary{TKey, TValue}"/> class and specifying the equality comparer to use for comparing the dictionary's keys. <br />
        /// The object that is returned from this method is thread-safe.
        /// </summary>
        /// <remarks>
        /// Generally try to provide an equality comparer that is non-collisible, at least for the use case you need this. <br />
        /// This class depends on hash codes in order to properly re-order the keys in the B-Trees.
        /// </remarks>
        /// <param name="comparer">An <see cref="IEqualityComparer{T}"/> implementation for comparing the keys of this dictionary. Can be <see langword="null"/>.</param>
        public static BTreeBasedDictionary<TKey, TValue> CreateSyncronized([AllowNull] IEqualityComparer<TKey> comparer) => new Syncronized(comparer);

        /// <summary>
        /// Constructs a new instance of the <see cref="BTreeBasedDictionary{TKey, TValue}"/> class by specifying the number of B-Trees that can be internally allocated for this dictionary,
        /// as well as the equality comparer to use for comparing the dictionary's keys. <br />
        /// The object that is returned from this method is thread-safe.
        /// </summary>
        /// <param name="hash_table_size">The internal hash table size. As larger this value is, the more and smaller B-Trees can be stored.</param>
        /// <param name="comparer">An <see cref="IEqualityComparer{T}"/> implementation for comparing the keys of this dictionary. Can be <see langword="null"/>.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="hash_table_size"/> is a negative value.</exception>
        public static BTreeBasedDictionary<TKey, TValue> CreateSyncronized(int hash_table_size, [AllowNull] IEqualityComparer<TKey> comparer) => new Syncronized(hash_table_size, comparer);

        #endregion

        #region Private Implementation Details

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int HashFunction(int hash_code) => Math.Abs(hash_code % HashTable.Length);

        private static BTreeNode FindNode(int key_hash_code, BTreeNode n) => n is null ? null : 
        (
                    (key_hash_code < n.HashCode) ? (
                            // Search in the left child node
                            FindNode(key_hash_code, n.LeftChild)
                    ) : (
                            (key_hash_code > n.HashCode) ? (
                                    // Search in the right child node
                                    FindNode(key_hash_code, n.RightChild)
                            ) : n
                    )
        );

        // Puts a new node in the B-Tree.
        [DebuggerHidden]
        [StackTraceHidden]
        private static BTreeNode PutInBTree(BTreeNode constructed, BTreeNode root)
        {
            if (root is null) {
                return constructed;
            } else if (constructed.HashCode < root.HashCode) {
                constructed.Parent = root; // Required so that our newly added node points to the correct parent
                root.LeftChild = PutInBTree(constructed, root.LeftChild);
            } else if (constructed.HashCode > root.HashCode) {
                constructed.Parent = root; // Required so that our newly added node points to the correct parent
                root.RightChild = PutInBTree(constructed, root.RightChild);
            } else if (root is DeletedBTreeNode dn) {
                // constructed.key == root.key will be true
                // Re-convert back to a node
                BTreeNode ret = dn.ToNode();
                ret.Value = constructed.Value;
                // Return it.
                return ret;
            } else {
                throw new ArgumentException($"The key '{constructed.Key}' has already been added to the dictionary!");
            }
            // Return the root node.
            return root;
        }

        private BTreeNode FindNodeReference(TKey key)
        {
            int k = comparer.GetHashCode(key);
            return FindNode(k, HashTable[HashFunction(k)]);
        }

        #endregion

        /// <inheritdoc />
        public int Count => count;

        /// <inheritdoc />
        public bool IsReadOnly => false;

        /// <inheritdoc />
        [Throws(typeof(ArgumentNullException), typeof(ArgumentNullException))]
        public virtual void Add([NotNull] TKey key, [AllowNull] TValue value)
        {
            ArgumentNullException.ThrowIfNull(key);

            int hc = comparer.GetHashCode(key), h = HashFunction(hc);

            BTreeNode constructed = new(key, value) { HashCode = hc };

            HashTable[h] = PutInBTree(constructed, HashTable[h]);
            count++;
        }

        /// <inheritdoc />
        public virtual TValue this[TKey key]
        {
            [Throws(typeof(ArgumentNullException))]
            get {
                if (!TryGetValue(key, out TValue v)) {
                    throw new KeyNotFoundException($"Key '{key}' could not be found.");
                }
                return v;
            }
            [Throws(typeof(ArgumentNullException))]
            set {
                ArgumentNullException.ThrowIfNull(key);
                BTreeNode fd = FindNodeReference(key);
                if (fd is null) {
                    int hc = comparer.GetHashCode(key), h = HashFunction(hc);

                    BTreeNode constructed = new(key, value) { HashCode = hc };

                    HashTable[h] = PutInBTree(constructed, HashTable[h]);
                    count++;
                } else {
                    fd.Value = value;
                }
            }
        }

        /// <inheritdoc />
        public virtual void Clear()
        {
            count = 0;
            for (int I = 0; I < HashTable.Length; I++) { HashTable[I] = null; }
        }

        /// <inheritdoc />
        [Throws(typeof(ArgumentNullException))]
        public virtual bool Remove([NotNull] TKey key)
        {
            ArgumentNullException.ThrowIfNull(key);
            BTreeNode f = FindNodeReference(key);
            if (f is null || f is DeletedBTreeNode) {
                return false;
            } else {
                if (f.Parent is null) {
                    HashTable[HashFunction(f.HashCode)] = new DeletedBTreeNode(f);
                } else if (ReferenceEquals(f.Parent.LeftChild, f)) {
                    f.Parent.LeftChild = new DeletedBTreeNode(f);
                } else {
                    f.Parent.RightChild = new DeletedBTreeNode(f);
                }
                count--;
                return true;
            }
        }

        /// <inheritdoc />
        [Throws(typeof(ArgumentNullException))]
        public virtual bool TryGetValue([NotNull] TKey key, [MaybeNullWhen(false)] out TValue value)
        {
            ArgumentNullException.ThrowIfNull(key);
            BTreeNode n = FindNodeReference(key);
            if (n is null || n is DeletedBTreeNode) {
                value = default;
                return false;
            } else {
                value = n.Value;
                return true;
            }
        }

        /// <inheritdoc />
        public virtual bool ContainsKey([AllowNull] TKey key) => key is not null && FindNodeReference(key) is not null;

        /// <inheritdoc />
        [Throws(typeof(ArgumentNullException), typeof(ArgumentOutOfRangeException), typeof(ArgumentException))]
        public virtual void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            ArgumentNullException.ThrowIfNull(array);
            if (arrayIndex < 0) {
                throw new ArgumentOutOfRangeException(nameof(arrayIndex), "Array index cannot be a negative value.");
            } else if (arrayIndex + count > array.Length) {
                throw new ArgumentException("The array does not have enough space to place all the elements of the current BTreeBasedDictionary object.", nameof(array));
            } else {
                Enumerator e = new(HashTable);
                try {
                    for (int I = arrayIndex; e.MoveNext(); I++) { array[I] = e.Current; }
                } finally {
                    e.Dispose();
                }
            }
        }
        
        #region Collection getters and enumerators

        /// <inheritdoc />
        public ICollection<TKey> Keys
        {
            get {
                int I = 0;
                TKey[] data = new TKey[count];
                Enumerator e = new(HashTable);
                try {
                    while (e.MoveNext()) { data[I++] = e.Current.Key; }
                    return data;
                } finally {
                    e.Dispose();
                }
            }
        }

        /// <inheritdoc />
        public ICollection<TValue> Values
        {
            get {
                int I = 0;
                TValue[] data = new TValue[count];
                Enumerator e = new(HashTable);
                try {
                    while (e.MoveNext()) { data[I++] = e.Current.Value; }
                    return data;
                } finally {
                    e.Dispose();
                }
            }
        }

        /// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
        public virtual GenericDictionaryEnumerator<TKey, TValue> GetEnumerator() => new Enumerator(HashTable);

        #endregion

        #region Explicit Interface Declarations

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            TKey key;

            if ((key = item.Key) is null)
            {
                throw new ArgumentException("The Key property of the key-value pair is null.", nameof(item));
            }
            else
            {
                int hc = comparer.GetHashCode(key), h = HashFunction(hc);

                BTreeNode constructed = new(key, item.Value) { HashCode = hc };

                HashTable[h] = PutInBTree(constructed, HashTable[h]);
                count++;
            }
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item) => Remove(item.Key);

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item) => ContainsKey(item.Key);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator() => GetEnumerator();

        #endregion
    }
}