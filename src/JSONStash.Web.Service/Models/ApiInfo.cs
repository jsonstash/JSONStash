using System.Reflection;

namespace JSONStash.Web.Service.Models
{

    /// <summary>
    /// Derived app information from assemly property groups.
    /// </summary>
    public static class ApiInfo
    {
        /// <summary>
        /// The app's version.
        /// </summary>
        public static string Name
        {
            get => GetName(Assembly.GetCallingAssembly());
        }

        /// <summary>
        /// The app's name.
        /// </summary>
        public static string Version
        {
            get => GetVersion(Assembly.GetCallingAssembly());
        }

        /// <summary>
        /// The app's description.
        /// </summary>
        public static string Description
        {
            get => GetDescription(Assembly.GetCallingAssembly());
        }

        /// <summary>
        /// The app banner containing name and version.
        /// </summary>
        public static string Banner 
        { 
            get
            {
                Assembly assembly = Assembly.GetCallingAssembly();
                string name = GetName(assembly);
                string description = GetDescription(assembly);
                string version = GetVersion(assembly);
                string seperator = $"\n\n{new string('-', description.Length + 10)}\n\n";

                return $"{seperator}  {name} (v{version})\n  {description}{seperator}";
            }
        }

        private static string GetVersion(Assembly assembly) => assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;

        private static string GetName(Assembly assembly) => assembly.GetCustomAttribute<AssemblyProductAttribute>().Product;

        private static string GetDescription(Assembly assembly) => assembly.GetCustomAttribute<AssemblyDescriptionAttribute>().Description;
    }
}
