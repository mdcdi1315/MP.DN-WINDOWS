


namespace MP.PlaylistManagement
{
    /// <summary>
    /// Defines the function that loads images in <see cref="PlaylistTrackTag"/> instances. <br />
    /// The byte array is loaded at demand to avoid keeping large arrays on memory.
    /// </summary>
    /// <param name="encodedkey">The encoded key reference to get the desired image.</param>
    /// <returns>The image data loaded as a single byte array.</returns>
    public delegate byte[] ImageSupplier(System.String encodedkey);
}