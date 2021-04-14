using System;
using Godot;
using Array = Godot.Collections.Array;
using Object = Godot.Object;

namespace MSG.Engine.Command
{
    public class GenericCommandInterface : ICommandInterface
    {
        public ICommandManager CommandManager;

        public GenericCommandInterface(ICommandManager commandManager)
        {
            CommandManager = commandManager;
        }

        public event Action<CommandArguments> OnExecute;

        public void AddHistory(string input)
        {
        }

        public T2 CastTo<T2>() where T2 : class => this as T2;

        public void Clear()
        {
        }

        public Error Connect(string signal, Object target, string method, Array binds = null,
            uint flags = 0)
            => CommandManager.Owner.GetParent().Connect(signal, target, method, binds, flags);

        public void Execute(string input)
        {
            foreach (var argList in CommandManager.GenerateCommandList(input))
                if (CommandManager.ExecuteCommand(this, argList) == null)
                    PrintLine("Command Unknown.");
        }

        public void OnCommandExecute(CommandArguments cmdArgs)
        {
            OnExecute?.Invoke(cmdArgs);
        }

        public string GetHistory(int index) => null;

        public Node GetParent() => CommandManager.Owner.GetParent();
        public SceneTree GetTree() => CommandManager.Owner.GetParent().GetTree();

        public void Print(string str)
            => GD.Print(str);

        public void Print(params string[] str)
            => GD.Print(str);

        public void Print(params object[] obj)
            => GD.Print(obj);

        public void PrintLine(string str = "")
            => GD.Print(str);

        public void PrintLine(params string[] str)
            => GD.Print(str);

        public void PrintLine(params object[] obj)
            => GD.Print(obj);
    }
}