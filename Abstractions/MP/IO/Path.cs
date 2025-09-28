
/*
  * Portions of code are obtained from .NET Foundation:
  * Licensed to the .NET Foundation under one or more agreements.
  * The .NET Foundation licenses this file to you under the MIT license.
  */

using System;
using MP.Utilities;
using System.Text;
using MP.Annotations;
using System.Runtime.CompilerServices;
using System.Diagnostics.CodeAnalysis;

namespace MP.IO
{
    /// <summary>Defines utilities for file system paths.</summary>
    [Preliminary]
    [RequiresNativeLayer]
    public static class Path
    {
        /// <summary>
        /// Defines the directory separator in file system paths.
        /// </summary>
        public const System.Char DirectorySeparator = '/';
        /// <summary>
        /// Defines the Windows OS directory separator in file system paths.
        /// </summary>
        public const System.Char WindowsDirectorySeparator = '\\';
        /// <summary>
        /// Defines the Windows OS volume separator in file system paths.
        /// </summary>
        public const System.Char WindowsVolumeSeparator = ':';
        /// <summary>
        /// Defines the common path separator for POSIX systems.
        /// </summary>
        public const System.Char PathSeparator = ':';
        /// <summary>
        /// Defines the common path separator for Windows systems.
        /// </summary>
        public const System.Char WindowsPathSeparator = ';';
        internal const System.Int32 MaxShortPath = 260;
        internal const System.Int32 MaxLongPath = 65535;
        internal const System.String ExtendedPathPrefix = @"\\?\";
        internal const System.String UncPathPrefix = @"\\";
        internal const System.String UncExtendedPrefixToInsert = @"?\UNC\";
        internal const System.String UncExtendedPathPrefix = @"\\?\UNC\";
        internal const System.String DevicePathPrefix = @"\\.\";
        internal const System.String ParentDirectoryPrefix = @"..\";
        internal const int MaxShortDirectoryPath = 248;
        // \\?\, \\.\, \??\
        internal const int DevicePrefixLength = 4;
        // \\
        internal const int UncPrefixLength = 2;
        // \\?\UNC\, \\.\UNC\
        internal const int UncExtendedPrefixLength = 8;

        /// <summary>
        /// True if the given character is a directory separator.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool IsDirectorySeparator(System.Char c) => c == WindowsDirectorySeparator || c == DirectorySeparator;

        /// <summary>
        /// Normalize separators in the given path. Converts forward slashes into back slashes and compresses slash runs, keeping initial 2 if present.
        /// Also trims initial whitespace in front of "rooted" paths (see PathStartSkip).
        /// 
        /// This effectively replicates the behavior of the legacy NormalizePath when it was called with fullCheck=false and expandShortpaths=false.
        /// The current NormalizePath gets directory separator normalization from Win32's GetFullPathName(), which will resolve relative paths and as
        /// such can't be used here (and is overkill for our uses).
        /// 
        /// Like the current NormalizePath this will not try and analyze periods/spaces within directory segments.
        /// </summary>
        /// <remarks>
        /// The only callers that used to use Path.Normalize(fullCheck=false) were Path.GetDirectoryName() and Path.GetPathRoot(). Both usages do
        /// not need trimming of trailing whitespace here.
        /// 
        /// GetPathRoot() could technically skip normalizing separators after the second segment- consider as a future optimization.
        /// 
        /// For legacy desktop behavior with ExpandShortPaths:
        ///  - It has no impact on GetPathRoot() so doesn't need consideration.
        ///  - It could impact GetDirectoryName(), but only if the path isn't relative (C:\ or \\Server\Share).
        /// 
        /// In the case of GetDirectoryName() the ExpandShortPaths behavior was undocumented and provided inconsistent results if the path was
        /// fixed/relative. For example: "C:\PROGRA~1\A.TXT" would return "C:\Program Files" while ".\PROGRA~1\A.TXT" would return ".\PROGRA~1". If you
        /// ultimately call GetFullPath() this doesn't matter, but if you don't or have any intermediate System.String handling could easily be tripped up by
        /// this undocumented behavior.
        /// 
        /// We won't match this old behavior because:
        /// 
        ///   1. It was undocumented
        ///   2. It was costly (extremely so if it actually contained '~')
        ///   3. Doesn't play nice with System.String logic
        ///   4. Isn't a cross-plat friendly concept/behavior
        /// </remarks>
        internal static System.String NormalizeDirectorySeparators(System.String path)
        {
            if (System.String.IsNullOrEmpty(path))
                return path;

            char current;

            // Make a pass to see if we need to normalize so we can potentially skip allocating
            bool normalized = true;

            for (int i = 0; i < path.Length; i++)
            {
                current = path[i];
                if (IsDirectorySeparator(current)
                    && (current != DirectorySeparator
                        // Check for sequential separators past the first position (we need to keep initial two for UNC/extended)
                        || (i > 0 && i + 1 < path.Length && IsDirectorySeparator(path[i + 1]))))
                {
                    normalized = false;
                    break;
                }
            }

            if (normalized)
                return path;

            StringBuilder builder = new(MaxShortPath);

            int start = 0;
            if (IsDirectorySeparator(path[start]))
            {
                start++;
                builder.Append(DirectorySeparator);
            }

            for (int i = start; i < path.Length; i++)
            {
                current = path[i];

                // If we have a separator
                if (IsDirectorySeparator(current))
                {
                    // If the next is a separator, skip adding this
                    if (i + 1 < path.Length && IsDirectorySeparator(path[i + 1]))
                    {
                        continue;
                    }

                    // Ensure it is the primary separator
                    current = DirectorySeparator;
                }

                builder.Append(current);
            }

            return builder.ToString();
        }

        /// <summary>
        /// Returns true if the path is effectively empty for the current OS.
        /// For unix, this is empty or null. For Windows, this is empty, null, or 
        /// just spaces ((char)32).
        /// </summary>
        internal static bool IsEffectivelyEmpty(ReadOnlySpan<System.Char> path)
        {
            if (path.IsEmpty)
                return true;

            foreach (char c in path)
            {
                if (c != ' ')
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Returns true if the given character is a valid drive letter
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool IsValidDriveChar(System.Char value) => value.IsEnglishCharacter();

        internal static bool EndsWithPeriodOrSpace([MaybeNull] System.String path)
        {
            if (System.String.IsNullOrEmpty(path))
                return false;

            char c = path[path.Length - 1];
            return c == ' ' || c == '.';
        }

        /// <summary>
        /// Returns true if the path specified is relative to the current drive or working directory.
        /// Returns false if the path is fixed to a specific drive or UNC path.  This method does no
        /// validation of the path (URIs will be returned as relative as a result).
        /// </summary>
        /// <remarks>
        /// Handles paths that use the alternate directory separator.  It is a frequent mistake to
        /// assume that rooted paths (Path.IsPathRooted) are not relative.  This isn't the case.
        /// "C:a" is drive relative- meaning that it will be resolved against the current directory
        /// for C: (rooted, but relative). "C:\a" is rooted and not relative (the current directory
        /// will not be used to modify the path).
        /// </remarks>
        internal static bool IsPartiallyQualified(ReadOnlySpan<System.Char> path)
        {
            if (path.Length < 2)
            {
                // It isn't fixed, it must be relative.  There is no way to specify a fixed
                // path with one character (or less).
                return true;
            }

            if (IsDirectorySeparator(path[0]))
            {
                // There is no valid way to specify a relative path with two initial slashes or
                // \? as ? isn't valid for drive relative paths and \??\ is equivalent to \\?\
                return !(path[1] == '?' || IsDirectorySeparator(path[1]));
            }

            // The only way to specify a fixed path that doesn't begin with two slashes
            // is the drive, colon, slash format- i.e. C:\
            return !((path.Length >= 3)
                && (path[1] == WindowsVolumeSeparator)
                && IsDirectorySeparator(path[2])
                // To match old behavior we'll check the drive character for validity as the path is technically
                // not qualified if you don't have a valid drive. "=:\" is the "=" file's default data stream.
                && IsValidDriveChar(path[0]));
        }

        /// <summary>
        /// Returns true if the path uses any of the DOS device path syntaxes. ("\\.\", "\\?\", or "\??\")
        /// </summary>
        internal static bool IsDevice(ReadOnlySpan<char> path)
        {
            // If the path begins with any two separators is will be recognized and normalized and prepped with
            // "\??\" for internal usage correctly. "\??\" is recognized and handled, "/??/" is not.
            return IsExtended(path)
                ||
                (
                    path.Length >= DevicePrefixLength
                    && IsDirectorySeparator(path[0])
                    && IsDirectorySeparator(path[1])
                    && (path[2] == '.' || path[2] == '?')
                    && IsDirectorySeparator(path[3])
                );
        }

        /// <summary>
        /// Returns true if the path is a device UNC (\\?\UNC\, \\.\UNC\)
        /// </summary>
        internal static bool IsDeviceUNC(ReadOnlySpan<char> path)
        {
            return path.Length >= UncExtendedPrefixLength
                && IsDevice(path)
                && IsDirectorySeparator(path[7])
                && path[4] == 'U'
                && path[5] == 'N'
                && path[6] == 'C';
        }

        /// <summary>
        /// Returns true if the path uses the canonical form of extended syntax ("\\?\" or "\??\"). If the
        /// path matches exactly (cannot use alternate directory separators) Windows will skip normalization
        /// and path length checks.
        /// </summary>
        internal static bool IsExtended(ReadOnlySpan<char> path)
        {
            // While paths like "//?/C:/" will work, they're treated the same as "\\.\" paths.
            // Skipping of normalization will *only* occur if back slashes ('\') are used.
            return path.Length >= DevicePrefixLength
                && path[0] == '\\'
                && (path[1] == '\\' || path[1] == '?')
                && path[2] == '?'
                && path[3] == '\\';
        }

        /// <summary>
        /// Adds the extended path prefix (\\?\) if not relative or already a device path.
        /// </summary>
        internal static string EnsureExtendedPrefix(string path)
        {
            // Putting the extended prefix on the path changes the processing of the path. It won't get normalized, which
            // means adding to relative paths will prevent them from getting the appropriate current directory inserted.

            // If it already has some variant of a device path (\??\, \\?\, \\.\, //./, etc.) we don't need to change it
            // as it is either correct or we will be changing the behavior. When/if Windows supports long paths implicitly
            // in the future we wouldn't want normalization to come back and break existing code.

            // In any case, all internal usages should be hitting normalize path (Path.GetFullPath) before they hit this
            // shimming method. (Or making a change that doesn't impact normalization, such as adding a filename to a
            // normalized base path.)
            var sp = path.AsSpan();
            if (IsPartiallyQualified(sp) || IsDevice(sp))
                return path;

            // Given \\server\share in longpath becomes \\?\UNC\server\share
            if (path.StartsWith(UncPathPrefix, StringComparison.OrdinalIgnoreCase))
                return path.Insert(2, UncExtendedPrefixToInsert);

            return ExtendedPathPrefix + path;
        }

        /// <summary>
        /// Adds the extended path prefix (\\?\) if not already a device path, IF the path is not relative,
        /// AND the path is more than 259 characters. (> MAX_PATH + null). This will also insert the extended
        /// prefix if the path ends with a period or a space. Trailing periods and spaces are normally eaten
        /// away from paths during normalization, but if we see such a path at this point it should be
        /// normalized and has retained the final characters. (Typically from one of the *Info classes)
        /// </summary>
        [return: NotNullIfNotNull(nameof(path))]
        internal static System.String EnsureExtendedPrefixIfNeeded(System.String path)
        {
            if (path != null && (path.Length >= MaxShortPath || EndsWithPeriodOrSpace(path))) {
                return EnsureExtendedPrefix(path);
            } else {
                return path;
            }
        }

        /// <summary>
        /// Gets the length of the root of the path (drive, share, etc.).
        /// </summary>
        internal static System.Int32 GetRootLength(ReadOnlySpan<System.Char> path)
        {
            int pathLength = path.Length;
            int i = 0;

            bool deviceSyntax = IsDevice(path);
            bool deviceUnc = deviceSyntax && IsDeviceUNC(path);

            if ((!deviceSyntax || deviceUnc) && pathLength > 0 && IsDirectorySeparator(path[0]))
            {
                // UNC or simple rooted path (e.g. "\foo", NOT "\\?\C:\foo")
                if (deviceUnc || (pathLength > 1 && IsDirectorySeparator(path[1])))
                {
                    // UNC (\\?\UNC\ or \\), scan past server\share

                    // Start past the prefix ("\\" or "\\?\UNC\")
                    i = deviceUnc ? UncExtendedPrefixLength : UncPrefixLength;

                    // Skip two separators at most
                    int n = 2;
                    while (i < pathLength && (!IsDirectorySeparator(path[i]) || --n > 0))
                        i++;
                }
                else
                {
                    // Current drive rooted (e.g. "\foo")
                    i = 1;
                }
            }
            else if (deviceSyntax)
            {
                // Device path (e.g. "\\?\.", "\\.\")
                // Skip any characters following the prefix that aren't a separator
                i = DevicePrefixLength;
                while (i < pathLength && !IsDirectorySeparator(path[i]))
                    i++;

                // If there is another separator take it, as long as we have had at least one
                // non-separator after the prefix (e.g. don't take "\\?\\", but take "\\?\a\")
                if (i < pathLength && i > DevicePrefixLength && IsDirectorySeparator(path[i]))
                    i++;
            }
            else if (pathLength >= 2
                && path[1] == WindowsVolumeSeparator
                && IsValidDriveChar(path[0]))
            {
                // Valid drive specified path ("C:", "D:", etc.)
                i = 2;

                // If the colon is followed by a directory separator, move past it (e.g "C:\")
                if (pathLength > 2 && IsDirectorySeparator(path[2]))
                    i++;
            }

            return i;
        }

        /// <summary>
        /// Check for known wildcard characters. '*' and '?' are the most common ones.
        /// </summary>
        internal static bool HasWildCardCharacters(ReadOnlySpan<char> path)
        {
            // Question mark is part of dos device syntax so we have to skip if we are
            int startIndex = IsDevice(path) ? ExtendedPathPrefix.Length : 0;

            // [MS - FSA] 2.1.4.4 Algorithm for Determining if a FileName Is in an Expression
            // https://msdn.microsoft.com/en-us/library/ff469270.aspx
            for (int i = startIndex; i < path.Length; i++)
            {
                char c = path[i];
                if (c <= '?') // fast path for common case - '?' is highest wildcard character
                {
                    if (c == '\"' || c == '<' || c == '>' || c == '*' || c == '?')
                        return true;
                }
            }

            return false;
        }

        /// <summary>
        /// The returned ReadOnlySpan contains the characters of the path that follows the last separator in path.
        /// </summary>
        public static ReadOnlySpan<char> GetFileName(ReadOnlySpan<char> path)
        {
            int root = GetPathRoot(path).Length;

            // We don't want to cut off "C:\file.txt:stream" (i.e. should be "file.txt:stream")
            // but we *do* want "C:Foo" => "Foo". This necessitates checking for the root.

            for (int i = path.Length; --i >= 0;)
            {
                if (i < root || IsDirectorySeparator(path[i]))
                    return path.Slice(i + 1, path.Length - i - 1);
            }

            return path;
        }

        // Returns the root portion of the given path. The resulting string
        // consists of those rightmost characters of the path that constitute the
        // root of the path. Possible patterns for the resulting string are: An
        // empty string (a relative path on the current drive), "\" (an absolute
        // path on the current drive), "X:" (a relative path on a given drive,
        // where X is the drive letter), "X:\" (an absolute path on a given drive),
        // and "\\server\share" (a UNC path for a given server and share name).
        // The resulting string is null if path is null. If the path is empty or
        // only contains whitespace characters an ArgumentException gets thrown.
        /// <summary>
        /// Gets the root path of the specified path.
        /// </summary>
        /// <param name="path">The path to get it's root.</param>
        /// <returns>The path's root directory.</returns>
        [return : NotNullIfNotNull(nameof(path))]
        public static System.String GetPathRoot([MaybeNull] System.String path)
        {
            var sp = path.AsSpan();
            if (IsEffectivelyEmpty(sp))
                return null;

            ReadOnlySpan<char> result = GetPathRoot(sp);
            if (path!.Length == result.Length)
                return NormalizeDirectorySeparators(path);

            return NormalizeDirectorySeparators(result.ToString());
        }

        /// <remarks>
        /// Get the root path of the specified path. <br />
        /// Unlike the string overload, this method will not normalize directory separators.
        /// </remarks>
        /// <param name="path">The path to get it's root.</param>
        /// <returns>The path's root directory.</returns>
        public static ReadOnlySpan<System.Char> GetPathRoot(ReadOnlySpan<System.Char> path)
        {
            if (IsEffectivelyEmpty(path))
                return ReadOnlySpan<char>.Empty;

            int pathRoot = GetRootLength(path);
            return pathRoot <= 0 ? ReadOnlySpan<char>.Empty : path.Slice(0, pathRoot);
        }

        /// <summary>
        /// Returns the characters between the last separator and last (.) in the path.
        /// </summary>
        /// <param name="path">The path to strip.</param>
        /// <returns>The file name, without it's extension.</returns>
        public static ReadOnlySpan<System.Char> GetFileNameWithoutExtension(ReadOnlySpan<System.Char> path)
        {
            ReadOnlySpan<System.Char> fileName = GetFileName(path);
            int lastPeriod = fileName.LastIndexOf('.');
            return lastPeriod == -1 ?
                fileName : // No extension was found
                fileName.Slice(0, lastPeriod);
        }

    }
}