
using System;
using MP.Utilities;

namespace MP.ProcessLaunching
{
    // For Command Line Schema
    public static class CommandLineSchemaUtils
    {
        /**
         * 
         * A valid variable in the Command Line Schema is as follows:
         * $VariableName
         * Where 'VariableName' can be any combination of English characters , digits and underscores.
         * The first character after the dollar sign must be only an English character.
         * 
         */

        private struct VariableConsumingResults
        {
            public System.Int32 ConsumedChars;
            public System.String VariableName;
            public System.Boolean IsInvalidName;
        }

        // Given that a variable has already been detected , get it's name.
        private static VariableConsumingResults GetVariableName(System.String variablenameext)
        {
            if (variablenameext[0].IsDigit()) { throw new FormatException("The first character of the variable name must not be a digit."); }
            VariableConsumingResults ret = new();
            if (variablenameext[0] == '$') { ret.ConsumedChars = 1; ret.IsInvalidName = true; return ret; }
            System.Text.StringBuilder sb = new(variablenameext.Length); // Worst case is the name to be the length of the provided string.
            foreach (var c in variablenameext)
            {
                if (c.IsDigit() || c.IsEnglishCharacter() || c == '_')
                {
                    ret.ConsumedChars++;
                    sb.Append(c);
                } else if (c == '`') {
                    ret.ConsumedChars++;
                    break;
                } else {
                    break;
                }
            }
            ret.VariableName = sb.ToString();
            sb.Clear();
            sb = null;
            return ret;
        }

        // Expands variables , as the Command Line Injector Schema specifies.
        public static System.String ExpandVariables(VariablesDictionary dict , System.String textdec)
        {
            dict ??= new();
            if (System.String.IsNullOrEmpty(textdec)) { throw new ArgumentNullException(nameof(textdec)); }
            System.Text.StringBuilder sbf = new(textdec.Length); // This is a good way to indicate the initial sb capacity.
            for (System.Int32 I = 0; I < textdec.Length; I++) 
            {
                if (textdec[I] == '$') {
                    var vcr = GetVariableName(textdec.Substring(I + 1));
                    I += vcr.ConsumedChars;
                    if (vcr.IsInvalidName) { continue; }
                    sbf.Append(dict[vcr.VariableName]);
                } else {
                    sbf.Append(textdec[I]); 
                }
            }
            return sbf.ToString();
        }
    }
}