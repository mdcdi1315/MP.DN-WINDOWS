


namespace MP.AudioLibrary.MediaFoundation
{
    /// <summary>
    /// MF Attributes store match type enumeration. <br />
    /// It is used to compare two <see cref="IMFAttributes"/> interfaces. <br />
    /// The enum values were retrieved from the Windows 10.0.19041.0 SDK. <br />
    /// See file mfobjects.idl, line 51 for more information.
    /// </summary>
    public enum MF_ATTRIBUTES_MATCH_TYPE
    {
        /// <summary>
        /// do all of our items exist in their store and have identical data?
        /// </summary>
        MF_ATTRIBUTES_MATCH_OUR_ITEMS = 0, 
        /// <summary>
        /// do all of their items exist in our store and have identical data?
        /// </summary>
        MF_ATTRIBUTES_MATCH_THEIR_ITEMS = 1, 
        /// <summary>
        /// do both stores have the same set of identical items?
        /// </summary>
        MF_ATTRIBUTES_MATCH_ALL_ITEMS = 2,   
        /// <summary>
        /// do the attributes that intersect match?
        /// </summary>
        MF_ATTRIBUTES_MATCH_INTERSECTION = 3,
        /// <summary>
        /// do all the attributes in the type that has fewer attributes match?
        /// </summary>
        MF_ATTRIBUTES_MATCH_SMALLER = 4,
    }
}