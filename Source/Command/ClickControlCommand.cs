﻿using Godot;
using MSG.Script.UI.Game;

namespace MSG.Command
{
    public class ClickControlCommand : BaseCommand
    {
        public override string Name => "sim_click";
        public override string Description => "Clicks the Control object containing the ID.";

        public override (ArgType type, string name, string description)[] Arguments =>
            new[]
            {
                (ArgType.Long, "ID", "The control to click.")
            };

        public override CompiledCommand GetCompiledCommand(ArgList args)
        {
            var cc = new CompiledCommand<Control>();
            var id = args.GetAs<ulong>(1);
            if (id != null && GD.InstanceFromId(id.Value) is Control ctrl)
            {
                cc.Args = ctrl;
                cc.OnInvoke += (s, control) =>
                {
                    var dummyInput = new InputEventMouseButton
                    {
                        Position = ctrl.RectGlobalPosition,
                        ButtonIndex = (int) ButtonList.Left,
                        Pressed = true
                    };
                    Input.ParseInputEvent(dummyInput);
                    dummyInput.Pressed = false;
                    Input.ParseInputEvent(dummyInput);
                    s.GrabFocus();
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
                    ButtonIndex = (int) ButtonList.Left,
                    Pressed = true
                };
                Input.ParseInputEvent(dummyInput);
                dummyInput.Pressed = false;
                Input.ParseInputEvent(dummyInput);
                console.CastTo<Console>()?.ConsoleLine.GrabFocus();
                console.PrintLine($"Clicking '{ctrl.Name}'");
            }
        }
    }
}