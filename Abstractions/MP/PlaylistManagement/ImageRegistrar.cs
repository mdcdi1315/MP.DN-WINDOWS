


namespace MP.PlaylistManagement
{
    /// <summary>
    /// Function for registering an image to the playlist. <br />
    /// This function has double usage: <br />
    /// When <paramref name="image"/> is non-null, the implementing function should add the specified image data to the playlist and the image cache. <br />
    /// When <paramref name="image"/> is null , the implementing function should remove the cover image from the playlist and the cache.
    /// </summary>
    /// <param name="image">The image data to register.</param>
    /// <param name="file">The playlist file requesting audio tag cover image registration.</param>
    /// <returns>A unique string referencing the image, making the image both accessible by cache and the playlist data.</returns>
    public delegate System.String ImageRegistrar(System.Byte[] image , PlaylistFile file);
}