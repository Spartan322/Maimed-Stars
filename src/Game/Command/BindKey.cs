using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using MSG.Engine.Command;
using MSG.Global;
using MSG.Global.Attribute;

namespace MSG.Game.Command
{
    public class BindKey : BaseCommand
    {
        internal static Dictionary<string, string> BindedActions = new Dictionary<string, string>();

        internal static Dictionary<string, List<CompiledCommand>> CompiledBind =
            new Dictionary<string, List<CompiledCommand>>();

        public override string Name => "bind";
        public override string Description => "Binds a command to an input event.";

        public override (ArgType type, string name, string description)[] Arguments =>
            new[]
            {
                (ArgType.String, "input", "The input event, action, or key that triggers the command."),
                (ArgType.String, "command", "The command to trigger when input is triggered.")
            };

        public override CompiledCommand GetCompiledCommand(ArgList args)
        {
            return CompiledCommand.Empty;
        }

        protected override void Execute(ICommandInterface console, ArgList arguments)
        {
            var bind = arguments[1];
            var command = arguments[2];
            var cmdArgs = CommandHandler.GenerateCommandList(command);
            var splitedCmd = command.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
            if (splitedCmd.Length > 0)
            {
                var actionName = "binded_" + bind.Replace(' ', (char) 0).ToLower().Trim();
                if (!InputMap.HasAction(actionName))
                {
                    InputMap.AddAction(actionName);
                    InputMap.ActionAddEvent(actionName, GenerateInputEventByName(bind));
                }

                foreach (var arg in cmdArgs)
                {
                    var cc = CommandHandler.GetCommand(arg[0]).GetCompiledCommand(arg);
                    if (cc?.IsEmpty != true)
                    {
                        if (cc != null)
                        {
                            if (!CompiledBind.ContainsKey(actionName))
                                CompiledBind[actionName] = new List<CompiledCommand> {cc};
                            else CompiledBind[actionName].Add(cc);
                        }
                        else if (BindedActions.ContainsKey(actionName))
                            BindedActions[actionName] += $";{arg}";
                        else BindedActions[actionName] = arg.ToString();
                    }
                }
            }
        }

        private static GenericCommandInterface<GlobalScript> commandInterface;

        [Input]
        public static void OnInput(GlobalScript script, InputEvent @event)
        {
            if (@event is InputEventAction actionEvent)
            {
                if (CompiledBind.TryGetValue(actionEvent.Action, out var ccl))
                    ccl.ForEach(cc => cc.Invoke(script));
                if (BindedActions.TryGetValue(actionEvent.Action, out var command))
                {
                    if (commandInterface == null)
                        commandInterface = new GenericCommandInterface<GlobalScript>(script);
                    else commandInterface.Node = script;
                    commandInterface.Execute(command);
                }

                if (ccl == null && command == null) return;
                script.GetTree().SetInputAsHandled();
            }
        }

        private static InputEvent GenerateInputEventByName(string name)
        {
            var scancode = (uint) OS.FindScancodeFromString(name);
            if (scancode > 0)
                return new InputEventKey {Scancode = scancode, Pressed = true};
            var splittedName = name.Split(new[] {'='}, StringSplitOptions.RemoveEmptyEntries);
            if (splittedName.Length > 1)
            {
                var data = new string[splittedName.Length / 2];
                for (int i = 1; i < splittedName.Length; i += 2)
                    data[i / 2] = splittedName[i];

                switch (splittedName[0].Remove(splittedName[0].IndexOf(':')).Trim())
                {
                    case "InputEventMouseButton":
                        int buttonIndex = 0;
                        if (!int.TryParse(data[0], out buttonIndex))
                        {
                            if (Enum.TryParse(data[0], out ButtonList buttonEnum))
                                buttonIndex = (int) buttonEnum;
                        }

                        return new InputEventMouseButton
                        {
                            ButtonIndex = buttonIndex,
                            Pressed = data[1].ToLower().Trim() == "true",
                            ButtonMask = int.Parse(data[3]),
                            Doubleclick = data[4].ToLower().Trim() == "true"
                        };
                }

                return null;
            }

            if (splittedName.Length == 1)
            {
                var simpleNameList = name.Trim().Replace('_', (char) 0).ToLower()
                    .Split(new[] {'+'}, StringSplitOptions.RemoveEmptyEntries).ToList();
                if (simpleNameList.Count < 1) return null;
                var hasShift = simpleNameList.IndexOf("shift") == 0;
                if (hasShift) simpleNameList.RemoveAt(0);
                var hasAlt = simpleNameList.IndexOf("alt") == 0;
                if (hasAlt) simpleNameList.RemoveAt(0);
                var hasCtrl = simpleNameList.IndexOf("ctrl") == 0;
                if (hasCtrl) simpleNameList.RemoveAt(0);
                var hasMeta = simpleNameList.IndexOf("meta") == 0;
                if (hasMeta) simpleNameList.RemoveAt(0);
                if (simpleNameList.Count < 1) return null;
                var hasDouble = simpleNameList[0].EndsWith("x2", StringComparison.InvariantCulture);
                switch (simpleNameList[0])
                {
                    case "mousewheelup":
                        return new InputEventMouseButton
                        {
                            ButtonIndex = (int) ButtonList.WheelUp, Shift = hasShift, Alt = hasAlt, Control = hasCtrl,
                            Meta = hasMeta, Doubleclick = hasDouble, Command = hasMeta, Pressed = true
                        };
                    case "mousewheeldown":
                        return new InputEventMouseButton
                        {
                            ButtonIndex = (int) ButtonList.WheelDown, Shift = hasShift, Alt = hasAlt, Control = hasCtrl,
                            Meta = hasMeta, Doubleclick = hasDouble, Command = hasMeta, Pressed = true
                        };
                    case "mousewheelleft":
                        return new InputEventMouseButton
                        {
                            ButtonIndex = (int) ButtonList.WheelLeft, Shift = hasShift, Alt = hasAlt, Control = hasCtrl,
                            Meta = hasMeta, Doubleclick = hasDouble, Command = hasMeta, Pressed = true
                        };
                    case "mousewheelright":
                        return new InputEventMouseButton
                        {
                            ButtonIndex = (int) ButtonList.WheelRight, Shift = hasShift, Alt = hasAlt,
                            Control = hasCtrl, Meta = hasMeta, Doubleclick = hasDouble, Command = hasMeta,
                            Pressed = true
                        };
                    case "mouseleft":
                        return new InputEventMouseButton
                        {
                            ButtonIndex = (int) ButtonList.Left, Shift = hasShift, Alt = hasAlt, Control = hasCtrl,
                            Meta = hasMeta, Doubleclick = hasDouble, Command = hasMeta, Pressed = true
                        };
                    case "mouseright":
                        return new InputEventMouseButton
                        {
                            ButtonIndex = (int) ButtonList.Left, Shift = hasShift, Alt = hasAlt, Control = hasCtrl,
                            Meta = hasMeta, Doubleclick = hasDouble, Command = hasMeta, Pressed = true
                        };
                    case "mousemiddle":
                        return new InputEventMouseButton
                        {
                            ButtonIndex = (int) ButtonList.Left, Shift = hasShift, Alt = hasAlt, Control = hasCtrl,
                            Meta = hasMeta, Doubleclick = hasDouble, Command = hasMeta, Pressed = true
                        };
                    case "mousexbutton1":
                        return new InputEventMouseButton
                        {
                            ButtonIndex = (int) ButtonList.Left, Shift = hasShift, Alt = hasAlt, Control = hasCtrl,
                            Meta = hasMeta, Doubleclick = hasDouble, Command = hasMeta, Pressed = true
                        };
                    case "mousexbutton2":
                        return new InputEventMouseButton
                        {
                            ButtonIndex = (int) ButtonList.Left, Shift = hasShift, Alt = hasAlt, Control = hasCtrl,
                            Meta = hasMeta, Doubleclick = hasDouble, Command = hasMeta, Pressed = true
                        };
                }
            }

            return null;
        }
    }
}