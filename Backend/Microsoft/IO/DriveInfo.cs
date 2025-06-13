// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace Microsoft.IO
{
    public sealed partial class DriveInfo
    {
        private readonly string _name;

        public DriveInfo(string driveName)
        {
            if (driveName == null)
            {
                throw new ArgumentNullException(nameof(driveName));
            }

            _name = NormalizeDriveName(driveName);
        }

        public string Name => _name;

        public bool IsReady => Directory.Exists(Name);

        public DirectoryInfo RootDirectory => new DirectoryInfo(Name);

        public override string ToString() => Name;
    }
}
