
using System;
using MP.Utilities;
using MP.Annotations;
using System.Reflection;
using MP.Annotations.CodeAnalysis;
using System.Diagnostics.CodeAnalysis;

namespace MP.NativeInterop.Windows.COM
{
    /// <summary>
    /// Provides base services for all COM objects created through the Abstractions Library.
    /// </summary>
    public unsafe static class ComInterop
    {
        /// <summary>Decrements the reference count on the given COM object.</summary>
        /// <param name="p_interface">The interface object to decrement the reference count for.</param>
        /// <returns>The new reference count, after one reference is removed.</returns>
        [MustNotReportException]
        public static System.UInt32 Release([AllowNull] void* p_interface) => p_interface is null ? 0U : MPComWrappersSubsystem.Release(p_interface);

        /// <summary>
        /// Decrements the reference count on the given COM object, <br />
        /// if the object implements the <see cref="INativeCOMObject"/> interface.
        /// </summary>
        /// <param name="o">The interface object to decrement the reference count for.</param>
        /// <returns>The new reference count, after one reference is removed.</returns>
        [MustNotReportException]
        public static System.UInt32 Release(System.Object o) => o is INativeCOMObject n ? MPComWrappersSubsystem.Release(n.Native) : 0U;

        /// <summary>
        /// Increments the reference count on the given COM object.
        /// </summary>
        /// <param name="p_interface">The interface object to increment the reference count for.</param>
        /// <returns>The new reference count, after one reference is added.</returns>
        [MustNotReportException]
        public static System.UInt32 AddRef([AllowNull] void* p_interface) => p_interface is null ? 0U : MPComWrappersSubsystem.AddRef(p_interface);

        /// <summary>
        /// Increments the reference count on the given COM object, <br />
        /// if the object implements the <see cref="INativeCOMObject"/> interface.
        /// </summary>
        /// <param name="o">The interface object to increment the reference count for.</param>
        /// <returns>The new reference count, after one reference is added.</returns>
        [MustNotReportException]
        public static System.UInt32 AddRef(System.Object o) => o is INativeCOMObject n ? MPComWrappersSubsystem.AddRef(n.Native) : 0U;

        /// <summary>
        /// Queries whether the given COM object implements the specified interface.
        /// </summary>
        /// <param name="p_interface">The input reference to the COM object to query.</param>
        /// <param name="guid">The GUID of the interface to be queried.</param>
        /// <param name="p_queried">On success, provides the queried interface pointer on return.</param>
        /// <returns>An <see cref="HRESULT"/> error code indicating success or failure.</returns>
        [MustNotReportException]
        public static HRESULT QueryInterface([AllowNull] void* p_interface, Guid guid, out void* p_queried)
        {
            if (p_interface is null) {
                p_queried = null;
                return CommonHResults.E_INVALIDARG;
            } else {
                return MPComWrappersSubsystem.QueryInterface(p_interface, guid, out p_queried);
            }
        }

        /// <summary>
        /// Gets the ID for a COM interface. <br />
        /// Returns <see langword="false"/> if the given interface is not a proper COM interface.
        /// </summary>
        /// <typeparam name="TI">The type of the interface to be examined for it's ID.</typeparam>
        /// <param name="guid">The ID of the given interface.</param>
        /// <returns>A value whether interface ID retrieval was successful.</returns>
        [SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "This implementation could be used anywhere COM is used, however it is meaningful only on Windows environments.")]
        public static bool GetComInterfaceID<TI>(out Guid guid)
            where TI : class
        {
            var c = typeof(TI).GetCustomAttribute<COMInterfaceAttribute>();
            if (c is null) {
                guid = default;
                return false;
            } else {
                guid = c.GUID;
                return true;
            }
        }

        /// <summary>Queries an interface on the COM object <paramref name="o"/>.</summary>
        /// <typeparam name="TI">The type of the COM interface to be queried.</typeparam>
        /// <param name="o">The COM object from where to query the interface.</param>
        /// <returns>The queried interface type, if available; otherwise, <see langword="null"/>.</returns>
        public static TI QueryInterface<TI>(System.Object o)
            where TI : class
        {
            // It is possible that a base interface was requested from the COM object, which the current RCW will possibly support.
            // We will return that instead if we find it.
            if (o is TI t) {
                return t;
            } else if (o is INativeCOMObject nc) {
                if (GetComInterfaceID<TI>(out var g)) {
                    if (QueryInterface(nc.Native, g, out void* p) == CommonHResults.S_OK) {
                        return CreateRCW<TI>(p);
                    } else {
                        return null;
                    }
                } else {
                    return null;
                }
            } else {
                return null;
            }
        }

        /// <summary>Queries an interface on the COM object <paramref name="o"/>.</summary>
        /// <typeparam name="TI">The type of the COM interface to be queried.</typeparam>
        /// <param name="o">The COM object from where to query the interface.</param>
        /// <param name="interface_inst">The queried interface type, if available; otherwise, <see langword="null"/>.</param>
        /// <returns>An <see cref="HRESULT"/> code indicating whether the operation succeeded or not.</returns>
        public static HRESULT QueryInterface<TI>(System.Object o, out TI interface_inst)
            where TI : class
        {
            // It is possible that a base interface was requested from the COM object, which the current RCW will possibly support.
            // We will return that instead if we find it.
            if (o is TI t) {
                interface_inst = t;
                return CommonHResults.S_OK;
            } else if (o is INativeCOMObject nc) {
                if (GetComInterfaceID<TI>(out var g)) {
                    HRESULT hr = QueryInterface(nc.Native, g, out void* pp);
                    if (hr.FAILED) {
                        interface_inst = null;
                    } else {
                        interface_inst = CreateRCW<TI>(pp);
                    }
                    return hr;
                } else {
                    interface_inst = null;
                    return CommonHResults.E_POINTER;
                }
            } else {
                interface_inst = null;
                return CommonHResults.E_POINTER;
            }
        }

        /// <summary>
        /// Attempts to create an RCW for the interface type specified through the <typeparamref name="TInterface"/> type.
        /// </summary>
        /// <typeparam name="TInterface">The interface type the COM object mainly implements.</typeparam>
        /// <param name="p_native">The native pointer to create an RCW for. This must be a valid COM object.</param>
        /// <param name="wrapper">The wrapper RCW object.</param>
        /// <returns>A value whether RCW creation succeeded or not.</returns>
        [MustNotReportException]
        public static bool TryCreateRCW<TInterface>([AllowNull] void* p_native, [NotNullIfNotNull(nameof(p_native))] [NotNullWhen(true)] out TInterface wrapper)
            where TInterface : class
        {
            if (p_native is null) {
                wrapper = null;
                return false;
            } else {
                try {
                    wrapper = MPComWrappersSubsystem.CreateNativeObjectFor(p_native, typeof(TInterface)).Cast<TInterface>();
                    return true;
                } catch (InvalidOperationException) {
                    wrapper = null;
                    return false;
                } catch (MissingMethodException) {
                    wrapper = null;
                    return false;
                }
            }
        }

        /// <summary>
        /// Creates an RCW for the interface type specified through the <typeparamref name="TInterface"/> type.
        /// </summary>
        /// <typeparam name="TInterface">The interface type the COM object mainly implements.</typeparam>
        /// <param name="p_native">The native pointer to create an RCW for. This must be a valid COM object.</param>
        /// <returns>The wrapper RCW object, cast to <typeparamref name="TInterface"/>.</returns>
        [return: NotNull]
        [Throws(typeof(InvalidOperationException), typeof(MissingMethodException), typeof(ArgumentNullException))]
        public static TInterface CreateRCW<TInterface>(void* p_native)
            where TInterface : class
        {
            ArgumentNullException.ThrowIfNull(p_native);
            return MPComWrappersSubsystem.CreateNativeObjectFor(p_native, typeof(TInterface)).Cast<TInterface>();
        }

        private static Type SelectRandomInterface(System.Object com_obj)
        {
            ArgumentNullException.ThrowIfNull(com_obj);
            foreach (Type _interface_ in com_obj.GetType().GetInterfaces())
            {
                if (_interface_.HasAttribute(typeof(COMInterfaceAttribute))) { return _interface_; }
            }
            throw new InvalidOperationException("The given object does not implement any COM interfaces and as such, a CCW cannot be created for it.");
        }

        /// <summary>
        /// Creates a CCW for the given managed object.
        /// </summary>
        /// <param name="com_obj">The managed object to be translated as a COM object. The object must implement an exportable, in COM terms, interface.</param>
        /// <returns>The CCW for the <paramref name="com_obj"/>.</returns>
        [return: NotNull]
        [Throws(typeof(InsufficientMemoryException), typeof(ArgumentNullException), typeof(InvalidOperationException))]
        public static void* CreateCCW(System.Object com_obj) => MPComWrappersSubsystem.CreateCCW(com_obj, SelectRandomInterface(com_obj));

        /// <summary>
        /// Creates a CCW for the given managed object.
        /// </summary>
        /// <param name="com_obj">The managed object to be translated as a COM object. The object must implement an exportable, in COM terms, interface.</param>
        /// <returns>The CCW for the <paramref name="com_obj"/>.</returns>
        /// <typeparam name="TInterface">The COM interface that is expected by the native signature.</typeparam>
        [return: NotNull]
        [Throws(typeof(InsufficientMemoryException), typeof(ArgumentNullException), typeof(InvalidOperationException))]
        public static void* CreateCCW<TInterface>(System.Object com_obj) where TInterface : class => MPComWrappersSubsystem.CreateCCW(com_obj , typeof(TInterface));

    }
}