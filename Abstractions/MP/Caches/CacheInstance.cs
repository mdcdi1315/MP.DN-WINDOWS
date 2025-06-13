using System;
using MP.Utilities;

namespace MP.Caches
{
    /// <summary>
    /// Defines a convenience abstract class used when you want to manage cache instances around sessions.
    /// </summary>
    /// <typeparam name="TW">A derived implementation of a <see cref="CacheWriter"/> instance to use as the cache writer when saving data.</typeparam>
    /// <typeparam name="TR">A derived implementation of a <see cref="CacheReader"/> instance to use as the cache reader when reading data.</typeparam>
    public abstract class CacheInstance<TW , TR> : ICacheInstance<TW , TR>
        where TR : CacheReader
        where TW : CacheWriter
    {
        /// <summary>Default constructor.</summary>
        /// <exception cref="InvalidCacheFrontendTypeException">The cache reader and writer type tokens were just <see cref="CacheReader"/> and/or <see cref="CacheWriter"/>.</exception>
        protected CacheInstance() 
        {
            if (typeof(TR) == typeof(CacheReader))
            {
                // Clean CacheReader type passed in , a CacheInstance cannot be created.
                throw new InvalidCacheFrontendTypeException("Expected a derived CacheReader token to be passed in but the user specified the actual CacheReader token.");
            }
            if (typeof(TW) == typeof(CacheWriter))
            {
                // Clean CacheWriter type passed in , a CacheInstance cannot be created.
                throw new InvalidCacheFrontendTypeException("Expected a derived CacheWriter token to be passed in but the user specified the actual CacheWriter token.");
            }
        }

        /// <inheritdoc />
        public abstract void LoadCache(TR reader);

        /// <inheritdoc />
        public abstract void SaveCache(TW writer);

        /// <inheritdoc />
        public abstract void ClearCacheEntries();

        /// <summary>
        /// Creates a temporary cache reader object from the specified <paramref name="stream"/>, and then calls the <see cref="LoadCache(TR)"/> method. <br />
        /// Note that reflection is performed to create the reader object, and the code requires that a public <see cref="System.IO.Stream"/> constructor is present. <br />
        /// Disposal of the cache reader object is happened automatically after the load operation has finished or even failed. <br />
        /// However , note that you must still dispose on your own the stream passed by the <paramref name="stream"/> parameter.
        /// </summary>
        /// <param name="stream">The stream to load cached data from.</param>
        /// <exception cref="MissingMethodException">A public constructor accepting only a <see cref="System.IO.Stream"/> parameter was not found.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="stream"/> was <see langword="null"/>.</exception>
        public void LoadCacheFromStream(System.IO.Stream stream) 
        {
            // Avoid to do the hard work if we know in advance that the stream parameter is null.
            if (stream is null) {
                throw new ArgumentNullException(nameof(stream));
            }
            foreach (var ci in typeof(TR).GetConstructors())
            {
                var pi = ci.GetParameters();
                if (pi.Length == 1 && pi[0].ParameterType.IsTypeOrDerivesFrom(typeof(System.IO.Stream)))
                {
                    TR readerobj;
                    try {
                        readerobj = ci.Invoke(new System.Object[] { stream }) as TR;
                    } catch (System.Reflection.TargetInvocationException tie) {
                        // Throw as if we statically knew the constructor, that would threw the exception directly.
                        throw tie.InnerException;
                    }
                    try {
                        readerobj.IsStreamOwner = false;
                        LoadCache(readerobj);
                    } finally {
                        readerobj?.Dispose();
                    }
                    return;
                }
            }
            throw new MissingMethodException("A public constructor with a System.IO.Stream as it's single parameter was not found.\nMethod name: .ctor.");
        }

        /// <summary>
        /// Creates a temporary cache writer object from the specified <paramref name="stream"/>, and then calls the <see cref="SaveCache(TW)"/> method. <br />
        /// Note that reflection is performed to create the writer object, and the code requires that a public <see cref="System.IO.Stream"/> constructor is present. <br />
        /// Disposal of the cache writer object is happened automatically after the load operation has finished or even failed. <br />
        /// However , note that you must still dispose on your own the stream passed by the <paramref name="stream"/> parameter.
        /// </summary>
        /// <param name="stream">The stream to save the cached data to.</param>
        /// <exception cref="MissingMethodException">A public constructor accepting only a <see cref="System.IO.Stream"/> parameter was not found.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="stream"/> was <see langword="null"/>.</exception>
        public void SaveCacheToStream(System.IO.Stream stream)
        {
            // Avoid to do the hard work if we know in advance that the stream parameter is null.
            if (stream is null) {
                throw new ArgumentNullException(nameof(stream));
            }
            foreach (var ci in typeof(TW).GetConstructors())
            {
                var pi = ci.GetParameters();
                if (pi.Length == 1 && pi[0].ParameterType.IsTypeOrDerivesFrom(typeof(System.IO.Stream)))
                {
                    TW writerobj;
                    try {
                        writerobj = ci.Invoke(new System.Object[] { stream }) as TW;
                    } catch (System.Reflection.TargetInvocationException tie) {
                        // Throw as if we knew the constructor, that would threw the exception directly.
                        throw tie.InnerException;
                    }
                    try {
                        writerobj.IsStreamOwner = false;
                        SaveCache(writerobj);
                    } finally {
                        writerobj?.Dispose();
                    }
                    return;
                }
            }
            throw new MissingMethodException("A public constructor with a System.IO.Stream as it's single parameter was not found.\nMethod name: .ctor.");
        }
    }
}
