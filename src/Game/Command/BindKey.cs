using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using MSG.Engine.Command;
using MSG.Script.Game.World;
using MSG.Global;
using MSG.Global.Attribute;

namespace MSG.Game.Command
{
    public class BindKey : BaseCommand
    {
        internal static Dictionary<string, string> BindedActions
            = new Dictionary<string, string>();

        internal static Dictionary<string, List<CompiledCommand>> CompiledBind
            = new Dictionary<string, List<CompiledCommand>>();

        public override string Name => "bind";
        public override string Description => "Binds a command to an input event.";

        public override (ArgType type, string name, string description)[] Arguments =>
            new[]
            {
                (ArgType.String, "input", "The input event, action, or key that triggers the command."),
                (ArgType.String, "command", "The command to trigger when input is triggered.")
            };

        public BindKey(ICommandManager commandManager) : base(commandManager)
        {
            commandManager.Owner.Connect("UnhandledInput", this, nameof(OnInput));
        }

        public override CompiledCommand GetCompiledCommand(ArgList args)
        {
            return CompiledCommand.Empty;
        }

        protected override void Execute(ICommandInterface console, ArgList arguments)
        {
            var bind = arguments[1];
            var command = arguments[2];
            var cmdArgs = CommandManager.GenerateCommandList(command);
            var splitedCmd = command.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (splitedCmd.Length > 0)
            {
                var actionName = "binded_" + bind.Replace(' ', (char)0).ToLower().Trim();
                if (!InputMap.HasAction(actionName))
                {
                    InputMap.AddAction(actionName);
                    InputMap.ActionAddEvent(actionName, GenerateInputEventByName(bind));
                }

                foreach (var arg in cmdArgs)
                {
                    var cc = CommandManager.GetCommand(arg[0]).GetCompiledCommand(arg);
                    if (cc?.IsEmpty != true)
                    {
                        if (cc != null)
                        {
                            if (!CompiledBind.ContainsKey(actionName))
                                CompiledBind[actionName] = new List<CompiledCommand> { cc };
                            else CompiledBind[actionName].Add(cc);
                        }
                        else if (BindedActions.ContainsKey(actionName))
                            BindedActions[actionName] += $";{arg}";
                        else BindedActions[actionName] = arg.ToString();
                    }
                }
            }
        }

        private static GenericCommandInterface commandInterface;

        public void OnInput(InputEvent @event)
        {
            if (@event is InputEventAction actionEvent)
            {
                if (CompiledBind.TryGetValue(actionEvent.Action, out var ccl))
                    ccl.ForEach(cc => cc.Invoke(CommandManager));
                if (BindedActions.TryGetValue(actionEvent.Action, out var command))
                {
                    if (commandInterface == null)
                        commandInterface = new GenericCommandInterface(CommandManager);
                    else
                        commandInterface.CommandManager = CommandManager;
                    commandInterface.Execute(command);
                }

                if (ccl == null && command == null) return;
                CommandManager.Owner.GetParent().GetTree().SetInputAsHandled();
            }
        }

        private static InputEvent GenerateInputEventByName(string name)
        {
            var scancode = (uint)OS.FindScancodeFromString(name);
            if (scancode > 0)
                return new InputEventKey { Scancode = scancode, Pressed = true };
            var splittedName = name.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
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
                                buttonIndex = (int)buttonEnum;
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
                var simpleNameList = name.Trim().Replace('_', (char)0).ToLower()
                    .Split(new[] { '+' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                if (simpleNameList.Count < 1) return null;
                var hasShift = simpleNameList.HasAndRemove("shift");
                var hasAlt = simpleNameList.HasAndRemove("alt");
                var hasCtrl = simpleNameList.HasAndRemove("ctrl");
                var hasMeta = simpleNameList.HasAndRemove("meta");
                if (simpleNameList.Count < 1) return null;
                var hasDouble = simpleNameList[0].EndsWith("x2", StringComparison.InvariantCulture);
                var buttonIndex = simpleNameList[0] switch
                {
                    "mousewheelup" => ButtonList.WheelUp,
                    "mousewheeldown" => ButtonList.WheelDown,
                    "mousewheelleft" => ButtonList.WheelLeft,
                    "mousewheelright" => ButtonList.WheelRight,
                    "mouseleft" => ButtonList.Left,
                    "mouseright" => ButtonList.Right,
                    "mousemiddle" => ButtonList.Middle,
                    "mousexbutton1" => ButtonList.Xbutton1,
                    "mousexbutton2" => ButtonList.Xbutton2,
                    _ => (ButtonList)0
                };
                if (buttonIndex == 0) return null;
                return new InputEventMouseButton
                {
                    ButtonIndex = (int)buttonIndex,
                    Shift = hasShift,
                    Alt = hasAlt,
                    Control = hasCtrl,
                    Meta = hasMeta,
                    Doubleclick = hasDouble,
                    Command = hasMeta,
                    Pressed = true
                };
            }

            return null;
        }
    }

    internal static class BindKeyIListExtension
    {
        public static bool HasAndRemove(this IList<string> list, string has)
        {
            var result = list.IndexOf(has) == 0;
            if (result) list.RemoveAt(0);
            return result;
        }
    }
}