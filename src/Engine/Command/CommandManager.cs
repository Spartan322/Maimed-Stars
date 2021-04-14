using System;
using System.Collections.Generic;
using System.Linq;
using SpartansLib;
using SpartansLib.Extensions;
using MSG.Script.Game.World;

namespace MSG.Engine.Command
{
    public class ManualCommandAttribute : Attribute
    {
    }

    public interface ICommandManager
    {
        ICommandInterface Owner { get; }
        void RegisterCommand(BaseCommand command);
        BaseCommand GetCommand(string name);
        IList<BaseCommand> GetAllCommands();
        BaseCommand ExecuteCommand(ICommandInterface cmdInterface, ArgList argList);
        IList<ArgList> GenerateCommandList(string input);
    }

    public class CommandManager : ICommandManager
    {
        public ICommandInterface Owner { get; }

        public CommandManager(ICommandInterface cmdDomain)
        {
            Owner = cmdDomain;
            foreach (var t in typeof(BaseCommand).Assembly.GetExportedTypes())
            {
                if (!t.IsSubclassOf(typeof(BaseCommand)) || t.HasAttribute<ManualCommandAttribute>())
                    continue;
                var ctors = t.GetConstructors();
                if (ctors.Length != 1) continue;

                var parameters = ctors[0].GetParameters();
                if (parameters.Length != 1 || parameters[0].ParameterType != typeof(ICommandManager))
                    continue;

                RegisterCommand((BaseCommand)Activator.CreateInstance(t, this));
            }
        }

        private readonly Dictionary<string, BaseCommand> commands = new Dictionary<string, BaseCommand>();

        public void RegisterCommand(BaseCommand command)
        {
            if (commands.ContainsKey(command.Name))
                throw new ArgumentException($"Command name '{command.Name}' already registered.", nameof(command));
            commands[command.Name] = command;
        }

        public BaseCommand GetCommand(string name)
        {
            Godot.GD.Print(name);
            Godot.GD.Print(commands.Count);
            if (commands.TryGetValue(name, out var cmd))
                return cmd;
            return null;
        }

        public IList<BaseCommand> GetAllCommands()
            => commands.Values.ToList();

        const string COMMAND_SEP = ";";
        const string NEWLINE = "\n";
        const string WHITESPACE = " \t" + NEWLINE;
        const string QUOTE = "\"";
        const string BACKSLASH = "\\";

        public IList<ArgList> GenerateCommandList(string input)
        {
            if (!input.ContainsAny(QUOTE + BACKSLASH + COMMAND_SEP))
            {
                return new List<ArgList>
                {
                    new ArgList(input.Split(WHITESPACE.ToCharArray(), StringSplitOptions.RemoveEmptyEntries), input)
                };
            }

            var escapedOutput = input.ParseEscapableString(QUOTE, WHITESPACE + COMMAND_SEP, BACKSLASH, out var indexes);
            if (!input.ContainsAny(COMMAND_SEP))
                return new List<ArgList> { new ArgList(escapedOutput, input) };
            var multiArgList = escapedOutput.ParseLineSeperator(COMMAND_SEP + NEWLINE, BACKSLASH, indexes);

            var argList = new List<string>();
            var result = new List<ArgList>();
            foreach (var (parsedString, endStatement) in multiArgList)
            {
                argList.Add(parsedString);
                if (endStatement)
                {
                    result.Add(new ArgList(argList, input));
                    argList.Clear();
                }
            }

            return result;
        }

        public BaseCommand ExecuteCommand(ICommandInterface cmdInterface, ArgList argList)
        {
            var cmd = GetCommand(argList[0]);
            if (cmd == null)
                return null;
            var cmdArgs = new CommandArguments { Interface = cmdInterface, Command = cmd, ArgumentList = argList };
            cmdInterface.OnCommandExecute(cmdArgs);
            cmd?.ExecuteCommand(cmdArgs);
            return cmd;
        }
    }
}