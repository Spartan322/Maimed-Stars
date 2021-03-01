using System;
using System.Collections.Generic;
using System.Linq;
using SpartansLib;
using SpartansLib.Extensions;

namespace MSG.Engine.Command
{
    public class ManualCommandAttribute : Attribute
    {
    }

    public static class CommandHandler
    {
        static CommandHandler()
        {
            foreach (var t in typeof(BaseCommand).Assembly.GetExportedTypes())
            {
                if (t.IsSubclassOf(typeof(BaseCommand))
                    && !t.HasAttribute<ManualCommandAttribute>())
                {
                    var ctors = t.GetConstructors();
                    if (ctors.Length < 2 && ctors[0].GetParameters().Length < 1)
                        RegisterCommand((BaseCommand)Activator.CreateInstance(t));
                }
            }
        }

        private static readonly Dictionary<string, BaseCommand> commands = new Dictionary<string, BaseCommand>();

        public static void RegisterCommand(BaseCommand command)
        {
            if (commands.ContainsKey(command.Name))
                throw new ArgumentException($"Command name '{command.Name}' already registered.", nameof(command));
            commands[command.Name] = command;
        }

        public static BaseCommand GetCommand(string name)
        {
            if (commands.TryGetValue(name, out var cmd))
                return cmd;
            return null;
        }

        public static List<BaseCommand> GetAllCommands()
            => commands.Values.ToList();

        const string COMMAND_SEP = ";";
        const string NEWLINE = "\n";
        const string WHITESPACE = " \t" + NEWLINE;
        const string QUOTE = "\"";
        const string BACKSLASH = "\\";

        public static List<ArgList> GenerateCommandList(string input)
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

        public static BaseCommand ExecuteCommand(ICommandInterface cmdInterface, ArgList argList)
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