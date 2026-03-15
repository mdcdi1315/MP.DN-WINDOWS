
using System;
using MP.Collections;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;

namespace MP
{
    /// <summary>
    /// Provides debugging services for the Music Player components.
    /// </summary>
    public static class DebugProvider
    {
        private static readonly System.Boolean supported;
        private static readonly SingleLinkedList<DebugSink> sinks;
        private static readonly SingleLinkedList<DebugSource> sources;

        static DebugProvider()
        {
            sinks = new SingleLinkedList<DebugSink>();
            sources = new SingleLinkedList<DebugSource>();
#if DEBUG
            supported = true;
#else
            supported = false;
#endif
        }

        /// <summary>
        /// Gets a value whether full debugging is allowed on this session.
        /// </summary>
        public static System.Boolean IsSupported => supported;

        /// <summary>
        /// Gets the currently defined debug sinks.
        /// </summary>
        public static IEnumerable<DebugSink> Sinks => sinks;

        /// <summary>
        /// Gets the currently bound debug sources.
        /// </summary>
        public static IEnumerable<DebugSource> Sources => sources;

        /// <summary>
        /// Adds a debug sink to the debugging provider.
        /// </summary>
        /// <param name="sink">The debug sink to add.</param>
        /// <exception cref="ArgumentNullException"><paramref name="sink"/> was null.</exception>
        public static void AddSink(DebugSink sink)
        {
            ArgumentNullException.ThrowIfNull(sink);
            sinks.Add(sink);
        }

        /// <summary>
        /// Adds a debug source to the debugging provider.
        /// </summary>
        /// <param name="source">The debug source to add.</param>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> was null.</exception>
        public static void AddSource(DebugSource source)
        {
            ArgumentNullException.ThrowIfNull(source);
            Monitor.Enter(sources);
            try {
                sources.Add(source);
            } finally {
                Monitor.Exit(sources);
            }
        }

        /// <summary>
        /// Clears all the currently defined sinks in the sink list.
        /// </summary>
        public static void CleanSinks()
        {
            foreach (var s in sinks)
            {
                s.Dispose();
            }
            sinks.Clear();
        }

        /// <summary>
        /// Clears all the currently defined sources in the source list.
        /// </summary>
        public static void CleanSources() => sources.Clear();

        /// <summary>
        /// Removes a known debug source with the specified source name.
        /// </summary>
        /// <param name="sourcename">The name of the source to be removed.</param>
        /// <returns>A value whether the source removal succeeded.</returns>
        public static System.Boolean RemoveSource(System.String sourcename)
        {
            if (System.String.IsNullOrEmpty(sourcename)) { return false; }
            for (System.Int32 I = 0; I < sources.Count; I++) 
            {
                var src = sources[I];
                if (src.SourceName == sourcename) { 
                    sources.RemoveAt(I);
                    return true; 
                }
            }
            return false;
        }

        // Internal method for logging from a debugging source.
        // Logs only when the source is actually bound to this instance.
        internal static void SourceLog(DebugSource src, System.String msg)
        {
            if (supported == false) { return; }
            if (src is null) { return; }
            foreach (var source in sources) 
            {
                if (source.SourceName == src.SourceName) 
                {
                    Write($"{src.SourceName}: {msg}");
                    break;
                }
            }
        }

        /// <summary>
        /// Generically writes a log line to the debug sinks.
        /// </summary>
        /// <param name="msg">The log line to write.</param>
        [Conditional("DEBUG")]
        public static void WriteLine(System.String msg) => Write($"{msg}\n");

        /// <summary>
        /// Generically writes a log line to the debug sinks, as a formatted string by the specified arguments.
        /// </summary>
        /// <param name="format">The formatted string to be written.</param>
        /// <param name="args">the arguments that are replaced on the <paramref name="format"/> string when expanded.</param>
        [Conditional("DEBUG")]
        public static void WriteFormattedLine(System.String format, params System.Object[] args) => Write(System.String.Concat(System.String.Format(format, args), "\n"));

        /// <summary>
        /// Generically writes log text to the debug sinks.
        /// </summary>
        /// <param name="msg">The log text to write.</param>
        [Conditional("DEBUG")]
        public static void Write(System.String msg) 
        {
            for (System.Int32 I = 0; I < sinks.Count; I++) 
            {
                var sink = sinks[I];
                if (sink.IsActive) { sink.Write(msg); }
            }
        }
    }
}