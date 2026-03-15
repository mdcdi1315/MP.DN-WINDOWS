

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace MP.SettingsTree
{
    /// <summary>
    /// Provides extension method around the <see cref="SettingsTreeBuilder"/> class.
    /// </summary>
    public static class SettingsTreeBuilderExtensions
    {
        internal static System.String IDynamicValueListProvider_Formatter<T>(T item) => item?.ToString();

        /// <summary>
        /// Gets the actual description string for the specified setting in the settings tree.
        /// </summary>
        /// <param name="builder">The settings tree that this setting is referenced to.</param>
        /// <param name="st">The setting.</param>
        /// <returns>The description string.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="st"/> was <see langword="null"/>.</exception>
        /// <exception cref="ResourceNotFoundException">The setting was referencing a resource ID, but that ID was not for the current instance.</exception>
        public static System.String GetSettingDescriptionString(this SettingsTreeBuilder builder, SettingsTreeSetting st)
        {
            ArgumentNullException.ThrowIfNull(st);
            if (st.DescriptionIsResource)
            {
                return builder.Resources.GetStringResource(st.Description);
            }
            return st.Description;
        }

        /// <summary>
        /// Looks for a specific settings tree node in the builder.
        /// </summary>
        /// <param name="builder">The <see cref="SettingsTreeBuilder"/> to lookup.</param>
        /// <param name="id">The ID of the node you wish to be returned.</param>
        /// <returns>The <see cref="SettingsTreeNode"/> that has as it's ID the contents of the <paramref name="id"/> parameter.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="id"/> was <see langword="null"/>.</exception>
        [return: MaybeNull]
        public static SettingsTreeNode GetNode(this SettingsTreeBuilder builder, System.String id)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(id);
            return BuilderHelpers.GetNode(builder.RootElement, id);
        }

        /// <summary>
        /// Gets all the setting objects defined for the current settings tree.
        /// </summary>
        /// <param name="builder">The builder to get all the settings from.</param>
        /// <returns>An enumerable containing all the settings that the tree contains.</returns>
        public static IEnumerable<SettingsTreeSetting> GetAllSettings(this SettingsTreeBuilder builder) => BuilderHelpers.EnumerateSettings(builder.RootElement);

    }
}