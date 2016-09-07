using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

using Windows.ApplicationModel.Resources;

namespace AppLimpia.WinPhone
{
    /// <summary>
    /// Resource manager to use resource loader for Windows RT platform.
    /// </summary>
    public class WindowsRuntimeResourceManager : ResourceManager
    {
        /// <summary>
        /// The WinRT resource loader.
        /// </summary>
        private readonly ResourceLoader resourceLoader;

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowsRuntimeResourceManager"/> class.
        /// </summary>
        /// <param name="baseName">
        ///   The root name of the resource file without its extension but including any fully qualified
        ///   namespace name.
        /// </param>
        /// <param name="assembly">The main assembly for the resources.</param>
        internal WindowsRuntimeResourceManager(string baseName, Assembly assembly)
            : base(baseName, assembly)
        {
            this.resourceLoader = ResourceLoader.GetForViewIndependentUse(baseName);
        }

        /// <summary>
        /// Returns the value of the string resource localized for the specified culture.
        /// </summary>
        /// <param name="name">The name of the resource to retrieve.</param>
        /// <param name="culture">An object that represents the culture for which the resource is localized.</param>
        /// <returns>
        ///   The value of the resource localized for the specified culture,
        ///   or <c>null</c> if name cannot be found in a resource set.
        /// </returns>
        public override string GetString(string name, CultureInfo culture)
        {
            return this.resourceLoader.GetString(name);
        }
    }
}
