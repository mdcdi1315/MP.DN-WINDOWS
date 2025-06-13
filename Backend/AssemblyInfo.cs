using System.Reflection;
using System.Runtime.Versioning;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Music Player On .NET 8 Windows Backend Library")]
[assembly: AssemblyDescription("")]
#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif
[assembly: AssemblyCompany("© MDCDI1315 - 2025")]
[assembly: AssemblyProduct("MP-DotNet8 - Backend Library")]
[assembly: AssemblyCopyright("Copyright © mdcdi1315 2025")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("3574d6bd-7e46-4dd3-bf07-833e9e437a24")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
[assembly: AssemblyVersion("1.0.9.6")]
[assembly: AssemblyFileVersion("1.0.9.6")]
[assembly: SupportedOSPlatform(MP.WindowsInterop.WindowsVersions.NTDDI_WIN7)]
[assembly: DefaultDllImportSearchPaths(DllImportSearchPath.UserDirectories | DllImportSearchPath.System32)]
