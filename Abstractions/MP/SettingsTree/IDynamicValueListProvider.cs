

using System;
using System.Collections.Generic;

namespace MP.SettingsTree
{
    /// <summary>
    /// Defines a factory for providing custom or dynamic data for a setting that is <see cref="SettingType.ValueList"/>.
    /// </summary>
    /// <typeparam name="T">The backing type of the setting element, making all the candidates that the setting can take at the call time.</typeparam>
    public interface IDynamicValueListProvider<T> : IDisposable
    {
        /// <summary>
        /// Optional property. <br />
        /// Indicates the number of elements that the return value of <see cref="GetItems"/> will contain. <br />
        /// This can definitely help when copying the items from the method to another collection. <br />
        /// If you do not know the number of elements at a given time, prefer to return something less than zero.
        /// </summary>
        public int ElementsCount => -1;

        /// <summary>
        /// Gets an enumerable containing the items to provide for possible values. <br />
        /// Users of the interface should also test the returned instance against the <see cref="IDisposable"/> interface to release native information if this is a natively-wrapped object.
        /// </summary>
        /// <returns>The items to provide as possible values.</returns>
        public IEnumerable<T> GetItems();

        /// <summary>
        /// Gets a function that can be used for appropriately formatting values. <br />
        /// By default, it is set to the ToString method of the object.
        /// </summary>
        public Func<T, System.String> Formatter => item => item?.ToString();
    }
}