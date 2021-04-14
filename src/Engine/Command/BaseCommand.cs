using System;
using Godot;
using SpartansLib.Extensions;
using SpartansLib.Structure;
using MSG.Script.Game.World;

namespace MSG.Engine.Command
{
    public abstract class BaseCommand : Reference
    {
        public event Action<CommandArguments> OnExecute;

        public abstract string Name { get; }
        public virtual string Description { get; } = "";

        public virtual (ArgType type, string name, string description)[] Arguments { get; } =
            new (ArgType, string, string)[0];

        public virtual Color NameBaseColor { get; } = ColorExt.FromRGB8(0xff_ff_66);
        public virtual Color TypeBaseColor { get; } = ColorExt.FromRGB8(0x4e_bd_c9);

        protected ICommandManager CommandManager;
        protected BaseCommand(ICommandManager commandManager)
        {
            CommandManager = commandManager;
        }

        public virtual CompiledCommand GetCompiledCommand(ArgList args)
        {
            return null;
        }

        protected abstract void Execute(ICommandInterface console, ArgList arguments);

        public void ExecuteCommand(CommandArguments cmdArguments)
        {
            OnExecute?.Invoke(cmdArguments);
            Execute(cmdArguments.Interface, cmdArguments.ArgumentList);
        }

        private string ConvertTypeToString(ArgType type)
        {
            var typeStr = type.ToString();
            if (typeStr.Contains(","))
                typeStr = typeStr.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .JoinString($"[/color] or[color=#{TypeBaseColor.ToHtml()}]");
            return typeStr;
        }

        public string FormatArguments()
            => Arguments == null || Arguments.Length < 1
                ? ""
                : $"\t* {Arguments.JoinString("\n\t* ", arg => $"[color=#{TypeBaseColor.ToHtml()}]{ConvertTypeToString(arg.type)}[/color] '{arg.name}'{(string.IsNullOrWhiteSpace(arg.description) ? "" : $"\n\t\t{arg.description}")}")}";

        public string FormatDescription()
        {
            var argFormat = FormatArguments();
            if (argFormat != string.Empty) argFormat = "\n" + argFormat;
            argFormat += "\n\t";
            return $"{FormatName()}:{argFormat}{(string.IsNullOrWhiteSpace(Description) ? "" : Description)}";
        }

        public string FormatName()
            => $"[color=#{NameBaseColor.ToHtml()}][url={Name}]{Name}[/url][/color]";
    }
}