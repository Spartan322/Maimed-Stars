using Godot;
using MSG.Script.UI.Game;
using SpartansLib;

namespace MSG.Command
{
    public class ToggleInterfaceCommand : BaseCommand
    {
        public override string Name => "gui_toggle";
        public override string Description => "Toggles the visibility of the general game GUI.";

        public override CompiledCommand GetCompiledCommand(ArgList args)
        {
            var cc = new CompiledCommandFilled();
            cc.OnInvoke += script => { Execute(null, null); };
            return cc;
        }

        protected override void Execute(ICommandInterface console, ArgList arguments)
        {
            var coreInterface = NodeRegistry.Get<Control>(nameof(InterfaceCore));
            coreInterface.Visible = !coreInterface.Visible;
        }
    }
}