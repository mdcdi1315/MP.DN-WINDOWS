
using System;
using Microsoft.IO;
using System.Collections.Generic;

namespace MP.Caches.StatisticsCache
{
    public sealed class StatisticsCacheInstance : CacheInstance<StatisticsCacheWriter , StatisticsCacheReader>
    {
        private List<Statistic> stats;

        public StatisticsCacheInstance()
        {
            stats = new(10);
        }

        public override void ClearCacheEntries() => stats.Clear();

        public override void LoadCache(StatisticsCacheReader reader)
        {
            if (reader is null) {
                throw new ArgumentNullException(nameof(reader));
            }
            stats.AddRange(reader.Statistics);
        }

        public override void SaveCache(StatisticsCacheWriter writer)
        {
            if (writer is null) {
                throw new ArgumentNullException(nameof(writer));
            }
            foreach (var stat in stats)
            {
                writer.WriteStatistic(stat);
            }
        }

        public System.Int32 Count => stats.Count;

        public void Add(Statistic st)
        {
            if (st is null) {
                throw new ArgumentNullException(nameof(st));
            }
            stats.Add(st);
        }

        public void Update(Statistic st)
        {
            if (st is null) {
                throw new ArgumentNullException(nameof(st));
            }
            for (System.Int32 I = 0; I < stats.Count; I++) 
            {
                if (stats[I].Name == st.Name)
                {
                    DebugProvider.WriteLine($"STATCACHE: Updating statistic {st.Name} to value {st.Value} ...");
                    stats[I] = st;
                    return;
                }
            }
            stats.Add(st);
        }

        public void UpdateAt(System.Int32 index, Statistic st)
        {
            DebugProvider.WriteLine($"STATCACHE: Updating statistic {st.Name} to value {st.Value} ...");
            stats[index] = st;
        }

        public Statistic Get(System.String name)
        {
            foreach (var s in stats)
            {
                if (s.Name == name) {
                    return s;
                }
            }
            return null;
        }

        public Statistic GetAt(System.Int32 index) => stats[index];

        public System.Boolean RemoveWithName(System.String name)
        {
            if (System.String.IsNullOrEmpty(name)) {
                throw new ArgumentNullException(nameof(name));
            }
            for (System.Int32 I = 0; I < stats.Count; I++) 
            {
                if (stats[I].Name == name) {
                    stats.RemoveAt(I);
                    return true;
                }
            }
            return false;
        }

        public void AddRange(IEnumerable<Statistic> list) => stats.AddRange(list);

        public IEnumerable<Statistic> AllStatistics => stats;

        public void SaveToFile(FileInfo file)
        {
            if (file is null) {
                throw new ArgumentNullException(nameof(file));
            }
            FileStream fsm = null;
            try {
                fsm = file.Open(FileMode.Create, FileAccess.ReadWrite, FileShare.Read);
                SaveCacheToStream(fsm);
            } finally {
                fsm?.Dispose();
            }
        }

        public void LoadFromFile(FileInfo file) 
        {
            if (file is null) {
                throw new ArgumentNullException(nameof(file));
            }
            FileStream fsm = null;
            try
            {
                fsm = file.OpenRead(); // Any exceptions about file existense are also thrown from this statement.
                LoadCacheFromStream(fsm);
            } catch (System.IO.FileNotFoundException e1) {
                // In the case that the file was not found we do not have to worry about.
                // Otherwise, SaveToFile will be possibly called at the session end.
                // However, report the error in Debug mode.
                DebugProvider.WriteLine($"STATCACHE: Failed to load statistics file {file.Name}: {e1}");
            } finally {
                fsm?.Dispose();
            }
        }
    }
}