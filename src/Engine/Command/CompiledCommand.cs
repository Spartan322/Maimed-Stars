using System;
using MSG.Global;
using MSG.Script.Game.World;

namespace MSG.Engine.Command
{
    public class CompiledCommand
    {
        public static readonly CompiledCommand Empty = new CompiledCommand();
        public readonly bool IsEmpty;
        public virtual object Arguments { get; set; }

        public CompiledCommand(bool empty = true)
        {
            IsEmpty = empty;
        }

        public virtual void Invoke(ICommandManager commandManager)
        {
        }
    }

    public sealed class CompiledCommandFilled : CompiledCommand
    {
        public event Action<ICommandManager> OnInvoke;

        public CompiledCommandFilled() : base(false)
        {
        }

        public override void Invoke(ICommandManager commandManager)
            => OnInvoke?.Invoke(commandManager);
    }

    public sealed class CompiledCommand<T> : CompiledCommand
    {
        public event Action<ICommandManager, T> OnInvoke;

        public CompiledCommand() : base(false)
        {
        }

        public T Args;

        public override object Arguments
        {
            get => Args;
            set => Args = (T)value;
        }

        public override void Invoke(ICommandManager commandManager)
        {
            OnInvoke?.Invoke(commandManager, Args);
        }
    }
}