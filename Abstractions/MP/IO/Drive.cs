
using System.Diagnostics.CodeAnalysis;

namespace MP.IO
{
    /// <summary>
    /// Provides the abstraction base for attached OS drives. <br />
    /// This class does not extend from the <see cref="FileSystemObject"/> as a drive is recognized as a piece of low-level hardware connected to software.
    /// </summary>
    public abstract class Drive
    {
        /// <summary>
        /// Gets the root directory of this <see cref="Drive"/> object.
        /// </summary>
        [NotNull]
        public abstract Directory Root { get; }

        /// <summary>Gets the name of the drive, as seen by the OS.</summary>
        public abstract System.String Name { get; }

        /// <summary>
        /// Gets the friendly name of the drive
        /// </summary>
        public abstract System.String FriendlyName { get; }

        /// <summary>
        /// Gets the file system that the drive is formatted with. <br />
        /// Can be the empty string if not recognized by the OS
        /// </summary>
        public virtual string DriveFormat { get => System.String.Empty; }

        /// <summary>
        /// Gets the available free space on the drive. <br />
        /// Can be 0 if not recognized by the OS
        /// </summary>
        public virtual ulong AvailableFreeSpace { get => 0UL; }

        /// <summary>
        /// Gets the total free space on the drive. <br />
        /// Can be 0 if not recognized by the OS
        /// </summary>
        public virtual ulong TotalFreeSpace { get => 0UL; }

        /// <summary>
        /// Gets the total size of the drive. <br />
        /// Can be 0 if not recognized by the OS
        /// </summary>
        public virtual ulong TotalSize { get => 0UL; }

        /// <summary>
        /// Gets the type of this drive. <br />
        /// Can be <see cref="DriveType.Unknown"/> if not recognized by the OS
        /// </summary>
        public virtual DriveType DriveType { get => DriveType.Unknown; }

        /// <summary>Gets a string that describes the current <see cref="Drive"/> object.</summary>
        /// <returns>A string describing this <see cref="Drive"/> object.</returns>
        public override string ToString() => Name;
    }
}