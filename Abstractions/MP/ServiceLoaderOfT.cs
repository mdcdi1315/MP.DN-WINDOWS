
using System;
using System.Collections;
using MP.ExceptionSystem;
using System.Collections.Generic;
using MP.Annotations.CodeAnalysis;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;

namespace MP
{
    /// <summary>Provides an abstract class for loading service interfaces from a location.</summary>
    /// <typeparam name="T">The type of the interface to be loaded.</typeparam>
    public abstract partial class ServiceLoader<T> : CriticalFinalizerObject, IEnumerable<T>, IDisposable
        where T : notnull, IService
    {
        private enum Flags : System.Byte
        {
            NONE = 0,
            DISPOSED = 1 << 0,
            LOADED = 1 << 1
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool HasFlagFast(Flags f) => (flags & f) == f;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool HasNotFlagFast(Flags f) => (flags & f) != f;

        private volatile Flags flags;

        /// <summary>
        /// Initializes the <see cref="ServiceLoader{T}"/> class.
        /// </summary>
        protected ServiceLoader() => flags = Flags.NONE;

        /// <summary>
        /// Executes the lookup and loading of all the discovered service objects.
        /// </summary>
        /// <exception cref="ObjectDisposedException">This instance has been disposed of.</exception>
        [Throws(typeof(ObjectDisposedException))]
        public void Load()
        {
            ObjectDisposedException.ThrowIf(HasFlagFast(Flags.DISPOSED), this);
            if (HasFlagFast(Flags.LOADED)) {
                throw new InvalidOperationException("The service loader has been already loaded before.");
            } else {
                try {
                    LoadImpl();
                } finally {
                    flags |= Flags.LOADED;
                }
            }
        }

        /// <summary>
        /// Defines the actual implementation of the <see cref="Load"/> method.
        /// </summary>
        [Throws]
        protected abstract void LoadImpl();

        /// <summary>
        /// Gets an enumerator that can enumerate through all of the declared services in the current object.
        /// </summary>
        /// <returns>An enumerator that enumerates through all of the declared services in the current object.</returns>
        [return: NotNull]
        [Throws(typeof(ObjectDisposedException))]
        public IEnumerator<T> GetEnumerator()
        {
            ObjectDisposedException.ThrowIf(HasFlagFast(Flags.DISPOSED), this);
            return GetEnumeratorImpl();
        }

        /// <summary>
        /// Gets an enumerator that can enumerate through all of the declared services in the current object.
        /// </summary>
        /// <returns>An enumerator that enumerates through all of the declared services in the current object.</returns>
        [return: NotNull]
        protected abstract IEnumerator<T> GetEnumeratorImpl();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// Gets a value whether the current <see cref="ServiceLoader{T}"/> has loaded all the services it could load. <br />
        /// (That is, the <see cref="Load"/> method has been already called once)
        /// </summary>
        public bool IsLoaded => HasFlagFast(Flags.LOADED);

        /// <summary>
        /// Disposes this <see cref="ServiceLoader{T}"/> instance. <br />
        /// A boolean parameter indicates whether this method is executing from the service loader finalizer method. <br />
        /// Implementers of this class needing to dispose their own resources should override this method 
        /// and call this one in the end by using the <see langword="base"/> convention.
        /// </summary>
        /// <param name="finalize">A value whether this method was called from the object's finalizer.</param>
        protected virtual void Dispose(bool finalize)
        {
            AggregateExceptionBuilder builder = new();
            builder.Message = "One or more exceptions were occurred while trying to dispose service implementations.";
            foreach (T item in this)
            {
                try { item.Dispose(); } catch (Exception ex) { builder.Add(ex); }
            }
            builder.ThrowIfHasExceptions();
        }

        /// <summary>
        /// Disposes this <see cref="ServiceLoader{T}"/> instance.
        /// </summary>
        /// <exception cref="AggregateException">One or more dispose failures were occurred while disposing all the loaded services.</exception>
        [Throws(typeof(AggregateException))]
        public void Dispose()
        {
            try {
                if (HasNotFlagFast(Flags.DISPOSED)) {
                    flags |= Flags.DISPOSED;
                    Dispose(false);
                }
            } finally {
                GC.SuppressFinalize(this);
            }
        }

        /// <summary>Default finalizer implementation delegating to <see cref="Dispose(bool)"/> method.</summary>
        ~ServiceLoader() => Dispose(true);
    }
}