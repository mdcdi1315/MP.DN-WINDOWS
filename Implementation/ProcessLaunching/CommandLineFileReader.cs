
using System;
using System.Collections.Generic;

namespace MP.ProcessLaunching
{
    public sealed class CommandLineFileReader : IDisposable
    {
        private System.Text.Json.JsonDocument jdt;
        private System.Text.Json.JsonElement jelbase;

        public CommandLineFileReader(System.IO.Stream strm)
        {
            try
            {
                jdt = System.Text.Json.JsonDocument.Parse(strm, new() { MaxDepth = 4, CommentHandling = System.Text.Json.JsonCommentHandling.Skip });
                jelbase = jdt.RootElement;
            }
            catch (System.Text.Json.JsonException ex)
            {
                throw new MP.ExceptionSystem.InvalidCmdLineFormatException("The given stream is not a Downloadable Dependencies JSON stream.", ex);
            }
            if (jelbase.ValueKind != System.Text.Json.JsonValueKind.Object)
            {
                jdt.Dispose();
                throw new MP.ExceptionSystem.InvalidCmdLineFormatException($"This is not correct JSON format. Expected object but got {jelbase.ValueKind} instead.");
            }
            try
            {
                if (GetBaseStringProperty("schema") != "MDCDI1315.MP.CMDLINE")
                {
                    throw new MP.ExceptionSystem.InvalidCmdLineFormatException("Schema string does not agree with the expected schema string.");
                }
                if (GetBaseIntProperty("version") > 1)
                {
                    throw new MP.ExceptionSystem.InvalidCmdLineFormatException("This reader can only read the version 1 of the Downloadable Dependencies JSON format.");
                }
            }
            catch (System.Exception ex)
            {
                throw new MP.ExceptionSystem.InvalidCmdLineFormatException("Cannot parse this Downloadable Dependencies JSON stream.", ex);
            }
        }

        private System.String GetBaseStringProperty(System.String name) => jelbase.GetProperty(name).GetString();

        private System.Int32 GetBaseIntProperty(System.String name) => jelbase.GetProperty(name).GetInt32();

        public System.String ExecutablePath => jelbase.GetProperty("CommandLine").GetProperty("ExecutablePath").GetString();

        public System.String ExecutableArguments
        {
            get {
                System.Text.StringBuilder sbf = new();
                foreach (var el in jelbase.GetProperty("CommandLine").GetProperty("Arguments").EnumerateArray())
                {
                    sbf.Append($"{el.GetString()} ");
                }
                return sbf.ToString();
            }
        }

        public System.String[] RequiredVariables
        {
            get {
                System.Text.Json.JsonElement baseel = jelbase.GetProperty("RequiresVariables");
                System.String[] strings = new System.String[baseel.GetArrayLength()];
                System.Text.Json.JsonElement te;
                for (System.Int32 I = 0; I < strings.Length; I++)
                {
                    te = baseel[I];
                    if (te.ValueKind != System.Text.Json.JsonValueKind.String) { throw new MP.ExceptionSystem.InvalidCmdLineFormatException("The RequiresVariables element must be an array with strings only."); }
                    strings[I] = te.GetString();
                }
                return strings;
            }
        }

        public EnvironmentVariable[] PreassignedVariables
        {
            get {
                System.Text.Json.JsonElement jbase = jelbase.GetProperty("SetVariables");
                List<EnvironmentVariable> list = new();
                foreach (var el in jbase.EnumerateObject()) 
                {
                    list.Add(new(el.Name , el.Value.GetString()));
                }
                return list.ToArray();
            }
        }

        public void Dispose()
        {
            jelbase = default;
            jdt.Dispose();
        }
    }
}