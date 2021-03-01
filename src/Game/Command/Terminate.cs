using Godot;
using MSG.Engine.Command;

namespace MSG.Game.Command
{
    public class Terminate : BaseCommand
    {
        public override string Name => "terminate";
        public override string Description => "Exits the game program.";

        public override (ArgType, string, string)[] Arguments =>
            new[]
            {
                (ArgType.Float, "WaitTime", "Optional wait time in seconds.")
            };

        private TerminateTimer termTimer = new TerminateTimer();

        protected override void Execute(ICommandInterface console, ArgList arguments)
        {
            if (termTimer.GetParent() == null)
                console.CastTo<Node>().AddChild(termTimer);
            var arg1 = arguments.GetAs<float>(1);
            if (arg1 != null)
                termTimer.Start(arg1.Value);
            else termTimer.Start(Mathf.Epsilon);
            console.PrintLine($"Shutting Down game{(arg1 != null ? $" in {arg1} seconds." : ".")}");
        }

        public class TerminateTimer : Timer
        {
            public override void _Ready()
            {
                Autostart = true;
                OneShot = true;
                Connect("timeout", this, nameof(Execute));
            }

            public void Execute() => GetTree().Quit();
        }
    }
}