using System.Threading;
using System.Runtime.CompilerServices;

namespace MP
{
    /// <summary>
    /// Marker interface for classes that their instances are always thread-safe.
    /// </summary>
    public interface ISyncronized { }

    /// <summary>
    /// Helpers around the <see cref="ISyncronized"/> and <see cref="ISyncronizedByObject"/> interfaces.
    /// </summary>
    public static class ISyncronizedExtensions
    {
        /// <summary>Gets a value whether the type <typeparamref name="T"/> is thread-safe.</summary>
        /// <remarks>
        /// Use this method when you are unsure of the nature of your incoming objects and you need to implement thread safety on your class. <br />
        /// It helps to efficiently choose different code paths at run-time. <br />
        /// Note also that this won't appropriately work on <see langword="null"/> objects.
        /// </remarks>
        /// <typeparam name="T">The type to compare thread-safety against.</typeparam>
        /// <param name="obj">The object to test whether it is a thread-safe object.</param>
        /// <returns><see langword="true"/> if <paramref name="obj"/> implements the <see cref="ISyncronized"/> interface; otherwise, <see langword="false"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)] // Typically, inlining this method should help.
        public static bool IsSyncronizedObject<T>(this T obj) => obj is ISyncronized;

        /// <summary>
        /// Ensures that the state of <see cref="ISyncronizedByObject.SyncObject"/> is not corrupted while getting it. <br />
        /// To achieve that, a <see cref="Interlocked.MemoryBarrier"/> is inserted. <br />
        /// Corruption of syncronization objects can be done for a number of reasons: <br />
        /// <list type="bullet">
        /// <item>
        ///     The object also implements the <see cref="System.IDisposable"/> interface and it is in the process of disposing itself.
        /// </item>
        /// <item>
        ///     The object's class may have assigned the <see langword="this"/> reference as <see cref="ISyncronizedByObject.SyncObject"/>.
        /// </item>
        /// </list>
        /// </summary>
        /// <param name="s">The object that implements the <see cref="ISyncronizedByObject"/> interface.</param>
        /// <returns>The value of <see cref="ISyncronizedByObject.SyncObject"/> property.</returns>
        public static object GetSyncObjectSafe(this ISyncronizedByObject s)
        {
            Interlocked.MemoryBarrier();
            return s.SyncObject;
        }
    }
}
