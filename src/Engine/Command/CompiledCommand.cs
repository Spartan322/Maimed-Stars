using System;
using MSG.Global;

namespace MSG.Engine.Command
{
    public class CompiledCommand
    {
        public static readonly CompiledCommand Empty = new CompiledCommand();
        public readonly bool IsEmpty;
        public virtual object Argument { get; set; }

        public CompiledCommand(bool empty = true)
        {
            IsEmpty = empty;
        }

        public virtual void Invoke(GlobalScript script)
        {
        }
    }

    public sealed class CompiledCommandFilled : CompiledCommand
    {
        public event Action<GlobalScript> OnInvoke;

        public CompiledCommandFilled() : base(false)
        {
        }

        public override void Invoke(GlobalScript script) => OnInvoke?.Invoke(script);
    }

    public sealed class CompiledCommand<T> : CompiledCommand
    {
        public event Action<GlobalScript, T> OnInvoke;

        public CompiledCommand() : base(false)
        {
        }

        public T Args;

        public override object Argument
        {
            get => Args;
            set => Args = (T) value;
        }

        public override void Invoke(GlobalScript script)
        {
            OnInvoke?.Invoke(script, Args);
        }
    }
}