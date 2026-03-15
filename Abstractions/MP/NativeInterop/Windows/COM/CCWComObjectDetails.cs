
using System;
using MP.Annotations;
using System.Threading;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Diagnostics.CodeAnalysis;

namespace MP.NativeInterop.Windows.COM
{
    /// <summary>
    /// Provides the storage data required to expose a .NET object that implements COM interfaces to COM. <br />
    /// Publicly exposed for the CCW wrappers and the source generators that depend on this.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct CCWComObjectDetails
    {
        private readonly IUnknownVirtualTable* Table; // Virtual table of the COM interface - IUnknown members (that are implemented below) are always first.

        // GCHandle pointer to the managed object, allocated with GCHandleType.Normal.
        private readonly IntPtr GCObjectReference;

        private volatile System.UInt32 NativeReferenceCount; // Ref count for the object - managed by the IUnknown implementations

        private readonly GCHandle GetAttachedObjectGCHandle() => GCHandle.FromIntPtr(GCObjectReference);

        /// <summary>
        /// Creates a new instance of the <see cref="CCWComObjectDetails"/> structure.
        /// </summary>
        /// <param name="managed">The managed object to wrap.</param>
        /// <param name="selected_interface">The interface to wrap. Can be <see langword="null"/>.</param>
        public CCWComObjectDetails(System.Object managed, [AllowNull] Type selected_interface)
        {
            ArgumentNullException.ThrowIfNull(managed);
            GCObjectReference = GCHandle.ToIntPtr(GCHandle.Alloc(managed, GCHandleType.Normal));
            int vtable_size = sizeof(IUnknownVirtualTable);
            if (selected_interface is null) {
                Table = (IUnknownVirtualTable*)MPComWrappersSubsystem.GetComAllocatorOrDefault().Allocate(vtable_size);
            } else {
                VirtualTableBuilder builder = MPComWrappersSubsystem.GetVirtualTableBuilderFor(selected_interface);
                Table = (IUnknownVirtualTable*)MPComWrappersSubsystem.GetComAllocatorOrDefault().Allocate(builder.NativeSize + vtable_size);
                builder.FillVirtualTable(((byte*)Table) + vtable_size);
            }
            Table->Release = &ReleaseImpl;
            Table->AddRef = &AddReferenceImpl;
            Table->QueryInterface = &QueryInterfaceImpl;
            NativeReferenceCount = 1U;
        }

        /// <summary>
        /// Gets the reference count of this COM object, as COM sees this object.
        /// </summary>
        public readonly System.UInt32 ReferenceCount => NativeReferenceCount;

        /// <summary>Gets a function pointer from the virtual table of this COM object.</summary>
        /// <param name="ordinal">The ordinal to load.</param>
        /// <returns>The function pointer for the specified ordinal. First three ordinals are occupied by IUnknown.</returns>
        [return: NotNull]
        public readonly void* GetFunctionPointer(int ordinal) => *(((void**)Table) + ordinal);

        /// <summary>Gets the managed object that is proxied to COM.</summary>
        /// <returns>The object that is wrapped. It is retrieved through a GCHandle reference.</returns>
        public readonly System.Object GetAttachedObject() => GetAttachedObjectGCHandle().Target;

        /// <summary>
        /// Gets an implemented interface on the managed object referenced and returned through the <see cref="GetAttachedObject"/> method.
        /// </summary>
        /// <typeparam name="TI">The interface type to return.</typeparam>
        /// <returns>An instance of the specified COM interface.</returns>
        public readonly TI GetInterface<TI>() => (TI)GetAttachedObject();

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
        [SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "This implementation could be used anywhere COM is used, however it is meaningful only on Windows environments.")]
        private static HRESULT QueryInterfaceImpl(void* pointer, GUID* p_guid, void** p_interface)
        {
            // Just make sure that the arguments are properly validated.
            if (p_guid is null || p_interface is null) { return CommonHResults.E_INVALIDARG; }
            Guid iid = p_guid->GetGuid();
            DebugProvider.WriteLine($"COM_CCW_WRAPPER: [INFO] QueryInterface requested for object {new IntPtr(pointer)}, GUID: {iid}");
            var obj = ((CCWComObjectDetails*)pointer)->GetAttachedObject();
            COMInterfaceAttribute cia;
            foreach (Type interface_com in obj.GetType().GetInterfaces())
            {
                if ((cia = interface_com.GetCustomAttribute<COMInterfaceAttribute>()) is not null && iid.Equals(cia.GUID))
                {
                    *p_interface = MPComWrappersSubsystem.CreateCCW(obj , interface_com);
                    DebugProvider.WriteLine($"COM_CCW_WRAPPER: [INFO] QueryInterface call succeeded for object {new IntPtr(pointer)}, returned object {new IntPtr(*p_interface)}.");
                    return CommonHResults.S_OK;
                }
            }
            DebugProvider.WriteLine($"COM_CCW_WRAPPER: [INFO] QueryInterface call failed for object {new IntPtr(pointer)}, no object was found for GUID {iid}.");
            return CommonHResults.E_NOINTERFACE;
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
        private static System.UInt32 ReleaseImpl(void* pointer)
        {
            System.Object mgd_ref;
            CCWComObjectDetails* details = (CCWComObjectDetails*)pointer;
            GCHandle handle = details->GetAttachedObjectGCHandle();
            Monitor.Enter(mgd_ref = handle.Target);
            try {
                if (details->NativeReferenceCount == 0U) {
                    return 0U;
                } else {
                    uint new_value = unchecked(details->NativeReferenceCount - 1U);
                    if ((details->NativeReferenceCount = new_value) == 0U)
                    {
                        handle.Free();
                        MPComWrappersSubsystem.GetComAllocatorOrDefault().Free(details->Table);
                        MPComWrappersSubsystem.DestroyCCW(details);
                    }
                    return new_value;
                }
            } finally {
                Monitor.Exit(mgd_ref);
            }
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
        private static System.UInt32 AddReferenceImpl(void* pointer)
        {
            CCWComObjectDetails* details = (CCWComObjectDetails*)pointer;
            object obj = details->GetAttachedObject();
            Monitor.Enter(obj);
            try {
                if (details->NativeReferenceCount == System.UInt32.MaxValue) {
                    DebugProvider.WriteLine($"COM_CCW_WRAPPER: [WARNING] Reference count for object {new IntPtr(pointer)} reached it's maximum value! This might be a programming mistake that should be taken into account.");
                    return System.UInt32.MaxValue;
                } else {
                    uint new_value = unchecked(details->NativeReferenceCount + 1U);
                    return details->NativeReferenceCount = new_value;
                }
            } finally {
                Monitor.Exit(obj);
            }
        }
    }
}