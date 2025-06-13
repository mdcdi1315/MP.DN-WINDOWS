

using System;
using Microsoft.IO;
using MP.Threading;
using System.Collections.Generic;
using Microsoft.Security.Cryptography;

namespace MP.DownloadableDeps
{
    public sealed class VerifierDependencyCheck
    {
        public DownloadableDependency Dependency;
        public IDictionary<System.String, System.String> VerificationHashes;
    }

    public enum InstallationVerifierStatus : System.Byte
    {
        VerifyingFile,
        VerifyingDependency,
        NumberOfTotalItemsToCheck,
        VerificationFailedOn,
        UpdateVerifiedItems
    }

    public delegate void InstallationVerifierStatusEventDelegate(InstallationVerifierStatus status , System.Object additionaldata);

    public sealed class DownloadInstallationVerifier : IDisposable
    {
        private MD5 hasher;
        private DirectoryInfo basedir;
        private DownloadableDepsVerificationFReader rdr;

        public DownloadInstallationVerifier(DirectoryInfo di , System.IO.Stream verfilestream)
        {
            if (di is null) { throw new ArgumentNullException(nameof(di)); }
            if (verfilestream is null) { throw new ArgumentNullException(nameof(verfilestream)); }
            this.basedir = di;
            OnStatusChanged = new(InstallationVerifierStatusEventDummyMethod);
            rdr = new(verfilestream);
            hasher = MD5.Create();
        }

        private static void InstallationVerifierStatusEventDummyMethod(InstallationVerifierStatus st , System.Object additional) { }

        private static System.String HashValueToString(System.Byte[] data)
        {
            System.Text.StringBuilder sb = new(data.Length * 2);
            foreach (var b in data) 
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }

        private System.Boolean VerifyDirectoryInstallation(VerifierDependencyCheck depchk , ICancellationToken<System.Byte> token , ref System.Int64 vi)
        {
            DirectoryInfo basedata = basedir.GetSubDirectory(depchk.Dependency.SavePath.RelativePath);
            System.Boolean result = true;
            FileInfo fi;
            foreach (var fe in depchk.VerificationHashes)
            {
                if (token.IsValid(token.InstanceId) == false) { break; }
                fi = basedata.GetFile(fe.Key);
                if (fi is null) {
                    result = false;
                    OnStatusChanged.Invoke(InstallationVerifierStatus.VerificationFailedOn, fe.Key);
                    break; 
                }
                FileStream fsm = null;
                try {
                    if (token.IsValid(token.InstanceId) == false) { break; }
                    fsm = fi.OpenRead();
                    OnStatusChanged.Invoke(InstallationVerifierStatus.VerifyingFile, fi.FullName);
                    if (token.IsValid(token.InstanceId) == false) { break; }
                    if (HashValueToString(hasher.ComputeHash(fsm)) != fe.Value)
                    {
                        result = false;
                        OnStatusChanged.Invoke(InstallationVerifierStatus.VerificationFailedOn, fsm.Name);
                        break;
                    }
                    vi++;
                    OnStatusChanged.Invoke(InstallationVerifierStatus.UpdateVerifiedItems, vi);
                } catch { 
                    result = false;
                    OnStatusChanged.Invoke(InstallationVerifierStatus.VerificationFailedOn, fe.Key);
                    break;
                } finally {
                    fsm?.Dispose();
                    fsm = null;
                }
            }
            return result;
        }

        private System.Int64 GetTotalItems()
        {
            System.Int64 total = 0;
            foreach (var intd in rdr.DepsInstalled)
            {
                if (intd.Dependency.SavePath.Type == PathDetailSaveType.Directory) {
                    total += intd.VerificationHashes.Count;
                } else {
                    total++;
                }
            }
            return total;
        }

        public System.Boolean VerifyInstallation(ICancellationToken<System.Byte> token)
        {
            if (token is null) { throw new ArgumentNullException(nameof(token)); }
            System.Boolean result = true;
            System.Int64 totalitems = 0;
            OnStatusChanged.Invoke(InstallationVerifierStatus.NumberOfTotalItemsToCheck , GetTotalItems());
            foreach (var intd in rdr.DepsInstalled)
            {
                OnStatusChanged.Invoke(InstallationVerifierStatus.VerifyingDependency, intd.Dependency.Name);
                // If it is a directory , verify all the files that the directory contains; 
                // otherwise verify the file only.
                if (intd.Dependency.SavePath.Type == PathDetailSaveType.Directory)
                {
                    if (token.IsValid(token.InstanceId) == false) { break; }
                    result = VerifyDirectoryInstallation(intd , token , ref totalitems);
                } else {
                    FileStream fsm = null;
                    try {
                        System.String sp;
                        if (token.IsValid(token.InstanceId) == false) { break; }
                        fsm = basedir.OpenReadFileStream(sp = intd.Dependency.SavePath.RelativePath);
                        OnStatusChanged.Invoke(InstallationVerifierStatus.VerifyingFile, fsm.Name);
                        if (token.IsValid(token.InstanceId) == false) { break; }
                        if (HashValueToString(hasher.ComputeHash(fsm)) != intd.VerificationHashes[sp])
                        {
                            result = false;
                            OnStatusChanged.Invoke(InstallationVerifierStatus.VerificationFailedOn , fsm.Name);
                            break;
                        }
                        totalitems++;
                        OnStatusChanged.Invoke(InstallationVerifierStatus.UpdateVerifiedItems , totalitems);
                    } catch {
                        result = false;
                        OnStatusChanged.Invoke(InstallationVerifierStatus.VerificationFailedOn, intd.Dependency.SavePath.RelativePath);
                        break;
                    } finally {
                        fsm?.Dispose();
                        fsm = null;
                    }
                }
                if (result == false) { break; }
            }
            return result;
        }

        public event InstallationVerifierStatusEventDelegate OnStatusChanged;

        public void Dispose() 
        {
            hasher?.Dispose();
            hasher = null;
            basedir = null;
            rdr?.Dispose();
            rdr = null;
        }
    }

    public sealed class DownloadInstallationVerifierFileWriter : IDisposable
    {
        private MD5 hasher;
        private DownloadableDepsVerificationFWriter wr;

        public DownloadInstallationVerifierFileWriter(System.IO.Stream datafilestream)
        {
            if (datafilestream is null) { throw new ArgumentNullException(nameof(datafilestream)); }
            hasher = MD5.Create();
            wr = new(datafilestream);
        }

        public System.Boolean IsStreamOwner
        {
            get => wr.IsStreamOwner;
            set => wr.IsStreamOwner = value;
        }

        private static System.String HashValueToString(System.Byte[] data)
        {
            System.Text.StringBuilder sb = new(data.Length * 2);
            foreach (var b in data)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }

        public void WriteFromDirectoryAndDeps(DirectoryInfo directory , DownloadableDependency[] deps)
        {
            if (directory is null) { throw new ArgumentNullException(nameof(directory)); }
            if (deps is null) { throw new ArgumentNullException(nameof(deps)); }
            VerifierDependencyCheck vdc;
            foreach (var dp in deps)
            {
                vdc = new();
                vdc.Dependency = dp;
                vdc.VerificationHashes = new Dictionary<System.String , System.String>();
                if (dp.SavePath.Type == PathDetailSaveType.File) {
                    FileStream fsm = null;
                    try {
                        fsm = directory.OpenReadFileStream(dp.SavePath.RelativePath);
                        vdc.VerificationHashes.Add(dp.SavePath.RelativePath, HashValueToString(hasher.ComputeHash(fsm)));
                    } finally {
                        fsm?.Dispose();
                        fsm = null;
                    }
                } else {
                    foreach (var fe in directory.GetSubDirectory(dp.SavePath.RelativePath).GetFiles())
                    {
                        FileStream fsm = null;
                        try {
                            fsm = fe.OpenRead();
                            vdc.VerificationHashes.Add(fe.Name, HashValueToString(hasher.ComputeHash(fsm)));
                        } finally {
                            fsm?.Dispose();
                            fsm = null;
                        }
                    }
                }
                wr.WriteInstalledDep(vdc);
            }
            wr.EnsureFlushed();
        }

        public void Dispose()
        {
            hasher?.Dispose();
            hasher = null;
            wr?.Dispose();
            wr = null;
        }
    }
}