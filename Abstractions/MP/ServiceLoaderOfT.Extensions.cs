
using System;
using MP.Collections;
using System.Collections;
using MP.ExceptionSystem;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace MP
{
    partial class ServiceLoader<T>
    {
        private sealed class Concatenating : ServiceLoader<T>
        {
            private readonly ServiceLoader<T>[] loaders;

            public Concatenating(ServiceLoader<T>[] loaders) => this.loaders = loaders;

            private sealed class ConcatenatingEnumerator : IEnumerator<T>
            {
                private Concatenating instance;

                private int index, bound;
                private IEnumerator<T> current;

                public ConcatenatingEnumerator(Concatenating c)
                {
                    index = -1;
                    bound = c.loaders.Length;
                    instance = c;
                } 

                public T Current => current.Current;

                object IEnumerator.Current => Current;

                public void Dispose()
                {
                    current = null;
                    instance = null;
                    index = 0;
                    bound = 0;
                }

                public bool MoveNext()
                {
                    if (current is null) {
                        while (++index < bound) {
                            current = instance.loaders[index].GetEnumeratorImpl();
                            if (current.MoveNext()) {
                                return true;
                            }
                        }
                        return false;
                    } else if (current.MoveNext()) {
                        return true;
                    } else {
                        current.Dispose();
                        current = null;
                        return false;
                    }
                }

                public void Reset()
                {
                    current.Dispose();
                    current = null;
                    index = -1;
                }
            }

            [return: NotNull]
            protected override IEnumerator<T> GetEnumeratorImpl() => new ConcatenatingEnumerator(this);

            protected override void LoadImpl() {
                foreach (ServiceLoader<T> loader in this.loaders) { loader.LoadImpl(); }
            }

            protected override void Dispose(bool finalize)
            {
                AggregateExceptionBuilder builder = new();
                builder.Message = "One or more exceptions were occurred while trying to dispose service implementations.";
                foreach (ServiceLoader<T> ldr in loaders)
                {
                    try {
                        ldr.Dispose();
                    } catch (Exception ex) {
                        builder.Add(ex);
                    }
                }
                builder.ThrowIfHasExceptions();
            }
        }

        private sealed class Fixed : ServiceLoader<T>
        {
            private readonly T[] services;

            public Fixed(T[] services) => this.services = services;

            [return: NotNull]
            protected override IEnumerator<T> GetEnumeratorImpl() => ArrayEnumerator<T>.Of(services);

            protected override void LoadImpl() { }

            protected override void Dispose(bool finalize)
            {
                AggregateExceptionBuilder builder = new();
                builder.Message = "One or more exceptions were occurred while trying to dispose service implementations.";
                foreach (T item in services)
                {
                    try { item.Dispose(); } catch (Exception ex) { builder.Add(ex); }
                }
                builder.ThrowIfHasExceptions();
            }
        }

        /// <summary>
        /// Concatenates two different <see cref="ServiceLoader{T}"/> instances and returns them as a single <see cref="ServiceLoader{T}"/> instance.
        /// </summary>
        /// <param name="loader_1">The first loader.</param>
        /// <param name="loader_2">The second loader.</param>
        /// <returns>A new instance of the <see cref="ServiceLoader{T}"/> class representing the concatenated result.</returns>
        public static ServiceLoader<T> Concat(ServiceLoader<T> loader_1, ServiceLoader<T> loader_2)
        {
            ArgumentNullException.ThrowIfNull(loader_1);
            ArgumentNullException.ThrowIfNull(loader_2);
            return new Concatenating(new[] { loader_1, loader_2 });
        }

        /// <summary>
        /// Concatenates three different <see cref="ServiceLoader{T}"/> instances and returns them as a single <see cref="ServiceLoader{T}"/> instance.
        /// </summary>
        /// <param name="loader_1">The first loader.</param>
        /// <param name="loader_2">The second loader.</param>
        /// <param name="loader_3">The third loader.</param>
        /// <returns>A new instance of the <see cref="ServiceLoader{T}"/> class representing the concatenated result.</returns>
        public static ServiceLoader<T> Concat(ServiceLoader<T> loader_1, ServiceLoader<T> loader_2, ServiceLoader<T> loader_3)
        {
            ArgumentNullException.ThrowIfNull(loader_1);
            ArgumentNullException.ThrowIfNull(loader_2);
            ArgumentNullException.ThrowIfNull(loader_3);
            return new Concatenating(new[] { loader_1, loader_2, loader_3 });
        }

        /// <summary>
        /// Concatenates a multiple of service loaders.
        /// </summary>
        /// <param name="loaders">The service loaders to be concatenated.</param>
        /// <returns>A new instance of the <see cref="ServiceLoader{T}"/> class representing the concatenated result.</returns>
        public static ServiceLoader<T> Concat(params ServiceLoader<T>[] loaders)
        {
            ArgumentNullException.ThrowIfNull(loaders);
            return new Concatenating(loaders);
        }

        /// <summary>
        /// Returns a new <see cref="ServiceLoader{T}"/> instance from an already existing service.
        /// </summary>
        /// <param name="service_1">The service instance that is already existing.</param>
        /// <returns>A new instance of the <see cref="ServiceLoader{T}"/> class containing the passed service.</returns>
        public static ServiceLoader<T> Of(T service_1)
        {
            ArgumentNullException.ThrowIfNull(service_1);
            return new Fixed(new[] { service_1 });
        }

        /// <summary>
        /// Returns a new <see cref="ServiceLoader{T}"/> instance from two already existing services.
        /// </summary>
        /// <param name="service_1">The first service instance that is already existing.</param>
        /// <param name="service_2">The second service instance that is already existing.</param>
        /// <returns>A new instance of the <see cref="ServiceLoader{T}"/> class containing the passed services.</returns>
        public static ServiceLoader<T> Of(T service_1, T service_2)
        {
            ArgumentNullException.ThrowIfNull(service_1);
            ArgumentNullException.ThrowIfNull(service_2);
            return new Fixed(new[] { service_1, service_2 });
        }

        /// <summary>
        /// Returns a new <see cref="ServiceLoader{T}"/> instance from three already existing services.
        /// </summary>
        /// <param name="service_1">The first service instance that is already existing.</param>
        /// <param name="service_2">The second service instance that is already existing.</param>
        /// <param name="service_3">The third service instance that is already existing.</param>
        /// <returns>A new instance of the <see cref="ServiceLoader{T}"/> class containing the passed services.</returns>
        public static ServiceLoader<T> Of(T service_1, T service_2, T service_3)
        {
            ArgumentNullException.ThrowIfNull(service_1);
            ArgumentNullException.ThrowIfNull(service_2);
            ArgumentNullException.ThrowIfNull(service_3);
            return new Fixed(new[] { service_1, service_2, service_3 });
        }

        /// <summary>
        /// Returns a new <see cref="ServiceLoader{T}"/> instance from already existing services.
        /// </summary>
        /// <param name="services_existing">The service instances that are already existing.</param>
        /// <returns>A new instance of the <see cref="ServiceLoader{T}"/> class containing the passed services.</returns>
        public static ServiceLoader<T> Of(params T[] services_existing)
        {
            ArgumentNullException.ThrowIfNull(services_existing);
            return new Fixed(services_existing);
        }
    }
}