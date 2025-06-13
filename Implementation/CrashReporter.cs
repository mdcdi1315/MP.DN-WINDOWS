

using System;
using Microsoft.IO;
using MP.Threading;
using System.Collections.Generic;

namespace MP
{
    public sealed class CrashReporter : IDisposable
    {
        private System.Text.Encoding enccurrent;
        private System.Byte[] temp;
        private FileStream stream;

        public CrashReporter(System.String SaveDirectory)
        {
            System.DateTime current = SystemInfo.Now;
            stream = new(Path.Join(SaveDirectory , 
                $"crash-report-{current.Year}-{current.Month}-{current.Day}-{current.Hour}_{current.Minute}.txt") , FileMode.Create);
            enccurrent = System.Text.Encoding.UTF8;
        }

        public void WriteStartingText()
        {
            WriteLine("MUSIC PLAYER FAILED ON SOME CODE PATHS!!!!");
            WriteLineTerminator();
            WriteLine("Crash Report for Music Player On .NET 8.");
            WriteLine("Some detailed aspects of the system...");
            WriteLineTerminator();
            WriteLine($"Time of generating this report: {SystemInfo.Now:yyyy/MM/ss hh:mm:ss.fffff tt zzz} LOCAL");
            WriteLine($"Music Player Version: {AppInfo.Version}");
            WriteLine($".NET Runtime Version: {Environment.Version}");
            WriteLine($"Windows User generating this crash: {SystemInfo.UserName}");
            WriteLine($"Windows User Hardware Profile: {SystemInfo.HardwareProfile}");
            WriteLine($"Windows User Hardware Profile ID: {SystemInfo.HardwareProfileID}");
            WriteLine($"Computer Name: {SystemInfo.ComputerName}");
            WriteLine($"Current Working Directory: {SystemInfo.CurrentDirectory}");
            WriteLine($"Current Process Directory: {SystemInfo.CurrentProcessDirectory}");
            WriteLine($"Is from a native VHD Boot: {SystemInfo.IsVirtualMachine}");
            WriteLine($"Has Administrator Priviledges: {SystemInfo.HasAdminPriviledges}");
            WriteLine($"Windows Version of reporting this crash: {SystemInfo.OperatingSystemVersion}");
            WriteLine($"CPU Architecture: {SystemInfo.ProcessorArchitecture}");
            WriteLine($"Process Memory Allocation Page Size: {SystemInfo.PageSize}");
            WriteLine($"CPU Revision: {(SystemInfo.LayerUsed as SystemInfo_Windows).ProcessorRevision}");
            WriteLine($"BIOS Firmware type: {SystemInfo.SystemFirmwareType}");
            WriteLine("Process Environment Block: ");
            foreach (var envvar in SystemInfo.GetEnvironmentVariables())
            {
                WriteLine($"{envvar.Name}={envvar.Value}");
            }
            WriteLineTerminator();
            WriteLineTerminator();
        }

        public void WriteLineTerminator() => stream.WriteByte(13);

        public void WriteLine(System.String line)
        {
            if (line is null) { line = System.String.Empty; }
            temp = enccurrent.GetBytes(line);
            stream.Write(temp, 0, temp.Length);
            stream.WriteByte(13);
            stream.Flush();
        }

        public void LogReportedFailedDispatchData(IEnumerable<OperationsTaskerWorkItemFailureData> data)
        {
            if (data is null) { return; }
            System.Int32 c = 1;
            foreach (var opf in data) 
            {
                WriteLine($"Exception #{c}: ");
                WriteLine("===================================");
                WriteLineTerminator();
                WriteLine($"Item ID that failed: {opf.WorkQueueItemHash}");
                WriteLine($"Task Start Time: {opf.StartTime:yyyy/MM/ss HH:mm:ss.fffff}");
                WriteLine($"Task Failure Time: {opf.FailureTime:yyyy/MM/ss HH:mm:ss.fffff}");
                WriteLine("==============Exception=============");
                WriteLine(opf.Exception.ToString());
                WriteLine("===================================");
                WriteLineTerminator();
                if (opf.FailedMethod is not null)
                {
                    WriteLine("A detailed report of the method failed , and where it is defined: ");
                    WriteLine($"Failed Method Name: {opf.FailedMethod.Name}");
                    WriteLine($"Method's containing type (short name): {opf.FailedMethod.DeclaringType.FullName}");
                    WriteLine($"Method's containing type (full name): {opf.FailedMethod.DeclaringType.AssemblyQualifiedName}");
                    WriteLine($"Method return type (short name): {opf.FailedMethod.ReturnParameter.ParameterType.FullName}");
                    WriteLine($"Method return type (full name): {opf.FailedMethod.ReturnParameter.ParameterType.AssemblyQualifiedName}");
                    WriteLine("Method arguments: ");
                    var ps = opf.FailedMethod.GetParameters();
                    System.Boolean argsvalid = opf.Arguments is not null;
                    if (ps.Length > 0)  {
                        foreach (var p in ps)
                        {
                            WriteLine($"Argument [{p.Position}]:");
                            WriteLine($"Argument type (short name): {p.ParameterType.FullName}");
                            WriteLine($"Argument type (full name): {p.ParameterType.AssemblyQualifiedName}");
                            WriteLine($"Argument name as defined in the method: {p.Name}");
                            WriteLine($"Argument Metadata Token: {p.MetadataToken}");
                            if (argsvalid && p.Position < opf.Arguments.Length) {
                                WriteLine($"Argument value passed to the method: {opf.Arguments[p.Position]}");
                            }
                            WriteLineTerminator();
                        }
                    } else {
                        WriteLine("No arguments are defined for this method.");
                        WriteLineTerminator();
                    }
                }
                WriteLineTerminator();
                WriteLineTerminator();
                c++;
            }
        }

        public void Dispose()
        {
            if (stream is not null)
            {
                stream.Flush();
                stream.Dispose();
                stream = null;
            }
            enccurrent = null;
        }
    }
}