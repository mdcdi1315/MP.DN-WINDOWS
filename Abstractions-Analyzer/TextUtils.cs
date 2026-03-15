

using System.Text;

namespace MP.AbstractionsLib.Analyzer
{
    public static class TextUtils
    {
        public static void AppendFormatLine(this StringBuilder builder, string format, object arg_0)
        {
            builder.AppendFormat(format , arg_0);
            builder.AppendLine();
        }

        public static void AppendFormatLine(this StringBuilder builder, string format, object arg_0, object arg_1)
        {
            builder.AppendFormat(format , arg_0 , arg_1);
            builder.AppendLine();
        }

        public static void AppendFormatLine(this StringBuilder builder, string format, params object[] objects)
        {
            builder.AppendFormat(format , args: objects);
            builder.AppendLine();
        }

        public static void AppendTabs(this StringBuilder builder, int times) => AppendTimes(builder, '\t', times);

        public static void AppendTimes(this StringBuilder builder, char c, int times)
        {
            if (times == 0) {
                return;
            } else if (times == 1) {
                builder.Append(c);
            } else {
                builder.Append(new string(c, times));
            }
        }

        public static void AppendLine(this StringBuilder builder, char c)
        {
            builder.Append(c);
            builder.AppendLine();
        }
    }
}