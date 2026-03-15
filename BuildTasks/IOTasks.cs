
using System.IO;
using Microsoft.Build.Utilities;

namespace MusicPlayer.BuildTasks
{
    public abstract class FileOperationBaseClass : Task
    {
        protected FileOperationBaseClass() { }

        protected abstract void TaskCodeBody();

        protected virtual System.Boolean TaskCodeValidationPhase() => true;

        public sealed override bool Execute()
        {
            if (System.String.IsNullOrEmpty(InputDirectory))
            {
                Log.LogErrorWithCode("MPCU0001", "The input directory must not be the empty string.");
                return false;
            }
            if (System.String.IsNullOrEmpty(TargetDirectory))
            {
                Log.LogErrorWithCode("MPCU0002", "The target directory must not be the empty string.");
                return false;
            }
            if (Directory.Exists(InputDirectory) == false)
            {
                Log.LogErrorWithCode("MPCU0004", "The input directory with path {0} does not exist.", InputDirectory);
                return false;
            }
            if (TaskCodeValidationPhase() == false) { return false; }
            try {
                TaskCodeBody();
            } catch (System.Exception ex) {
                Log.LogErrorWithCode("MPCU0000", "Task failed from an internal exception.\nDetails: {0}", ex);
                return false;
            }
            return true;
        }

        public System.String InputDirectory { get; set; }

        public System.String TargetDirectory { get; set; }
    }

    public sealed class CopyFilesMatchingWin32Filter : FileOperationBaseClass
    {
        public CopyFilesMatchingWin32Filter() { }

        protected override void TaskCodeBody()
        {
            DirectoryInfo dit = new DirectoryInfo(TargetDirectory);
            if (dit.Exists == false)
            {
                Log.LogWarningWithCode("MPCU0005", "The directory with name {0} does not exist , creating it now.", dit.Name);
                dit.Create();
            }
            DirectoryInfo dib = new DirectoryInfo(InputDirectory);
            foreach (FileInfo fi in dib.GetFiles(Filter))
            {
                fi.CopyToDirectory(dit);
            }
        }

        protected override bool TaskCodeValidationPhase()
        {
            if (System.String.IsNullOrEmpty(Filter))
            {
                Log.LogErrorWithCode("MPCU0003", "The file seperation filter must not be the empty string.");
                return false;
            }
            return true;
        }

        public System.String Filter { get; set; }
    }

    public sealed class MoveFilesMatchingWin32Filter : FileOperationBaseClass
    {
        public MoveFilesMatchingWin32Filter() { }

        protected override void TaskCodeBody()
        {
            DirectoryInfo dit = new DirectoryInfo(TargetDirectory);
            if (dit.Exists == false)
            {
                Log.LogWarningWithCode("MPCU0005", "The directory with name {0} does not exist , creating it now.", dit.Name);
                dit.Create();
            }
            DirectoryInfo dib = new DirectoryInfo(InputDirectory);
            foreach (FileInfo fi in dib.GetFiles(Filter))
            {
                fi.MoveToDirectory(dit);
            }
        }

        protected override bool TaskCodeValidationPhase()
        {
            if (System.String.IsNullOrEmpty(Filter))
            {
                Log.LogErrorWithCode("MPCU0003", "The file seperation filter must not be the empty string.");
                return false;
            }
            return true;
        }

        public System.String Filter { get; set; }
    }

    public sealed class DeleteFilesMatchingWin32Filter : FileOperationBaseClass
    {
        public DeleteFilesMatchingWin32Filter() { TargetDirectory = "EDT"; }

        protected override void TaskCodeBody()
        {
            DirectoryInfo dib = new DirectoryInfo(InputDirectory);
            foreach (FileInfo fi in dib.GetFiles(Filter))
            {
                fi.Delete();
            }
        }

        protected override bool TaskCodeValidationPhase()
        {
            if (System.String.IsNullOrEmpty(Filter))
            {
                Log.LogErrorWithCode("MPCU0003", "The file seperation filter must not be the empty string.");
                return false;
            }
            return true;
        }

        public System.String Filter { get; set; }
    }
}