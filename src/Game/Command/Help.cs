using MSG.Engine.Command;

namespace MSG.Game.Command
{
    public class Help : BaseCommand
    {
        public override string Name => "help";
        public override string Description => $"Prints a {FormatName()} message.";

        public override (ArgType, string, string)[] Arguments =>
            new[]
            {
                (ArgType.Int | ArgType.String, "HelpSpecifier", "Optional index/name for specific command help.")
            };

        protected override void Execute(ICommandInterface console, ArgList arguments)
        {
            var cmds = CommandHandler.GetAllCommands();
            var arg1 = arguments.GetAs<int>(1);
            if (arg1 != null)
            {
                arg1 = arg1.Value - 1;
                if (arg1.Value > -1 && cmds.Count > arg1.Value)
                {
                    console.PrintLine(cmds[arg1.Value].FormatDescription());
                    return;
                }
            }
            else if (arguments.Count == 2)
            {
                var command = cmds.Find(cmd => cmd.Name.ToLower() == arguments[1].ToLower());
                if (command != null)
                {
                    console.PrintLine(command.FormatDescription());
                    return;
                }
            }

            foreach (var cmd in cmds)
                console.PrintLine(cmd.FormatDescription());
        }
    }
}