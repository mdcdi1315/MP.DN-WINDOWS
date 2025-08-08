

using System;
using MP.ExceptionSystem;
using System.Collections;
using System.Collections.Generic;

namespace MP.Collections
{
    /// <summary>
    /// A specialized dictionary for managing environment variables.
    /// </summary>
    public sealed class EnvironmentVariablesDictionary :
            IList<EnvironmentVariable>,
            IGettableSettable<System.String, System.String>,
            IGettableSettable<System.Int32 , EnvironmentVariable>
    {
        private List<EnvironmentVariable> data;

        /// <summary>
        /// Initializes an empty environment variable dictionary instance.
        /// </summary>
        public EnvironmentVariablesDictionary() => data = new(5);

        /// <summary>
        /// Initializes an empty environment variable dictionary instance with the specified initial capacity.
        /// </summary>
        /// <param name="capacity">The initial capacity that the newly created dictionary should have.</param>
        public EnvironmentVariablesDictionary(int capacity) => data = new(capacity);

        /// <summary>
        /// Initializes the environment variable dictionary by copying the elements specified in <paramref name="enumerable"/>.
        /// </summary>
        /// <param name="enumerable">The elements to copy to the newly created <see cref="EnvironmentVariablesDictionary"/> object.</param>
        public EnvironmentVariablesDictionary(IEnumerable<EnvironmentVariable> enumerable)
        {
            ArgumentNullException.ThrowIfNull(enumerable);
            if (enumerable is ICollection<EnvironmentVariable> c)
            {
                data = new(c);
            }
            else
            {
                data = new(10);
                foreach (var e in enumerable)
                {
                    data.Add(e);
                }
            }
        }

        /// <summary>
        /// Retrieves by index the specified environment variable. <br />
        /// Setting by index is not allowed and throws <see cref="NotSupportedException"/>; use the <see cref="Set"/> API instead.
        /// </summary>
        /// <param name="index">The index where to retrieve the specified environment variable from the array.</param>
        /// <returns>The environment variable at <paramref name="index"/>.</returns>
        /// <exception cref="NotSupportedException">Setting environment variables through direct indexing is not allowed. Use the <see cref="Set"/> API instead.</exception>
        public EnvironmentVariable this[int index]
        {
            get => data[index];
            set => throw new NotSupportedException("Cannot directly set an environment variable to a specified position. Use the Set API instead.");
        }

        /// <summary>
        /// Gets or sets the specified environment variable. <br />
        /// It's name is given in the <paramref name="index"/> parameter.
        /// </summary>
        /// <param name="index">The name of the environment variable to set or retrieve.</param>
        /// <returns>The current value of the requested environment variable.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="index"/> parameter was <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="index"/> parameter was represented the empty string.</exception>
        ///  <exception cref="EnvironmentVariableNotFoundException">When getting, the specified environment variable does not exist into this instance.</exception>
        public string this[string index]
        {
            get => Get(index).Value;
            set => Set(new(index, value));
        }

        /// <summary>
        /// Gets the number of stored environment variables of this instance.
        /// </summary>
        public int Count => data.Count;

        /// <summary>
        /// Environment variable dictionaries are always mutable objects.
        /// </summary>
        public bool IsReadOnly => false;

        /// <summary>
        /// Adds a new environment variable into this dictionary object.
        /// </summary>
        /// <param name="item">The environment variable to add.</param>
        /// <exception cref="ArgumentNullException">The <see cref="EnvironmentVariable.Name"/> field was <see langword="null"/> or represented the empty string.</exception>
        public void Add(EnvironmentVariable item)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(item.Name, nameof(item));
            data.Add(item);
        }

        /// <summary>
        /// Gets the specified environment variable item, or if it does not exist in this dictionary, it throws <see cref="EnvironmentVariableNotFoundException"/>.
        /// </summary>
        /// <param name="name">The name of the environment variable you wish to retrieve.</param>
        /// <returns>The environment variable's name and value packaged into a <see cref="EnvironmentVariable"/> instance.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="name"/> parameter was <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="name"/> parameter was represented the empty string.</exception>
        /// <exception cref="EnvironmentVariableNotFoundException">The specified environment variable does not exist into this instance.</exception>
        public EnvironmentVariable Get(System.String name)
        {
            int I = IndexOf(name);
            if (I > -1)
            {
                return data[I];
            }
            throw new EnvironmentVariableNotFoundException(name);
        }

        /// <summary>
        /// Adds or updates the specified environment variable with the specified value.
        /// </summary>
        /// <param name="variable">The environment variable which to add or update it's held value.</param>
        /// <returns>A value whether an internal add operation was required to set the specified variable.</returns>
        public System.Boolean Set(EnvironmentVariable variable)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(variable.Name, nameof(variable));
            int I = IndexOf(variable.Name);
            if (I > -1)
            {
                data[I] = variable;
                return false;
            }
            else
            {
                data.Add(variable);
                return true;
            }
        }

        /// <summary>
        /// Removes all the environment variables defined into this object.
        /// </summary>
        public void Clear() => data.Clear();

        /// <summary>
        /// Checks out whether the specified environment variable name provided in the <paramref name="name"/> parameter exists in this dictionary object. <br />
        /// Equivalent to calling the <see cref="IndexOf(System.String)"/> method and testing whether it's return value is greater than -1.
        /// </summary>
        /// <param name="name">The name of the environment variable to test for existense.</param>
        /// <returns>A value whether the specified variable does exist in this instance or not.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="name"/> parameter was <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="name"/> parameter was represented the empty string.</exception>
        public bool Contains(System.String name) => IndexOf(name) > -1;

        /// <summary>
        /// Checks out whether the specified environment variable provided in the <paramref name="item"/> parameter exists in this dictionary object. <br />
        /// Equivalent to calling the <see cref="IndexOf(EnvironmentVariable)"/> method and testing whether it's return value is greater than -1.
        /// </summary>
        /// <param name="item">The environment variable to test for existense.</param>
        /// <returns>A value whether the specified variable does exist in this instance or not.</returns>
        /// <exception cref="ArgumentNullException">The <see cref="EnvironmentVariable.Name"/> field of the <paramref name="item"/> parameter was <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">The <see cref="EnvironmentVariable.Name"/> field of the <paramref name="item"/> parameter was represented the empty string.</exception>
        public bool Contains(EnvironmentVariable item) => Contains(item.Name);

        /// <inheritdoc />
        public void CopyTo(EnvironmentVariable[] array, int arrayIndex) => data.CopyTo(array, arrayIndex);

        /// <summary>
        /// Gets an enumerator able to enumerate through all the defined environment variables of this instance.
        /// </summary>
        /// <returns>An enumerator that is able to iterate through all the held environment variables of this instance.</returns>
        public IEnumerator<EnvironmentVariable> GetEnumerator() => data.GetEnumerator();

        /// <summary>
        /// Returns the location of the specified environment variable in the internal array, named by the contents of the <paramref name="name"/> parameter.
        /// </summary>
        /// <param name="name">The environment variable to find it's location.</param>
        /// <returns>The location of the specified environment variable in the internal array, or -1 if it does not exist in the current instance.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="name"/> parameter was <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="name"/> parameter was represented the empty string.</exception>
        public int IndexOf(System.String name)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(name);
            for (int I = 0; I < data.Count; I++)
            {
                if (data[I].Name == name) { return I; }
            }
            return -1;
        }

        /// <summary>
        /// Returns the location of the specified environment variable in the internal array.
        /// </summary>
        /// <param name="item">The environment variable to find it's location.</param>
        /// <returns>The location of the specified environment variable in the internal array, or -1 if it does not exist in the current instance.</returns>
        /// <exception cref="ArgumentNullException">The <see cref="EnvironmentVariable.Name"/> field of the <paramref name="item"/> parameter was <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">The <see cref="EnvironmentVariable.Name"/> field of the <paramref name="item"/> parameter was represented the empty string.</exception>
        public int IndexOf(EnvironmentVariable item) => IndexOf(item.Name);

        /// <inheritdoc />
        public void Insert(int index, EnvironmentVariable item) => data.Insert(index, item);

        /// <summary>
        /// Removes the environment variable by the name specified in the <paramref name="name"/> parameter.
        /// </summary>
        /// <param name="name">The name of the environment variable you wish to be deleted.</param>
        /// <returns>A value whether the specified environment variable was found and deleted.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="name"/> parameter was <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="name"/> parameter was represented the empty string.</exception>
        public bool Remove(System.String name)
        {
            int I = IndexOf(name);
            if (I > -1)
            {
                data.RemoveAt(I);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Removes the environment variable specified by the <paramref name="item"/> parameter.
        /// </summary>
        /// <param name="item">The environment variable to remove from this instance.</param>
        /// <returns>A value whether the specified environment variable was found and deleted.</returns>
        /// <exception cref="ArgumentNullException">The <see cref="EnvironmentVariable.Name"/> field of the <paramref name="item"/> parameter was <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">The <see cref="EnvironmentVariable.Name"/> field of the <paramref name="item"/> parameter was represented the empty string.</exception>
        public bool Remove(EnvironmentVariable item) => Remove(item.Name);

        /// <summary>
        /// Removes the specified environment variable by it's location in the internal array.
        /// </summary>
        /// <param name="index">The location in the internal array where it is located the environemnt variable you wish to be deleted.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than 0. -or- <paramref name="index"/> is equal to or greater than <see cref="Count"/>.</exception>
        public void RemoveAt(int index) => data.RemoveAt(index);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}