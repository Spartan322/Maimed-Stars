using System.Collections.Generic;
using System.Linq;
using MSG.Engine.Command;

namespace MSG.Game.Command
{
    public class Help : BaseCommand
    {
        private const int HELP_PAGE_CMD_COUNT = 5;

        public override string Name => "help";
        public override string Description => $"Prints a {FormatName()} message.";

        public override (ArgType, string, string)[] Arguments =>
            new[]
            {
                (ArgType.Int | ArgType.String, "HelpIndex", "Optional command name or page index.")
            };

        public Help(ICommandManager commandManager) : base(commandManager) { }

        protected override void Execute(ICommandInterface console, ArgList arguments)
        {
            var cmds = CommandManager.GetAllCommands();
            var arg1 = arguments.GetAs<int>(1);
            if (arg1 != null)
            {
                if (arg1 > -1)
                {
                    PrintPage(arg1.Value);
                    return;
                }
            }
            else if (arguments.Count == 2)
            {
                var command = cmds.FirstOrDefault(cmd => cmd.Name.ToLower() == arguments[1].ToLower());
                if (command != null)
                {
                    console.PrintLine(command.FormatDescription());
                    return;
                }
            }
            if (arguments.Count != 0)
                console.PrintLine("Unrecognized command or page index, displaying first page of help.");

            PrintPage();

            void PrintPage(int page = 1)
            {
                IEnumerable<BaseCommand> commands = cmds;
                page--;
                if (page > -1 && page * HELP_PAGE_CMD_COUNT < cmds.Count)
                    commands = commands.Skip(page * HELP_PAGE_CMD_COUNT);
                page = 0;
                foreach (var cmd in commands)
                {
                    console.PrintLine(cmd.FormatDescription());
                    if (++page == HELP_PAGE_CMD_COUNT) break;
                }
            }
        }
    }
}