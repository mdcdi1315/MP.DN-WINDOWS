using System;

namespace MP.Annotations
{
    /// <summary>
    /// Specifications of this attribute specify that the P/Invoke method or any native method call (through Windows COM, for example) does assign the thread's last error code. <br />
    /// While this is most relevant in Windows with the <see href="https://learn.microsoft.com/en-us/windows/win32/api/errhandlingapi/nf-errhandlingapi-setlasterror">SetLastError</see> function, in OSX we have the ERRNO codes which are similar to this pattern.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class AssignsLastErrorAttribute : Attribute { }
}
