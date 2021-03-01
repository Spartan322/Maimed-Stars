using System;
using Godot;
using Array = Godot.Collections.Array;
using Object = Godot.Object;

namespace MSG.Engine.Command
{
    public interface ICommandInterface
    {
        event Action<CommandArguments> OnExecute;
        void Print(string str);
        void Print(params string[] str);
        void Print(params object[] obj);
        void PrintLine(string str = "");
        void PrintLine(params string[] str);
        void PrintLine(params object[] obj);
        void Clear();
        void AddHistory(string input);
        string GetHistory(int index);
        void Execute(string input);
        void OnCommandExecute(CommandArguments cmdArgs);
        T CastTo<T>() where T : class;
        SceneTree GetTree();
        Node GetParent();

        Error Connect(string signal, Object target, string method, Array binds = null,
            uint flags = 0u);
    }
}