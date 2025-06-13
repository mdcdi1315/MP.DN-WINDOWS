

using System;

namespace MP.ProcessLaunching
{
    public sealed class PreCompiledProcessLauncher : IDisposable
    {
        private ProcessLauncher plr;
        private CommandLineFileReader reader;

        public PreCompiledProcessLauncher(System.IO.Stream commandfile)
        {
            plr = new();
            reader = new CommandLineFileReader(commandfile);
        }

        public VariablesDictionary EnvironmentBlock => plr.EnvironmentBlock;

        public System.String[] RequiredVariables => reader.RequiredVariables;

        public ProcessCreationFlags CreationFlags
        {
            get => plr.CreationFlags;
            set => plr.CreationFlags = value;
        }

        public System.String ExecutablePath => plr.ExecutablePath;

        public System.String CommandLine => plr.CommandLine;

        public System.UInt32 ExitCode => plr.ExitCode;

        public void ApplyVariableTransformations()
        {
            foreach (var d in reader.PreassignedVariables)
            {
                EnvironmentVariable built = new(d.Name, null);
                built.Value = CommandLineSchemaUtils.ExpandVariables(plr.EnvironmentBlock , d.Value);
                if (plr.EnvironmentBlock.Contains(built.Name)) {
                    plr.EnvironmentBlock.Set(built.Name , built.Value);
                } else {
                    plr.EnvironmentBlock.Add(built);
                }
            }
            plr.ExecutablePath = CommandLineSchemaUtils.ExpandVariables(plr.EnvironmentBlock, reader.ExecutablePath);
            plr.CommandLine = CommandLineSchemaUtils.ExpandVariables(plr.EnvironmentBlock, reader.ExecutableArguments);
        }

        public void LaunchAndWaitToFinish()
        {
            plr.Launch();
            System.Threading.Thread.Sleep(100);
            plr.WaitUntilTerminated();
        }

        public void Dispose() 
        {
            plr?.Dispose();
            plr = null;
            reader?.Dispose();
            reader = null;
        }
    }
}