
using MP;
using System;
using DotNetResourcesExtensions;

namespace MusicPlayer.BuildTasks
{
    public sealed class InputFileReferenceReader : IResourceEntry
    {
        private System.Object transformed;
        private InputFileReferenceFileData data;

        public InputFileReferenceReader(InputFileReferenceFileData data)
        {
            if (data is null) { throw new ArgumentNullException(nameof(data)); }
            this.data = data;
            transformed = null;
        }

        private void ReadString()
        {
            System.IO.FileStream fsm = new(data.FileName, System.IO.FileMode.Open);
            try {
                System.Byte[] data = fsm.ReadBytes(fsm.Length);
                transformed = this.data.AsEncoding().GetString(data);
                data = null;
            } finally {
                fsm?.Dispose();
            }
        }

        private void ReadBytes()
        {
            System.IO.FileStream fsm = new(data.FileName, System.IO.FileMode.Open);
            try {
                transformed = fsm.ReadBytes(fsm.Length);
            } finally {
                fsm?.Dispose();
            }
        }

        public void ReadAndCreateObject()
        {
            switch (data.StandardType)
            {
                case FileReferenceStandardType.String:
                    ReadString();
                    break;
                case FileReferenceStandardType.ByteArray: 
                    ReadBytes(); 
                    break;
            }
        }

        public System.String Name => data.TransformedEntryName;

        public System.Type TypeOfValue => data.SavingType;

        public System.Object Value => transformed;
    }
}