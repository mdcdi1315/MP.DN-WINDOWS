using System.Windows;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;


[assembly: ThemeInfo(
    ResourceDictionaryLocation.None,            //where theme specific resource dictionaries are located
                                                //(used if a resource is not found in the page,
                                                // or application resource dictionaries)
    ResourceDictionaryLocation.SourceAssembly   //where the generic resource dictionary is located
                                                //(used if a resource is not found in the page,
                                                // app, or any theme specific resource dictionaries)
)]
[assembly: ComVisible(false)]
[assembly: AssemblyVersion(AppInfo.Version)]
[assembly: AssemblyFileVersion(AppInfo.Version)]
[assembly: AssemblyCompany("© MDCDI1315 - 2025")]
[assembly: AssemblyFlags(AssemblyNameFlags.None)]
[assembly: AssemblyProduct("MP-DotNet8-Impl-Windows")]
[assembly: System.Runtime.Versioning.TargetPlatform("windows")]
[assembly: RuntimeCompatibility(WrapNonExceptionThrows = false)]
[assembly: AssemblyTitle("Music Player On .NET 8 - Windows Implementation")]
[assembly: System.Runtime.Versioning.SupportedOSPlatform("windows6.1")] // Windows 7
[assembly: AssemblyDescription("Music Player On .NET 8 Main Implementation Assembly for Windows")]

internal static class AppInfo
{
    public const System.String Version = "1.6.1.5";

    // This is the Discord Application Token. DO NOT MODIFY!!!!
    public const System.Int64 DiscordAPIToken = 1338618967371288628;

    public const System.String DisplayVersionStringLoadForm = $"\n                 Version: {Version}";

    public static System.String FormatWindowTitle(System.String format) => System.String.Format(format, Version);
}