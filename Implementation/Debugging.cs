
using System;
using Microsoft.IO;

namespace MP
{
    public sealed class ConsoleSink : DebugSink
    {
        private System.Boolean active;

        public ConsoleSink() { 
            active = ConsoleBackend.Create();
        }

        public override bool IsActive => active;

        // Call ConsoleBackend.Destroy from the program stub
        public override void Dispose() {}

        public override void Write(string message)
        {
            if (active)
            {
                ConsoleBackend.WriteConsole($"{SystemInfo.UtcNow.ToString("[yyyy-MM-dd {hh:mm:ss.ffff}]:")} {message}");
            }
        }
    }

    public sealed class LogFileSink : DebugSink
    {
        private FileStream fst;
        private System.Boolean active;
        private System.Text.Encoding encoding;

        public LogFileSink(System.String fp) 
        {
            try {
                fst = new(fp, FileMode.Create , FileAccess.ReadWrite , FileShare.Read);
                encoding = System.Text.Encoding.Unicode;
                active = true;
            } catch {
                active = false;
            }
        }

        public override bool IsActive => active;

        public override void Write(string message)
        {
            System.Byte[] temp = encoding.GetBytes(SystemInfo.UtcNow.ToString("[yyyy-MM-dd {hh:mm:ss.ff}]: "));
            fst.Write(temp , 0 , temp.Length);
            temp = encoding.GetBytes(message);
            fst.Write(temp, 0, temp.Length);
            temp = null;
        }

        public override void Dispose() 
        {
            active = false;
            if (fst is not null)
            {
                fst.Flush();
                fst.Dispose();
                fst = null;
            }
            encoding = null;
        }
    }
}