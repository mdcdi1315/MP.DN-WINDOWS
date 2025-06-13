

using MP.WindowsInterop;
using System;
using System.Runtime.InteropServices;

namespace MP.ComInterop
{
    /// <summary>
    /// Defines static methods for interoperating COM parts with .NET .
    /// </summary>
    public static unsafe class ComMarshalling
    {
        /// <summary>
        /// Frees a <see cref="PROPVARIANT"/> from a <see cref="System.IntPtr"/>.
        /// </summary>
        /// <param name="pvar">The <see cref="PROPVARIANT"/> to free.</param>
        public static void PropVariantClear(System.IntPtr pvar)
        {
            var ret = Interop.Ole32.PropVariantClear((PROPVARIANT*)pvar.ToPointer());
            if (ret.FAILED) { throw ret.MappingException; }
        }

        /// <summary>
        /// Dispose an array of property variants. <br />
        /// This method may fail. Catch the exception to see what went wrong.
        /// </summary>
        /// <param name="propvariants">The array of variants to dispose.</param>
        public static void Dispose(this PROPVARIANT[] propvariants)
        {
            ArgumentNullException.ThrowIfNull(propvariants, nameof(propvariants));
            HRESULT hr = Interop.Ole32.FreePropVariantArray(propvariants);
            if (hr.FAILED) { throw hr.MappingException; }
        }

        [System.Diagnostics.StackTraceHidden]
        private static void EnsureIsComImportType(System.Type t)
        {
            if (t is null) {
                throw new ArgumentNullException(nameof(t));
            }
            if (t.Attributes.HasFlag(System.Reflection.TypeAttributes.Import) == false) {
                throw new ArgumentException(InternalResources.MP_COMINTEROP_NOTCOMIMPORTTYPE);
            }
        }

        [System.Diagnostics.StackTraceHidden]
        private static void EnsureIsComInterfaceType(Type t)
        {
            if (t.Attributes.HasFlag(System.Reflection.TypeAttributes.Interface) == false) {
                throw new ArgumentException(InternalResources.MP_COMINTEROP_NOTCOMINTERFACE);
            }
        }

        [System.Diagnostics.StackTraceHidden]
        private static Guid GetDefaultCOMInterfaceObjectGuidAttributeData(System.Type t)
        {
            System.Type defciogattriute = typeof(Annotations.DefaultCOMInterfaceObjectGuidAttribute);
            System.Object[] d = t.GetCustomAttributes(defciogattriute, false);
            if (d.Length != 1) {
                throw new ArgumentException(InternalResources.MP_COMINTEROP_NOTMARKEDWITHDEFAULTCOMOBJECTATTRIBUTE);
            }
            Guid g;
            try {
                g = new(defciogattriute.GetProperty(nameof(Annotations.DefaultCOMInterfaceObjectGuidAttribute.ObjectIdentifier), System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance).GetValue(d[0], null) as System.String);
            } catch (Exception e) {
                throw new ArgumentException(InternalResources.MP_COMINTEROP_NOTMARKEDWITHDEFAULTCOMOBJECTATTRIBUTE, e);
            }
            return g;
        }

        [System.Diagnostics.StackTraceHidden]
        private static Guid GetComInterfaceIDInternal(System.Type t)
        {
            var guidattrtype = typeof(GuidAttribute);
            foreach (var attr in t.GetCustomAttributes(true))
            {
                if (attr.GetType() == guidattrtype) {
                    return new(guidattrtype.GetProperty(nameof(GuidAttribute.Value)).GetValue(attr) as System.String);
                }
            }
            throw new ArgumentException(InternalResources.MP_COMINTEROP_NOTCOMIMPORTTYPE);
        }

        /// <summary>
        /// Given a specified COM interface that is provided as a <see cref="Type"/> object, it returns it's GUID as a <see cref="Guid"/> instance.
        /// </summary>
        /// <param name="t">The COM interface to retrieve it's registration GUID.</param>
        /// <returns>The registration GUID from the contents of the given <see cref="Type"/> object in <paramref name="t"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="t"/> was <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="t"/> was not a COM Import interface.</exception>
        public static Guid GetComInterfaceID(Type t)
        {
            EnsureIsComImportType(t);
            EnsureIsComInterfaceType(t);
            return GetComInterfaceIDInternal(t);
        }

        /// <summary>
        /// Given a specified COM interface that is provided as a <see cref="Type"/> object, when that has correctly defined the <see cref="Annotations.DefaultCOMInterfaceObjectGuidAttribute"/>,
        /// it returns that GUID from the contents of that attribute.
        /// </summary>
        /// <param name="t">The COM interface to retrieve it's coclass GUID that can create a COM object that implements the interface.</param>
        /// <returns>The coclass GUID that can create a new instance of the interface represented by <paramref name="t"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="t"/> was <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="t"/> was not a COM Import interface. <br />
        /// -or- <br />
        /// <paramref name="t"/> did not properly defined the <see cref="Annotations.DefaultCOMInterfaceObjectGuidAttribute"/> <br />
        /// -or- <br />
        /// The defined <see cref="Annotations.DefaultCOMInterfaceObjectGuidAttribute"/> GUID is not in correct format.
        /// </exception>
        public static Guid GetComClassInstanceID(Type t)
        {
            EnsureIsComImportType(t);
            EnsureIsComInterfaceType(t);
            return GetDefaultCOMInterfaceObjectGuidAttributeData(t);
        }

        /// <summary>
        /// Does the same as <see cref="GetComInterfaceID(Type)"/>, but it can be used to statically retrieve the interface's registration GUID.
        /// </summary>
        /// <typeparam name="TINT">The COM interface to retrieve it's registration GUID.</typeparam>
        /// <returns>The registration GUID from the contents of the given <typeparamref name="TINT"/> type token.</returns>
        /// <exception cref="ArgumentException"><paramref name="t"/> was not a COM Import interface.</exception>
        public static Guid GetComInterfaceID<TINT>() => GetComInterfaceID(typeof(TINT));

        /// <summary>
        /// Does the same as <see cref="GetComClassInstanceID(Type)"/>, but it can be used to statically retrieve the coclass GUID.
        /// </summary>
        /// <typeparam name="TINT">The COM interface to retrieve it's coclass GUID that can create a COM object that implements the interface.</typeparam>
        /// <returns>The coclass GUID that can create a new instance of the interface represented by <typeparamref name="TINT"/>.</returns>
        /// <exception cref="ArgumentException">
        /// <typeparamref name="TINT"/> was not a COM Import interface. <br />
        /// -or- <br />
        /// <typeparamref name="TINT"/> did not properly defined the <see cref="Annotations.DefaultCOMInterfaceObjectGuidAttribute"/> <br />
        /// -or- <br />
        /// The defined <see cref="Annotations.DefaultCOMInterfaceObjectGuidAttribute"/> GUID is not in correct format.
        /// </exception>
        public static Guid GetComClassInstanceID<TINT>() => GetComClassInstanceID(typeof(TINT));

        /// <summary>
        /// Calls the native <strong>CoCreateInstance</strong> function to get a COM object of the specified implementation class.
        /// </summary>
        /// <typeparam name="TINT">The casted marshalled .NET object to return.</typeparam>
        /// <param name="classid">The COM CLSID of the class object.</param>
        /// <param name="interfaceid">The COM Interface ID that the class object implements and you wish to be returned.</param>
        /// <param name="context">The COM Server creation flags to pass.</param>
        /// <returns>The created COM object.</returns>
        public static TINT GetClassInstanceAsInterface<TINT>(System.Guid classid , System.Guid interfaceid , CLSCTX context)
        {
            void* comobj;
            Interop.GUID g1 = Interop.GUID.FromGUID(classid), g2 = Interop.GUID.FromGUID(interfaceid);
            var errorc = Interop.Ole32.CoCreateInstance(&g1, null, context, &g2, &comobj);
            if (errorc.FAILED) { throw errorc.MappingException; }
            return (TINT)CreateInteropObject(comobj, -1);
        }

        /// <summary>
        /// Calls the native <strong>CoCreateInstance</strong> function to get a COM object of the specified implementation class.
        /// </summary>
        /// <typeparam name="TINT">The casted marshalled .NET object to return.</typeparam>
        /// <param name="classid">The COM CLSID of the class object that implements the ID held by <typeparamref name="TINT"/>.</param>
        /// <param name="context">The COM Server creation flags to pass.</param>
        /// <returns>The created COM object.</returns>
        public static TINT GetClassInstanceAsInterface<TINT>(Guid classid , CLSCTX context)
        {
            Type tinterface = typeof(TINT); // This will either fill the 'tinterface' with the type data or will fail.
            EnsureIsComImportType(tinterface);
            EnsureIsComInterfaceType(tinterface);
            void* comobj;
            Interop.GUID g1 = Interop.GUID.FromGUID(classid);
            Interop.GUID g2 = Interop.GUID.FromGUID(GetComInterfaceIDInternal(tinterface));
            var errorc = Interop.Ole32.CoCreateInstance(&g1, null, context, &g2, &comobj);
            if (errorc.FAILED) { throw errorc.MappingException; }
            return (TINT)CreateInteropObject(comobj, -1);
        }

        /// <summary>
        /// Calls the native <strong>CoCreateInstance</strong> function to get a COM object of the specified interface, plus
        /// getting the class id to create the instance from the <see cref="Annotations.DefaultCOMInterfaceObjectGuidAttribute"/>.
        /// </summary>
        /// <typeparam name="TINT">The casted marshalled .NET object to return.</typeparam>
        /// <param name="context">The COM Server creation flags to pass.</param>
        /// <returns>The created COM object.</returns>
        /// <exception cref="InvalidOperationException">The interface passed in <typeparamref name="TINT"/> is not marked with <see cref="Annotations.DefaultCOMInterfaceObjectGuidAttribute"/>, or the GUID passed as it's argument is malformed.</exception>
        public static TINT GetClassInstanceAsInterface<TINT>(CLSCTX context)
        {
            Type tinterface = typeof(TINT); // This will either fill the 'tinterface' with the type data or will fail.
            EnsureIsComImportType(tinterface);
            EnsureIsComInterfaceType(tinterface);
            void* comobj;
            Interop.GUID g1 = Interop.GUID.FromGUID(GetDefaultCOMInterfaceObjectGuidAttributeData(tinterface));
            Interop.GUID g2 = Interop.GUID.FromGUID(GetComInterfaceIDInternal(tinterface));
            var errorc = Interop.Ole32.CoCreateInstance(&g1, null, context, &g2, &comobj);
            if (errorc.FAILED) { throw errorc.MappingException; }
            return (TINT)CreateInteropObject(comobj , -1);
        }

        public static HRESULT QueryInterface(void* pci, Guid iid, out void* ppv)
        {
            ArgumentNullException.ThrowIfNull(pci);

            void* ppvout;
            GUID guid = GUID.FromGUID(iid);
            HRESULT i = ((delegate* unmanaged<void*, GUID*, void**, HRESULT>)(*(*(void***)pci + 0 /* IUnknown.QueryInterface slot */)))(pci, &guid, &ppvout);
            ppv = ppvout;
            return i;
        }

        public static System.UInt32 AddRef(void* pci)
        {
            ArgumentNullException.ThrowIfNull(pci);

            return ((delegate* unmanaged<void*, System.UInt32>)(*(*(void***)pci + 1 /* IUnknown.AddRef slot */)))(pci);
        }

        public static System.UInt32 Release(void* pUnk)
        {
            ArgumentNullException.ThrowIfNull(pUnk);

            return ((delegate* unmanaged<void*, System.UInt32>)(*(*(void***)pUnk + 2 /* IUnknown.Release slot */)))(pUnk);
        }

        public static void* GetComFunctionPointer(void* punk, System.UInt32 ordinal)
        {
            ArgumentNullException.ThrowIfNull(punk);
            if (ordinal < 0) {
                throw new ArgumentOutOfRangeException(nameof(ordinal) , "ordinal must not be negative.");
            }

            return *(*(void***)punk + ordinal /* Any interface slot */);
        }

        /// <summary>
        /// Works exactly the same as <see cref="Marshal.GetObjectForIUnknown"/> but it rectifies the object to set it to the desired reference count,
        /// because the Marshal API can emit a lot of reference counts.
        /// </summary>
        /// <param name="punk">The pointer to the IUnknown interface.</param>
        /// <param name="desiredrefcount">The desired reference count.</param>
        /// <returns>The interop object.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="punk"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="desiredrefcount"/> is less than 1.</exception>
        public static System.Object CreateInteropObject(System.IntPtr pUnk , System.Int32 desiredrefcount = 1)
            => CreateInteropObject(pUnk.ToPointer() , desiredrefcount);

        /// <summary>
        /// Works exactly the same as <see cref="Marshal.GetObjectForIUnknown"/> but it rectifies the object to set it to the desired reference count,
        /// because the Marshal API can emit a lot of reference counts.
        /// </summary>
        /// <param name="punk">The pointer to the IUnknown interface.</param>
        /// <param name="desiredrefcount">The desired reference count.</param>
        /// <returns>The interop object.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="punk"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="desiredrefcount"/> is less than 1.</exception>
        public static System.Object CreateInteropObject(void* punk , System.Int32 desiredrefcount = 1)
        {
            ArgumentNullException.ThrowIfNull(punk);
            if (desiredrefcount < 1 && desiredrefcount != -1) {
                throw new ArgumentOutOfRangeException(nameof(desiredrefcount), "The finally desired reference count must be more or equal to 1!!!");
            }
            System.IntPtr ptr = new(punk);
            System.Object obj = Marshal.GetObjectForIUnknown(ptr);
            if (desiredrefcount == -1) {
                DebugProvider.WriteLine($"ObjectGraphTracker: User requested for COM object {ptr} and hash code 0x{obj.GetHashCode():x2} to NOT fix it up. This might cause issues if this call is a result of a QueryInterface returning a different COM object.");
                return obj;
            }
            // Just AddRef it once to learn it's reference count - so that to make it to become into the desired reference count.
            System.UInt32 rfc = AddRef(punk); 
            DebugProvider.WriteLine($"ObjectGraphTracker: COM object {ptr} has now a reference count of {rfc}.");
            if (rfc > desiredrefcount)
            {
#if DEBUG
                System.UInt32 referencec;
                do {
                    referencec = Release(punk);
                    DebugProvider.WriteLine($"ObjectGraphTracker: Reference count for COM object {ptr} became {referencec} .");
                } while (referencec > desiredrefcount);
#else
                while (Release(punk) > desiredrefcount) ;
#endif
            }
            DebugProvider.WriteLine($"ObjectGraphTracker: COM object {ptr} was correctly rectified to the requested value. It's managed hash code is 0x{obj.GetHashCode():x2}.");
            return obj;
        }

        /// <summary>
        /// Properly releases an interop object created with the <see cref="CreateInteropObject"/> method.
        /// </summary>
        /// <param name="obj">The object to be released.</param>
        public static void ReleaseInteropObject(System.Object obj) 
        {
            if (obj is null) { return; }
            DebugProvider.WriteLine($"ObjectGraphTracker: Attempting to release COM object with hash code 0x{obj.GetHashCode():x2}.");
            System.Int32 rfc = Marshal.ReleaseComObject(obj);
            DebugProvider.WriteLine($"ObjectGraphTracker: Released COM Object with hash code 0x{obj.GetHashCode():x2}. The object has now a reference count of {rfc}.");
        }

        /// <summary>
        /// Initializes combase.dll on the current thread.
        /// The thread will enter a multi-threaded apartment.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.DoesNotReturn]
        [Obsolete("Thread API's call this method implicitly. Thus, this method will always fail.")]
        public static void InitializeCOMLibraryMTAMode()
        {
            var err = Interop.Ole32.CoInitializeEx(null, Interop.Ole32.COINIT.COINIT_MULTITHREADED);
            if (err.FAILED) { throw err.MappingException; }
        }

        /// <summary>
        /// Initializes combase.dll on the current thread.
        /// The thread will enter a single-threaded apartment.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.DoesNotReturn]
        [Obsolete("Thread API's call this method implicitly. Thus, this method will always fail.")]
        public static void InitializeCOMLibrarySTAMode()
        {
            var err = Interop.Ole32.CoInitializeEx(null, Interop.Ole32.COINIT.COINIT_APARTMENTTHREADED);
            if (err.FAILED) { throw err.MappingException; }
        }
    }
}