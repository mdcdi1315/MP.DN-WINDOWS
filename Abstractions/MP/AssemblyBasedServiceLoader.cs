
using System;
using MP.Utilities;
using MP.Annotations;
using MP.Collections;
using System.Reflection;
using System.Collections.Generic;
using MP.Annotations.CodeAnalysis;
using System.Diagnostics.CodeAnalysis;

namespace MP
{   
    /// <summary>
    /// Provides an <see cref="ServiceLoader{T}"/> implementation that is able to load services from .NET assemblies. <br />
    /// It is currently the most easy and practical way to load services.
    /// </summary>
    /// <typeparam name="T">The type of the service to be loaded.</typeparam>
    [DoesNotSupportDisposeAllExtensionMethod]
    public sealed class AssemblyBasedServiceLoader<T> : ServiceLoader<T>
        where T : IService
    {
        private ArrayBasedList<T> service_objects;
        private ArrayBasedList<Assembly> assemblies;

        /// <summary>
        /// Initializes a new 
        /// </summary>
        public AssemblyBasedServiceLoader()
        {
            assemblies = new();
            service_objects = new();
            assemblies.AddRange(AppDomain.CurrentDomain.GetAssemblies());
        }

        /// <summary>
        /// Adds an assembly to be searched by the service loader.
        /// </summary>
        /// <param name="a">The assembly object to be searched once the <see cref="ServiceLoader{T}.Load"/> method is called.</param>
        [Throws(typeof(ArgumentNullException))]
        public void AddAssemblyToSearch(Assembly a)
        {
            ArgumentNullException.ThrowIfNull(a);
            if (IsLoaded) {
                throw new InvalidOperationException("Assemblies cannot be added once the service loader has loaded all the services.");
            }
            assemblies.Add(a);
        }

        /// <inheritdoc />
        [return: NotNull]
        protected override IEnumerator<T> GetEnumeratorImpl() => service_objects.GetEnumerator();

        /// <inheritdoc />
        protected override void LoadImpl()
        {
            Type t;
            Attribute temp_instance;
            foreach (Assembly a in assemblies)
            {
                temp_instance = a.GetCustomAttribute(typeof(ServiceLoaderServiceAttribute<>));
                if (temp_instance is not null && (t = temp_instance.GetType().GenericTypeArguments[0]).ImplementsInterface<IService>())
                {
                    try {
                        service_objects.Add((T)Activator.CreateInstance(t));
                    } catch (Exception ex) {
                        DebugProvider.WriteLine($"SERVICELOADER: Exception occurred while creating service of type {t.FullName}: \n{ex}\nThis object will not be included in the loaded services list.");
                    }
                }
            }
            service_objects.TrimExcess();
        }

        /// <inheritdoc />
        protected override void Dispose(bool finalize)
        {
            base.Dispose(finalize);
            assemblies = null;
            service_objects = null;
        }
    }
}