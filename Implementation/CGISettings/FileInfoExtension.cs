
using Microsoft.IO;
using System;

namespace MP.CGISettings
{
    public sealed class FileInfoExtension : ICGISettingExtension
    {
        public CGISettingType RegisteredType => CGISettingType.FileInfo;

        public Type WrappingType => typeof(FileInfo);

        public object LoadObject(ICGISettingExtensionDataSource source)
        {
            if (source is null) { throw new ArgumentNullException(nameof(source)); }
            System.Byte[] data = new System.Byte[source.Length];
            source.Read(data, 0, data.Length);
            return new FileInfo(source.StringEncoding.GetString(data));
        }

        public void SaveObject(ICGISettingExtensionDataSource source, object obj)
        {
            if (source is null) { throw new ArgumentNullException(nameof(source)); }
            if (obj is null) { throw new ArgumentNullException(nameof(obj)); }
            System.Byte[] data = source.StringEncoding.GetBytes(((FileInfo)obj).FullName);
            source.Write(data, 0, data.Length);
        }
    }
}