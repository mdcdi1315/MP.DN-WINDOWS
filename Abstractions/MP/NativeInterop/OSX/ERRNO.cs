

namespace MP.NativeInterop.OSX
{
    /// <summary>Defines OSX kernel errno codes.</summary>
    public enum ERRNO : System.Int32
    {
        /// <summary></summary>
        None = 0,
        /// <summary>Operation not permitted.</summary>
        EPERM,
        /// <summary>No such file or directory.</summary>
        ENOENT,
        /// <summary>No such process.</summary>
        ESRCH,
        /// <summary>Interrupted system call.</summary>
        EINTR,
        /// <summary>I/O error occured.</summary>
        EIO,
        /// <summary>Device not configured.</summary>
        ENXIO,
        /// <summary>Argument list too long</summary>
        E2BIG,
        /// <summary>Exec format error</summary>
        ENOEXEC,
        /// <summary>Bad file descriptor</summary>
        EBADF,
        /// <summary>No child processes.</summary>
        ECHILD,
        /// <summary>Resource deadlock avoided</summary>
        EDEADLK,
        /// <summary>Cannot allocate memory.</summary>
        ENOMEM,
        /// <summary>Permission denied</summary>
        EACCES,
        /// <summary>Bad address</summary>
        EFAULT,
        /// <summary>Block device required</summary>
        ENOTBLK,
        /// <summary>Device / Resource busy</summary>
        EBUSY,
        /// <summary>File exists</summary>
        EEXIST,
        /// <summary>Cross-device link</summary>
        EXDEV,
        /// <summary>Operation not supported by device</summary>
        ENODEV,
        /// <summary>Not a directory</summary>
        ENOTDIR,
        /// <summary>Is a directory</summary>
        EISDIR,
        /// <summary>Invalid argument </summary>
        EINVAL,
        /// <summary>Too many open files in system</summary>
        ENFILE,
        /// <summary></summary>
        EMFILE,
        /// <summary>Inappropriate ioctl for device </summary>
        ENOTTY,
        /// <summary>Text file busy</summary>
        ETXTBSY, 
        /// <summary>File too large</summary>
        EFBIG,
        /// <summary>No space left on device</summary>
        ENOSPC,
        /// <summary>Illegal seek</summary>
        ESPIPE,
        /// <summary>Read-only file system</summary>
        EROFS,
        /// <summary>Too many links</summary>
        EMLINK,                        
        /// <summary>Broken pipe</summary>
        EPIPE,
        /// <summary>Numerical argument out of domain</summary>
        EDOM,
        /// <summary>Result too large</summary>
        ERANGE,                      
    }
}