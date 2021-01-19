using System;
using System.Linq;

namespace MSG.Script.UI.Game
{
    public class ConsoleFormatter : IFormatProvider, ICustomFormatter
    {
        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            if (arg == null) return "(Null)";
            if (arg is IFormattable fmt) return fmt.ToString(format, formatProvider);
            return arg.ToString();
        }

        public object GetFormat(Type formatType)
        {
            if (formatType == typeof(ICustomFormatter)) return this;
            return null;
        }
    }

    public static class ConsoleFormatHandler
    {
        public static readonly ConsoleFormatter printFormatter = new ConsoleFormatter();

        public static string Format(params string[] str)
        {
            if (str.Length > 0)
                return string.Format(printFormatter, str[0] ?? "(Null)", str.Skip(1).ToArray());
            return "";
        }

        public static string Format(params object[] str)
        {
            if (str.Length > 0)
                return string.Format(printFormatter, str[0]?.ToString() ?? "(Null)", str.Skip(1).ToArray());
            return "";
        }
    }
}