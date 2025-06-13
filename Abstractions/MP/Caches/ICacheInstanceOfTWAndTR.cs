


namespace MP.Caches
{
    /// <summary>
    /// The lower-level abstraction for cache instances. <br />
    /// A user should typically use the <see cref="CacheInstance{TW, TR}"/> class 
    /// but if he wants more explicit layout of the instance class, he should create a new one
    /// and should implement this interface only instead.
    /// </summary>
    /// <typeparam name="TW">A derived implementation of a <see cref="CacheWriter"/> instance to use as the cache writer when saving data.</typeparam>
    /// <typeparam name="TR">A derived implementation of a <see cref="CacheReader"/> instance to use as the cache reader when reading data.</typeparam>
    public interface ICacheInstance<TW , TR>
        where TR : CacheReader
        where TW : CacheWriter
    {
        /// <summary>
        /// Loads the cache into this instance by the specified compatible cache reader.
        /// </summary>
        /// <param name="reader">The cache reader to use.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="reader"/> was <see langword="null"/>.</exception>
        public void LoadCache(TR reader);

        /// <summary>
        /// Saves the previously loaded and modified cache data with the specified compatible cache writer.
        /// </summary>
        /// <param name="writer">The cache writer to use.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="writer"/> was <see langword="null"/>.</exception>
        public void SaveCache(TW writer);

        /// <summary>
        /// Clears the cached entries either created by calls to this instance , or by the cached data loaded by the <see cref="LoadCache(TR)"/> method.
        /// </summary>
        public void ClearCacheEntries();
    }
}