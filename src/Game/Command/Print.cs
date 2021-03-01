using MSG.Engine.Command;

namespace MSG.Game.Command
{
    public class Print : BaseCommand
    {
        public override string Name => "print";

        public override string Description => "Prints a line.";

        public override (ArgType type, string name, string description)[] Arguments =>
            new[]
            {
                (ArgType.String, "statement", "The text to print to the console.")
            };

        protected override void Execute(ICommandInterface console, ArgList arguments)
        {
            string print = arguments.FullExecution.Remove(0, Name.Length + 1);
            if (print.Trim()[0] == '"') print = arguments[1];
            console.PrintLine(print);
        }
    }
}