using Godot;
using MSG.Script.Gui.Console;
using SpartansLib;

namespace MSG.Engine
{
    public static class GameLogger
    {
        private static readonly string INFO_COLOR = Colors.LightBlue.ToHtml();
        private static readonly string WARNING_COLOR = Colors.Yellow.ToHtml();
        private static readonly string ERROR_COLOR = Colors.DarkRed.ToHtml();

        private static BaseConsole _baseConsole;
        static GameLogger()
        {
            _baseConsole = NodeRegistry.Get<BaseConsole>();
        }

        public static void Info(string input)
        {
            _baseConsole.PrintLine($"[color=#{INFO_COLOR}]Game Info: {input}[/color]");
        }

        public static void Warning(string input)
        {
            _baseConsole.PrintLine($"[color=#{WARNING_COLOR}]Game Warning: {input}[/color]");
        }

        public static void Error(string input)
        {
            _baseConsole.PrintLine($"[color=#{ERROR_COLOR}]Game ERROR: {input}[/color]");
        }
    }
}