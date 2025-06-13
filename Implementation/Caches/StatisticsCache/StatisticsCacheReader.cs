
using System;
using System.Collections.Generic;

namespace MP.Caches.StatisticsCache
{
    public sealed class StatisticsCacheReader : CacheReader
    {
        public StatisticsCacheReader(System.IO.Stream stream) 
        {
            Initialize(stream, nameof(stream));
            if (UniqueTypeIdentifier != 18951463003966544)
            {
                throw new CacheFormatInvalidException("This is not the Music Player Statistics cache format. Invalid data provided?");
            }
        }

        private System.Object LoadObject(STATISTICENTRY ent)
        {
            switch (ent.Type)
            {
                case StatisticType.Null:
                    if (ent.Size != 0)
                    {
                        throw new InvalidOperationException("The null type must be zero bytes.");
                    }
                    return null;
                case StatisticType.Boolean:
                    System.Boolean ret;
                    if (ent.Size != 2)
                    {
                        throw new InvalidOperationException("The boolean type must be two bytes.");
                    }
                    ret = Reader.ReadByte() == 1;
                    _ = Reader.ReadByte(); // Effectively discard the next byte
                    return ret;
                case StatisticType.LongNumber:
                    if (ent.Size != sizeof(System.Int64))
                    {
                        throw new InvalidOperationException("The long number type must be eight bytes.");
                    } 
                    return Reader.ReadInt64();
                case StatisticType.UnsignedLongNumber:
                    if (ent.Size != sizeof(System.UInt64))
                    {
                        throw new InvalidOperationException("The unsigned long number type must be eight bytes.");
                    }
                    return Reader.ReadUInt64();
                case StatisticType.String:
                    return Reader.ReadUTF16LEString(ent.Size.ToInt32());
                case StatisticType.DateTime:
                    if (ent.Size != sizeof(System.Int64))
                    {
                        throw new InvalidOperationException("A date/time type must be eight bytes.");
                    }
                    return new System.DateTime(Reader.ReadInt64());
                case StatisticType.TimeSpan:
                    if (ent.Size != sizeof(System.Int64))
                    {
                        throw new InvalidOperationException("A time span type must be eight bytes.");
                    }
                    return new System.TimeSpan(Reader.ReadInt64());
                default:
                    throw new CacheFormatInvalidException($"Invalid type code {ent.Type} provided in the input.");
            }
        }

        public IEnumerable<Statistic> Statistics
        {
            get {
                // Read statistics one by one , until the entire stream is processed
                Reader.Seek(BaseOffsetToData, System.IO.SeekOrigin.Begin);
                while (Reader.Position < Reader.Length)
                {
                    STATISTICENTRY entry = Reader.ReadStructure<STATISTICENTRY>();
                    System.String st = Reader.ReadUTF16LEString((entry.NameLength * sizeof(System.Char)).ToInt32());
                    yield return new(st, LoadObject(entry));
                }
            }
        }
    }
}