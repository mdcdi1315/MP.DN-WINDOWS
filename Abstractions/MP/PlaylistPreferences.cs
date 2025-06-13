using System;
using System.Collections.Generic;

namespace MP
{
    /// <summary>
    /// Holds a list of reconstuctible playlist preferences.
    /// </summary>
    public sealed class PlaylistPreferences
    {
        private List<Preference> prefs;

        /// <summary>
        /// Initializes an empty instance of <see cref="PlaylistPreferences"/> class.
        /// </summary>
        public PlaylistPreferences()
        {
            prefs = new();
        }

        /// <summary>
        /// Adds a new preference to be saved.
        /// </summary>
        /// <param name="pref">The preference to be saved.</param>
        public void Add(Preference pref) => prefs.Add(pref);

        /// <summary>
        /// Adds a new preference to be saved , or updates the preference if it is already saved.
        /// </summary>
        /// <param name="pref">The preference to be saved.</param>
        public void AddOrUpdate(Preference pref)
        {
            Remove(pref.Name);
            prefs.Add(pref);
        }

        /// <summary>
        /// Updates a preference's value.
        /// </summary>
        /// <param name="Name">The name of the preference to update.</param>
        /// <param name="Value">The new value of the preference.</param>
        /// <exception cref="InvalidOperationException">The preference must exist.</exception>
        public void Update(System.String Name, System.String Value) 
        {
            Preference old = Get(Name);
            if (Remove(Name) == false) {
                throw new InvalidOperationException("Update operation requires that the old value has been deleted.");
            }
            Add(new(Name, Value, old.Description));
        }

        /// <summary>
        /// Updates a preference's value.
        /// </summary>
        /// <param name="Name">The name of the preference to update.</param>
        /// <param name="Value">The new value of the preference.</param>
        /// <exception cref="InvalidOperationException">The preference must exist.</exception>
        public void Update(System.String Name, System.Int64 Value)
        {
            Preference old = Get(Name);
            if (Remove(Name) == false) {
                throw new InvalidOperationException("Update operation requires that the old value has been deleted.");
            }
            Add(new(Name, Value, old.Description));
        }

        /// <summary>
        /// Updates a preference's value.
        /// </summary>
        /// <param name="Name">The name of the preference to update.</param>
        /// <param name="Value">The new value of the preference.</param>
        /// <exception cref="InvalidOperationException">The preference must exist.</exception>
        public void Update(System.String Name, System.Boolean Value)
        {
            Preference old = Get(Name);
            if (Remove(Name) == false) {
                throw new InvalidOperationException("Update operation requires that the old value has been deleted.");
            }
            Add(new(Name, Value, old.Description));
        }

        /// <summary>
        /// Gets a preference from the specified name.
        /// </summary>
        /// <param name="Name">The preference name to retrieve.</param>
        /// <returns>The found preference data.</returns>
        /// <exception cref="KeyNotFoundException">The <paramref name="Name"/> is not registered in the preference list.</exception>
        public Preference Get(System.String Name)
        {
            foreach (var pf in prefs)
            {
                if (pf.Name == Name) { return pf; }
            }
            throw new KeyNotFoundException($"The preference \'{Name}\' was not found.");
        }

        /// <summary>
        /// Removes a preference from the specified name.
        /// </summary>
        /// <param name="Name">The preference name that is to be deleted.</param>
        /// <returns><see langword="true"/> if the preference was found and deleted; otherwise , <see langword="false"/>.</returns>
        public System.Boolean Remove(System.String Name)
        {
            Preference prf;
            for (System.Int32 I = 0; I < prefs.Count; I++)
            {
                prf = prefs[I];
                if (prf.Name == Name)
                {
                    prefs.RemoveAt(I);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Gets the preference at the specified index in the specified internal data array.
        /// </summary>
        /// <param name="index">The index of the preference to look up.</param>
        /// <returns>The preference at <paramref name="index"/>.</returns>
        public Preference this[int index] => prefs[index];

        /// <summary>
        /// Gets a preference from the specified preference name.
        /// </summary>
        /// <param name="name">The name of the preference to retrieve.</param>
        /// <returns>The requested preference with name of parameter <paramref name="name"/>.</returns>
        public Preference this[System.String name] => Get(name);

        /// <summary>Clears all the held preferences.</summary>
        public void Clear() => prefs.Clear();

        /// <summary>Gets the number of preferences contained in this object.</summary>
        public System.Int32 Count => prefs.Count;
    }
}
