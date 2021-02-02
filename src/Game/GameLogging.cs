using Godot;
using MSG.Script.UI.Game;
using SpartansLib;

namespace MSG.Game
{
    public static class GameLogging
    {
        private static readonly string INFO_COLOR = Colors.LightBlue.ToHtml();
        private static readonly string WARNING_COLOR = Colors.Yellow.ToHtml();
        private static readonly string ERROR_COLOR = Colors.DarkRed.ToHtml();

        private static Console _console;
        static GameLogging()
        {
            _console = NodeRegistry.Get<Console>();
        }

        public static void Info(string input)
        {
            _console.PrintLine($"[color=#{INFO_COLOR}]Game Info: {input}[/color]");
        }

        public static void Warning(string input)
        {
            _console.PrintLine($"[color=#{WARNING_COLOR}]Game Warning: {input}[/color]");
        }

        public static void Error(string input)
        {
            _console.PrintLine($"[color=#{ERROR_COLOR}]Game ERROR: {input}[/color]");
        }
    }
}