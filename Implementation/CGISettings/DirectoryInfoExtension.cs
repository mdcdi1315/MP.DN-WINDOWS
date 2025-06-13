
using Microsoft.IO;
using System;

namespace MP.CGISettings
{
    public sealed class DirectoryInfoExtension : ICGISettingExtension
    {
        public CGISettingType RegisteredType => CGISettingType.DirectoryInfo;

        public Type WrappingType => typeof(DirectoryInfo);

        public object LoadObject(ICGISettingExtensionDataSource source)
        {
            if (source is null) { throw new ArgumentNullException(nameof(source)); }
            System.Byte[] data = new System.Byte[source.Length];
            source.Read(data, 0, data.Length);
            return new DirectoryInfo(source.StringEncoding.GetString(data));
        }

        public void SaveObject(ICGISettingExtensionDataSource source, object obj)
        {
            if (source is null) { throw new ArgumentNullException(nameof(source)); }
            if (obj is null) { throw new ArgumentNullException(nameof(obj)); }
            System.Byte[] data = source.StringEncoding.GetBytes(((DirectoryInfo)obj).FullName);
            source.Write(data, 0, data.Length);
        }
    }
}