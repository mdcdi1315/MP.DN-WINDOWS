

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace MP.DownloadableDeps
{
    public sealed class DownloadDependencyComputer
    {
        private DownloadableDependency maindep;
        private DownloadableDependency[] childdeps;

        public DownloadDependencyComputer(DownloadableDependency maindep, DownloadableDependency[] childdeps)
        {
            this.maindep = maindep;
            this.childdeps = childdeps;
        }

        private static System.Boolean CheckPlatformRule(System.String platreq)
            => platreq switch {
                "x64" => RuntimeInformation.OSArchitecture == Architecture.X64,
                "x86" => RuntimeInformation.OSArchitecture == Architecture.X86,
                "arm64" => RuntimeInformation.OSArchitecture == Architecture.Arm64,
                "any" => true, // always evaluates to true if 'any'.
                "" or null => true,
                _ => false,
            };

        private static System.Boolean RequiredByOtherDependencyWithName(System.String depname , List<DownloadableDependency> selecteddeps)
        {
            if (System.String.IsNullOrEmpty(depname)) { return true; }
            foreach (var dep in selecteddeps) 
            {
                if (dep.Name == depname) { return true; }
            }
            return false;
        }

        private static System.Boolean CheckOSRule(System.String osreq)
            => osreq switch {
                "" or null => true,
                "windows" => RuntimeInformation.IsOSPlatform(OSPlatform.Windows),
                "linux" => RuntimeInformation.IsOSPlatform(OSPlatform.Linux),
                "macos" => RuntimeInformation.IsOSPlatform(OSPlatform.OSX),
                "bsd" => RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD),
                _ => false
            };

        public DownloadableDependency[] ComputeDependencies()
        {
            List<DownloadableDependency> depstemp = new() { maindep }; // Main dependency always required and it is the first in the list.
            foreach (var dp in childdeps)
            {
                System.Boolean include = true;
                foreach (var sr in dp.InclusionRules)
                {
                    System.String[] parts = sr.Split('=');
                    switch (parts[0].ToLower())
                    {
                        case "platform":
                            include = CheckPlatformRule(parts[1]);
                            break;
                        case "requiredby":
                            include = RequiredByOtherDependencyWithName(parts[1] , depstemp);
                            break;
                        case "os":
                            include = CheckOSRule(parts[1].ToLower());
                            break;
                    }
                    if (include == false) { break; }
                }
                if (include == false) { continue; }
                depstemp.Add(dp);
            }
            ValidateDependencies(depstemp);
            return depstemp.ToArray();
        }
    
        private static void ValidateDependencies(IList<DownloadableDependency> registered)
        {
            for (System.Int32 I = 0; I < registered.Count; I++)
            {
                System.String n1 = registered[I].Name;
                for (System.Int32 J = 0; J < registered.Count; J++) 
                {
                    if (I == J) { continue; }
                    if (n1 == registered[J].Name) { throw new InvalidOperationException($"Conflicts found!! The dependency name {n1} is registered twice. This could result in a cyclic dependency."); }
                }
            }
        }
    }
}