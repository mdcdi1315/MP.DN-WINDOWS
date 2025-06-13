

using System;

namespace MP.ComInterop
{
    /// <summary>
    /// The <strong>STGC</strong> enumeration constants specify the conditions for performing the commit operation in the IStorage::Commit and <see cref="IStream.Commit"/> methods.
    /// </summary>
    /// <remarks>
    /// You can specify <see cref="STGC_DEFAULT"/> or some combination of <see cref="STGC_OVERWRITE"/>, <see cref="STGC_ONLYIFCURRENT"/>, and <see cref="STGC_DANGEROUSLYCOMMITMERELYTODISKCACHE"/> for normal commit operations. 
    /// You can specify <see cref="STGC_CONSOLIDATE"/> with any other <see cref="STGC"/> flags. <br /> <br />
    /// 
    /// Typically, use <see cref="STGC_ONLYIFCURRENT"/> to protect the storage object in cases where more than one user can edit the object simultaneously.
    /// </remarks>
    [Flags]
    public enum STGC : System.UInt32
    {
        /// <summary>
        /// You can specify this condition with <see cref="STGC_CONSOLIDATE"/>, or some combination of the other three flags in this list of elements. Use this value to increase the readability of code.
        /// </summary>
        STGC_DEFAULT = 0,
        /// <summary>
        /// The commit operation can overwrite existing data to reduce overall space requirements.
        /// This value is not recommended for typical usage because it is not as robust as the default value. 
        /// In this case, it is possible for the commit operation to fail after the old data is overwritten, but before the new data is completely committed. 
        /// Then, neither the old version nor the new version of the storage object will be intact. <br /> <br />
        /// 
        /// You can use this value in the following cases: <br />
        /// <list type="bullet">
        ///     <item>The user is willing to risk losing the data.</item>
        ///     <item>The low-memory save sequence will be used to safely save the storage object to a smaller file.</item>
        ///     <item>A previous commit returned <see cref="CommonHResults.STG_E_MEDIUMFULL"/>, but overwriting the existing data would provide enough space to commit changes to the storage object.</item>
        /// </list> <br /> <br />
        /// Be aware that the commit operation verifies that adequate space exists before any overwriting occurs. 
        /// Thus, even with this value specified, if the commit operation fails due to space requirements, the old data is safe. 
        /// It is possible, however, for data loss to occur with the <see cref="STGC_OVERWRITE"/> value specified if the commit operation fails for any reason other than lack of disk space.
        /// </summary>
        STGC_OVERWRITE = 1,
        /// <summary>
        /// Prevents multiple users of a storage object from overwriting each other's changes. 
        /// The commit operation occurs only if there have been no changes to the saved storage object because the user most recently opened it. 
        /// Thus, the saved version of the storage object is the same version that the user has been editing. 
        /// If other users have changed the storage object, the commit operation fails and returns the <see cref="CommonHResults.STG_E_NOTCURRENT"/> value.
        /// To override this behavior, call the IStorage::Commit or IStream::Commit method again using the <see cref="STGC_DEFAULT"/> value.
        /// </summary>
        STGC_ONLYIFCURRENT = 2,
        /// <summary>
        /// Commits the changes to a write-behind disk cache, but does not save the cache to the disk. 
        /// In a write-behind disk cache, the operation that writes to disk actually writes to a disk cache, thus increasing performance.
        /// The cache is eventually written to the disk, but usually not until after the write operation has already returned. 
        /// The performance increase comes at the expense of an increased risk of losing data if a problem occurs before the cache is saved and the data in the cache is lost. <br /> <br />
        /// 
        /// If you do not specify this value, then committing changes to root-level storage objects is robust even if a disk cache is used. 
        /// The two-phase commit process ensures that data is stored on the disk and not just to the disk cache.
        /// </summary>
        STGC_DANGEROUSLYCOMMITMERELYTODISKCACHE = 4,
        /// <summary>
        /// Windows 2000 and Windows XP: Indicates that a storage should be consolidated after it is committed, resulting in a smaller file on disk. 
        /// This flag is valid only on the outermost storage object that has been opened in transacted mode.
        /// It is not valid for streams. The <see cref="STGC_CONSOLIDATE"/> flag can be combined with any other STGC flags.
        /// </summary>
        STGC_CONSOLIDATE = 8
    }
}