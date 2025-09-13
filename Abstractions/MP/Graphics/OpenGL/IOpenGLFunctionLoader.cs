
using System;
using MP.Annotations.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Reflection;

namespace MP.Graphics.OpenGL
{
    /// <summary>
    /// Defines the object responsible for loading all the OpenGL functions.
    /// </summary>
    public interface IOpenGLFunctionLoader
    {
        /// <summary>
        /// Gets a function to be loaded from the current OpenGL context.
        /// </summary>
        /// <typeparam name="T">The type of the .NET delegate to translate the specified OpenGL function.</typeparam>
        /// <returns>The translated OpenGL function delegate.</returns>
        /// <exception cref="UnloadableOpenGLFunctionException">The specified function could not be found.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="function"/> was <see langword="null"/>.</exception>
        /// <exception cref="MarshalDirectiveException">The <typeparamref name="T"/> delegate type is invalid for native marshalling.</exception>
        [Throws(
            typeof(ArgumentNullException) , 
            typeof(UnloadableOpenGLFunctionException) ,
            typeof(MarshalDirectiveException)
        )]
        public T GetFunction<T>(String function)
            where T : Delegate;
    }

    /// <summary>
    /// Extension methods for the <see cref="IOpenGLFunctionLoader"/> interface.
    /// </summary>
    internal static class OpenGLFunctionLoaderExtensions
    {
        private static readonly MethodInfo getfunctionmi;

        static OpenGLFunctionLoaderExtensions() {
            getfunctionmi = typeof(IOpenGLFunctionLoader).GetMethod(nameof(IOpenGLFunctionLoader.GetFunction) , BindingFlags.Public | BindingFlags.Instance , new[] { typeof(System.String) });
        }

        public static System.Object GetFunction(this IOpenGLFunctionLoader loader , String function , Type delegatetype)
            => getfunctionmi.MakeGenericMethod(delegatetype).Invoke(loader, new object[] { function });
    }
}
