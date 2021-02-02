using System.Linq;
using Godot;
using MSG.Command;
using SpartansLib.Extensions;

namespace MSG.Command
{
    public class ExecCommand : BaseCommand
    {
        public override string Name => "exec";

        public override string Description => "Executes a statement. [color=yellow]WARNING: CAN RUN UNSAFE CODE AND DOES NOT NATURALLY DELIMIT EXECUTIONS.[/color]";

        public override (ArgType type, string name, string description)[] Arguments =>
            new[]
            {
                (ArgType.String, "statement", "The line statement to execute in GDScript. Can delimit executions using \" but all executions need to be escaped by \\.")
            };

        protected override void Execute(ICommandInterface console, ArgList arguments)
        {
            //HACK: resort to using Godot Modules instead, results in debug error breaking
            var execute = arguments.FullExecution.Remove(0, Name.Length + 1);
            if (execute.Trim()[0] == '"') execute = arguments[1];
            var script = new GDScript
            {
                SourceCode = "extends Node\nfunc eval():" + execute
            };
            var err = script.Reload();
            if (err != Error.Ok)
            {
                console.PrintLine($"Execution failed: {err}");
                return;
            }
            var node = new Node();
            node.SetScript(script);
            var root = console.GetTree().Root;
            root.AddChild(node);
            var result = node.Call("eval");
            root.RemoveChild(node);
            node.QueueFree();
            if (result != null) console.PrintLine(result);
        }
    }
}