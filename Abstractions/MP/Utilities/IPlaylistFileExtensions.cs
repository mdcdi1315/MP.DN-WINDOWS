
using MP.PlaylistManagement;

namespace MP.Utilities
{
    /// <summary>
    /// Defines generic purpose extension methods around the <see cref="PlaylistFile"/> class.
    /// </summary>
    public static class IPlaylistFileExtensions
    {
        /// <summary>
        /// Uses the <see cref="PlaylistFile.Name"/> property to strip out only the file's name, tossing out it's extension part.
        /// </summary>
        /// <param name="info">The playlist file to get it's stripped name.</param>
        /// <returns>The stripped name of the file. If the file does not have an extension , it returns the <see cref="PlaylistFile.Name"/> property as-is.</returns>
        public static System.String GetNameOnly(this PlaylistFile info)
        {
            System.Int32 occ = info.Name.LastIndexOf('.');
            if (occ == -1) {
                return info.Name;
            } else {
                return info.Name.Remove(occ);
            }
        }

        /// <summary>
        /// Gets the file size to a human-friendly formatted string, suitable to be passed into a UI element.
        /// </summary>
        /// <param name="file">The playlist file to retrieve it's friendly size string.</param>
        /// <returns>A human-friendly string describing the file's length.</returns>
        public static System.String GetFileSizeAsFriendlyString(this PlaylistFile file)
        {
            System.Int64 num = file.Length;
            System.Int64 nump = 0;
            System.Byte I = 0;
            System.String format = "B";
            while (num > 0)
            {
                I++;
                nump = num;
                num /= 1024;
            }
            switch (I)
            {
                case 0:
                    // Returns Empty because the file size is 0.
                    return "Empty";
                case 1:
                    // Directly yield execution to format string.
                    break;
                case 2:
                    format = "KB";
                    break;
                case 3:
                    format = "MB";
                    break;
                case 4:
                    format = "GB";
                    break;
                case 5:
                    format = "TB";
                    break;
                default:
                    return "<N/A>";
            }
            return System.String.Format("{0} {1}", nump, format);
        }
    }
}