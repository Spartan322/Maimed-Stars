using Godot;
using MSG.Engine.Command;
using MSG.Script.Gui.Console;

namespace MSG.Game.Command
{
    public class SimulateClick : BaseCommand
    {
        public override string Name => "sim_click";
        public override string Description => "Simulates a clicks ontop of the Control object's (0,0) coordinate containing the ID.";

        public override (ArgType type, string name, string description)[] Arguments =>
            new[]
            {
                (ArgType.Long, "ID", "The control to click.")
            };

        public SimulateClick(ICommandManager commandManager) : base(commandManager) { }

        public override CompiledCommand GetCompiledCommand(ArgList args)
        {
            var cc = new CompiledCommand<Control>();
            var id = args.GetAs<ulong>(1);
            if (id != null && GD.InstanceFromId(id.Value) is Control ctrl)
            {
                cc.Args = ctrl;
                cc.OnInvoke += (d, control) =>
                {
                    var dummyInput = new InputEventMouseButton
                    {
                        Position = control.RectGlobalPosition,
                        ButtonIndex = (int)ButtonList.Left,
                        Pressed = true
                    };
                    Input.ParseInputEvent(dummyInput);
                    dummyInput.Pressed = false;
                    Input.ParseInputEvent(dummyInput);
                };
                return cc;
            }

            return CompiledCommand.Empty;
        }

        protected override void Execute(ICommandInterface console, ArgList arguments)
        {
            var id = arguments.GetAs<ulong>(1);
            if (id != null && GD.InstanceFromId(id.Value) is Control ctrl)
            {
                var dummyInput = new InputEventMouseButton
                {
                    Position = ctrl.RectGlobalPosition,
                    ButtonIndex = (int)ButtonList.Left,
                    Pressed = true
                };
                Input.ParseInputEvent(dummyInput);
                dummyInput.Pressed = false;
                Input.ParseInputEvent(dummyInput);
                console.CastTo<BaseConsole>()?.ConsoleLine.GrabFocus();
                console.PrintLine($"Clicking '{ctrl.Name}'");
            }
        }
    }
}