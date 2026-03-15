
using System;
using MP.Collections;
using System.Collections.Generic;
using MP.Annotations.CodeAnalysis;
using System.Diagnostics.CodeAnalysis;

namespace MP.IO
{
    /// <summary>
    /// Provides the means for accessing <see cref="FileSystem"/> implementations in a unified manner.
    /// </summary>
    public static class FileSystemProvider
    {
        private static FileSystem fs_default;
        private static readonly SingleLinkedList<KeyValuePair<System.String, FileSystem>> fs_dict;

        static FileSystemProvider() {
            fs_dict = new();
            fs_default = null;
        }

        /// <summary>
        /// Provides the ID for the default file system provided by the app's backend library.
        /// </summary>
        public const System.String DEFAULT_FS_ID = "DEFAULT";

        /// <summary>Looks up a file system by the specified unique ID.</summary>
        /// <param name="str">The ID of the file system object to be returned.</param>
        /// <param name="fs">The <see cref="FileSystem"/> object for <paramref name="str"/>.</param>
        /// <returns><see langword="true"/>, if the lookup was successfull; otherwise, <see langword="false"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="str"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="str"/> is the empty (&quot;&quot;) string.</exception>
        [Throws(typeof(ArgumentNullException), typeof(ArgumentException))]
        public static bool Lookup(System.String str, [NotNullWhen(true)] out FileSystem fs)
        {
            ArgumentException.ThrowIfNullOrEmpty(str);
            if (str == DEFAULT_FS_ID) {
                return (fs = fs_default) is not null;
            } else {
                foreach (var kvp in fs_dict)
                {
                    if (kvp.Key == str) { fs = kvp.Value; return true; }
                }
                fs = null;
                return false;
            }
        }

        /// <summary>Registers a <see cref="FileSystem"/> implementation to the file system provider.</summary>
        /// <param name="str">The ID of this file system registration.</param>
        /// <param name="fs">The <see cref="FileSystem"/> object to register by <paramref name="str"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="str"/> and/or <paramref name="fs"/> are <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="str"/> is the empty (&quot;&quot;) string. <br /> <br />
        /// 
        /// -or- <br /> <br />
        /// 
        /// There is already a <see cref="FileSystem"/> object registered under the specified ID.
        /// </exception>
        [Throws(typeof(ArgumentNullException), typeof(ArgumentException))]
        public static void Register(System.String str, FileSystem fs)
        {
            ArgumentNullException.ThrowIfNull(fs);
            ArgumentException.ThrowIfNullOrEmpty(str);
            if (str == DEFAULT_FS_ID) {
                if (fs_default is not null) {
                    throw new ArgumentException("There is already a file system object registered with this name: " + str, nameof(str));
                } else {
                    fs_default = fs;
                }
            } else {
                foreach (var kvp in fs_dict)
                {
                    if (kvp.Key == str)
                    {
                        throw new ArgumentException("There is already a file system object registered with this name: " + str, nameof(str));
                    }
                }
                fs_dict.Add(new(str, fs));
            }
        }

        /// <summary>
        /// Provides the default <see cref="FileSystem"/> object. <br />
        /// Typically, this is provided by the app's backend library.
        /// </summary>
        [MaybeNull]
        public static FileSystem Default => fs_default;

    }
}