using System;

// ReSharper disable once CheckNamespace
namespace AppLimpia.Properties
{
    /// <summary>
    /// A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     This class is declared partial to avoid using of reflections when ResourceManager is required to
    ///     be changed. To compile this code change class to partial in Localization.Designer.cs after altering
    ///     the resource file.
    ///   </para>
    /// </remarks>
    internal partial class Localization
    {
        /// <summary>
        /// Sets the resource manager used by the current application.
        /// </summary>
        /// <param name="resourceManager">The new resource manager to be used by the current application.</param>
        internal static void SetResourceManager(System.Resources.ResourceManager resourceManager)
        {
            Localization.resourceMan = resourceManager;
        }
    }
}
