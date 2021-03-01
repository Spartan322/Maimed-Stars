using System;

namespace MSG.Engine.Command
{
    public class CommandArguments : EventArgs
    {
        public ICommandInterface Interface;
        public BaseCommand Command = null;
        public ArgList ArgumentList;
        public bool IgnoreNullCommand = false;
    }
}