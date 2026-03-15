

using System;
using MP.Utilities;
using System.Collections.Generic;

namespace MP.SettingsTree
{
    /// <summary>
    /// Defines a <see cref="IDynamicValueListProvider{TSetting}"/> that can map between another dynamic value list provider. <br />
    /// Mostly defined for compatibility with settings that were indirectly defined as other primitive types.
    /// </summary>
    /// <typeparam name="TOld">The actual type of the value list provider.</typeparam>
    /// <typeparam name="TSetting">The mapped and actual setting type of the underlying setting type.</typeparam>
    public abstract class DynamicValueListProviderMapper<TOld, TSetting> : IDynamicValueListProvider<TSetting>
    {
        private TOld temp;
        private System.Boolean disposeifidisposable;
        private IDynamicValueListProvider<TOld> wrapping;

        /// <summary>
        /// Creates a new mapping value list provider, specifying the actual provider to wrap and a boolean specifying to call <see cref="IDisposable.Dispose"/> if <typeparamref name="TOld"/> implements <see cref="IDisposable"/>.
        /// </summary>
        /// <param name="wrapping">The actual provider to wrap. Must not be <see langword="null"/>.</param>
        /// <param name="calldisposeifidisposable">A value whether the mapper should call <see cref="IDisposable.Dispose"/> after an element is processed.</param>
        protected DynamicValueListProviderMapper(IDynamicValueListProvider<TOld> wrapping, System.Boolean calldisposeifidisposable)
        {
            ArgumentNullException.ThrowIfNull(wrapping);
            this.wrapping = wrapping;
            disposeifidisposable = calldisposeifidisposable;
        }

        /// <summary>
        /// Defines the function that can transform a single element of the <typeparamref name="TOld"/> type to the <typeparamref name="TSetting"/> type.
        /// </summary>
        public abstract Func<TOld, TSetting> Mapper { get; }

        /// <summary>
        /// Optional property. <br />
        /// Indicates the number of elements that the return value of <see cref="GetItems"/> will contain. <br />
        /// This can definitely help when copying the items from the method to another collection. <br />
        /// If you do not know the number of elements at a given time, prefer to return something less than zero.
        /// </summary>
        public virtual int ElementsCount => wrapping.ElementsCount;

        /// <summary>
        /// Disposes the underlying value list provider.
        /// </summary>
        public void Dispose()
        {
            wrapping.Dispose();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Gets a function that can be used for appropriately formatting values. <br />
        /// NOTE: You cannot override this method. This method is implicitly defined by the wrapping provider.
        /// Modify the wrapper's <see cref="Formatter"/> property instead.
        /// </summary>
        public Func<TSetting, System.String> Formatter => item => wrapping.Formatter(temp);

        /// <summary>
        /// Gets a mapped enumerable by using the mapper specified in the <see cref="Mapper"/> property.
        /// </summary>
        /// <returns>The mapped enumerable collection.</returns>
        public IEnumerable<TSetting> GetItems()
        {
            var mp = Mapper;
            if (disposeifidisposable && typeof(TOld).ImplementsInterface(typeof(IDisposable))) {
                foreach (var old in wrapping.GetItems())
                {
                    temp = old;
                    yield return mp(old);
                    ((IDisposable)temp).Dispose();
                }
            } else {
                foreach (var old in wrapping.GetItems())
                {
                    temp = old;
                    yield return mp(old);
                }
            }
            temp = default;
        }
    }
}