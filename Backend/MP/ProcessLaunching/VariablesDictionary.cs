
using System.Collections;
using MP.ExceptionSystem;
using System.Collections.Generic;

namespace MP.ProcessLaunching
{
    /// <summary>
    /// Declares a specialized dictionary class for retrieveing, setting and removing environment variables. <br />
    /// Implements the base list interface.
    /// </summary>
    public sealed class VariablesDictionary : IList<EnvironmentVariable>
    {
        private List<EnvironmentVariable> variables;

        /// <summary>
        /// Initializes a new variable dictionary that has only the process environment variables.
        /// </summary>
        public VariablesDictionary() {
            variables = new(SystemInfo.GetEnvironmentVariables());
        }

        /// <summary>
        /// Initializes a new variable dictionary that has only the process environment variables.
        /// </summary>
        /// <param name="capacity">The additional capacity to add to the internal environment variable list.</param>
        public VariablesDictionary(System.Int32 capacity) {
            EnvironmentVariable[] inheritedvars = SystemInfo.GetEnvironmentVariables();
            variables = new(inheritedvars.Length + capacity);
            variables.AddRange(inheritedvars);
        }

        /// <summary>
        /// Gets the specified environment variable in the <paramref name="index"/> position of the variable list.
        /// </summary>
        /// <param name="index">The index to get the corresponding variable.</param>
        /// <returns>The retrieved environment variable at <paramref name="index"/>.</returns>
        /// <exception cref="System.NotSupportedException">Setting an explicit value to a specified list position is not allowed.</exception>
        /// <exception cref="System.IndexOutOfRangeException">The index was outside the list's bounds.</exception>
        public EnvironmentVariable this[System.Int32 index] 
        { 
            get => variables[index]; 
            set => throw new System.NotSupportedException("Setting an environment variable struct to any value is not permitted. Use the Set method instead."); 
        }

        /// <summary>
        /// Gets the value of , or sets the value of an environment variable with the name specified in <paramref name="name"/> parameter.
        /// </summary>
        /// <param name="name">The environment variable whose value to set or get.</param>
        /// <returns>The environment variable value.</returns>
        /// <exception cref="VariableNotFoundException">When getting a variable , the specified variable does not exist.</exception>
        /// <exception cref="System.ArgumentNullException">The variable name was null or the empty string.</exception>
        public System.String this[System.String name]
        {
            get {
                if (System.String.IsNullOrEmpty(name)) { throw new System.ArgumentNullException(nameof(name), "Variable name cannot be null or empty."); }
                System.Int32 idx = IndexOf(name);
                if (idx < 0) { throw new VariableNotFoundException(name); }
                return variables[idx].Value;
            }
            set {
                if (System.String.IsNullOrEmpty(name)) { throw new System.ArgumentNullException(nameof(name), "Variable name cannot be null or empty."); }
                System.Int32 idx = IndexOf(name);
                EnvironmentVariable envfinalvar = new(name, value);
                if (idx == -1) { variables.Add(envfinalvar); return; }
                variables[idx] = envfinalvar;
            }
        }

        public System.Int32 Count => variables.Count;

        public System.Boolean IsReadOnly => false;

        public void Add(System.String name, System.String value)
        {
            if (System.String.IsNullOrEmpty(name)) { throw new System.ArgumentNullException(nameof(name) , "Variable name cannot be null or empty."); }
            Add(new(name, value ?? System.String.Empty)); // value is not necessary to be non-null but to ensure correct behavior pass it as empty string when null.
        }

        public void Add(EnvironmentVariable item)
        {
            if (System.String.IsNullOrEmpty(item.Name)) { throw new System.ArgumentNullException(nameof(item), "Variable name cannot be null or empty."); }
            if (Contains(item.Name)) {
                throw new System.InvalidOperationException($"The environment variable with name {item.Name} does already exist. Use the Set method instead to modify it's value.");
            }
            variables.Add(item);
        }

        public void Clear() => variables.Clear();

        public System.Boolean Contains(EnvironmentVariable item) => Contains(item.Name);

        public System.Boolean Contains(System.String name)
        {
            if (System.String.IsNullOrEmpty(name)) { return false; }
            for (System.Int32 I = 0; I < variables.Count; I++)
            {
                if (name.Equals(variables[I].Name, System.StringComparison.OrdinalIgnoreCase)) { return true; }
            }
            return false;
        }

        public void CopyTo(EnvironmentVariable[] array, System.Int32 arrayIndex) => variables.CopyTo(array, arrayIndex);

        public IEnumerator<EnvironmentVariable> GetEnumerator() => variables.GetEnumerator();

        public System.Int32 IndexOf(EnvironmentVariable item) => IndexOf(item.Name);

        public System.Int32 IndexOf(System.String name)
        {
            if (System.String.IsNullOrEmpty(name)) { return -1; }
            for (System.Int32 I = 0; I < variables.Count; I++)
            {
                if (name.Equals(variables[I].Name , System.StringComparison.OrdinalIgnoreCase)) { return I; }
            }
            return -1;
        }

        public System.Boolean Set(System.String name , System.String newvalue)
        {
            System.Int32 idx = IndexOf(name);
            if (idx == -1) { return false; }
            EnvironmentVariable variable = new() { Name = name, Value = newvalue };
            variables[idx] = variable;
            return true;
        }

        public void Insert(System.Int32 index, EnvironmentVariable item) => variables.Insert(index, item);

        public System.Boolean Remove(EnvironmentVariable item)
        {
            if (System.String.IsNullOrEmpty(item.Name)) { return false; }
            for (System.Int32 I = 0; I < variables.Count; I++)
            {
                if (item.Name.Equals(variables[I].Name, System.StringComparison.OrdinalIgnoreCase)) {
                    variables.RemoveAt(I);
                    return true;
                }
            }
            return false;
        }

        public void RemoveAt(System.Int32 index) => variables.RemoveAt(index);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}