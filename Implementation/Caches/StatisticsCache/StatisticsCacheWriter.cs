
using System;

namespace MP.Caches.StatisticsCache
{
    public sealed class StatisticsCacheWriter : CacheWriter
    {
        public StatisticsCacheWriter(System.IO.Stream stream) 
        {
            UniqueTypeIdentifier = 18951463003966544;
            Version = 1;
            Initialize(stream, nameof(stream));
            WriteHeader();
        }

        private void WriteStatisticValue(System.Object value , System.String name , STATISTICENTRY entryfinal) 
        {
            // A statistic value is laid out as follows:
            // First 4 bytes - unsigned integer indicating the length in bytes of the value
            // Next bytes - the statistic's value.
            switch (entryfinal.Type)
            {
                case StatisticType.Null:
                    entryfinal.Size = 0;
                    Writer.WriteStructure(entryfinal);
                    Writer.WriteUTF16LEString(name);
                    break;
                case StatisticType.Boolean:
                    entryfinal.Size = 2;
                    Writer.WriteStructure(entryfinal);
                    Writer.WriteUTF16LEString(name);
                    Writer.WriteByte((((System.Boolean)value) ? 1 : 0).ToByte());
                    Writer.WriteByte(0);
                    break;
                case StatisticType.LongNumber:
                    entryfinal.Size = sizeof(System.Int64);
                    Writer.WriteStructure(entryfinal);
                    Writer.WriteUTF16LEString(name);
                    Writer.WriteInt64((System.Int64)value);
                    break;
                case StatisticType.UnsignedLongNumber:
                    entryfinal.Size = sizeof(System.UInt64);
                    Writer.WriteStructure(entryfinal);
                    Writer.WriteUTF16LEString(name);
                    Writer.WriteUInt64((System.UInt64)value);
                    break;
                case StatisticType.String:
                    System.String s = value as System.String;
                    entryfinal.Size = (sizeof(System.Char) * s.Length).ToUInt32();
                    Writer.WriteStructure(entryfinal);
                    Writer.WriteUTF16LEString(name);
                    Writer.WriteUTF16LEString(s);
                    break;
                case StatisticType.DateTime:
                    entryfinal.Size = sizeof(System.Int64);
                    Writer.WriteStructure(entryfinal);
                    Writer.WriteUTF16LEString(name);
                    Writer.WriteInt64(((System.DateTime)value).Ticks);
                    break;
                case StatisticType.TimeSpan:
                    entryfinal.Size = sizeof(System.Int64);
                    Writer.WriteStructure(entryfinal);
                    Writer.WriteUTF16LEString(name);
                    Writer.WriteInt64(((System.TimeSpan)value).Ticks);
                    break;
            }
        }

        public void WriteStatistic(Statistic stat)
        {
            if (stat is null) {
                throw new ArgumentNullException(nameof(stat));
            }
            STATISTICENTRY ent = new();
            ent.Type = stat.TypeOfValue;
            ent.NameLength = stat.Name.Length.ToUInt32();
            WriteStatisticValue(stat.Value , stat.Name , ent);
        }
    }
}