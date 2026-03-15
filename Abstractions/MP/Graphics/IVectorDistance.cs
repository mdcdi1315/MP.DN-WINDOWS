
using System.Numerics;

namespace MP.Graphics
{
    /// <summary>
    /// Provides an interface for vectors to implement their distancing functionality.
    /// </summary>
    /// <typeparam name="TSelf">The type of the vector to compute it's distance.</typeparam>
    /// <typeparam name="TResult">The result value of the distance.</typeparam>
    public interface IVectorDistance<TSelf, TResult>
        where TResult : struct, INumber<TResult>
        where TSelf : IVectorDistance<TSelf, TResult>
    {
        /// <summary>Computes the absolute distance of the specified vector.</summary>
        /// <param name="self">The vector to compute it's distance.</param>
        /// <returns>The absolute distance (displacement) of <paramref name="self"/> from it's starting point.</returns>
        public static abstract TResult GetDistance(TSelf self);
    }
}