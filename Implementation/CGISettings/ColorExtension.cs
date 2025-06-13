
using System;

namespace MP.CGISettings
{
    public sealed class ColorExtension : ICGISettingExtension
    {
        public CGISettingType RegisteredType => CGISettingType.Color;

        public Type WrappingType => typeof(System.Drawing.Color);

        public object LoadObject(ICGISettingExtensionDataSource source)
        {
            if (source is null) { throw new ArgumentNullException(nameof(source)); }
            System.Byte[] data = new System.Byte[source.Length];
            source.Read(data, 0, data.Length);
            return System.Drawing.Color.FromArgb(data.ToInt32(0));
        }

        public void SaveObject(ICGISettingExtensionDataSource source, object obj)
        {
            if (source is null) { throw new ArgumentNullException(nameof(source)); }
            if (obj is null) { throw new ArgumentNullException(nameof(obj)); }
            System.Drawing.Color cl = (System.Drawing.Color)obj;
            System.Byte[] data = cl.ToArgb().GetBytes();
            source.Write(data, 0, data.Length);
        }
    }
}