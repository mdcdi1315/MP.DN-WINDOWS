

using System;
using System.Diagnostics.CodeAnalysis;
using MP.Annotations.CodeAnalysis;

namespace MP.SettingsTree
{
    /// <summary>
    /// An interface for providing to a settings tree builder string and image resources.
    /// </summary>
    public interface IResourcesProvider : IDisposable
    {
        /// <summary>
        /// Attempts to get an image resource with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the image resource.</param>
        /// <param name="image">The image bytes.</param>
        /// <returns>A value whether lookup succeeded or not.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="id"/> was <see langword="null"/>.</exception>
        [Throws(typeof(ArgumentNullException))]
        public System.Boolean TryGetImageResource(System.String id, [NotNullWhen(true)] out System.Byte[] image);

        /// <summary>
        /// Attempts to get a string resource with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the string resource.</param>
        /// <param name="resource">The string resource value.</param>
        /// <returns>A value whether lookup succeeded or not.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="id"/> was <see langword="null"/>.</exception>
        [Throws(typeof(ArgumentNullException))]
        public System.Boolean TryGetStringResource(System.String id, [NotNullWhen(true)] out System.String resource);
    }

    /// <summary>
    /// Provides extensions around the <see cref="IResourcesProvider"/> interface.
    /// </summary>
    public static class IResourcesProviderExtensions
    {
        /// <summary> 
        /// Get an image resource with the specified ID.
        /// </summary>
        /// <param name="provider">The resources provider to retrieve the image resource from.</param>
        /// <param name="id">The ID of the image resource.</param>
        /// <returns>The image bytes , if lookup was successfull.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="id"/> was <see langword="null"/>.</exception>
        /// <exception cref="ResourceNotFoundException">The contents of the <paramref name="id"/> parameter are not a valid resource ID for the current instance.</exception>
        [Throws(typeof(ArgumentNullException) , typeof(ResourceNotFoundException))]
        public static System.Byte[] GetImageResource(this IResourcesProvider provider, System.String id)
        {
            if (provider.TryGetImageResource(id, out System.Byte[] ret))
            {
                return ret;
            }
            throw new ResourceNotFoundException(id);
        }

        /// <summary>
        /// Gets a string resource with the specified ID.
        /// </summary>
        /// <param name="provider">The resources provider to retrieve the string resource from.</param>
        /// <param name="id">The ID of the desired string resource.</param>
        /// <returns>The string resource value, if lookup was successfull.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="id"/> was <see langword="null"/>.</exception>
        /// <exception cref="ResourceNotFoundException">The contents of the <paramref name="id"/> parameter are not a valid resource ID for the current instance.</exception>
        [Throws(typeof(ArgumentNullException) , typeof(ResourceNotFoundException))]
        public static System.String GetStringResource(this IResourcesProvider provider, System.String id)
        {
            if (provider.TryGetStringResource(id, out System.String str))
            {
                return str;
            }
            throw new ResourceNotFoundException(id);
        }
    }
}